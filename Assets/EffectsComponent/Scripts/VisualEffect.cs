using UnityEngine;

public class VisualEffect : MonoBehaviour
{
    [Header("エフェクト本体")]
    [SerializeField]
    private ParticleSystem particleEffect;

    private void Update()
    {
        if (particleEffect != null)
            if (!particleEffect.isPlaying)
                Destroy(gameObject);
    }
}