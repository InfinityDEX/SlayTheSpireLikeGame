using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.AI;

/// <summary>
/// デッキマネージャクラス
///
/// 山札、捨て札、手札を管理する。
/// </summary>
public class DeckManager : MonoBehaviour
{
    // デッキマネージャーインスタンス
    private static DeckManager instance;
    public static DeckManager Instance { get {return instance;} }

    [field: Header("カードビュワー"), SerializeField]
    private CardViewer cardViewer;

    /// <summary>
    /// 山札
    /// </summary>
    public List<CardData> DrawPile { get ; private set;} = new();

    /// <summary>
    /// 手札
    /// </summary>
    public List<CardInHand> Hand { get ; private set;} = new();

    /// <summary>
    /// 捨て札
    /// </summary>
    public List<CardData> DiscardPile { get ; private set;} = new();

    /// <summary>
    /// デフォルトデッキのファイルパス(Resourcesフォルダ内)
    /// </summary>
    private const string DefaultDeckDataPath = "MetaData/DefaultDeckData.json";

    /// <summary>
    /// デッキ中全てのカードのリスト
    /// Getアクセサのみ
    ///
    /// 山札手札捨て札全てのカードを集約して返す。
    /// これでバトル中でも所持している全てのカードのリストが取得できる
    /// </summary>
    public List<CardData> DeckCardList { get {
        return DrawPile // 山札に
            .Concat(Hand.Select(cardInHand => cardInHand.cardData).ToList()) // 手札と
            .Concat(DiscardPile).ToList() // 捨て札を連結して
            .OrderBy(cardData => cardData.CardID ) // IDで昇順ソートして
            .ToList(); // リストにして返す
        }
    }

    // デッキデータファイルのパス
    private const string DeckDataFilePath = "SaveData/DeckData.json";

    private void Start()
    {
        // シングルトン
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // DrawPile.Clear();

        LoadDeckFromJson();

        // 山札をシャッフル
        Shuffle(DrawPile);
    }

    /// <summary>
    /// DeckData.jsonからデータを読み込み、drawPileを構築する処理
    /// </summary>
    private void LoadDeckFromJson()
    {
        // StreamingAssetsからDeckData.jsonを読み込む
        string filePath = System.IO.Path.Combine(Application.dataPath, DeckDataFilePath);
        if (!System.IO.File.Exists(filePath))
        {
            Debug.Log($"{filePath}が見つかりません。Resourcesフォルダ内の{DefaultDeckDataPath}を複製して新規作成します: ");

            // デフォルトデッキデータのパス
            TextAsset defaultDeckTextAsset = Resources.Load<TextAsset>(DefaultDeckDataPath.Replace(".json", ""));
            if (defaultDeckTextAsset != null)
            {
                // DefaultDeckData.jsonの内容をDeckData.jsonに新規作成
                System.IO.File.WriteAllText(filePath, defaultDeckTextAsset.text);
                Debug.Log("DefaultDeckData.jsonをDeckData.jsonとしてコピーしました。");
            }
            else
            {
                Debug.LogError("DefaultDeckData.jsonがResources/MetaData/DefaultDeckDataとして見つかりません。");
                return;
            }
        }

        string json = System.IO.File.ReadAllText(filePath);

        DeckDataJson deckData = JsonUtility.FromJson<DeckDataJson>(json);

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
                DrawPile.Add(cardData);
            }
        }
    }

    /// <summary>
    /// 現在のデッキ状況を削除する
    /// </summary>
    public void DeleteDeckData()
    {
        string filePath = System.IO.Path.Combine(Application.dataPath, DeckDataFilePath);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
            Debug.Log("Deckデータファイルを削除しました: " + filePath);
        }
        else
        {
            Debug.LogWarning("Deckデータファイルが存在しません: " + filePath);
        }
    }

    /// <summary>
    /// 現在のデッキデータをDeckData.jsonに保存する
    /// </summary>
    public void SaveDeck()
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
                CardEntry entry = new CardEntry{
                    cardId = cardData.CardID.ToString(),
                    count = 1
                };
                deckDataJson.CurrentDeck.Add(entry);
            }
        }

        deckDataJson.CurrentDeck.Sort((cardEntryA, cardEntryB) =>
            int.Parse(cardEntryA.cardId).CompareTo(int.Parse(cardEntryB.cardId))
        );

        string json = JsonUtility.ToJson(deckDataJson, true);

        string filePath = System.IO.Path.Combine(Application.dataPath, DeckDataFilePath);
        System.IO.File.WriteAllText(filePath, json);
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
    /// カードの追加
    /// </summary>
    /// <param name="cardData">追加したいカード</param>
    public void AddCard(CardData cardData)
    {
        DrawPile.Add(cardData);
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
    public void DiscardCardAll()
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
    /// デッキにあるカードをカードビュワーで表示
    /// </summary>
    public void OpenDrawPailView()
    {
        cardViewer?.EnableViewer(DrawPile);
    }

    /// <summary>
    /// 捨て札にあるカードをカードビュワーで表示
    /// </summary>
    public void OpenDiscardPileView()
    {
        cardViewer?.EnableViewer(DiscardPile);
    }
}