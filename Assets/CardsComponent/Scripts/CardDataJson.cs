using System.Collections.Generic;
using OneOf;

namespace CardEffect.CardDataJson
{
    /// <summary>
    /// カードデータ本体(カードリスト)
    /// </summary>
    [System.Serializable]
    public class CardDataListEntity
    {
        // カードデータリスト
        public List<CardDataEntity> CardDatas;
    }

    /// <summary>
    /// 単一カードのデータ
    /// </summary>
    [System.Serializable]
    public class CardDataEntity
    {
        /// <summary>
        /// カードのID
        /// </summary>
        public int CardId;

        /// <summary>
        /// カードのコスト
        /// </summary>
        public int CardCost;

        /// <summary>
        /// カード名
        /// </summary>
        public string CardName;

        /// <summary>
        /// カードタイプ（Attack, Spell, Power）
        /// </summary>
        public int CardType;

        /// <summary>
        /// カードに表示する画像のパス(Resourcesディレクトリからの相対パス)
        /// </summary>
        public string CardImagePath;

        /// <summary>
        /// カード使用時に再生する音声データのパス(Resourcesディレクトリからの相対パス)
        /// </summary>
        public string CardCastSoundPath;

        /// <summary>
        /// カードの効果説明
        /// </summary>
        public string CardDescription;

        /// <summary>
        /// カードの効果対象
        /// </summary>
        public int CardTarget;

        /// <summary>
        /// カード処理時に描画するビジュアルエフェクトのID
        /// </summary>
        public int VisualEffectId;

        /// <summary>
        /// 強化先カードのID
        /// </summary>
        public int UpgradedCardId;

        /// <summary>
        /// 強化済みのカードか？
        /// </summary>
        public bool IsUpgraded;

        /// <summary>
        /// カードの効果リスト(処理順で格納)
        /// </summary>
        public List<CardActionEntity> CardActionsData;
    }

    /// <summary>
    /// カード効果処理モジュール(CardActionModule)の構造データクラス
    /// </summary>
    [System.Serializable]
    public class CardActionEntity
    {
        /// <summary>
        /// 識別ID
        /// </summary>
        public int CardActionId;

        /// <summary>
        /// 効果処理に使用する引数
        /// </summary>
        public List<OneOf<CardActionEntity, CardExpressionEntity>> Args;
    }

    /// <summary>
    /// カード効果処理で使用する式(IValueExpression)の構造データクラス
    /// </summary>
    [System.Serializable]
    public class CardExpressionEntity
    {
        /// <summary>
        /// 識別ID
        /// </summary>
        public int ExpressionId;
        /// <summary>
        /// 計算に使用する引数
        /// </summary>
        public List<OneOf<float, string, CardActionEntity, CardExpressionEntity>> args;
    }
}