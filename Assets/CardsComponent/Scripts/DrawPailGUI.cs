using UnityEngine;
using TMPro;
using VContainer;
using UnityEngine.UI;

/// <summary>
/// 山札GUIクラス
/// </summary>
public class DrawPailGUI : MonoBehaviour
{
    [SerializeField, Header("山札残り枚数")]
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
        cardViewerEnableButton.onClick.AddListener(() => deckManager.OpenDrawPileView());
    }

    private void Update()
    {
        counter.text = $"{deckManager.DrawPileCount}";
    }
}
