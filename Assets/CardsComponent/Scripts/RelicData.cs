using UnityEngine;

[CreateAssetMenu(fileName = "RelicData", menuName = "Relic Data")]
public class RelicData : ScriptableObject
{
    public string relicName; // 遺物の名前
    public string relicDescription; // 遺物の説明
    public int relicValue; // 遺物の価値
}