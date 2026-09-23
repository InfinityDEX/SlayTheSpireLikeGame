using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミーマネージャ
/// 
/// 現在のバトルに参加している全てのエネミーを管理する
/// </summary>
public class EnemyManager : MonoBehaviour
{
    [field:SerializeField, Header("現在のバトルに参加しているエネミーリスト")]
    public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

    /// <summary>
    /// エネミーの追加
    /// </summary>
    /// <param name="enemyCombinationData">エネミーコンビネーションデータ（バトルに参加させる全エネミーの情報）</param>
    public void SetEnemy(EnemyCombinationData enemyCombinationData)
    {
        foreach (var enemyData in enemyCombinationData.enemyCombinations)
        {
            var enemy = Instantiate(enemyData.enemyPrefab, this.transform);
            enemy.transform.position = enemyData.spawnPosition;
            Enemies.Add(enemy);
        }
    }
}
