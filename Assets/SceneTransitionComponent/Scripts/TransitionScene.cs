using UnityEngine;

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
        // タイトルシーンに移行する際に、セーブデータインスタンスを破棄する
        if (SaveDataHolder.Instance != null) Destroy(SaveDataHolder.Instance);

        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    /// <summary>
    /// マップシーンに移動
    /// </summary>
    public void GoToMapScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MapScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
