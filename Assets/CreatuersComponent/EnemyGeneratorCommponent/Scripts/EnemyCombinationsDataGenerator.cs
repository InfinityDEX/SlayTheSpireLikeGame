using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCombinationsDataGenerator : MonoBehaviour
{
    [Header("敵生成情報リスト(ゲーム開始時にランダムに選ばれる)")]
    [SerializeField]
    private List<EnemyCombinationData> enemyCombinationDatas;
 
    public void GenerateData()
    {
        var go = new GameObject();
        go.AddComponent<EnemyCombinationsDataManager>();
        var selectedCombination = enemyCombinationDatas[Random.Range(0, enemyCombinationDatas.Count)];
        go.GetComponent<EnemyCombinationsDataManager>().SetData(selectedCombination);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
    }
}
