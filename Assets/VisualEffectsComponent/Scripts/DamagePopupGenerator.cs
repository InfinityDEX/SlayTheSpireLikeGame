using UnityEngine;

/// <summary>
/// ダメージポップアップマネージャクラス
/// 
/// ダメージポップアップを指定した場所に生成する
/// </summary>
public class DamagePopupGenerator : MonoBehaviour
{
    [field:SerializeField, Header("ダメージポップアップの位置を以下の範囲でランダムにズラす")]
    private Vector2 randomizeRange = new Vector2(0.5f, 0.5f);
    
    [field:SerializeField, Header("ダメージポップアップ(Prefab)")]
    private DamagePopup damagePopupPrefab;
    

    public void GenerateDamagePopup(Vector2 pos, int damage)
    {
        // randomizeRangeの範囲でランダムに座標をずらす
        Vector2 randomizedPos = pos + new Vector2(
            UnityEngine.Random.Range(-randomizeRange.x, randomizeRange.x),
            UnityEngine.Random.Range(-randomizeRange.y, randomizeRange.y)
        );
        var damagePopup = Instantiate(damagePopupPrefab);
        damagePopup.transform.parent = transform;
        damagePopup.transform.position = randomizedPos;
        damagePopup.SetDamage(damage);
    }
}
