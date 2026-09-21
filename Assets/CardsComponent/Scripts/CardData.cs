using UnityEngine;

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(fileName = "CardData", menuName = "Create Card Data")]
public class CardData : ScriptableObject
{
    /// <summary>
    /// カード種別
    /// </summary>
    public enum CardType
    {
        Attack, // 攻撃
        Skill, // スキル
        Power, // パワー
    }

    /// <summary>
    /// カード効果対象種別
    /// </summary>
    public enum Target
    {
        Enemy, // 敵
        Player, // プレイヤー
    }

    [Header("カード効果ID")]
    public int cardEffectId;

    [Header("カードの名前")]
    public string cardName;

    [Header("カードの画像")]
    public Sprite cardImage;

    [Header("使用時のエフェクトのID")]
    public int visualEffectID = -1;
    
    [Header("カード使用時のSE")]
    public AudioClip castCardSE;

    [Header("コスト")]
    public int cost;
    
    [Header("カードの種類")]
    public CardType type;
    
    [Header("カードの説明")]
    public string cardDescription;
    
    [Header("カード効果の対象")]
    public Target target;
    
    [Header("強化先カード")]
    public CardData upgradedCard;
}