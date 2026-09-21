using UnityEngine;

public class CardDragHandler : MonoBehaviour
{
    [SerializeField, Header("ドラッグ先のレイヤー")]
    private LayerMask targetLayers = ~0; // 必要なら Layer を絞る

    [SerializeField, Header("カードプレイ判定とする為にカードを動かす必要のある最短距離")]
    private float cardPlayDragDistance = 2;

    /// <summary>
    /// 現在ホールド中のカード実体
    /// </summary>
    private Card holdCard = null;

    /// <summary>
    /// マウスをホールド開始した位置
    /// </summary>
    private Vector3 holdPos;

    /// <summary>
    /// マウスホールド開始位置から掴んでいるカードの中心位置とのオフセット
    /// </summary>
    private Vector3 dragOffset;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button down");

            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 point = new Vector2(world.x, world.y);
            Collider2D hit = Physics2D.OverlapPoint(point, targetLayers);
            holdCard = hit?.GetComponent<Card>();
            if (holdCard == null) return;
            holdPos = holdCard.transform.position;
            // クリックした位置とのズレを保持（カード中心がマウスに吸い付かない）
            dragOffset = holdPos - world;
        }

        if (Input.GetMouseButton(0) && holdCard != null)
        {
            Vector3 world = GetMouseWorldPosition();
            world.z = 0; // z座標は変更する必要がないのでそのまま
            holdCard.transform.position = world + dragOffset;
        }
        if (Input.GetMouseButtonUp(0) && holdCard != null)
        {
            // コストが支払えるか確認して、払えたらカードの効果を適用する。
            if(BattleManager.Instance.EnergyManager.UseEnergy(holdCard.data.cost))
            {
                Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 point = new Vector2(world.x, world.y);

                switch(holdCard.data.target)
                {
                    case CardData.Target.Enemy:
                        Collider2D hit = Physics2D.OverlapPoint(point, targetLayers);
                        var selectCreature = hit?.GetComponent<Creature>();
                        if (selectCreature != null && selectCreature.gameObject.tag == "Enemy")
                        {
                            holdCard.Play(selectCreature);
                            AudioController.Instance?.PlaySE(holdCard.data.castCardSE);
                            GameObject visualEffectPrefab = BattleManager.Instance?.VisualEffectLibrary.GetEffectById(holdCard.data.visualEffectID);
                            // ビジュアルエフェクトを生成
                            if (visualEffectPrefab != null)
                            {
                                GameObject ve = Instantiate(visualEffectPrefab);
                                ve.transform.position = selectCreature.transform.position;
                            }
                            // カードを捨て札に置く
                            DeckManager deckManager = FindObjectOfType<DeckManager>();
                            if (deckManager != null)
                            {
                                deckManager.DiscardCardById(holdCard.id);
                            }
                            else
                            {
                                Debug.LogError("DeckManager が見つかりません。カードを捨て札にできませんでした。");
                            }
                            Destroy(holdCard.gameObject); // カードをゲーム空間から削除
                        }
                        else
                        {
                            // 敵をターゲットに取らなかった場合は使用したコストをもとに戻す
                            BattleManager.Instance.EnergyManager.RecoveryEnergy(holdCard.data.cost);
                        }
                        break;
                    case CardData.Target.Player:
                        if(Vector3.Distance(holdPos, point) >= cardPlayDragDistance)
                        {
                            Debug.Log($"クリックしてからマウスを離すまでのマウスの移動距離：{Vector3.Distance(holdPos, point)}");
                            holdCard.Play(BattleManager.Instance.Player);
                            AudioController.Instance?.PlaySE(holdCard.data.castCardSE);
                            DeckManager deckManager = FindObjectOfType<DeckManager>();
                            if (deckManager != null)
                            {
                                deckManager.DiscardCardById(holdCard.id);
                            }
                            else
                            {
                                Debug.LogError("DeckManager が見つかりません。カードを捨て札にできませんでした。");
                            }
                            Destroy(holdCard.gameObject); // カードをゲーム空間から削除
                        }
                        else
                        {
                            // カードを選択した状態で、指定した長さだけマウスホバーしなかった場合は、カードを使用しなかったものとして
                            // 消費したコストをもとに戻す
                            BattleManager.Instance.EnergyManager.RecoveryEnergy(holdCard.data.cost);
                        }
                        break;
                }
            }
            holdCard.transform.position = holdPos; // 元の位置に戻す
            holdCard = null;
        }
    }

    /// <summary>
    /// ゲーム内のワールド座標系で見たときのマウス座標を取得する
    /// </summary>
    /// <returns>ワールド座標系上のマウス座標</returns>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 screenPos = Input.mousePosition;
        // カードのZ深度を使うと、2Dでも正確に変換できる
        if (holdCard != null)
            screenPos.z = Camera.main.WorldToScreenPoint(holdCard.transform.position).z;
        else
            screenPos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}
