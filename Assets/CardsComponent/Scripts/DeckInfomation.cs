using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VContainer;

namespace CardsComponent
{
    /// <summary>
    /// デッキ情報クラス
    /// </summary>
    public class DeckInfomation
    {
        /// <summary>
        /// デッキ
        /// </summary>
        private List<CardData> deck = new();

        /// <summary>
        /// 山札
        /// </summary>
        private List<CardData> drawPile = new();

        /// <summary>
        /// 手札
        /// </summary>
        private List<CardInHand> hand = new();

        /// <summary>
        /// 捨て札
        /// </summary>
        private List<CardData> discardPile = new();

        /// <summary>
        /// デッキの枚数
        /// </summary>
        public int DeckCount { get { return deck.Count; } }

        /// <summary>
        /// 山札の枚数
        /// </summary>
        public int DrawPileCount { get { return drawPile.Count; } }

        /// <summary>
        /// 手札の枚数
        /// </summary>
        public int HandCount { get { return hand.Count; } }

        /// <summary>
        /// 捨札の枚数
        /// </summary>
        public int DiscardPileCount { get { return discardPile.Count; } }

        /// <summary>
        /// デッキのリスト(昇順)
        /// 
        /// リストはコピーなので受取先でソートしても問題なし（削除はダメ）
        /// </summary>
        public List<CardData> DeckList { get { return deck.OrderBy(cardData => cardData.CardID).ToList(); } }

        /// <summary>
        /// デッキ中の未強化カードのリスト(昇順)
        /// 
        /// リストはコピーなので受取先でソートしても問題なし（削除はダメ）
        /// </summary>
        public List<CardData> UnUpgradeCardList { get { return deck.Where(cardData => !cardData.IsUpgraded).OrderBy(cardData => cardData.CardID).ToList(); } }

        /// <summary>
        /// 山札のリスト(昇順)
        /// 
        /// リストはコピーなので受取先でソートしても問題なし（削除はダメ）
        /// </summary>
        public List<CardData> DrawPileList { get { return drawPile.OrderBy(cardData => cardData.CardID).ToList(); } }

        /// <summary>
        /// 手札のリスト(昇順)
        /// 
        /// リストはコピーなので受取先でソートしても問題なし（削除はダメ）
        /// </summary>
        public List<CardInHand> HandList { get { return hand.OrderBy(cardInHand => cardInHand.cardData.CardID ).ToList(); } }

        /// <summary>
        /// 山札のリスト(昇順)
        /// 
        /// リストはコピーなので受取先でソートしても問題なし（削除はダメ）
        /// </summary>
        public List<CardData> DiscardPileList { get { return discardPile.OrderBy(cardData => cardData.CardID).ToList(); } }

        /// <summary>
        /// DeckData.json取得用のカードエントリクラス
        /// </summary>
        [System.Serializable]
        public class CardEntry
        {
            public string cardId; // jsonはstring
            public int count;
        }

        /// <summary>
        /// デッキデータのJson構造定義クラス
        /// </summary>
        [System.Serializable]
        public class DeckDataJson
        {
            public List<CardEntry> CurrentDeck;
        }

        /// <summary>
        /// 手札クラス
        /// </summary>
        [System.Serializable]
        public class CardInHand
        {
            /// <summary>
            /// カードデータ
            /// </summary>
            public CardData cardData;

            /// <summary>
            /// 手札の何番目かを表すインデックス
            /// 右にあるカードから順に連番
            /// </summary>
            public int cardIdx;

            public CardInHand(CardData data, int idx)
            {
                cardData = data;
                cardIdx = idx;
            }
        }

        /// <summary>
        /// デッキデータをセットする
        /// </summary>
        /// <param name="deckDatas">デッキ内のカードリスト</param>
        public void SetDeckData(List<CardData> deckDatas)
        {
            this.deck = deckDatas.ToList();
        }

