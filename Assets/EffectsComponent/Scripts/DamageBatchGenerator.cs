using System.IO;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Timeline;

public class DamageBatchGenerator : MonoBehaviour
{
    [Header("ダメージバッチの位置を以下の範囲でランダムにズラす")]
    [SerializeField]
    private Vector2 randomizeRange = new Vector2(0.5f, 0.5f);
    
    [Header("ダメージバッチ(Prefab)")]
    [SerializeField]
    private DamageBatch damageBatchPrefab;
    

    public void GenerateDamageBatch(Vector2 pos, int damage)
    {
        // randomizeRangeの範囲でランダムに座標をずらす
        Vector2 randomizedPos = pos + new Vector2(
            UnityEngine.Random.Range(-randomizeRange.x, randomizeRange.x),
            UnityEngine.Random.Range(-randomizeRange.y, randomizeRange.y)
        );
        var damageBatch = Instantiate(damageBatchPrefab);
        damageBatch.transform.parent = transform;
        damageBatch.transform.position = randomizedPos;
        damageBatch.SetDamage(damage);
    }
}
