using UnityEngine;

/// <summary>
/// オーディオコントローラークラス
/// 
/// 音系の操作を管理する。
/// シングルトン。
/// </summary>
public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; } // シングルトン

    [SerializeField, Header("BGM用AudioSource")]
    private AudioSource bgmSource;
    
    [SerializeField, Header("SE用AudioSource")]
    private AudioSource seSource;
    
    [SerializeField, Header("マスター音量")]
    [Range(0f, 1f)]
    private float masterVolume = 1f;
    
    [SerializeField, Header("BGM音量")]
    [Range(0f, 1f)]
    private float bgmVolume = 0.7f;
    
    [SerializeField, Header("SE音量")]
    [Range(0f, 1f)]
    private float seVolume = 1f;

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

    /// <summary>
    /// BGMの再生処理
    /// </summary>
    /// <param name="clip">再生したい音源</param>
    /// <param name="loop">ループするか？</param>
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

    /// <summary>
    /// BGMを停止する
    /// </summary>
    public void StopBGM() => bgmSource.Stop();

    /// <summary>
    /// SEの再生
    /// </summary>
    /// <param name="clip">再生したい音源</param>
    public void PlaySE(AudioClip clip)
    {
        // 音源がnullだった場合再生しない
        if (clip == null) return;
        seSource.PlayOneShot(clip);
    }

    /// <summary>
    /// マスター音量の設定
    /// </summary>
    /// <param name="value">設定するボリューム</param>
    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        ApplyVolume(); // AudioSourceのボリュームを再計算
    }

    /// <summary>
    ///BGM音量の設定
    /// </summary>
    /// <param name="volume">設定するボリューム</param>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        ApplyVolume(); // AudioSourceのボリュームを再計算
    }

    /// <summary>
    /// SE音源の設定
    /// </summary>
    /// <param name="volume">設定するボリューム</param>
    public void SetSEVolume(float volume)
    {
        seVolume = volume;
        ApplyVolume(); // AudioSourceのボリュームを再計算
    }

    /// <summary>
    /// 音量の適用
    /// 
    /// マスター音量とBGM・SE音量の値を計算し、
    /// 実際のAudioSourceに適用する。
    /// </summary>
    private void ApplyVolume()
    {
        bgmSource.volume = bgmVolume * masterVolume;
        seSource.volume = seVolume * masterVolume;
    }
}