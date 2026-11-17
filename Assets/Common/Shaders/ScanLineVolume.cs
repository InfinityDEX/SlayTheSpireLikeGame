using UnityEngine.Rendering;

namespace VolumeScripts.ScanLineShader
{
    /// <summary>
    /// 走査線シェーダーボリューム
    /// </summary>
    public class ScanLineVolume : VolumeComponent
    {
        public ClampedFloatParameter opacityScanLine = new ClampedFloatParameter(0.8f, 0f, 2f);
        public ClampedFloatParameter opacityNoise = new ClampedFloatParameter(0.1f, 0f, 1f);
        public ClampedFloatParameter flickeringSpeed = new ClampedFloatParameter(600f, 0f, 1000f);
        public ClampedFloatParameter flickeringStrength = new ClampedFloatParameter(0.02f, 0f, 0.1f);

        public bool IsActive => 
            opacityScanLine.value > 0f && 
            opacityNoise.value > 0f &&
            flickeringSpeed.value > 0f && 
            flickeringStrength.value > 0f;
    }
}