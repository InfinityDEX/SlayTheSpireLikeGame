using UnityEngine;
using VContainer;

public class SoundTrigger : MonoBehaviour
{
    [field: SerializeField, Header("再生したい音声ソース")]
    private AudioClip audioClip;

    [Inject]
    private AudioController audioController;

    public void Play()
    {
        if (audioController != null && audioClip != null)
        {
            audioController.PlaySE(audioClip);
        }
    }
}
