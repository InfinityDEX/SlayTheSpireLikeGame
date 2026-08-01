using System;
using UnityEngine;

public class CardDragHandler : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers = ~0; // 必要なら Layer を絞る
    [SerializeField] private float cardPlayDragDistance = 2; // カードを使用する為にカードを動かす必要のある最短距離

    // [SerializeField] private AudioClip cardPlaySound;
    private Card holdCard = null;
    private Vector3 dragOffset;
    private Vector3 holdPos;

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
            if(BattleManager.Instance.energyManager.UseEnergy(holdCard.data.cost))
            {
                Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 point = new Vector2(world.x, world.y);

                switch(holdCard.data.target)
                {
                    case Target.Enemy:
                        Collider2D hit = Physics2D.OverlapPoint(point, targetLayers);
                        var selectCreature = hit?.GetComponent<Creature>();
                        if (selectCreature != null && selectCreature.gameObject.tag == "Enemy")
                        {
                            holdCard.Play(selectCreature);
                            AudioController.Instance?.PlaySE(holdCard.data.castCardSE);
                            GameObject visualEffectPrefab = BattleManager.Instance?.visualEffectLibrary.GetEffectById(holdCard.data.visualEffectID);
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
                            BattleManager.Instance.energyManager.AddEnergy(holdCard.data.cost);
                        }
                        break;
                    case Target.Player:
                        if(Vector3.Distance(holdPos, point) >= cardPlayDragDistance)
                        {
                            Debug.Log($"クリックしてからマウスを離すまでのマウスの移動距離：{Vector3.Distance(holdPos, point)}");
                            holdCard.Play(BattleManager.Instance.player);
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
                        break;
                }
            }
            holdCard.transform.position = holdPos; // 元の位置に戻す
            holdCard = null;
        }
    }

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
