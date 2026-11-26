using UnityEngine;
using VContainer;
using AudioComponent;
using CardsComponent;
using CreaturesComponent;
using VisualEffectsComponent;

namespace BattleComponent
{
    /// <summary>
    /// カードをマウスで掴んで使用する機能を提供するクラス
    /// </summary>
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

        /// <summary>
        /// バトルマネージャー
        /// </summary>
        private BattleManager battleManager;

        /// <summary>
        /// エナジーマネージャー
        /// </summary>
        private EnergyManager energyManager;

        /// <summary>
        /// プレイヤーマネージャー
        /// </summary>
        private PlayerManager playerManager;

        /// <summary>
        /// エネミーマネージャー
        /// </summary>
        private EnemyManager enemyManager;

        /// <summary>
        /// ビジュアルエフェクトライブラリ
        /// </summary>
        private VisualEffectLibrary visualEffectLibrary;

        /// <summary>
        /// オーディオコントローラー
        /// </summary>
        private AudioController audioController;

        /// <summary>
        /// デッキマネージャー
        /// </summary>
        private DeckManager deckManager;

        /// <summary>
        /// 依存性注入
        /// </summary>
        /// <param name="battleManager">バトルマネージャー</param>
        /// <param name="energyManager">エナジーマネージャー</param>
        /// <param name="visualEffectLibrary">ビジュアルエフェクトライブラリ</param>
        /// <param name="playerManager">プレイヤーマネージャー</param>
        /// <param name="enemyManager">エネミーマネージャー</param>
        /// <param name="audioController">オーディオコントローラー</param>
        /// <param name="deckManager">デッキマネージャー</param>
        [Inject]
        public void Init(
            BattleManager battleManager,
            EnergyManager energyManager,
            VisualEffectLibrary visualEffectLibrary,
            PlayerManager playerManager,
            EnemyManager enemyManager,
            AudioController audioController,
            DeckManager deckManager)
        {
            this.battleManager = battleManager;
            this.energyManager = energyManager;
            this.visualEffectLibrary = visualEffectLibrary;
            this.enemyManager = enemyManager;
            this.playerManager = playerManager;
            this.audioController = audioController;
            this.deckManager = deckManager;
        }

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
                if(energyManager.UseEnergy(holdCard.ThisCardData.Cost))
                {
                    Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 point = new Vector2(world.x, world.y);

                    switch(holdCard.ThisCardData.TargetType)
                    {
                        case CardData.Target.OneEnemy:
                            Collider2D hit = Physics2D.OverlapPoint(point, targetLayers);
                            var selectCreature = hit?.GetComponent<Creature>();
                            if (selectCreature != null && selectCreature.gameObject.tag == "Enemy")
                            {
                                // 掴んでいるカードを実行する
                                holdCard.Play(selectCreature);
                                
                                // 音声の再生
                                audioController.PlaySE(holdCard.ThisCardData.CastCardSE);

                                // ビジュアルエフェクトを生成
                                GameObject visualEffectPrefab = visualEffectLibrary.GetEffectById(holdCard.ThisCardData.VisualEffectID);
                                if (visualEffectPrefab != null)
                                {
                                    GameObject ve = Instantiate(visualEffectPrefab);
                                    ve.transform.position = selectCreature.transform.position;
                                }

                                // カードを捨て札に置く
                                deckManager.DiscardCardById(holdCard.HandIdx);
                                
                                // カードをゲーム空間から削除
                                Destroy(holdCard.gameObject);

                                // 手札の配置リセット
                                battleManager.ResetHandCardView();
                            }
                            else
                            {
                                // 敵をターゲットに取らなかった場合は使用したコストをもとに戻す
                                energyManager.RecoveryEnergy(holdCard.ThisCardData.Cost);
                            }
                            break;
                        case CardData.Target.AllEnemy:
                            if(Vector3.Distance(holdPos, point) >= cardPlayDragDistance)
                            {
                                Debug.Log($"クリックしてからマウスを離すまでのマウスの移動距離：{Vector3.Distance(holdPos, point)}");
                                
                                // 全てのエネミーに対してカードの効果を適用する。
                                foreach (var enemy in enemyManager.Enemies)
                                {
                                    // 掴んでいるカードを実行する
                                    holdCard.Play(enemy);
                                }

                                // 音声の再生
                                audioController.PlaySE(holdCard.ThisCardData.CastCardSE);

                                // ビジュアルエフェクトを生成
                                GameObject visualEffectPrefab = visualEffectLibrary.GetEffectById(holdCard.ThisCardData.VisualEffectID);
                                if (visualEffectPrefab != null)
                                {
                                    foreach (var enemy in enemyManager.Enemies)
                                    {
                                        GameObject ve = Instantiate(visualEffectPrefab);
                                        ve.transform.position = enemy.transform.position;
                                    }
                                }
                                
                                // カードを捨て札に置く
                                deckManager.DiscardCardById(holdCard.HandIdx);

                                // カードをゲーム空間から削除
                                Destroy(holdCard.gameObject);

                                // 手札の配置リセット
                                battleManager.ResetHandCardView();
                            }
                            else
                            {
                                // カードを選択した状態で、指定した長さだけマウスホバーしなかった場合は、カードを使用しなかったものとして
                                // 消費したコストをもとに戻す
                                energyManager.RecoveryEnergy(holdCard.ThisCardData.Cost);
                            }
                            break;
                        case CardData.Target.OnePlayer:
                            if(Vector3.Distance(holdPos, point) >= cardPlayDragDistance)
                            {
                                Debug.Log($"クリックしてからマウスを離すまでのマウスの移動距離：{Vector3.Distance(holdPos, point)}");
                                
                                // 掴んでいるカードを実行する
                                Creature player = playerManager.Player;
                                holdCard.Play(player);

                                // 音声の再生
                                audioController.PlaySE(holdCard.ThisCardData.CastCardSE);

                                // ビジュアルエフェクトを生成
                                GameObject visualEffectPrefab = visualEffectLibrary.GetEffectById(holdCard.ThisCardData.VisualEffectID);
                                if (visualEffectPrefab != null)
                                {
                                    GameObject ve = Instantiate(visualEffectPrefab);
                                    ve.transform.position = player.transform.position;
                                }
                            
                                // カードを捨て札に置く
                                deckManager.DiscardCardById(holdCard.HandIdx);

                                // カードをゲーム空間から削除
                                Destroy(holdCard.gameObject);

                                // 手札の配置リセット
                                battleManager.ResetHandCardView();
                            }
                            else
                            {
                                // カードを選択した状態で、指定した長さだけマウスホバーしなかった場合は、カードを使用しなかったものとして
                                // 消費したコストをもとに戻す
                                energyManager.RecoveryEnergy(holdCard.ThisCardData.Cost);
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
}
