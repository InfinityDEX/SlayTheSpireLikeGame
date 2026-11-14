using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VContainer;

/// <summary>
/// デッキ情報クラス
/// </summary>
public class DeckInfomation
{
    /// <summary>
    /// 山札
    /// </summary>
    public List<CardData> DrawPile { get; private set; } = new();

    /// <summary>
    /// 手札
    /// </summary>
    public List<CardInHand> Hand { get; private set; } = new();

    /// <summary>
    /// 捨て札
    /// </summary>
    public List<CardData> DiscardPile { get; private set; } = new();

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
    /// 指定IDのカード(Cardクラスのid)を手札から捨て札へ
    /// </summary>
    /// <param name="idx">捨てるカード(Card)のidx</param>
    public void DiscardCardById(int idx)
    {
        var cardInHand = Hand.Find(c => c.cardIdx == idx);
        if (cardInHand != null)
        {
            Hand.Remove(cardInHand);
            DiscardPile.Add(cardInHand.cardData);
        }
    }

    /// <summary>
    /// 手札を全て破棄する
    /// </summary>
    public void DiscardHandCardAll()
    {
        // 手札の全カードを捨て札に移動する
        foreach (var cardInHand in Hand)
        {
            DiscardPile.Add(cardInHand.cardData);
        }
        Hand.Clear();
    }

    /// <summary>
    /// カードをドローする処理（カードのidをCardクラス用に付与）
    /// </summary>
    public void DrawCard()
    {
        // 山札が空の場合は捨て札を山札に加える
        if (DrawPile.Count == 0)
        {
            DrawPile.AddRange(DiscardPile);
            DiscardPile.Clear();

            if (DrawPile.Count == 0) return; // 墓地をデッキの戻しても山札枚数が0 → すべてのデッキのカードが手札にある
        }

        // 山札からカードを引く
        var cardData = DrawPile[0];
        DrawPile.RemoveAt(0);

        int assignedIdx = HandCardIndexGenerator.GetNextIdx();
        Hand.Add(new CardInHand(cardData, assignedIdx));
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
        for (int i = 0; i < DrawPile.Count; i++)
        {
            CardData temp = DrawPile[i];
            int randomIndex = Random.Range(i, DrawPile.Count);
            DrawPile[i] = DrawPile[randomIndex];
            DrawPile[randomIndex] = temp;
        }
    }

    /// <summary>
    /// デッキ中全てのカードのリスト
    /// Getアクセサのみ
    ///
    /// 山札手札捨て札全てのカードを集約して返す。
    /// これでバトル中でも所持している全てのカードのリストが取得できる
    /// </summary>
    public List<CardData> DeckCardList
    {
        get
        {
            return DrawPile // 山札に
                .Concat(Hand.Select(cardInHand => cardInHand.cardData).ToList()) // 手札と
                .Concat(DiscardPile).ToList() // 捨て札を連結して
                .OrderBy(cardData => cardData.CardID) // IDで昇順ソートして
                .ToList(); // リストにして返す
        }
    }

    /// <summary>
    /// 全てのカードをデッキに戻す
    /// </summary>
    public void ReturnAllCardsToDrawPile()
    {
        // 手札の全カードを山札に戻す
        foreach (var cardInHand in Hand)
        {
            DrawPile.Add(cardInHand.cardData);
        }
        Hand.Clear();
        // 捨て札の全カードを山札に戻す
        DrawPile.AddRange(DiscardPile);
        DiscardPile.Clear();
    }

    /// <summary>
    /// 現在のデッキデータをDeckData.jsonに保存する
    /// </summary>
    public void WriteDeckData(string deckDataFileName, string deckFolderPath)
    {
        DeckDataJson deckDataJson = new();
        deckDataJson.CurrentDeck = new();
        foreach (var cardData in DeckCardList)
        {
            // 既に登録したかどうか確認
            if (deckDataJson.CurrentDeck.Exists(cardEntry => cardEntry.cardId == cardData.CardID.ToString()))
            {
                // 既に登録済みなら登録したCardEntryのCountを+1する
                deckDataJson.CurrentDeck.Find(cardEntry => cardEntry.cardId == cardData.CardID.ToString()).count++;
            }
            else
            {
                // 初めて追加する場合はCardEntryを新規作成して登録
                CardEntry entry = new CardEntry
                {
                    cardId = cardData.CardID.ToString(),
                    count = 1
                };
                deckDataJson.CurrentDeck.Add(entry);
            }
        }

        deckDataJson.CurrentDeck.Sort((cardEntryA, cardEntryB) =>
            int.Parse(cardEntryA.cardId).CompareTo(int.Parse(cardEntryB.cardId))
        );

        // デッキデータの書き込み
        string json = JsonUtility.ToJson(deckDataJson, true);
        string folderPath = Path.Combine(Application.persistentDataPath, deckFolderPath);
        string filePath = Path.Combine(folderPath, deckDataFileName);

        //  フォルダが存在しない場合は作成する
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ファイルに書き込む
        File.WriteAllText(filePath, json);
    }
}

