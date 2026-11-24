using System.Collections.Generic;
using CardEffect;
using CardEffect.CardActionModules;
using CardEffect.CardDataJson;
using CardEffect.Expressions;
using Newtonsoft.Json.Linq;
using OneOf;
using UnityEngine;
using VContainer;
using CreaturesComponent;

namespace CardsComponent
{
    /// <summary>
    /// カードアクションデータファクトリークラス
    /// </summary>
    public class CardActionDataFactory : ICardDataFactory
    {
        /// <summary>
        /// カードデータファイルのパス
        /// </summary>
        private static string cardDataPath = "MetaData/CardData.json";

        /// <summary>
        /// カード効果コンポーネントレジストリ
        /// </summary>
        private CardEffectRegistry<CardActionComponent> cardActionRegistry = new();

        /// <summary>
        /// カード式コンポーネントレジストリ
        /// </summary>
        private CardEffectRegistry<ValueExpressionComponent> valueExpressionRegistry = new();

        /// <summary>
        /// プレイヤーマネージャー
        /// </summary>
        private PlayerManager playerManager;

        /// <summary>
        /// デッキ情報クラス
        /// </summary>
        private DeckInfomation deckInfomation;

        /// <summary>
        /// DI用初期化メソッド
        /// </summary>
        /// <param name="deckInfomation">デッキ情報クラス</param>
        [Inject]
        public void Init(DeckInfomation deckInfomation, PlayerManager playerManager)
        {
            this.deckInfomation = deckInfomation;
            this.playerManager = playerManager;
        }

        /// <summary>
        /// カードデータを生成する
        /// </summary>
        /// <returns>カードデータ</returns>
        public List<CardData> CreateCardDataList()
        {
            // カード効果とカード式のリスト登録
            CreateCardActionAndValueExpressionList();

            // Resourcesフォルダからカードデータファイルを読み込む形に変更
            TextAsset cardDataTextAsset = Resources.Load<TextAsset>(cardDataPath.Replace(".json", ""));
            if (cardDataTextAsset == null)
            {
                Debug.LogError($"CardData.jsonがResources/{cardDataPath}として見つかりません。");
                return null;
            }
            string json = cardDataTextAsset.text;

            // Jsonテキストをエンティティクラスに変更して、変換する
            CardDataListEntity cardDataList = GenerateCardDataListEntity(json);
            List<CardData> cardDatas = new();
            foreach (var cardDataEntity in cardDataList.CardDatas)
            {
                CardData cardData = new();

                cardData.SetCardDataJsonEntity(cardDataEntity);

                // 登録されている順番にカード効果を適用する
                foreach (var cardActionData in cardDataEntity.CardActionsData)
                {
                    var action = GenerateCardAction(cardActionData);
                    cardData.CardActions.Add(action);
                }
                cardDatas.Add(cardData);
            }

            return cardDatas;
        }

        /// <summary>
        /// カード効果、カード式のリスト初期化
        /// </summary>
        private void CreateCardActionAndValueExpressionList()
        {
            // カード効果リスト初期化
            SetAction(0, new Attack(playerManager));
            SetAction(1, new AddBlock());
            SetAction(2, new AddMuscle());
            SetAction(3, new AddWeakened());
            SetAction(100, new DrawCard(deckInfomation));

            // カード式リスト初期化
            SetValueExpression(0, new Number(0));
            SetValueExpression(1, new String(""));
            SetValueExpression(2, new Add());
            SetValueExpression(3, new Sub());
            SetValueExpression(4, new Mul());
            SetValueExpression(5, new Div());
            SetValueExpression(30, new HandCount(deckInfomation));
        }

        /// <summary>
        /// カード効果コンポーネントの登録
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="cardAction">登録するカード効果コンポーネント</param>
        private void SetAction(int id, CardActionComponent cardAction)
        {
            cardActionRegistry.Register(id, cardAction);
        }

        /// <summary>
        /// カード式コンポーネントの登録
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="valueExpression">登録するカード式コンポーネント</param>
        private void SetValueExpression(int id, ValueExpressionComponent valueExpression)
        {
            valueExpressionRegistry.Register(id, valueExpression);
        }

        /// <summary>
        /// カード効果コンポーネントの取得
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>IDに紐づけられたカード効果コンポーネント</returns>
        private CardActionComponent GetCardAction(int id)
        {
            if (!cardActionRegistry.TryGet(id, out var ret))
            {
                throw new KeyNotFoundException($"ID:[{id}]を持つカード効果コンポーネントは実装・登録されていません");
            }
            return ret;
        }

