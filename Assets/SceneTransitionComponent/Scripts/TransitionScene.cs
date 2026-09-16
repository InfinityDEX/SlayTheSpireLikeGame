using UnityEngine;

public class TransitionScene : MonoBehaviour
{
    public void GoToTitleScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void GoToMapScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
