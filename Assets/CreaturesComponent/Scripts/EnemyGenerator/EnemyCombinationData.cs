using UnityEngine;

/// <summary>
/// エネミースポーンデータ
/// </summary>
[System.Serializable]
public class EnemySpawnData
{
    /// <summary>
    /// 敵キャラのプレハブ
    /// </summary>
    public Enemy enemyPrefab; 

    /// <summary>
    /// 生成位置
    /// </summary>
    public Vector3 spawnPosition;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="enemyPrefab">エネミープレハブ</param>
    /// <param name="spawnPosition">生成位置</param>
    public EnemySpawnData(Enemy enemyPrefab, Vector3 spawnPosition)
    {
        this.enemyPrefab = enemyPrefab;
        this.spawnPosition = spawnPosition;
    }
}

/// <summary>
/// ステージに生成するエネミーの組み合わせデータクラス
/// </summary>
[CreateAssetMenu(fileName = "EnemyCombinationData", menuName = "Game/EnemyCombinationData", order = 1)]
public class EnemyCombinationData : ScriptableObject
{
    /// <summary>
    /// スポーンさせたいエネミーのリスト
    /// </summary>
    public EnemySpawnData[] enemyCombinations;
}