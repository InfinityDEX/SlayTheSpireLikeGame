using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// デッキマネージャクラス
///
/// 山札、捨て札、手札を管理する。
/// </summary>
public class DeckManager : MonoBehaviour
{
    [field: Header("カードビュワー"), SerializeField]
    private CardViewer cardViewer;

    /// <summary>
    /// 山札
    /// </summary>
    public List<CardData> drawPile = new();

    /// <summary>
    /// 手札
    /// </summary>
    public List<CardInHand> hand = new List<CardInHand>();

    /// <summary>
    /// 捨て札
    /// </summary>
    public List<CardData> discardPile = new();

    private void Start()
    {
        LoadDeckFromJson();
    }

    /// <summary>
    /// DeckData.jsonからデータを読み込み、drawPileを構築する処理
    /// </summary>
    private void LoadDeckFromJson()
    {
        // StreamingAssetsからDeckData.jsonを読み込む
        string filePath = System.IO.Path.Combine(Application.dataPath, "SaveData/DeckData.json");
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("DeckData.jsonが見つかりません: " + filePath);
            return;
        }

        string json = System.IO.File.ReadAllText(filePath);

        DeckDataJson deckData = JsonUtility.FromJson<DeckDataJson>(json);

        drawPile.Clear();

        foreach (var cardEntry in deckData.CurrentDeck)
        {
            // cardId(string)→int
            if (!int.TryParse(cardEntry.cardId, out int cardId))
                continue;

            var cardData = CardDataLibrary.Instance?.SearchCardData(cardId);
            if (cardData == null)
                continue;

            for (int i = 0; i < cardEntry.count; i++)
            {
                drawPile.Add(cardData);
            }
        }

        // 山札をシャッフル
        Shuffle(drawPile);
    }

    /// <summary>
    /// カードをシャッフルする処理
    /// </summary>
    /// <param name="list">山札</param>
    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardData temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    /// <summary>
    /// DeckData.json取得用のカードエントリクラス
    /// </summary>
    [System.Serializable]
    private class CardEntry
    {
        public string cardId; // jsonはstring
        public int count;
    }

    /// <summary>
    /// デッキデータのJson構造定義クラス
    /// </summary>
    [System.Serializable]
    private class DeckDataJson
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
        var cardInHand = hand.Find(c => c.cardIdx == idx);
        if (cardInHand != null)
        {
            hand.Remove(cardInHand);
            discardPile.Add(cardInHand.cardData);
        }
    }

    /// <summary>
    /// 手札を全て破棄する
    /// </summary>
    public void DiscardCardAll()
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
            drawPile.AddRange(discardPile);
            discardPile.Clear();

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
    /// デッキにあるカードをカードビュワーで表示
    /// </summary>
    public void OpenDrawPailView()
    {
        cardViewer?.EnableViewer(ref drawPile);
    }

    /// <summary>
    /// 捨て札にあるカードをカードビュワーで表示
    /// </summary>
    public void OpenDiscardPileView()
    {
        cardViewer?.EnableViewer(ref discardPile);
    }
}