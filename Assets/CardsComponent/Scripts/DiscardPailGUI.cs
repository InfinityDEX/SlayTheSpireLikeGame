using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// 捨て札UIクラス
/// </summary>
public class DiscardPailGUI : MonoBehaviour
{
    [SerializeField, Header("捨て札数")]
    private TextMeshProUGUI counter;

    [SerializeField, Header("カードビューア有効化ボタン")]
    private Button cardViewerEnableButton;

    /// <summary>
    /// デッキマネージャー
    /// </summary>
    private DeckManager deckManager;

    /// <summary>
    /// 依存性注入
    /// </summary>
    /// <param name="deckManager">デッキマネージャー</param>
    [Inject]
    public void Init(DeckManager deckManager)
    {
        this.deckManager = deckManager;
    }

    private void Start()
    {
        cardViewerEnableButton.onClick.AddListener(() => deckManager.OpenDiscardPileView());
    }

    private void Update()
    {
        counter.text = $"{deckManager.DiscardPileCount}";
    }
}
