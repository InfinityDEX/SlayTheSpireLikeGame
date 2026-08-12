using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("敵")]
    [SerializeField]
    private List<Enemy> enemysInspector = new List<Enemy>();

    public List<Enemy> enemies { get; private set; }

    private void Awake()
    {
        enemies = enemysInspector;
    }

    public void SetEnemy(EnemyCombinationData data)
    {
        foreach (var enemyData in data.enemyCombinations)
        {
            var enemy = Instantiate(enemyData.enemyPrefab, this.transform);
            enemy.transform.position = enemyData.spawnPosition;
            enemies.Add(enemy);
        }
    }
}
