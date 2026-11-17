// =====================================================================
// 走査線（スキャンライン）ポストエフェクト用シェーダー
//
// 【URP のポストエフェクト用に書き換えたポイント】
//  1. CGPROGRAM / UnityCG.cginc → HLSLPROGRAM / URP の ShaderLibrary
//     URP では CG 系インクルードは非推奨。SRP Batcher や XR 対応など、
//     URP の機能は HLSL 側のライブラリ前提で作られている。
//
//  2. 頂点シェーダーは自作せず Blit.hlsl の Vert を使う
//     Render Graph の Blit（Blitter.BlitTexture）は頂点バッファを使わず、
//     SV_VertexID から「画面全体を覆う三角形」をプロシージャル生成して描画する。
//     そのため UnityObjectToClipPos(v.vertex) のような従来の書き方では
//     正しく描画されない。Blit.hlsl の Vert はこの方式に対応している。
//
//  3. 入力テクスチャは _MainTex ではなく _BlitTexture
//     Blitter が自動でセットしてくれる名前。XR（VR）のテクスチャ配列にも
//     対応するため SAMPLE_TEXTURE2D_X マクロで読む。
// =====================================================================
Shader "Custom/ScanLine"
{
    // Properties はマテリアルのインスペクター表示用。
    // 実際の値は C# 側（ScanLineRendererFeature）が Volume の値で毎フレーム上書きする。
    // ※ _MainTex は不要になったので削除（入力は _BlitTexture）。
    Properties
    {
        _Intensity ("Intensity", Range(0, 1)) = 1
        _OpacityScanLine ("Opacity ScanLine", Range(0, 2)) = 0.8
        _OpacityNoise ("Opacity Noise", Range(0, 1)) = 0.1
        _FlickeringSpeed ("Flickering Speed", Range(0, 1000)) = 600
        _FlickeringStrength ("Flickering Strength", Range(0, 0.1)) = 0.02
    }

    SubShader
    {
        // "RenderPipeline"="UniversalPipeline" を付けると、
        // URP 以外のパイプラインではこの SubShader が使われなくなる（誤用防止）。
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        // フルスクリーン処理なので深度テスト・深度書き込み・カリング・ブレンドは全部不要。
        ZWrite Off
        ZTest Always
        Cull Off
        Blend Off

        Pass
        {
            // Pass 名は Frame Debugger / Render Graph Viewer で見分けるのに役立つ。
            Name "ScanLine"

            HLSLPROGRAM
            #pragma vertex Vert      // Blit.hlsl が提供する全画面三角形用の頂点シェーダー
            #pragma fragment Frag

            // Core.hlsl … URP の基本マクロ・行列・_Time などを定義
            // Blit.hlsl … Vert / Varyings / _BlitTexture / sampler_LinearClamp などを定義
            // ※ 順番が大事。Core.hlsl を先に include する。
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // C# から Material.SetFloat で渡される値。
            // マテリアルを1つしか使わない Blit 用シェーダーなので CBUFFER は不要
            // （SRP Batcher はメッシュ描画向けの仕組みで、Blit には関係しない）。
            float _Intensity;
            float _OpacityScanLine;
            float _OpacityNoise;
            float _FlickeringSpeed;
            float _FlickeringStrength;

            // 疑似乱数（定番の sin ハッシュ）。0〜1 の値を返す。
            float Random(float2 st)
            {
                return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453123);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                // XR のシングルパスインスタンシングで「どちらの目か」を確定させるマクロ。
                // 非 XR 環境では何もしないので、常に書いておくのがお作法。
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                // 元の画面色を取得
                half4 img = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // ---- 走査線：Y 座標に沿った縞模様 ----
                float s = sin(uv.y * 1000.0);
                float c = cos(uv.y * 1000.0);
                float3 col = float3(c, s, c) * _OpacityScanLine;

                // ---- ノイズ：時間で変化する砂嵐 ----
                // 旧シェーダーの `uv * _Time` は float4 → float2 への暗黙切り捨てで
                // _Time.xy（= t/20, t）が使われていた。見た目を変えないよう明示的に書く。
                // （暗黙の切り捨ては HLSL でワーニングになるので避けるのがベター）
                float r = Random(uv * _Time.xy);
                col += r * _OpacityNoise;

                // ---- フリッカー：画面全体の明滅 ----
                // 旧 `sin(_FlickeringSpeed * _Time)` は _Time.x（= t/20）に切り捨てられていた。
                float flash = sin(_FlickeringSpeed * _Time.x);
                col += flash * _FlickeringStrength;

                // 乗算でエフェクトを適用。
                // sin/cos は負になるので、そのままだと「負の色」が出力される。
                // HDR ターゲットでは負値が後段（ブルーム等）で NaN/黒点の原因になるため 0 で下限を切る。
                half3 effected = img.rgb * max(col, 0.0);

                // _Intensity で元画像とブレンド。0 なら完全に元の画面 = エフェクト OFF と同じ。
                // Volume のフェード（ブレンド距離や Weight）を自然に効かせるための定番の作り。
                return half4(lerp(img.rgb, effected, _Intensity), img.a);
            }
            ENDHLSL
        }
    }
}
