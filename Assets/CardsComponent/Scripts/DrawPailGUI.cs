using UnityEngine;
using TMPro;
using VContainer;
using UnityEngine.UI;

namespace CardsComponent
{
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
        /// デッキ情報
        /// </summary>
        private DeckInfomation deckInfomation;

        /// <summary>
        /// 依存性注入
        /// </summary>
        /// <param name="deckManager">デッキマネージャー</param>
        [Inject]
        public void Init(DeckManager deckManager, DeckInfomation deckInfomation)
        {
            this.deckManager = deckManager;
            this.deckInfomation = deckInfomation;
        }

        private void Start()
        {
            cardViewerEnableButton.onClick.AddListener(() => deckManager.OpenDrawPileListView());
        }

        private void Update()
        {
            counter.text = $"{deckInfomation.DrawPileCount}";
        }
    }
}
