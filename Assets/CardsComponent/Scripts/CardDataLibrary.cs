using System.Collections.Generic;
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
}