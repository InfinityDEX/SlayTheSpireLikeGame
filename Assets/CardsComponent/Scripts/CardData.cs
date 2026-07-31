using UnityEngine;

public enum CardType
{
    Attack, // 攻撃
    Skill, // スキル
    Power, // パワー
}
public enum Target
{
    Enemy, // 敵
    Player, // プレイヤー
}
[CreateAssetMenu(fileName = "CardData", menuName = "Create Card Data")]
public class CardData : ScriptableObject
{
    [Header("カード効果ID")]
    public int cardEffectId; // カード効果ID
    [Header("カードの名前")]
    public string cardName; // カードの名前
    [Header("カードの画像")]
    public Sprite cardImage; // カードの画像
    [Header("カード使用時のSE")]
    public AudioClip castCardSE; // カードの画像
    [Header("コスト")]
    public int cost; // コスト  
    [Header("カードの種類")]
    public CardType type; // カードの種類
    [Header("カードの説明")]
    public string cardDescription; // カードの説明
    [Header("カード効果の対象")]
    public Target target; // カード効果の対象
    [Header("強化先カード")]
    public CardData upgradedCard; // このカードを1回強化したときの強化先カード
}