using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Create Creature Data/Enemy Data")]
public class EnemyData : CreatureData
{
    [Header("攻撃力")]
    public int attackPower; // 攻撃力
    [Header("経験値")]
    public int experience; // 経験値
    [Header("ゴールド")]
    public int gold; // ゴールド
    // [Header("ドロップ率(%)")]
    // public int dropRate; // ドロップ率(%)
}
