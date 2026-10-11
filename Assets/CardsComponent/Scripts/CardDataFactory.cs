using System.Collections.Generic;
using CardEffect;
using CardEffect.CardActionModules;
using CardEffect.CardDataJson;
using CardEffect.Expressions;
using OneOf;
using UnityEngine;

/// <summary>
/// カードデータファクトリークラス
/// </summary>
class CardDataFactory
{
    /// <summary>
    /// カードデータファイルのパス
    /// </summary>
    private static string cardDataPath = "SaveData/CardData.json";

    /// <summary>
    /// カード処理コンポーネントとIDの組
    /// </summary>
    private struct CardActionWithID
    {
        public int ID ;
        public CardActionComponent cardAction;
    }

    /// <summary>
    /// カード式コンポーネントとIDの組
    /// </summary>
    private struct ValueExpressionWithID
    {
        public int ID;
        public ValueExpressionComponent valueExpression;
    }

    /// <summary>
    /// カード処理コンポーネントリスト
    /// </summary>
    private List<CardActionWithID> actionWithIDs = new();

    /// <summary>
    /// カード式コンポーネントリスト
    /// </summary>
    private List<ValueExpressionWithID> valueExpressionWithIDs = new();

    public List<CardData> CreateCardDataList()
    {
        // カード効果とカード式のリスト登録
        CreateCardActionAndValueExpressionList();

        // カードデータファイルを読み込み
        string filePath = System.IO.Path.Combine(Application.dataPath, cardDataPath);
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("CardData.jsonが見つかりません: " + filePath);
            return null;
        }
        string json = System.IO.File.ReadAllText(filePath);

        // Jsonテキストをエンティティクラスに変更して、変換する
        CardDataListEntity cardDataList = JsonUtility.FromJson<CardDataListEntity>(json);
        List<CardData> cardDatas = new();
        foreach (var cardDataEntity in cardDataList.CardDatas)
        {
            CardData cardData = new();
            cardData.CardID = cardDataEntity.CardId;
            cardData.Cost = cardDataEntity.CardCost;
            cardData.CardName = cardDataEntity.CardName;
            cardData.Type = (CardData.CardType)cardDataEntity.CardType;
            cardData.CardImage = Resources.Load<Sprite>(cardDataEntity.CardImagePath);
            cardData.VisualEffectID = cardDataEntity.VisualEffectId;
            cardData.CastCardSE = Resources.Load<AudioClip>(cardDataEntity.CardCastSoundPath);
            cardData.CardDescription = cardDataEntity.CardDescription;
            cardData.TargetType = (CardData.Target)cardDataEntity.CardTarget;
            cardData.UpgradedCardId = cardDataEntity.UpgradedCardId;

            // 登録されている順番にカード効果を適用する
            foreach (var cardActionData in cardDataEntity.CardActionsData)
            {
                var action = GenerateCardAction(cardActionData);
                cardData.CardActions.Add(action);
            }
        }

        return cardDatas;
    }

    /// <summary>
    /// カード処理、カード式のリスト初期化
    /// </summary>
    private void CreateCardActionAndValueExpressionList()
    {
        // カード処理リスト初期化
        SetAction(0, new Attack());
        SetAction(1, new AddBlock());
        SetAction(2, new AddMuscle());
        SetAction(3, new AddWeakened());

        // カード式リスト初期化
        SetValueExpression(0, new Number());
        SetValueExpression(2, new Add());
        SetValueExpression(3, new Sub());
        SetValueExpression(4, new Mul());
        SetValueExpression(5, new Div());
    }

    private void SetAction(int _id, CardActionComponent _cardAction)
    {
        if (valueExpressionWithIDs.Exists( cardActionWithID => { return cardActionWithID.ID == _id; }))
        {
            Debug.LogError($"CardActionWithID(ID: {_id}) は既に登録されています。");

            return;
        }
        actionWithIDs.Add(new CardActionWithID{
            ID = _id,
            cardAction = _cardAction
        });
    }

    private void SetValueExpression(int _id, ValueExpressionComponent _valueExpression)
    {
        if (valueExpressionWithIDs.Exists( valueExpressionWithID => { return valueExpressionWithID.ID == _id; }))
        {
            Debug.LogError($"ValueExpression(ID: {_id}) は既に登録されています。");

            return;
        }
        valueExpressionWithIDs.Add(new ValueExpressionWithID{
            ID = _id,
            valueExpression = _valueExpression
        });
    }

    private CardActionComponent GetCardAction(int id)
    {
        return actionWithIDs.Find( actionWithID => { return actionWithID.ID == id; }).cardAction;
    }

    private ValueExpressionComponent GetValueExpression(int id)
    {
        return valueExpressionWithIDs.Find( valueExpressionWithID => { return valueExpressionWithID.ID == id;}).valueExpression;
    }

    /// <summary>
    /// カード効果の生成
    /// </summary>
    /// <param name="cardActionEntity">カード効果エンティティ</param>
    /// <returns>カード効果コンポーネントのインスタンス</returns>
    private CardActionComponent GenerateCardAction(CardActionEntity cardActionEntity)
    {
        int actionId = cardActionEntity.CardActionId;
        var action = GetCardAction(actionId);
        var args = GenerateArgs(cardActionEntity.Args);
        action.SetArgs(args);
        return action;
    }

    /// <summary>
    /// カード効果の変数リストの生成
    /// </summary>
    /// <param name="argsEntity">カード効果の変数エンティティ</param>
    /// <returns>カード効果の変数リスト</returns>
    private List<OneOf<CardActionComponent, ValueExpressionComponent>> GenerateArgs(List<OneOf<CardActionEntity, CardExpressionEntity>> argsEntity)
    {
        List<OneOf<CardActionComponent, ValueExpressionComponent>> ret = new();
        foreach (var arg in argsEntity)
        {
            if (arg.IsT0)
            {
                var action = GenerateCardAction(arg.AsT0);
                ret.Add(action);
            }
            else if (arg.IsT1)
            {
                var valueExpression = GetValueExpression(arg.AsT1.ExpressionId);
                var generatedArgs = GenerateArgs(arg.AsT1.args);
                valueExpression.SetArgs(generatedArgs);

            }
        }
        return ret;
    }
    private List<OneOf<CardActionComponent, ValueExpressionComponent>> GenerateArgs(List<OneOf<float, string, CardActionEntity, CardExpressionEntity>> argsEntity)
    {
        List<OneOf<CardActionComponent, ValueExpressionComponent>> ret = new();
        foreach (var arg in argsEntity)
        {
            if (arg.IsT0)
            {
                // 数値情報の登録
                var numComp = new Number();
                numComp.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>>{
                    arg.AsT0
                });
                ret.Add(numComp);
            }
            else if (arg.IsT1)
            {
                // 文字列情報情報の登録
                var strComp = new String();
                strComp.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>>{
                    arg.AsT1
                });
                ret.Add(strComp);
            }
            else if (arg.IsT2)
            {
                // カード効果コンポーネントの生成と登録
                var action = GenerateCardAction(arg.AsT2);
                ret.Add(action);
            }
            else if (arg.IsT3)
            {
                // カード式コンポーネントの生成と登録
                var valueExpression = GetValueExpression(arg.AsT3.ExpressionId);
                valueExpression.SetArgs(GenerateArgs(arg.AsT3.args));
            }
        }
        return ret;
    }
}