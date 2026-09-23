using UnityEngine;

/// <summary>
///  生物データ定義クラス
/// 
/// 派生先のEnemyDataとPlayerDataのどちらかデデータを作ってほしいため、
/// CreateAssetMenuは用意しない（Unity上で「CreatureData」のScriptableObjectは生成できない）
/// </summary>
public class CreatureData : ScriptableObject
{
    [field:SerializeField, Header("名前")]
    public string creatureName;
    
    [field:SerializeField, Header("最大体力")]
    public int maxHealth;

    [field:SerializeField, Header("生物の見た目")]
    public Sprite creatureSprite;
}
