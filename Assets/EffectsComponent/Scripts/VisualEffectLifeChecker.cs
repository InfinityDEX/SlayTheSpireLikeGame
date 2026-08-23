using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class VisualEffectLifeChecker : MonoBehaviour
{
    [Header("エフェクト本体(ParticleSystem)")]
    [SerializeField]
    private List<ParticleSystem> particleEffects;

    [Header("エフェクト本体(VFX Graph)")]
    [SerializeField]
    private List<VisualEffect> visualEffects;

    private void Update()
    {
        // 全てのエフェクトが止まったらエフェクト本体ごと削除する
        bool particleEffectsDone = particleEffects.Count == 0 || particleEffects.All(effect => !effect.isPlaying);
        bool visualEffectsDone = visualEffects.Count == 0 || visualEffects.All(effect => effect.aliveParticleCount == 0);
        if (particleEffectsDone && visualEffectsDone)
   
            Destroy(gameObject);
    }
}