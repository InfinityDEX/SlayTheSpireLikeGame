using UnityEngine;
using UnityEngine.UI;

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

    [field: SerializeField, Header("選択中であることを表すカーソル")]
    private Image selectingCursor;

    /// <summary>
    /// 選択中か
    /// </summary>
    private bool isSelecting = false;

    private SalesCardManager salesCardManager;

    public int SalesPrice { get { return card.ThisCardData.SalesPrice;} }

    public CardData CardData { get { return card.ThisCardData;} }

    private void Update()
    {
        selectingCursor.gameObject.SetActive(isSelecting);
    }

    /// <summary>
    /// 選択状態にする(UIの変更)
    /// </summary>
    public void Select()
    {
        isSelecting = true;
    }

    /// <summary>
    /// 選択解除状態にする(UIの変更)
    /// </summary>
    public void Unselect()
    {
        isSelecting = false;
    }

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