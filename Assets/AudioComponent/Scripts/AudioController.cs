using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

/// <summary>
/// オーディオコントローラークラス
///
/// 音系の操作を管理する。
/// シングルトン。
/// </summary>
public class AudioController : MonoBehaviour
{
    [SerializeField, Header("マスター音量")]
    [Range(0f, 1f)]
    private float masterVolume = 1f;

    [SerializeField, Header("BGM音量")]
    [Range(0f, 1f)]
    private float bgmVolume = 0.7f;

    [SerializeField, Header("SE音量")]
    [Range(0f, 1f)]
    private float seVolume = 1f;

    [SerializeField, Header("Wwise Sound Bankリスト")]
    private List<AK.Wwise.Bank> banks;

    [Inject]
    public void Init()
    {
        foreach (var bank in banks)
        {
            var akBank = gameObject.AddComponent<AkBank>();
            akBank.data = bank;
            Debug.Log($"Bank Loading: {bank.Name}");
            akBank.data.Load();
            Debug.Log($"Bank Loaded: {bank.Name}");
        }
    }

    private void Awake()
    {
        ApplyVolume();
    }

    /// <summary>
    /// WwiseのEventをトリガー(発火)させる
    /// </summary>
    /// <param name="e">ワイズイベント</param>
    /// <returns></returns>
    public bool FireWwiseEvent(AK.Wwise.Event e)
    {
        try
        {
            e.Post(gameObject);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return false;
        }
    }

    /// <summary>
    /// BGMを停止する
    /// </summary>
    public void StopBGM(){
        // bgmSource.Stop();
    }

    /// <summary>
    /// SEの再生
    /// </summary>
    /// <param name="clip">再生したい音源</param>
    public void PlaySE(AudioClip clip)
    {
        // 音源がnullだった場合再生しない
        if (clip == null) return;
        // seSource.PlayOneShot(clip);
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
        // bgmSource.volume = bgmVolume * masterVolume;
        // seSource.volume = seVolume * masterVolume;
    }
}