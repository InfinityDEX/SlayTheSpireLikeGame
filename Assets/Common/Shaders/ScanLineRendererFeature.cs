using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace VolumeScripts.ScanLineShader
{
    /// <summary>
    /// 走査線ポストエフェクトを URP のレンダラーに差し込む RendererFeature
    /// </summary>
    /// <remarks>
    /// 【全体の流れ】
    ///   ScanLineVolume（値）  ──┐
    ///                            ├─▶ ScanLinePass（Render Graph に Blit パスを登録）
    ///   Custom/ScanLine（絵）──┘
    ///
    /// 【登場人物の役割分担】
    /// ・ScriptableRendererFeature … レンダラーアセットに追加される「設定＋寿命管理」役。
    ///                                マテリアルの生成/破棄と、パスを積むかどうかの判断をする。
    /// ・ScriptableRenderPass      … 実際の描画処理。Unity 6（URP 17）以降は
    ///                                RecordRenderGraph で「何を読んで何に書くか」を宣言する。
    ///
    /// 【Render Graph とは】
    /// URP 17 以降の標準の描画方式。各パスは即座に描画するのではなく
    /// 「このテクスチャを読んで、このテクスチャに書く」というグラフを登録する。
    /// Unity がグラフ全体を見て、不要なパスの削除・テクスチャのメモリ再利用・
    /// RenderPass のマージなどを自動で最適化してくれる。
    /// </remarks>
    public sealed class ScanLineRendererFeature : ScriptableRendererFeature
    {
        // ------------------------------------------------------------------
        // インスペクター設定
        // ------------------------------------------------------------------

        /// <remarks>
        /// 【Shader.Find ではなくシリアライズ参照にする理由】
        /// ビルド時、どこからも参照されていないシェーダーはビルドから除外される。
        /// Shader.Find("Custom/ScanLine") だとエディターでは動くのに実機で null になる、
        /// という典型的な罠がある。アセットとして参照しておけば確実にビルドに含まれる。
        /// </remarks>
        [SerializeField, Tooltip("Custom/ScanLine シェーダーを指定")]
        private Shader shader;

        /// <remarks>
        /// 描画タイミング。AfterRenderingPostProcessing にすると、
        /// トーンマッピングやカラーグレーディング「後」の最終画に走査線が乗るので、
        /// ブラウン管らしい見た目になる（走査線の黒がブルームで光ったりしない）。
        /// </remarks>
        [SerializeField]
        private RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

        // ------------------------------------------------------------------
        // ランタイムで生成するもの
        // ------------------------------------------------------------------

        private Material _material;
        private ScanLinePass _pass;

        /// <summary>
        /// フィーチャーの初期化。
        /// レンダラーアセットの読み込み時や、インスペクターで値を変えた時などに呼ばれる。
        /// （＝何度も呼ばれ得るので、毎フレームの処理は書かない）
        /// </summary>
        public override void Create()
        {
            if (shader == null)
            {
                return;
            }

            // CoreUtils.CreateEngineMaterial は HideFlags.HideAndDontSave 付きでマテリアルを作る。
            // シーンに保存されず、アセットも汚さない「ランタイム専用マテリアル」になる。
            // → プロジェクトにマテリアルアセット（Custom_ScanLine.mat）を用意しなくてよい。
            _material = CoreUtils.CreateEngineMaterial(shader);

            _pass = new ScanLinePass(_material)
            {
                renderPassEvent = injectionPoint,
            };
        }

        /// <summary>
        /// カメラごと・フレームごとに呼ばれ、パスを積むかどうかを決める。
        /// ここで早期 return するほど無駄な GPU 処理が減る。
        /// </summary>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_material == null || _pass == null)
            {
                return;
            }

            // シーンビューのマテリアルプレビューや反射プローブ用カメラには掛けない
            CameraType cameraType = renderingData.cameraData.cameraType;
            if (cameraType == CameraType.Preview || cameraType == CameraType.Reflection)
            {
                return;
            }

            // カメラの「Post Processing」チェックが OFF ならポストエフェクトは掛けない
            // （URP 標準エフェクトと挙動を揃える）
            if (!renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            // VolumeManager.instance.stack には「このカメラから見た」ブレンド済みの値が入っている。
            // intensity が 0 ならパス自体を積まない（全画面 Blit 1回分のコスト削減）。
            ScanLineVolume volume = VolumeManager.instance.stack.GetComponent<ScanLineVolume>();
            if (volume == null || !volume.IsActive())
            {
                return;
            }

            renderer.EnqueuePass(_pass);
        }

        /// <summary>
        /// フィーチャー破棄時の後始末。
        /// CreateEngineMaterial で作ったマテリアルは自動では消えないので、必ず破棄する（リーク防止）。
        /// CoreUtils.Destroy はエディター/再生中で Destroy と DestroyImmediate を使い分けてくれる。
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_material);
            _material = null;
            _pass = null;
        }

        // ==================================================================
        // 描画パス本体
        // ==================================================================

        /// <summary>
        /// 走査線を描画する Render Graph パス
        /// </summary>
        private sealed class ScanLinePass : ScriptableRenderPass
        {
            // Shader.PropertyToID は文字列ハッシュ計算を伴うので、static readonly でキャッシュする。
            // SetFloat("_Intensity", ...) のように毎フレーム文字列で渡すのは避けるのが定石。
            private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
            private static readonly int OpacityScanLineId = Shader.PropertyToID("_OpacityScanLine");
            private static readonly int OpacityNoiseId = Shader.PropertyToID("_OpacityNoise");
            private static readonly int FlickeringSpeedId = Shader.PropertyToID("_FlickeringSpeed");
            private static readonly int FlickeringStrengthId = Shader.PropertyToID("_FlickeringStrength");

            private const string PassName = "ScanLine";

            private readonly Material _material;

            public ScanLinePass(Material material)
            {
                _material = material;

                // Frame Debugger 等での表示名
                profilingSampler = new ProfilingSampler(PassName);

                // 「カメラ色を読み取る」パスなので、中間テクスチャ（オフスクリーン RT）が必須。
                // これを true にしておくと、URP がバックバッファへ直接描く最適化を抑止してくれる。
                // （バックバッファは読み取れないため、false のままだと環境によって何も起きない）
                requiresIntermediateTexture = true;
            }

            /// <summary>
            /// Render Graph にパスを登録する（Unity 6 以降の描画のエントリーポイント）。
            /// </summary>
            /// <param name="renderGraph">パスを登録する先のグラフ</param>
            /// <param name="frameData">カメラ情報やテクスチャなど、フレームのデータ一式</param>
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                // UniversalResourceData … URP が管理するテクスチャ（カメラ色・深度など）のハンドル群
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                // 念のための安全策：バックバッファは読めないので何もしない
                if (resourceData.isActiveTargetBackBuffer)
                {
                    return;
                }

                // ---- 1. Volume の値をマテリアルへ反映 ----
                ScanLineVolume volume = VolumeManager.instance.stack.GetComponent<ScanLineVolume>();
                _material.SetFloat(IntensityId, volume.intensity.value);
                _material.SetFloat(OpacityScanLineId, volume.opacityScanLine.value);
                _material.SetFloat(OpacityNoiseId, volume.opacityNoise.value);
                _material.SetFloat(FlickeringSpeedId, volume.flickeringSpeed.value);
                _material.SetFloat(FlickeringStrengthId, volume.flickeringStrength.value);

                // ---- 2. 入力（現在のカメラ色）と出力先テクスチャを用意 ----
                TextureHandle source = resourceData.activeColorTexture;

                // 入力と同じ設定（解像度・フォーマット・MSAA 等）で出力先を作る。
                // Render Graph が作るテクスチャは「グラフ内だけで使う一時テクスチャ」で、
                // メモリは自動で確保・再利用・解放される（RTHandle の自前管理が不要）。
                TextureDesc destinationDesc = renderGraph.GetTextureDesc(source);
                destinationDesc.name = $"CameraColor-{PassName}";
                destinationDesc.clearBuffer = false; // 全画面を上書きするのでクリア不要
                TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

                // ---- 3. Blit パスを登録 ----
                // 【なぜ source → source に直接書かないのか】
                // 同じテクスチャを「読みながら書く」ことは GPU では基本的にできない。
                // そこで別テクスチャに書き出し、次の手順でカメラ色を差し替える。
                var blitParams = new RenderGraphUtils.BlitMaterialParameters(source, destination, _material, 0);
                renderGraph.AddBlitPass(blitParams, passName: PassName);

                // ---- 4. 以降のパスが参照するカメラ色を、今描いたテクスチャに差し替える ----
                // これにより「元に戻すための2回目の Blit」が不要になる（Unity 6 で推奨される書き方）。
                // 後続のパス（他のエフェクトや最終出力）は自動的に destination を読むようになる。
                resourceData.cameraColor = destination;
            }
        }
    }
}
