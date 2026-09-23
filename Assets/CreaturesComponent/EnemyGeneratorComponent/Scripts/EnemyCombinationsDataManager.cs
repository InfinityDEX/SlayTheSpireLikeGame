using UnityEngine;

/// <summary>
/// エネミー組み合わせデータ管理クラス
/// 
/// このクラスが持つエネミー組み合わせデータを基に
/// バトルシーンにエネミー群を生成する。
/// シングルトン
/// </summary>
public class EnemyCombinationsDataManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static EnemyCombinationsDataManager Instance { get; private set; }

    /// <summary>
    /// エネミー組み合わせデータ
    /// </summary>
    private EnemyCombinationData enemyCombinationData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// エネミー組み合わせデータのセット
    /// </summary>
    /// <param name="data">セットするデータ</param>
    public void SetData(EnemyCombinationData data)
    {
        enemyCombinationData = data;
    }


    /// <summary>
    /// エネミー組み合わせデータの取得
    /// </summary>
    /// <returns>エネミー組み合わせデータ</returns>
    public EnemyCombinationData GetData()
    {
        // データを明け渡したらこのインスタンスを消去する。
        Destroy(gameObject);
        return enemyCombinationData;
    }
}
