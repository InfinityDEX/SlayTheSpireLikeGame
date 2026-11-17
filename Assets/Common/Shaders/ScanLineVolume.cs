using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VolumeScripts.ScanLineShader
{
    /// <summary>
    /// 走査線ポストエフェクトのパラメーター（Volume に追加して使う）
    /// </summary>
    /// <remarks>
    /// 【VolumeComponent の役割】
    /// ・「値の入れ物」だけを担当し、描画処理は持たない（描画は RendererFeature 側）。
    /// ・シーン内の Volume（Global / Local）の値を VolumeManager が自動でブレンドし、
    ///   VolumeManager.instance.stack.GetComponent&lt;ScanLineVolume&gt;() で最終値を取得できる。
    ///
    /// 【ベストプラクティス：デフォルト値は「エフェクト OFF」にする】
    /// Volume に Override が無いカメラ・シーンでもデフォルト値が使われるため、
    /// デフォルトで効いてしまうと「Volume を置いていないのにエフェクトがかかる」状態になる。
    /// そこで intensity のデフォルトを 0 にし、Volume 側で intensity を上げた時だけ有効にする。
    /// （URP 標準の Bloom や Vignette も同じ考え方で作られている）
    /// </remarks>
    // Volume の「Add Override」メニューに表示される名前
    [Serializable, VolumeComponentMenu("Custom/ScanLine")]
    // URP 以外のパイプラインでは Add Override に出さない
    [SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
    public sealed class ScanLineVolume : VolumeComponent
    {
        /// <summary>エフェクト全体の強さ。0 で OFF。</summary>
        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        /// <summary>走査線の濃さ</summary>
        public ClampedFloatParameter opacityScanLine = new ClampedFloatParameter(0.8f, 0f, 2f);

        /// <summary>ノイズの濃さ</summary>
        public ClampedFloatParameter opacityNoise = new ClampedFloatParameter(0.1f, 0f, 1f);

        /// <summary>明滅の速さ</summary>
        public ClampedFloatParameter flickeringSpeed = new ClampedFloatParameter(600f, 0f, 1000f);

        /// <summary>明滅の強さ</summary>
        public ClampedFloatParameter flickeringStrength = new ClampedFloatParameter(0.02f, 0f, 0.1f);

        /// <summary>
        /// エフェクトを描画する必要があるか。
        /// false の時はパス自体を積まないことで、GPU コスト（全画面 Blit 1回分）をゼロにする。
        /// </summary>
        /// <remarks>
        /// 旧実装は各パラメーターを && で繋いでいたため、
        /// 例えばノイズだけ 0 にするとエフェクト全体が消えてしまっていた。
        /// 「ON/OFF の判定は intensity 一本に絞る」のが分かりやすく安全。
        /// </remarks>
        public bool IsActive() => active && intensity.value > 0f;
    }
}
