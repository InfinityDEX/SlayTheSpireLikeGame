using UnityEngine;

/// <summary>
/// 販売カードクラス
///
/// カードの販売に必要な価格情報やカードデータ本体の
/// 明け渡しを管理する。
/// </summary>
public class SalesCard : MonoBehaviour
{
    [field: SerializeField, Header("カード本体")]
    private Card card;

    private SalesCardManager salesCardManager;

    public int SalesPrice { get { return card.ThisCardData.SalesPrice;} }
    public CardData CardData { get { return card.ThisCardData;} }


    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="salesCardManager">販売カードマネージャ</param>
    public void Init(SalesCardManager salesCardManager)
    {
        this.salesCardManager = salesCardManager;
        Debug.Log("DIを用いて販売カードマネージャの登録に成功しました");
    }

    /// <summary>
    /// このカードを選択する
    /// </summary>
    public void SelectThisCard()
    {
        salesCardManager.SelectedCard(this);
    }
}