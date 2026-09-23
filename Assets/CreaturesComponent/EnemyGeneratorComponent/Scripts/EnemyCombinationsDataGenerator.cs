using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミーの組み合わせデータジェネレータクラス
/// 
/// 指定したエネミー種別に応じて、
/// 対応するエネミー組み合わせデータからランダムにピックアップし、
/// 上記の組み合わせデータを管理するEnemyCombinationDataManagerの実体を生成する。
/// （バトルシーン移行後にこのマネージャの持つデータを基にエネミーを生成する）
/// </summary>
public class EnemyCombinationsDataGenerator : MonoBehaviour
{
    public enum EnemyType
    {
        Enemy,
        EliteEnemy,
        Boss
    }

    /// <summary>
    /// エネミー組み合わせデータマネージャの生成
    /// </summary>
    /// <param name="enemyType">エネミーの種別</param>
    /// <param name="stageData">ステージデータ</param>
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
