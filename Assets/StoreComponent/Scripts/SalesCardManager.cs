using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class SalesCardManager : MonoBehaviour
{
    [field:SerializeField, Header("プレビューカードのプレハブ")]
    private GameObject previewCardPrefab;

    /// <summary>
    /// ストア画面で表示しているカードの一覧
    /// </summary>
    private List<SalesCard> salesCards;

    /// <summary>
    /// 選択中のカード
    /// </summary>
    private SalesCard selectedCard = null;

    // TODO:バグって全体が動かなくなったので今度実装する
    // private CardDataLibrary cardDataLibrary;
    // private DeckManager deckManager;

    // [Inject]
    // public void Init(CardDataLibrary cardDataLibrary, DeckManager deckManager)
    // {
    //     this.cardDataLibrary = cardDataLibrary;
    //     this.deckManager = deckManager;
    // }

    private void Start()
    {
        salesCards = new();

        // 9個のランダムなカードデータを抽出
        var cardDatas = CardDataLibrary.Instance?.GetRandomCards(9);
        foreach (var cardData in cardDatas)
        {
            // TODO：previewCardPrefabにSalesCardViewとSalesCardが登録されていることを前提にしているが、
            // もう少し安全に上記クラスのインスタンスを受け取れるようにしたい。
            var salesCardObj = Instantiate(previewCardPrefab, transform);
            var salesCardView = salesCardObj.GetComponent<SalesCardView>();
            salesCardView.SetSalesCardData(cardData);
            var salesCard = salesCardObj.GetComponent<SalesCard>();
            salesCard.Init(this);
            salesCards.Add(salesCard);
        }
    }

    /// <summary>
    /// カードを選択状態にする
    /// </summary>
    /// <param name="selectedCard">選択状態にするカード</param>
    public void SelectedCard(SalesCard selectedCard)
    {
        // 販売カード一覧にあるカードなら選択中カードにセット
        if (salesCards.Exists(card => card == selectedCard))
        {
            // 全てのカードを一旦選択解除状態のUIに更新する。
            salesCards.ForEach( card => card.Unselect());

            // 選択カード更新
            this.selectedCard = selectedCard;
            this.selectedCard.Select();
        }
    }

    /// <summary>
    /// カードの購入を試す
    ///
    /// 購入ができない場合は何も行わない
    /// </summary>
    public void BuySelectedCard()
    {
        if (selectedCard != null)
        {
            if (SaveDataHolder.Instance?.UseMoney(selectedCard.SalesPrice) == true)
            {
                DeckManager.Instance?.AddCard(selectedCard.CardData);
                DeckManager.Instance?.SaveDeck();

                salesCards.Remove(selectedCard);
                Destroy(selectedCard.gameObject);
            }
        }
    }
}
