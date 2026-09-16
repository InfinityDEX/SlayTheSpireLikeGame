using UnityEngine;

// UnityのUI（Button）で画面遷移したい場合にGameObjectに本コンポーネントを
// アタッチしてシーン移動のメソッドを呼び出す。
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
