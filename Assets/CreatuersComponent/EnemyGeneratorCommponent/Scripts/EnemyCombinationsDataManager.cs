using UnityEngine;

public class EnemyCombinationsDataManager : MonoBehaviour
{
    public static EnemyCombinationsDataManager Instance { get; private set; }
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

    public void SetData(EnemyCombinationData data)
    {
        enemyCombinationData = data;
    }

    public EnemyCombinationData GetData()
    {
        // データを明け渡したら消去する。
        Destroy(gameObject);
        return enemyCombinationData;
    }
}
