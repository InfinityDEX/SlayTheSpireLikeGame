using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<CardData> drawPile = new(); // 山札
    public List<CardInHand> hand = new List<CardInHand>(); // 手札
    public List<CardData> discardPile = new(); // 捨て札

    // DeckManagerのインスタンス生成時に、DeckData.jsonから山札を生成する
    [Header("カードライブラリ（全カードデータ）")]
    public CardLibrary cardLibrary;

    private void Awake()
    {
        LoadDeckFromJson();
    }

    // DeckData.jsonからデータを読み込み、drawPileを構築する
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

            var cardWithID = cardLibrary.cardDatas.Find(c => c.cardId == cardId);
            if (cardWithID.cardData == null)
                continue;

            for (int i = 0; i < cardEntry.count; i++)
            {
                drawPile.Add(cardWithID.cardData);
            }
        }

        // 必要なら山札をシャッフル
        Shuffle(drawPile);
    }

    // カードをシャッフル
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

    // DeckData.json取得構造体
    [System.Serializable]
    private class CardEntry
    {
        public string cardId; // jsonはstring
        public int count;
    }

    [System.Serializable]
    private class DeckDataJson
    {
        public List<CardEntry> CurrentDeck;
    }

    /// <summary>
    /// 手札(CardInHand)のCardにセットされるCard.idを使ってカードを捨て札（墓地）に置く
    /// </summary>
    [System.Serializable]
    public class CardInHand
    {
        public CardData cardData;
        public int cardId; // Cardクラスのidを利用

        public CardInHand(CardData data, int id)
        {
            cardData = data;
            cardId = id;
        }
    }

    /// <summary>
    /// 指定IDのカード(Cardクラスのid)を手札から捨て札へ
    /// </summary>
    /// <param name="id">捨てるカード(Card)のid</param>
    public void DiscardCardById(int id)
    {
        var cardInHand = hand.Find(c => c.cardId == id);
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
        }

        // 山札からカードを引く
        var cardData = drawPile[0];
        drawPile.RemoveAt(0);

        // Card.csのidに対応する形で採番して付与
        int assignedId = CardIdGenerator.GetNextId();
        hand.Add(new CardInHand(cardData, assignedId));
    }
}

/// <summary>
/// Cardクラスに設定するIDを生成する静的クラス。
/// </summary>
public static class CardIdGenerator
{
    private static int nextId = 1;
    public static int GetNextId()
    {
        return nextId++;
    }
}