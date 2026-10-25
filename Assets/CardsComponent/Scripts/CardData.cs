using System.Collections.Generic;
using CardEffect;
using CardEffect.CardDataJson;
using UnityEngine;

/// <summary>
/// カードデータクラス
/// </summary>
public class CardData
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
        NonTarget, // ターゲット無し
        OneEnemy, // 単一エネミー
        AllEnemy, // 全エネミー
        OnePlayer, // 単一プレイヤー
        AllPlayer, // 全プレイヤー
    }

    /// <summary>
    /// カードID
    /// </summary>
    public int CardID { get; set; }

    /// <summary>
    /// コスト
    /// </summary>
    public int Cost { get; set; }

    /// <summary>
    /// カード名
    /// </summary>
    public string CardName { get; set; }

    /// <summary>
    /// カード種別
    /// </summary>
    public CardType Type { get; set; }

    /// <summary>
    /// カードに表示する画像
    /// </summary>
    public Sprite CardImage { get; set; }

    /// <summary>
    /// カード効果処理時に描画するビジュアルエフェクトのID
    /// </summary>
    public int VisualEffectID { get; set; } = -1;

    /// <summary>
    /// カード使用時に再生する音声データ
    /// </summary>
    public AudioClip CastCardSE { get; set; }

    /// <summary>
    /// カードの説明
    /// </summary>
    public string CardDescription { get; set; }

    /// <summary>
    /// カード効果の対象
    /// </summary>
    public Target TargetType { get; set; }

    /// <summary>
    /// 強化先カード
    /// </summary>
    public int UpgradedCardId { get; set; }

    /// <summary>
    /// ショップでの販売価格
    /// </summary>
    public int SalesPrice { get; set; }

    /// <summary>
    /// 強化済みカードか？
    /// </summary>
    public bool IsUpgraded { get; set; }

    /// <summary>
    /// カード効果のリスト（カードの効果は先頭に登録されている物から再生）
    /// </summary>
    public List<CardActionComponent> CardActions { get; set; } = new();

    /// <summary>
    /// CardDataEntityをカードデータに登録(CardDataクラスに合わせてコンバート)する
    /// </summary>
    /// <param name="cardDataEntity">渡すCardDataEntityインスタンス</param>
    public void SetCardDataJsonEntity(CardDataEntity cardDataEntity)
    {
        CardID = cardDataEntity.CardId;
        Cost = cardDataEntity.CardCost;
        CardName = cardDataEntity.CardName;
        Type = (CardData.CardType)cardDataEntity.CardType;
        CardImage = Resources.Load<Sprite>(cardDataEntity.CardImagePath);
        VisualEffectID = cardDataEntity.VisualEffectId;
        CastCardSE = Resources.Load<AudioClip>(cardDataEntity.CardCastSoundPath);
        CardDescription = cardDataEntity.CardDescription;
        SalesPrice = cardDataEntity.SalesPrice;
        TargetType = (CardData.Target)cardDataEntity.CardTarget;
        UpgradedCardId = cardDataEntity.UpgradedCardId;
        IsUpgraded = cardDataEntity.IsUpgraded;
    }

    /// <summary>
    /// カード効果の実行
    /// </summary>
    /// <param name="target">効果のターゲット</param>
    public void Play(Creature target)
    {
        foreach (var cardActionModule in CardActions)
        {
            cardActionModule.Execute(target);
        }
    }
}