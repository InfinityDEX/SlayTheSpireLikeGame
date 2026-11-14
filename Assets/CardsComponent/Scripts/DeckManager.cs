using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using VContainer;
using static DeckInfomation;

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
    public int DrawPileCount { get { return deckInfo.DrawPile.Count; } }

    /// <summary>
    /// 手札枚数
    /// </summary>
    public int HandCount { get { return deckInfo.Hand.Count; } }

    /// <summary>
    /// 捨札枚数
    /// </summary>
    public int DiscardPileCount { get { return deckInfo.DiscardPile.Count; } }

    /// <summary>
    /// 手札
    /// </summary>
    public List<CardInHand> Hand { get { return deckInfo.Hand; } }

    // デッキデータファイルのパス
    private const string DeckDataFolderPath = "SaveData";
    private const string DeckDataFileName = "DeckData.json";

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
    /// 依存性注入
    /// </summary>
    /// <param name="cardDataLibrary">カードデータライブラリ</param>
    /// <param name="deckInfomation">デッキ情報</param>
    /// <param name="cardViewer">カードビュワー</param>
    [Inject]
    public void Init(CardDataLibrary cardDataLibrary, DeckInfomation deckInfomation, CardViewer cardViewer)
    {
        this.cardDataLibrary = cardDataLibrary;
        deckInfo = deckInfomation;
        this.cardViewer = cardViewer;
    }

    private void Start()
    {
        LoadDeckFromJson();

        // 山札をシャッフル
        deckInfo.Shuffle();
    }

    /// <summary>
    /// DeckData.jsonからデータを読み込み、drawPileを構築する処理
    /// </summary>
    private void LoadDeckFromJson()
    {
        // DeckData.jsonを読み込む
        string folderPath = Path.Combine(Application.persistentDataPath, DeckDataFolderPath);
        string filePath = Path.Combine(folderPath, DeckDataFileName);

        //  フォルダが存在しない場合は作成する
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ファイルがない場合はデフォルトデッキデータをコピーして新規作成する。
        if (!File.Exists(filePath))
        {
            Debug.Log($"{filePath}が見つかりません。Resourcesフォルダ内の{DefaultDeckDataPath}を複製して新規作成します: ");

            // デフォルトデッキデータのパス
            TextAsset defaultDeckTextAsset = Resources.Load<TextAsset>(DefaultDeckDataPath.Replace(".json", ""));
            if (defaultDeckTextAsset != null)
            {
                // DefaultDeckData.jsonの内容をDeckData.jsonに新規作成
                File.WriteAllText(filePath, defaultDeckTextAsset.text);
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

            var cardData = cardDataLibrary.SearchCardData(cardId);
            if (cardData == null)
                continue;

            for (int i = 0; i < cardEntry.count; i++)
            {
                deckInfo.DrawPile.Add(cardData);
            }
        }
    }

    /// <summary>
    /// 現在のデッキ状況を削除する
    /// </summary>
    public void DeleteDeckData()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, DeckDataFolderPath);
        string filePath = Path.Combine(folderPath, DeckDataFileName);

        //  フォルダが存在しない場合は削除することはできないので以降の処理をスキップする
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deckデータファイルを削除しました: " + filePath);
        }
        else
        {
            Debug.LogWarning("Deckデータファイルが存在しません: " + filePath);
        }
    }

    /// <summary>
    /// 全てのカードをデッキに戻す
    /// </summary>
    public void ReturnAllCardsToDrawPile()
    {
        deckInfo.ReturnAllCardsToDrawPile();
    }

    /// <summary>
    /// 現在のデッキデータをDeckData.jsonに保存する
    /// </summary>
    public void WriteDeckData()
    {
        deckInfo.WriteDeckData(DeckDataFileName, DeckDataFolderPath);
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
        deckInfo.DrawPile.Add(cardData);
    }

    /// <summary>
    /// カードの削除
    /// </summary>
    /// <param name="cardData">削除したいカード</param>
    public void RemoveCard(CardData cardData)
    {
        // 渡されたカードのカードIDと最初に一致したカードを除去する
        // (選択したカードと直接紐づいているものを除去しないが、処理後のデッキ構成は変わらない為これで問題なし)
        var idx = deckInfo.DrawPile.FindIndex(currentCardData => currentCardData.CardID == cardData.CardID);
        deckInfo.DrawPile.Remove(deckInfo.DrawPile[idx]);
    }

    /// <summary>
    /// カードの強化
    /// </summary>
    /// <param name="cardData">強化したいカード</param>
    public void UpgradeCard(CardData cardData)
    {
        // 渡されたカードのカードIDと最初に一致したカードをアップグレードする
        // (選択したカードと直接紐づいているものをアップグレードしないが、処理後のデッキ構成は変わらない為これで問題なし)
        var idx = deckInfo.DrawPile.FindIndex(currentCardData => currentCardData.CardID == cardData.CardID);
        deckInfo.DrawPile[idx] = cardDataLibrary.SearchCardData(deckInfo.DrawPile[idx].UpgradedCardId);
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
    public void OpenDrawPileView()
    {
        cardViewer?.EnableViewer(deckInfo.DrawPile);
    }

    /// <summary>
    /// 捨て札にあるカードをカードビュワーで表示
    /// </summary>
    public void OpenDiscardPileView()
    {
        cardViewer?.EnableViewer(deckInfo.DiscardPile);
    }
}