        /// <summary>
        /// デッキにカード追加
        /// </summary>
        /// <param name="cardData">追加したいカードデータ</param>
        public void AddCard(CardData cardData)
        {
            if (cardData == null)
            {
                throw new ArgumentNullException(nameof(cardData), "追加カード情報としてnullが渡されました。カードは追加できません。");
            }
            deck.Add(cardData);
        }

        /// <summary>
        /// デッキからカードを削除
        /// </summary>
        /// <param name="cardData">削除したいカードのデータ</param>
        /// <exception cref="ArgumentNullException">引数[cardData]にnullが渡された</exception>
        /// <exception cref="InvalidOperationException">渡されたカードデータにある指定されたIDを持っているデータがデッキ内に無かった</exception>
        public void RemoveCard(CardData cardData)
        {
            if (cardData == null)
            {
                throw new ArgumentNullException(nameof(cardData), "削除対象のカードの情報にnullが渡されました。カードは削除できません。");
            }
            // 渡されたカードのカードIDと最初に一致したカードを除去する
            // 選択したカードと直接紐づいているものを除去するわけではないが、処理後のデッキ構成は変わらない為これで問題なし
            var idx = deck.FindIndex(deckCardData => deckCardData.CardID == cardData.CardID);
            if (idx == -1)
            {
                throw new System.InvalidOperationException($"指定されたID[{cardData.CardID}]を持つカードはデッキに登録されていません。カードは削除できません。");
            }
            deck.RemoveAt(idx);
        }

        /// <summary>
        /// 指定したカードを強化版に変更する
        /// </summary>
        /// <param name="cardData">強化したいカードデータ</param>
        /// <param name="upgradeCardData">強化後のカードデータ</param>
        /// <exception cref="ArgumentNullException">引数[cardData]または[upgradeCardData]にnullが渡された</exception>
        /// <exception cref="InvalidOperationException">渡されたカードデータにある指定されたIDを持っているデータがデッキ内に無かった</exception>
        /// <exception cref="KeyNotFoundException">渡されたIDのカードデータがカードリストに無かった(CardDataLibrary.SearchCardDataメソッドから呼び出し)</exception>
        public void UpgradeCard(CardData cardData, CardData upgradeCardData)
        {
            if (cardData == null)
            {
                throw new ArgumentNullException(nameof(cardData), "強化対象のカードの情報にnullが渡されました。カードは強化できません。");
            }
            if (upgradeCardData == null)
            {
                throw new ArgumentNullException(nameof(upgradeCardData), "強化先のカードの情報にnullが渡されました。カードは強化できません。");
            }
            // 渡されたカードのカードIDと最初に一致したカードを強化する
            // 選択したカードと直接紐づいているものを強化するわけではないが、処理後のデッキ構成は変わらない為これで問題なし
            var idx = deck.FindIndex(deckCardData => deckCardData.CardID == cardData.CardID);
            if (idx == -1)
            {
                throw new System.InvalidOperationException($"指定されたID[{cardData.CardID}]を持つカードはデッキに登録されていません。カードは強化できません。");
            }

            // 未強化のカードを強化版に置き換える
            deck[idx] = upgradeCardData;
        }

        /// <summary>
        /// 指定IDのカード(Cardクラスのid)を手札から捨て札へ
        /// </summary>
        /// <param name="idx">捨てるカード(Card)のidx</param>
        public void DiscardCardById(int idx)
        {
            var cardInHand = hand.Find(c => c.cardIdx == idx);
            if (cardInHand != null)
            {
                hand.Remove(cardInHand);
                discardPile.Add(cardInHand.cardData);
            }
            else
            {
                throw new System.InvalidOperationException($"指定されたID[{idx}]の手札インデックスを持つカードは手札にありません。");
            }
        }

        /// <summary>
        /// 手札を全て破棄する
        /// </summary>
        public void DiscardHandCardAll()
        {
            // 手札の全カードを捨て札に移動する
            foreach (var cardInHand in hand)
            {
                discardPile.Add(cardInHand.cardData);
            }
            hand.Clear();
        }

