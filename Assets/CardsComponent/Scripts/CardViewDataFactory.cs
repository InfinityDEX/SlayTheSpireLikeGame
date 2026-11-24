using System.Collections.Generic;
using CardEffect.CardDataJson;
using Newtonsoft.Json.Linq;
using OneOf;
using UnityEngine;
using VContainer;

namespace CardsComponent
{
    /// <summary>
    /// カードビューデータファクトリークラス
    /// </summary>
    public class CardViewDataFactory : ICardDataFactory
    {
        /// <summary>
        /// カードデータファイルのパス
        /// </summary>
        private static string cardDataPath = "MetaData/CardData.json";


        /// <summary>
        /// カードデータを生成する
        /// </summary>
        /// <returns>カードデータ</returns>
        public List<CardData> CreateCardDataList()
        {
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

            // JsonUtilityを使用して、JSON文字列をCardDataListEntityに変換(カード描画用の情報だけほしいので、効果クラスの生成処理は行わない)
            foreach (var cardDataEntity in cardDataList.CardDatas)
            {
                CardData cardData = new();
                cardData.SetCardDataJsonEntity(cardDataEntity);
                cardDatas.Add(cardData);
            }

            return cardDatas;
        }


        private CardDataListEntity GenerateCardDataListEntity(string jsonText)
        {
            return JsonUtility.FromJson<CardDataListEntity>(jsonText); ;
        }
    }
}
