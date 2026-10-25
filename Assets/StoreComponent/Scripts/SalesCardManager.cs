using System.Collections.Generic;
using UnityEngine;

public class SalesCardManager : MonoBehaviour
{
    [field:SerializeField, Header("プレビューカードのプレハブ")]
    private SalesCardView previewCardPrefab;

    /// <summary>
    /// ストア画面で表示しているカードの一覧
    /// </summary>
    private List<SalesCardView> salesCardViews;

    private void Start()
    {
        salesCardViews = new();

        // 9個のランダムなカードデータを抽出
        var cardDatas = CardDataLibrary.Instance.GetRandomCards(9);
        foreach (var cardData in cardDatas)
        {
            var cardView = Instantiate(previewCardPrefab, transform);
            cardView.SetSalesCardData(cardData);
            salesCardViews.Add(cardView);
        }
    }
}
