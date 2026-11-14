using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using VContainer;

/// <summary>
/// ストア内で実行できるアクションを管理するクラス
/// </summary>
public class StoreManager : MonoBehaviour
{
    [field: SerializeField, Header("体力回復料金")]
    private int recoveryPrice;

    [field: SerializeField, Header("カードアップグレード料金")]
    private int cardUpgradePrice;

    [field: SerializeField, Header("カード除去料金")]
    private int cardDeletePrice;

    [field: SerializeField, Header("販売カード入れ替え料金")]
    private int changeCardLineupPrice;

    /// <summary>
    /// 販売カード管理クラス
    /// </summary>
    [Inject]
    private SalesCardManager salesCardManager;

    /// <summary>
    /// カードビュワー
    /// </summary>
    [Inject]
    private CardViewer cardViewer;

    /// <summary>
    /// 仮のマップデータ
    /// </summary>
    [Inject]
    private TemporaryMapData temporaryMapData;

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

    /// <summary>
    /// 体力全回復
    /// </summary>
    public void FullRecovery()
    {
        if (SaveDataHolderInstance != null)
        {
            if (SaveDataHolderInstance.UseMoney(recoveryPrice) == true)
            {
                SaveDataHolderInstance.FullRecovery();
            }
        }
    }

    /// <summary>
    /// カード強化メニューを開く
    /// </summary>
    public void CardUpgradeMenuOpen()
    {
        if (SaveDataHolderInstance != null && DeckManagerInstance != null && CardDataLibraryInstance != null && cardViewer != null)
        {
            DeckManagerInstance.OpenDrawPileView();
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
                        if (SaveDataHolderInstance.UseMoney(cardUpgradePrice) == false)
                        {
                            // 払えないのであれば何もしない。
                            return;
                        }
                        // 選択したカードのカードIDと最初に一致したカードをアップグレードする
                        DeckManagerInstance.UpgradeCard(card.ThisCardData);

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
        if (SaveDataHolderInstance != null && DeckManagerInstance != null && CardDataLibraryInstance != null && cardViewer != null)
        {
            DeckManagerInstance.OpenDrawPileView();
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
                        if (SaveDataHolderInstance.UseMoney(cardDeletePrice) == false)
                        {
                            // 払えないのであれば何もしない。
                            return;
                        }

                        // 選択したカードを除去する
                        DeckManagerInstance.RemoveCard(card.ThisCardData);

                        // 除去が済んだらビュワーを非表示にする
                        cardViewer.DisableViewer();
                    });
                }
            }
        }
    }

    /// <summary>
    /// カードの購入を試す(SalesCardManagerにある関数のラッパー)
    ///
    /// 購入ができない場合は何も行わない
    /// </summary>
    public void BuySelectedCard()
    {
        if (salesCardManager != null)
        {
            salesCardManager.BuySelectedCard();
        }
    }

    /// <summary>
    /// 店に展示されているカードのラインナップを変更する(SalesCardManagerにある関数のラッパー)
    /// </summary>
    public void ChangeShowcaseLineup()
    {
        if (salesCardManager != null && SaveDataHolderInstance != null && SaveDataHolderInstance.UseMoney(changeCardLineupPrice) == true)
        {
            salesCardManager.ChangeShowcaseLineup();
        }
    }

    /// <summary>
    /// ショップを退店する
    /// </summary>
    public void LeavingStore()
    {
        // 一時的な情報としていたダンジョンマップデータを保存する
        temporaryMapData.SaveDungeonMapData();

        DeckManagerInstance.ReturnAllCardsToDrawPile(); // デッキのリセット
        DeckManagerInstance.WriteDeckData(); // デッキの保存
        SaveDataHolderInstance.WriteSaveData(); // セーブデータのJson保存
        SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
    }
}
