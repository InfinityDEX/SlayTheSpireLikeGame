using UnityEngine;

[CreateAssetMenu(fileName = "PotionData", menuName = "Potion Data")]
public class PotionData : ScriptableObject
{
    public string potionName; // ポーションの名前
    public string potionDescription; // ポーションの説明
    public int potionValue; // ポーションの価値
}