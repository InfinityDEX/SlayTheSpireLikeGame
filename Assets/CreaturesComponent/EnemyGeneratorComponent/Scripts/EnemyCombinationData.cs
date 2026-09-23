using UnityEngine;

// ステージに生成する敵キャラの組み合わせデータ

[System.Serializable]
public class EnemySpawnData
{
    public Enemy enemyPrefab; // 敵キャラのプレハブ
    public Vector3 spawnPosition;  // 生成位置

    public EnemySpawnData(Enemy enemyPrefab, Vector3 spawnPosition)
    {
        this.enemyPrefab = enemyPrefab;
        this.spawnPosition = spawnPosition;
    }
}

[CreateAssetMenu(fileName = "EnemyCombinationData", menuName = "Game/EnemyCombinationData", order = 1)]
public class EnemyCombinationData : ScriptableObject
{
    public EnemySpawnData[] enemyCombinations;
}