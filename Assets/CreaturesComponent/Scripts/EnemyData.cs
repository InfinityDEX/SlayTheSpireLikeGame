using UnityEngine;

/// <summary>
/// エネミーデータクラス
/// 
/// ボスかノーマルかにかかわらず全てのエネミーのデータを
/// このクラスで定義する
/// </summary>
[CreateAssetMenu(fileName = "Enemy Data", menuName = "Create Creature Data/Enemy Data")]
public class EnemyData : CreatureData
{
    [field:SerializeField, Header("攻撃力")]
    public int attackPower;
    
    [field:SerializeField, Header("ゴールド")]
    public int gold;
}