        /// <summary>
        /// カードをドローする処理（カードのidをCardクラス用に付与）
        /// </summary>
        public void DrawCard()
        {
            // 山札が空の場合は捨て札を山札に加える
            if (drawPile.Count == 0)
            {
                ReturnDiscardCardsToDrawPile();

                if (drawPile.Count == 0) return; // 墓地をデッキの戻しても山札枚数が0 → すべてのデッキのカードが手札にある
            }

            // 山札からカードを引く
            var cardData = drawPile[0];
            drawPile.RemoveAt(0);

            int assignedIdx = HandCardIndexGenerator.GetNextIdx();
            hand.Add(new CardInHand(cardData, assignedIdx));
        }

        /// <summary>
        /// 手札に設定するIDを生成する静的クラス。
        /// </summary>
        public static class HandCardIndexGenerator
        {
            private static int nextId = 1;
            public static int GetNextIdx()
            {
                return nextId++;
            }
        }

        /// <summary>
        /// カードをシャッフルする処理
        /// </summary>
        public void Shuffle()
        {
            for (int i = 0; i < drawPile.Count; i++)
            {
                CardData temp = drawPile[i];
                int randomIndex = UnityEngine.Random.Range(i, drawPile.Count);
                drawPile[i] = drawPile[randomIndex];
                drawPile[randomIndex] = temp;
            }
        }

        /// <summary>
        /// 全てのカードをデッキに戻す
        /// </summary>
        public void ReturnAllCardsToDrawPile()
        {
            // 手札の全カードを山札に戻す
            foreach (var cardInHand in hand)
            {
                drawPile.Add(cardInHand.cardData);
            }
            hand.Clear();
            // 捨て札の全カードを山札に戻す
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        /// <summary>
        /// 捨札のカードをデッキに戻す
        /// </summary>
        public void ReturnDiscardCardsToDrawPile()
        {
            // 捨て札の全カードを山札に戻す
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        ///// <summary>
        ///// 現在のデッキデータをDeckData.jsonに保存する
        ///// </summary>
        //public void WriteDeckData(string deckDataFileName, string deckFolderPath)
        //{
        //    DeckDataJson deckDataJson = new();
        //    deckDataJson.CurrentDeck = new();
        //    foreach (var cardData in DeckList)
        //    {
        //        // 既に登録したかどうか確認
        //        if (deckDataJson.CurrentDeck.Exists(cardEntry => cardEntry.cardId == cardData.CardID.ToString()))
        //        {
        //            // 既に登録済みなら登録したCardEntryのCountを+1する
        //            deckDataJson.CurrentDeck.Find(cardEntry => cardEntry.cardId == cardData.CardID.ToString()).count++;
        //        }
        //        else
        //        {
        //            // 初めて追加する場合はCardEntryを新規作成して登録
        //            CardEntry entry = new CardEntry
        //            {
        //                cardId = cardData.CardID.ToString(),
        //                count = 1
        //            };
        //            deckDataJson.CurrentDeck.Add(entry);
        //        }
        //    }

        //    deckDataJson.CurrentDeck.Sort((cardEntryA, cardEntryB) =>
        //        int.Parse(cardEntryA.cardId).CompareTo(int.Parse(cardEntryB.cardId))
        //    );

        //    // デッキデータの書き込み
        //    string json = JsonUtility.ToJson(deckDataJson, true);
        //    string folderPath = Path.Combine(Application.persistentDataPath, deckFolderPath);
        //    string filePath = Path.Combine(folderPath, deckDataFileName);

        //    //  フォルダが存在しない場合は作成する
        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    // ファイルに書き込む
        //    File.WriteAllText(filePath, json);
        //}

        /// <summary>
        /// バトル中のデッキ情報のリセット
        /// </summary>
        public void ResetPlayedDeck()
        {
            drawPile.Clear();
            drawPile = deck.ToList(); // デッキをコピー
            hand.Clear();
            discardPile.Clear();

            // デッキをシャッフル
            Shuffle();
        }

        /// <summary>
        /// デッキ情報の削除
        /// </summary>
        public void RemoveDeck()
        {
            deck.Clear();
            drawPile.Clear();
            hand.Clear();
            discardPile.Clear();
        }
    }
}
