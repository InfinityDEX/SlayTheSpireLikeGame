using System.Collections;
using System.Collections.Generic;
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

    public void SetEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
}
