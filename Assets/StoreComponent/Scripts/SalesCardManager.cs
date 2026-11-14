using System.Collections.Generic;
using UnityEngine;
using VContainer;
// using VContainer;

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

    /// <summary>
    /// カードデータライブラリ
    /// </summary>
    [Inject]
    public CardDataLibrary CardDataLibraryInstance { get; private set; }

    /// <summary>
    /// デッキマネージャー
    /// </summary>
    [Inject]
    public DeckManager DeckManagerInstance { get; private set; }

    /// <summary>
    /// セーブデータホルダー
    /// </summary>
    [Inject]
    public SaveDataHolder SaveDataHolderInstance { get; private set; }


    private void Start()
    {
        salesCards = new();

        ChangeShowcaseLineup();
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
    /// ※StoreManagerがショップのボタンの機能や料金を管理しているため、
    /// この関数はボタンに直接紐づけない。StoreManager側でこのメソッドのラッパーを作成して、
    /// カード購入ボタンを押したときにStoreManager側のラッパー経由で本処理を発火する。
    /// </summary>
    public void BuySelectedCard()
    {
        if (selectedCard != null)
        {
            if (SaveDataHolderInstance.UseMoney(selectedCard.SalesPrice) == true)
            {
                DeckManagerInstance.AddCard(selectedCard.CardData);
                DeckManagerInstance.WriteDeckData();

                salesCards.Remove(selectedCard);
                Destroy(selectedCard.gameObject);
            }
        }
    }

    /// <summary>
    /// 店に展示されているカードのラインナップを変更する
    /// </summary>
    public void ChangeShowcaseLineup()
    {
        // すべてのカードを非アクティブ化する
        salesCards.ForEach(card => card.gameObject.SetActive(false));
        // 9個のランダムなカードデータを抽出
        var cardDatas = CardDataLibraryInstance.GetRandomCards(9);
        foreach (var cardData in cardDatas)
        {
            // TODO：previewCardPrefabにSalesCardViewとSalesCardが登録されていることを前提にしているが、
            // もう少し安全に上記クラスのインスタンスを受け取れるようにしたい。

            // 再利用可能なカードが無いか確認
            var salesCardObj = salesCards.Find(card => card.gameObject.activeSelf == false)?.gameObject;
            var salesCard = salesCardObj?.GetComponent<SalesCard>();

            // 再利用可能なオブジェクトがなければ新規で作成する
            if(salesCardObj == null)
            {
                salesCardObj = Instantiate(previewCardPrefab, transform);
                salesCard = salesCardObj?.GetComponent<SalesCard>(); // nullになっているはずなので、もう一度salesCardを取得しなおす。
                if (salesCard != null) salesCards.Add(salesCard); // 新規作成した展示カードオブジェクトをオブジェクトプールに追加
            }
            salesCardObj.SetActive(true);
            // カード本体のデータの更新
            var salesCardView = salesCardObj.GetComponent<SalesCardView>();
            salesCardView.SetSalesCardData(cardData);

            // 販売カードオブジェクトの初期化(値札等の表示更新)
            salesCard?.Init(this);
        }
    }
}
