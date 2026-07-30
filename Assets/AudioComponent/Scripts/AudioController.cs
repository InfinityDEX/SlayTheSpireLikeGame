using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; } // シングルトン

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float bgmVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float seVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyVolume();
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        // 音源がnullだった場合再生しない
        if (clip == null) return;

        // 再生している音源と同じ音源を再生しようとしたか、前のBGMがまだ再生中の場合、再生しない。
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();

    public void PlaySE(AudioClip clip)
    {
        // 音源がnullだった場合再生しない
        if (clip == null) return;
        seSource.PlayOneShot(clip);
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        ApplyVolume(); // AudioSourceのボリュームを再計算
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        ApplyVolume(); // AudioSourceのボリュームを再計算
    }

    public void SetSEVolume(float volume)
    {
        seVolume = volume;
        ApplyVolume(); // AudioSourceのボリュームを再計算
    }

    private void ApplyVolume()
    {
        bgmSource.volume = bgmVolume * masterVolume;
        seSource.volume = seVolume * masterVolume;
    }
}