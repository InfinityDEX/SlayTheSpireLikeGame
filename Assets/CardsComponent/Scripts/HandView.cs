using UnityEngine;
using VContainer;

namespace CardsComponent
{
    /// <summary>
    /// 手札の見た目制御クラス
    /// </summary>
    public class HandView : MonoBehaviour
    {
        [field: SerializeField, Header("手札表示エリア(親オブジェクト)")]
        private Transform handArea;

        [field: SerializeField, Header("カードプレハブ")]
        private GameObject cardTemplatePrefab;

        /// <summary>
        /// デッキマネージャー
        /// </summary>
        private DeckManager deckManager;

        /// <summary>
        /// 依存性注入
        /// </summary>
        /// <param name="deckManager">デッキマネージャー</param>
        [Inject]
        public void Init(
            DeckManager deckManager)
        {
            this.deckManager = deckManager;
        }

        /// <summary>
        /// 手札の見た目をリセットする
        /// </summary>
        public void ResetHandCardView()
        {
            // 手札のカードをすべて表示（現状全消し→再生成方式）
            if (handArea != null && cardTemplatePrefab != null)
            {
                // 既存の子オブジェクトを全削除
                foreach (Transform child in handArea)
                {
                    Destroy(child.gameObject);
                }

                // 手札の配置（アーチ状）のパラメータ
                int cardCount = deckManager.HandCount;
                float archRadius = 10f; // 弧の大きさ（調整可。単位はローカル座標）
                float archAngle = 50f; // 全体で使う角度の最大値(度)。180より小
                float angleStep = cardCount > 1 ? archAngle / (cardCount - 1) : 0;
                float startAngle = -archAngle / 2f;

                // 右から順に配置（handの先頭が右端に来る）
                for (int i = 0; i < cardCount; i++)
                {
                    int reverseIndex = cardCount - 1 - i; // handの先頭が右端
                    var handCard = deckManager.Hand[reverseIndex];

                    // プレハブの複製
                    GameObject cardObj = Instantiate(cardTemplatePrefab, handArea);

                    // アーチ上の位置決定
                    float angle = startAngle + angleStep * i;
                    float radians = angle * Mathf.Deg2Rad;
                    Vector3 localPos = new Vector3(
                        Mathf.Sin(radians) * archRadius,
                        Mathf.Cos(radians) * archRadius - archRadius, // 0を中心とするため
                        0f
                    );

                    // カード配置
                    cardObj.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                    cardObj.transform.localPosition = localPos;
                    cardObj.transform.localRotation = Quaternion.Euler(0f, 0f, angle * -0.5f); // カードをアーチの接線方向に少し傾け、自然なファンに

                    // Cardスクリプトの取得
                    Card cardScript = cardObj.GetComponent<Card>();

                    // カード情報を渡す
                    if (cardScript != null)
                    {
                        cardScript.Setup(handCard.cardData, handCard.cardIdx);
                    }
                }
            }
        }
    }
}
