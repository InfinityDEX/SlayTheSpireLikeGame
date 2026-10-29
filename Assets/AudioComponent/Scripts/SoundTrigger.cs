using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [field: SerializeField, Header("再生したい音声ソース")]
    private AudioClip audioClip;

    public void Play()
    {
        var audioController = AudioController.Instance;
        if (audioController != null && audioClip != null)
        {
            audioController.PlaySE(audioClip);
        }
    }
}
