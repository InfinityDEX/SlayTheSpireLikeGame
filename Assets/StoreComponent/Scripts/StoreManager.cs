using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// ストア内で実行できるアクションを管理するクラス
/// </summary>
public class StoreManager : MonoBehaviour
{
    [field: SerializeField, Header("カードビュワー")]
    private CardViewer cardViewer;

    [field: SerializeField, Header("体力回復料金")]
    private int recoveryPrice;

    [field: SerializeField, Header("カードアップグレード料金")]
    private int cardUpgradePrice;

    [field: SerializeField, Header("カード除去料金")]
    private int cardDeletePrice;

    /// <summary>
    /// 体力全回復
    /// </summary>
    public void FullRecovery()
    {
        var saveDataHolder = SaveDataHolder.Instance;
        if (saveDataHolder != null)
        {
            if (saveDataHolder.UseMoney(recoveryPrice) == true)
            {
                saveDataHolder.FullRecovery();
            }
        }
    }

    /// <summary>
    /// カード強化メニューを開く
    /// </summary>
    public void CardUpgradeMenuOpen()
    {
        var saveDataHolder = SaveDataHolder.Instance;
        var deckManager = DeckManager.Instance;
        var cardDataLibrary = CardDataLibrary.Instance;
        if (saveDataHolder != null && deckManager != null && cardDataLibrary != null && cardViewer != null)
        {
            deckManager.OpenDrawPailView();
            var cardList = cardViewer.GetActiveCards();
            foreach (var card in cardList)
            {
                var controller = card.GetComponent<CardOverlayButtonController>();
                if (controller != null)
                {
                    controller.EnableButton();

                    // アップグレード済みのカードは、オーバーレイボタンは表示するが、非活性状態で表示して選択できないことを明示する。
                    if (card.ThisCardData.IsUpgraded)
                    {
                        controller.InActiveButton();
                        continue;
                    }
                    else
                    {
                        controller.ActiveButton();
                    }

                    controller.RegisterOnClickListener(() =>
                    {
                        // 強化料金が支払えるか確認する。
                        if (saveDataHolder.UseMoney(cardUpgradePrice) == false)
                        {
                            // 払えないのであれば何もしない。
                            return;
                        }
                        // 選択したカードのカードIDと最初に一致したカードをアップグレードする
                        // (選択したカードと直接紐づいているものをアップグレードしないが、処理後のデッキ構成は変わらない為これで問題なし)
                        var idx = deckManager.DrawPile.FindIndex(cardData => cardData.CardID == card.ThisCardData.CardID);
                        deckManager.DrawPile[idx] = cardDataLibrary.SearchCardData(deckManager.DrawPile[idx].UpgradedCardId);

                        // 強化が済んだらビュワーを非表示にする
                        cardViewer.DisableViewer();
                    });
                }
            }
        }
    }

    /// <summary>
    /// カード除去メニューを開く
    /// </summary>
    public void CardDeleteMenuOpen()
    {
        var saveDataHolder = SaveDataHolder.Instance;
        var deckManager = DeckManager.Instance;
        var cardDataLibrary = CardDataLibrary.Instance;
        if (saveDataHolder != null && deckManager != null && cardDataLibrary != null && cardViewer != null)
        {
            deckManager.OpenDrawPailView();
            var cardList = cardViewer.GetActiveCards();
            foreach (var card in cardList)
            {
                var controller = card.GetComponent<CardOverlayButtonController>();
                if (controller != null)
                {
                    controller.EnableButton();

                    // アップグレード済みのカードは、オーバーレイボタンは表示するが、非活性状態で表示して選択できないことを明示する。
                    if (card.ThisCardData.IsUpgraded)
                    {
                        controller.InActiveButton();
                        continue;
                    }
                    else
                    {
                        controller.ActiveButton();
                    }

                    controller.RegisterOnClickListener(() =>
                    {
                        // カード除去料金が支払えるか確認する。
                        if (saveDataHolder.UseMoney(cardDeletePrice) == false)
                        {
                            // 払えないのであれば何もしない。
                            return;
                        }
                        // 選択したカードのカードIDと最初に一致したカードを除去する
                        // (選択したカードと直接紐づいているものを除去しないが、処理後のデッキ構成は変わらない為これで問題なし)
                        var idx = deckManager.DrawPile.FindIndex(cardData => cardData.CardID == card.ThisCardData.CardID);
                        deckManager.DrawPile.Remove(deckManager.DrawPile[idx]);

                        // 強化が済んだらビュワーを非表示にする
                        cardViewer.DisableViewer();
                    });
                }
            }
        }
    }

}
