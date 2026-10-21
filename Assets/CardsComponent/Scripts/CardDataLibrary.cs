using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// カードライブラリクラス
/// </summary>
public class CardDataLibrary : MonoBehaviour
{
    /// <summary>
    /// シングルトン
    /// </summary>
    public static CardDataLibrary Instance { get; private set;} = null;

    /// <summary>
    /// カードリスト
    /// </summary>
    private List<CardData> cardList = new();

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // シーンを跨ぐ際に破壊されない
        DontDestroyOnLoad(this.gameObject);

        // CardDataListの初期化
        var factory = new CardDataFactory();
        cardList = factory.CreateCardDataList();
    }

    /// <summary>
    /// カードデータの検索
    /// </summary>
    /// <param name="id">カードID</param>
    /// <returns>カードデータ</returns>
    public CardData SearchCardData(int id)
    {
        return cardList.Find( cardData => cardData.CardID == id );
    }

    /// <summary>
    /// ライブラリ内にある未強化のカードデータをランダムに取得
    /// </summary>
    /// <returns>カードデータ</returns>
    public CardData GetRandomCard()
    {
        // 未強化のカードデータだけピックアップ
        var unUpgradeCardList = cardList.Where(card => !card.IsUpgraded).ToList();

        // 未強化カードデータのプールからランダムにカード取得
        return unUpgradeCardList[Random.Range(0, unUpgradeCardList.Count - 1)];
    }
}