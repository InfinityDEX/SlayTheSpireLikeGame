using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCombinationsDataGenerator : MonoBehaviour
{
    public enum EnemyType
    {
        Enemy,
        EliteEnemy,
        Boss
    }


    public void GenerateData(EnemyType enemyType, StageData stageData)
    {
        var go = new GameObject();
        go.AddComponent<EnemyCombinationsDataManager>();
        List<EnemyCombinationData> enemyCombinationDatas = null;
        switch (enemyType)
        {
            case EnemyType.Enemy:
                enemyCombinationDatas = stageData.normalEnemies;
                break;
            case EnemyType.EliteEnemy:
                enemyCombinationDatas = stageData.eliteEnemies;
                break;
            case EnemyType.Boss:
                enemyCombinationDatas = new List<EnemyCombinationData> { stageData.bossEnemy }; // コード簡略化の為、stageData.bossEnemyもList化して以降の処理を共通化する。
                break;
        }
   
        var selectedCombination = enemyCombinationDatas[Random.Range(0, enemyCombinationDatas.Count)];
        go.GetComponent<EnemyCombinationsDataManager>().SetData(selectedCombination);
    }
}
