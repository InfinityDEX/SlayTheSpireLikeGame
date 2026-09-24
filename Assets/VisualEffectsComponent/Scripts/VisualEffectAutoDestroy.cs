using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// 視覚エフェクトの自動削除クラス
/// 
/// 視覚エフェクトの親にこのクラスをInspectorで登録して、
/// さらにこのエフェクトを構成するパーティクルエフェクトやVFXを全て登録することで、
/// それが全て再生終了したかどうかを自動チェックし、
/// 全てのエフェクト効果が再生終了したら、このクラスが登録されているインスタンスを削除する
/// </summary>
public class VisualEffectAutoDestroy : MonoBehaviour
{
    [field:SerializeField, Header("エフェクト本体(ParticleSystem)")]
    private List<ParticleSystem> particleEffects;

    [field:SerializeField, Header("エフェクト本体(VFX Graph)")]
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