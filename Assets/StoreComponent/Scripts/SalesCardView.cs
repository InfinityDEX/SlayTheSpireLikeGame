using TMPro;
using UnityEngine;

public class SalesCardView : MonoBehaviour
{
    [field:SerializeField, Header("カード本体")]
    private Card card;

    [field:SerializeField, Header("販売価格表示テキスト欄")]
    private TextMeshProUGUI salesPriceView;

    /// <summary>
    /// 販売カードのデータ登録
    /// </summary>
    /// <param name="cardData">登録するカードのデータ</param>
    public void SetSalesCardData(CardData cardData)
    {
        card?.Setup(cardData, -1);

        if (salesPriceView != null)
        {
            salesPriceView.text = $"{card.ThisCardData.SalesPrice}コイン";
        }
    }
}
