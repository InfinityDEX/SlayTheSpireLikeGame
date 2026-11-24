using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VContainer;
using static CardsComponent.DeckInfomation;

namespace CardsComponent
{
    /// <summary>
    /// デッキマネージャクラス
    ///
    /// 山札、捨て札、手札を管理する。
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        /// <summary>
        /// デフォルトデッキのファイルパス(Resourcesフォルダ内)
        /// </summary>
        private const string DefaultDeckDataPath = "MetaData/DefaultDeckData.json";

        /// <summary>
        /// 山札枚数
        /// </summary>
        public int DrawPileCount { get { return deckInfo.DrawPileCount; } }

        /// <summary>
        /// 手札枚数
        /// </summary>
        public int HandCount { get { return deckInfo.HandCount; } }

        /// <summary>
        /// 捨札枚数
        /// </summary>
        public int DiscardPileCount { get { return deckInfo.DiscardPileCount; } }

        /// <summary>
        /// 手札
        /// </summary>
        public List<CardInHand> Hand { get { return deckInfo.HandList; } }

        ////デッキデータファイルのパス
        //private const string DeckDataFolderPath = "SaveData";
        //private const string DeckDataFileName = "DeckData.json";

        /// <summary>
        /// カードデータライブラリ
        /// </summary>
        private CardDataLibrary cardDataLibrary;

        /// <summary>
        /// デッキ情報
        /// </summary>
        private DeckInfomation deckInfo;

        /// <summary>
        /// カードビュワー
        /// </summary>
        private CardViewer cardViewer;

        /// <summary>
        /// デッキデータIO
        /// </summary>
        private IDeckDataIO deckDataIO;

        /// <summary>
        /// 依存性注入
        /// </summary>
        /// <param name="cardDataLibrary">カードデータライブラリ</param>
        /// <param name="deckInfomation">デッキ情報</param>
        /// <param name="cardViewer">カードビュワー</param>
        /// <param name="deckDataIO">デッキデータIO</param>
        [Inject]
        public void Init(CardDataLibrary cardDataLibrary, DeckInfomation deckInfomation, CardViewer cardViewer, IDeckDataIO deckDataIO)
        {
            this.cardDataLibrary = cardDataLibrary;
            this.deckInfo = deckInfomation;
            this.cardViewer = cardViewer;
            this.deckDataIO = deckDataIO;
        }

        private void Start()
        {
            // デッキデータの読み込み
            // 読み込む際は毎回デッキの中身をリセットしてから読み込む
            deckInfo.RemoveDeck();
            LoadDeckFromJson();
        }

        /// <summary>
        /// DeckData.jsonからデータを読み込み、drawPileを構築する処理
        /// </summary>
        private void LoadDeckFromJson()
        {
            //// DeckData.jsonを読み込む
            //string folderPath = Path.Combine(Application.persistentDataPath, DeckDataFolderPath);
            //string filePath = Path.Combine(folderPath, DeckDataFileName);

            ////  フォルダが存在しない場合は作成する
            //if (!Directory.Exists(folderPath))
            //{
            //    Directory.CreateDirectory(folderPath);
            //}

            //// ファイルがない場合はデフォルトデッキデータをコピーして新規作成する。
            //if (!File.Exists(filePath))
            //{
            //    Debug.Log($"{filePath}が見つかりません。Resourcesフォルダ内の{DefaultDeckDataPath}を複製して新規作成します: ");

            //    // デフォルトデッキデータのパス
            //    TextAsset defaultDeckTextAsset = Resources.Load<TextAsset>(DefaultDeckDataPath.Replace(".json", ""));
            //    if (defaultDeckTextAsset != null)
            //    {
            //        // DefaultDeckData.jsonの内容をDeckData.jsonに新規作成
            //        File.WriteAllText(filePath, defaultDeckTextAsset.text);
            //        Debug.Log("DefaultDeckData.jsonをDeckData.jsonとしてコピーしました。");
            //    }
            //    else
            //    {
            //        Debug.LogError("DefaultDeckData.jsonがResources/MetaData/DefaultDeckDataとして見つかりません。");
            //        return;
            //    }
            //}

            //string json = System.IO.File.ReadAllText(filePath);

            DeckDataJson deckData = deckDataIO.LoadDeckDataFromJsonFile();

            List<CardData> cards = new(); 
            foreach (var cardEntry in deckData.CurrentDeck)
            {
                // cardId(string)→int
                if (!int.TryParse(cardEntry.cardId, out int cardId))
                    continue;

                var cardData = cardDataLibrary.SearchCardData(cardId);

                for (int i = 0; i < cardEntry.count; i++)
                {
                    cards.Add(cardData);
                }
            }

            // デッキ情報インスタンスにDeckDataIOから読み取ったデッキカードデータをセットする
            deckInfo.SetDeckData(cards);
        }

        /// <summary>
        /// 現在のデッキ状況を削除する
        /// </summary>
        public void DeleteDeckData()
        {
            //string folderPath = Path.Combine(Application.persistentDataPath, DeckDataFolderPath);
            //string filePath = Path.Combine(folderPath, DeckDataFileName);

            ////  フォルダが存在しない場合は削除することはできないので以降の処理をスキップする
            //if (!Directory.Exists(folderPath))
            //{
            //    return;
            //}

            //if (File.Exists(filePath))
            //{
            //    File.Delete(filePath);
            //    Debug.Log("Deckデータファイルを削除しました: " + filePath);
            //}
            //else
            //{
            //    Debug.LogWarning("Deckデータファイルが存在しません: " + filePath);
            //}

            // デッキデータファイルを削除する
            deckDataIO.DeleteDeckDataJsonFile();

            // デッキ情報を削除する
            deckInfo.RemoveDeck();
        }

