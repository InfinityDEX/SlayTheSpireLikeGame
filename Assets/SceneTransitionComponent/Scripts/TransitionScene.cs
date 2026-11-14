using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン移動クラス
///
/// UnityのUI（Button）で画面遷移したい場合にGameObjectに本コンポーネントを
/// アタッチしてシーン移動のメソッドを呼び出す。
/// </summary>
public class TransitionScene : MonoBehaviour
{
    /// <summary>
    /// タイトルシーンに移動
    /// </summary>
    public void GoToTitleScene()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }

    /// <summary>
    /// マップシーンに移動
    /// </summary>
    public void GoToMapScene()
    {
        SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }
}