        /// <summary>
        /// カード式コンポーネントの取得
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>IDに紐づけられたカード式コンポーネント</returns>
        private ValueExpressionComponent GetValueExpression(int id)
        {
            if(!valueExpressionRegistry.TryGet(id, out var ret))
            {
                throw new KeyNotFoundException($"ID:[{id}]を持つカード式コンポーネントは実装・登録されていません");
            }
            return ret;
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
            if (args != null)
            {
                action.SetArgs(args);
            }
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
                    var generatedArgs = GenerateValueExpressionArgs(arg.AsT1.args);
                    valueExpression.SetArgs(generatedArgs);
                    ret.Add(valueExpression);
                }
            }
            return ret;
        }

        private List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> GenerateValueExpressionArgs(List<OneOf<float, string, CardActionEntity, CardExpressionEntity>> argsEntity)
        {
            List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>> ret = new();
            foreach (var arg in argsEntity)
            {
                if (arg.IsT0)
                {
                    // 数値情報の登録
                    ret.Add(arg.AsT0);
                }
                else if (arg.IsT1)
                {
                    // 文字列情報情報の登録
                    ret.Add(arg.AsT1);
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
                    var generatedArgs = GenerateValueExpressionArgs(arg.AsT3.args);
                    valueExpression.SetArgs(generatedArgs);
                    ret.Add(valueExpression);
                }
            }
            return ret;
        }

        private CardDataListEntity GenerateCardDataListEntity(string jsonText)
        {
            var ret = JsonUtility.FromJson<CardDataListEntity>(jsonText);
            var jObject = JObject.Parse(jsonText);
            var cardDataEntities = (JArray)jObject["CardDatas"];
            foreach (var cardDataJToken in cardDataEntities)
            {
                // JsonUtility.FromJsonではOneOfで設定したパラメータをCardDataListEntityにコンバートできない為、ここで初期化する。
                // 各CardDataについて、JObjectからCardActionsDataやCardEffect(Args)を明示的にパースし、OneOf型に変換して格納し直す
                var cardDataEntityIndex = cardDataEntities.IndexOf(cardDataJToken);

                // CardDataListEntity内の対象となるCardDataを取得
                var cardDataEntity = ret.CardDatas[cardDataEntityIndex];

                // CardEffect(カードの効果群)を再構築
                var cardEffectsJArray = (JArray)cardDataJToken["CardEffect"]; // JArrayで抽出
                cardDataEntity.CardActionsData = new List<CardActionEntity>();
                foreach (var cardEffectJToken in cardEffectsJArray)
                {
                    var cardActionEntity = new CardActionEntity();
                    cardActionEntity.CardActionId = (int)cardEffectJToken["CardActionId"];
                    cardActionEntity.Args = new List<OneOf<CardActionEntity, CardExpressionEntity>>();

                    var argsJArray = (JArray)cardEffectJToken["Args"];
                    foreach (var argJToken in argsJArray)
                    {
                        var type = (string)argJToken["Type"];
                        if (type == "Action")
                        {
                            // 再帰的にCardActionEntityとしてパースし、Argsも再帰的に確認
                            var actionValue = argJToken["Value"];
                            var nestedAction = ParseActionEntity(actionValue);
                            cardActionEntity.Args.Add(nestedAction);

                        }
                        else if (type == "Expression")
                        {
                            // ExpressionIdとArgsを持つCardExpressionEntityとして生成
                            var exprValue = argJToken["Value"];
                            var exprEntity = ParseExpressionEntity(exprValue);
                            cardActionEntity.Args.Add(exprEntity);

                        }
                    }
                    cardDataEntity.CardActionsData.Add(cardActionEntity);
                }
            }
            return ret;
        }

        private CardActionEntity ParseActionEntity(JToken actionValue)
        {
            var cardActionEntity = new CardActionEntity();
            cardActionEntity.CardActionId = (int)actionValue["CardActionId"];
            cardActionEntity.Args = new List<OneOf<CardActionEntity, CardExpressionEntity>>();

            if (actionValue["Args"] is JArray argsJArray)
            {
                foreach (var argJToken in argsJArray)
                {
                    var type = (string)argJToken["Type"];
                    if (type == "Action")
                    {
                        // CardActionEntityとして再帰的にパース
                        var nestedActionValue = argJToken["Value"];
                        var nestedActionEntity = ParseActionEntity(nestedActionValue);
                        cardActionEntity.Args.Add(nestedActionEntity);
                    }
                    else if (type == "Expression")
                    {
                        // CardExpressionEntityとして再帰的にパース
                        var exprValue = argJToken["Value"];
                        var exprEntity = ParseExpressionEntity(exprValue);
                        cardActionEntity.Args.Add(exprEntity);
                    }
                }
            }

            return cardActionEntity;
        }

        private CardExpressionEntity ParseExpressionEntity(JToken exprValue)
        {
            var exprEntity = new CardExpressionEntity();
            exprEntity.ExpressionId = (int)exprValue["ExpressionId"];
            exprEntity.args = new List<OneOf<float, string, CardActionEntity, CardExpressionEntity>>();
            if (exprValue["Args"] is JArray nestedArgs)
            {
                foreach (var nestedArgJToken in nestedArgs)
                {
                    var nestedType = (string)nestedArgJToken["Type"];
                    if (nestedType == "float")
                    {
                        exprEntity.args.Add((float)nestedArgJToken["Value"]);
                    }
                    else if (nestedType == "string")
                    {
                        exprEntity.args.Add((string)nestedArgJToken["Value"]);
                    }
                    else if (nestedType == "Action")
                    {
                        var nestedActionValue = nestedArgJToken["Value"];
                        var nestedActionEntity = nestedActionValue.ToObject<CardActionEntity>();
                        exprEntity.args.Add(nestedActionEntity);
                    }
                    else if (nestedType == "Expression")
                    {
                        var deeplyNestedExprValue = nestedArgJToken["Value"];
                        var deeplyNestedExprEntity = ParseExpressionEntity(deeplyNestedExprValue);
                        exprEntity.args.Add(deeplyNestedExprEntity);
                    }
                }
            }
            return exprEntity;
        }
    }
}