        /// <summary>
        /// 全てのカードをデッキに戻す
        /// </summary>
        public void ReturnAllCardsToDrawPile()
        {
            deckInfo.ReturnAllCardsToDrawPile();
        }

        /// <summary>
        /// 捨札のカードをデッキに戻す
        /// </summary>
        public void ReturnDiscardCardsToDrawPile()
        {
            deckInfo.ReturnDiscardCardsToDrawPile();
        }

        /// <summary>
        /// 現在のデッキデータをDeckData.jsonに保存する
        /// </summary>
        public void WriteDeckData()
        {
            // デッキのカードをカードIDごとにまとめて枚数を数え、Json保存用のデータに変換する（カードIDの昇順）
            DeckDataJson deckDataJson = new()
            {
                CurrentDeck = deckInfo.DeckList
                    .GroupBy(cardData => cardData.CardID)
                    .OrderBy(group => group.Key)
                    .Select(group => new CardEntry { cardId = group.Key.ToString(), count = group.Count() })
                    .ToList()
            };

            deckDataIO.SaveDeckDataToJsonFile(deckDataJson);
        }

        /// <summary>
        /// デッキ情報の初期化
        /// </summary>
        public void ClearDeck()
        {
            deckInfo.ResetPlayedDeck();
        }

        /// <summary>
        /// カードを引く
        /// </summary>
        public void DrawCard()
        {
            deckInfo.DrawCard();
        }

        /// <summary>
        /// カードのデッキ自体に追加
        /// </summary>
        /// <param name="cardData">追加したいカード</param>
        public void AddCard(CardData cardData)
        {
            deckInfo.AddCard(cardData);
        }

        /// <summary>
        /// カードの削除
        /// </summary>
        /// <param name="cardData">削除したいカード</param>
        public void RemoveCard(CardData cardData)
        {
            //if (cardData == null)
            //{
            //    throw new ArgumentNullException(nameof(cardData), "削除対象のカードの情報にnullが渡されました。カードは削除できません。");
            //}
            ////渡されたカードのカードIDと最初に一致したカードを除去する
            ////選択したカードと直接紐づいているものを除去するわけではないが、処理後のデッキ構成は変わらない為これで問題なし
            //var idx = deckInfo.DrawPileList.FindIndex(currentCardData => currentCardData.CardID == cardData.CardID);
            //if (idx == -1)
            //{
            //    throw new InvalidOperationException($"指定されたID[{cardData.CardID}]を持つカードはデッキに登録されていません。カードは削除できません。");
            //}
            //deckInfo.DrawPile.RemoveAt(idx);

            // カードをデッキから削除
            deckInfo.RemoveCard(cardData);
        }

        /// <summary>
        /// カードの強化
        /// </summary>
        /// <param name="cardData">強化したいカード</param>
        public void UpgradeCard(CardData cardData)
        {
            if (cardData == null)
            {
                throw new ArgumentNullException(nameof(cardData), "強化対象のカードの情報にnullが渡されました。カードは強化できません。");
            }
            // 渡されたカードのカードIDと最初に一致したカードをアップグレードする
            // 選択したカードと直接紐づいているものをアップグレードするわけではないが、処理後のデッキ構成は変わらない為これで問題なし
            var idx = deckInfo.DeckList.FindIndex(currentCardData => currentCardData.CardID == cardData.CardID);
            if (idx == -1)
            {
                throw new InvalidOperationException($"指定されたID[{cardData.CardID}]を持つカードはデッキに登録されていません。カードは強化できません。");
            }

            // カードを強化
            deckInfo.UpgradeCard(cardData, cardDataLibrary.SearchCardData(deckInfo.DeckList[idx].UpgradedCardId));
        }

        /// <summary>
        /// 手札のカードを捨て札に送る
        /// </summary>
        /// <param name="idx">墓場に送るカードの手札インデックス</param>
        public void DiscardCardById(int idx)
        {
            deckInfo.DiscardCardById(idx);
        }

        /// <summary>
        /// 手札をすべて捨てる
        /// </summary>
        public void DiscardHandCardAll()
        {
            deckInfo.DiscardHandCardAll();
        }

        /// <summary>
        /// デッキにあるカードをカードビュワーで表示
        /// </summary>
        public void OpenDeckListView()
        {
            cardViewer?.EnableViewer(deckInfo.DeckList);
        }

        /// <summary>
        /// デッキにある未強化のカードをカードビュワーで表示
        /// </summary>
        public void OpenUnUpgradeDeckListView()
        {
            cardViewer?.EnableViewer(deckInfo.UnUpgradeCardList);
        }

        /// <summary>
        /// 山札にあるカードをカードビュワーで表示
        /// </summary>
        public void OpenDrawPileListView()
        {
            cardViewer?.EnableViewer(deckInfo.DrawPileList);
        }

        /// <summary>
        /// 捨て札にあるカードをカードビュワーで表示
        /// </summary>
        public void OpenDiscardPileListView()
        {
            cardViewer?.EnableViewer(deckInfo.DiscardPileList);
        }
    }
}
