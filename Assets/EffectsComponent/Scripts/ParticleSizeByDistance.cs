using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSizeByDistance : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.Particle[] m_Particles;

    // インスペクターから調整用：最大サイズと最小サイズ
    public float minSize = 0.1f;
    public float maxSize = 1.0f;

    void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        InitializeParticlesArray();

        // 現在生存しているパーティクルを取得
        int numParticlesAlive = m_ParticleSystem.GetParticles(m_Particles);
        
        // Shapeモジュールの最大半径を取得（基準値として使用）
        float maxDistance = m_ParticleSystem.shape.radius;

        for (int i = 0; i < numParticlesAlive; i++)
        {
            // 生成直後のパーティクル（残り寿命＝最大寿命）のみサイズを計算
            if (Mathf.Approximately(m_Particles[i].remainingLifetime, m_Particles[i].startLifetime))
            {
                // ローカル座標系での中心からの距離を計算
                float distance = m_Particles[i].position.magnitude;
                
                // 距離の割合（0～1）を計算
                float t = Mathf.Clamp01(distance / maxDistance);

                // 遠いほど小さくなるように線形補間（t=1のときminSize、t=0のときmaxSize）
                float targetSize = Mathf.Lerp(maxSize, minSize, t);

                // パーティクルの初期サイズを上書き
                m_Particles[i].startSize = targetSize;
            }
        }

        // 変更したパーティクルデータをシステムに再適用
        m_ParticleSystem.SetParticles(m_Particles, numParticlesAlive);
    }

    private void InitializeParticlesArray()
    {
        if (m_Particles == null || m_Particles.Length < m_ParticleSystem.main.maxParticles)
        {
            m_Particles = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
        }
    }
}
