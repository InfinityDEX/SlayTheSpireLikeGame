using UnityEngine;

// 生物データ
// [CreateAssetMenu(fileName = "CreatureData", menuName = "Creature Data")]
public class CreatureData : ScriptableObject
{
    [Header("名前")]
    public string creatureName; // 名前
    [Header("最大体力")]
    public int maxHealth; // 最大体力
    [Header("生物の見た目")]
    public Sprite creatureSprite; // 生物の見た目
}
