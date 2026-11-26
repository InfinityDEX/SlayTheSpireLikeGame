using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using CardsComponent;
using CreaturesComponent;
using DungeonMapComponent;
using SaveDataComponent;
using VisualEffectsComponent;

namespace BattleComponent
{
    /// <summary>
    /// バトルマネージャクラス
    ///
    /// バトルの進行や、プレイヤーおよびエネミーの死亡判定を行う。
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [field: SerializeField, Header("プレイヤー")]
        private Creature player;

        [field:SerializeField, Header("エナジー管理オブジェクト")]
        private EnergyManager energyManager;

        [field: SerializeField, Header("リワード画面マネージャ")]
        private BattleRewardManager battleRewardManager;

        [SerializeField, Header("現在のフェーズ")]
        private BattlePhase currentPhase = BattlePhase.Idle;

        /// <summary>
        /// 一時的なダンジョンデータ
        /// </summary>
        private TemporaryMapData temporaryMapData;

        /// <summary>
        /// カードデータライブラリ
        /// </summary>
        private CardDataLibrary cardDataLibrary;

        /// <summary>
        /// デッキマネージャー
        /// </summary>
        private DeckManager deckManager;

        /// <summary>
        /// エネミーマネージャー
        /// </summary>
        private EnemyManager enemyManager;

        /// <summary>
        /// プレイヤーマネージャー
        /// </summary>
        private PlayerManager playerManager;

        /// <summary>
        /// セーブデータホルダー
        /// </summary>
        private SaveDataHolder saveDataHolder;

        /// <summary>
        /// エネミー組み合わせデータマネージャー
        /// </summary>
        private EnemyCombinationsDataManager enemyCombinationsDataManager;

        /// <summary>
        /// 手札の見た目制御コンポーネント
        /// </summary>
        private HandView handView;

        /// <summary>
        /// 依存性注入
        /// </summary>
        /// <param name="temporaryMapData">仮マップデータ</param>
        /// <param name="cardDataLibrary">カードデータライブラリ</param>
        /// <param name="deckManager">デッキマネージャー</param>
        /// <param name="enemyManager">エネミーマネージャー</param>
        /// <param name="playerManager">プレイヤーマネージャー</param>
        /// <param name="saveDataHolder">セーブデータマネージャー</param>
        /// <param name="enemyCombinationsDataManager">エネミー組み合わせデータマネージャー</param>
        /// <param name="handView">手札の見た目制御コンポーネント</param>
        [Inject]
        public void Init(
            TemporaryMapData temporaryMapData,
            CardDataLibrary cardDataLibrary,
            DeckManager deckManager,
            EnemyManager enemyManager,
            PlayerManager playerManager,
            SaveDataHolder saveDataHolder,
            EnemyCombinationsDataManager enemyCombinationsDataManager,
            HandView handView)
        {
            this.temporaryMapData = temporaryMapData;
            this.cardDataLibrary = cardDataLibrary;
            this.deckManager = deckManager;
            this.enemyManager = enemyManager;
            this.playerManager = playerManager;
            this.saveDataHolder = saveDataHolder;
            this.enemyCombinationsDataManager = enemyCombinationsDataManager;
            this.handView = handView;
        }

        /// <summary>
        /// バトル中かどうか
        /// </summary>
        private bool isBattleActive = true;

        /// <summary>
        /// 最初のアップデート処理か？
        /// </summary>
        private bool isFirstUpdate = true;

        private void Start()
        {
            // プレイヤーマネージャーにプレイヤーをセット(カード効果モジュールに渡す用)
            playerManager.SetPlayer(player);

            // エナジーの初期化（回復）
            energyManager.RefreshEnergy();

            // EnemyCombinationDataManagerから今回生成するエネミー群データを取得してEnemyManagerもセットする
            enemyManager.SetEnemy(enemyCombinationsDataManager.GetData());

            // バトルフェーズを初期化フェーズにする
            currentPhase = BattlePhase.InitializePhase;
        }

        private void Update()
        {
            if (isFirstUpdate)
            {
                // プレイヤーの体力値をセーブデータの値で更新する際、
                // PlayerインスタンスのStart処理の完了を待たないといけない為、
                // ここで一回だけＨＰの書き換え処理を行う。

                // プレイヤーの体力値をセーブデータから取得
                if (saveDataHolder != null)
                    player.SetHealth(saveDataHolder.SaveData.currentHealth);
                isFirstUpdate = false;

                // デッキデータのクリーンナップ(シャッフルも行う)
                deckManager.ClearDeck();
            }

            // セーブデータエンティティの更新
            saveDataHolder?.SetMaxHealth(player.MaxHealth);
            saveDataHolder?.SetCurrentHealth(player.Hp);

            // バトルループ
            if (isBattleActive)
            {
                BattleLoop();
            }
        }

        // バトルのフローチャートに基づき、バトル進行用の状態管理・更新
        // ターン管理や進行をUpdate内から管理（例：状態遷移やループ）

        // 簡易的なバトルステート列挙体
        enum BattlePhase
        {
            InitializePhase,
            DrawPhase,
            UseCardPhase,
            PlayerEndPhase,
            PlayerBuffRefreshPhase,
            EnemyBuffRefreshPhase,
            EnemyActionPhase,
            EndBattle,
            Idle
        }

        /// <summary>
        /// バトルループ
        /// </summary>
        private void BattleLoop()
        {
            switch (currentPhase)
            {
                case BattlePhase.InitializePhase:
                    // 全ての敵クリーチャーがStart処理を終えるまで待つ
                    if(enemyManager.Enemies.All(e => e.IsEndStartFunc()))
                    {
                        // 敵の次の行動を示す
                        enemyManager.Enemies.ForEach(e => e.RefreshActionIcon());
                        // ドローフェーズへ
                        currentPhase = BattlePhase.DrawPhase;
                    }
                    break;

                case BattlePhase.DrawPhase:
                    // ドローフェーズ開始処理
                    StartPlayerTurn();
                    // プレイヤーカード使用フェーズへ
                    currentPhase = BattlePhase.UseCardPhase;
                    break;

                case BattlePhase.UseCardPhase:
                    // プレイヤーor敵の死亡チェック例（具体的な判定は実装に応じて）
                    if (IsPlayerDead() || AreEnemiesDefeated())
                    {
                        currentPhase = BattlePhase.EndBattle;
                    }
                    break;

                case BattlePhase.PlayerEndPhase:
                    // プレイヤーエンドフェーズ
                    deckManager.DiscardHandCardAll(); // 手札全捨て処理関数
                    // 敵の行動フェーズへ
                    currentPhase = BattlePhase.EnemyBuffRefreshPhase;
                    break;

                case BattlePhase.EnemyBuffRefreshPhase:
                    RefreshEnemiesBuffs();
                    // 敵の行動フェーズへ
                    currentPhase = BattlePhase.EnemyActionPhase;
                    break;

                case BattlePhase.EnemyActionPhase:

                    // プレイヤーor敵の死亡チェック
                    if (IsPlayerDead() || AreEnemiesDefeated())
                    {
                        currentPhase = BattlePhase.EndBattle;
                    }
                    // 敵の行動処理
                    else if(EnemyAction())
                    {
                        // もう1ターン続行
                        currentPhase = BattlePhase.PlayerBuffRefreshPhase;
                    }
                    break;

                case BattlePhase.PlayerBuffRefreshPhase:
                    // バフや状態異常のリフレッシュ処理
                    RefreshPlayerBuffs();
                    energyManager.RefreshEnergy();

                    // 敵の次の行動を示す
                    enemyManager.Enemies.ForEach(e => e.RefreshActionIcon());

                    currentPhase = BattlePhase.DrawPhase;
                    break;

                case BattlePhase.EndBattle:
                    // バトル終了処理
                    isBattleActive = false;
                    EndBattleFactor result = EndBattleFactor.ERROR;
                    bool playerDead = IsPlayerDead();
                    bool enemiesDefeated = AreEnemiesDefeated();
                    if (enemiesDefeated && !playerDead)
                    {
                        if (temporaryMapData.CurrentFloorIsFinal)
                        {
                            result = EndBattleFactor.GameClear;
                        }
                        else
                        {
                            result = EndBattleFactor.PlayerWin;
                        }
                    }
                    else if (playerDead)
                    {
                        result = EndBattleFactor.PlayerLoss;
                    }
                    EndBattle(result);
                    currentPhase = BattlePhase.Idle;
                    break;

                case BattlePhase.Idle:
                default:
                    // 何もしない
                    break;
            }
        }

        /// <summary>
        /// プレイヤーターンの開始時処理
        /// </summary>
        private void StartPlayerTurn()
        {
            // プレイヤーターン開始時の処理: カードを5枚引く
            for (int i = 0; i < 5; i++)
                deckManager.DrawCard();

            // 手札の見た目を修整
            handView.ResetHandCardView();

            // TODO：今後、他のプレイヤーターン開始処理をここに追加予定
        }

        /// <summary>
        /// プレイヤーターン終了処理
        /// </summary>
        public void EndPlayerTurn(){
            Debug.Log("ターン終了");
            if(currentPhase == BattlePhase.UseCardPhase)
            {
                currentPhase = BattlePhase.PlayerEndPhase;
            }
        }

        /// <summary>
        /// バトル終了判定／敵全滅判定など
        /// </summary>
        /// <returns></returns>
        private bool IsPlayerDead()
        {
            Debug.Log($"プレイヤーのHP：{player.Hp}");
            return player.Hp <= 0;
        }

        /// <summary>
        /// エネミーの生存チェック
        /// </summary>
        /// <returns>全てのエネミーが死んでいるか？</returns>
        private bool AreEnemiesDefeated()
        {
            enemyManager.Enemies.RemoveAll(enemy => enemy == null);

            // 体力が0ならクリーチャを破壊する
            foreach (var enemy in enemyManager.Enemies)
            {
                if (enemy.Hp <= 0)
                {
                    // EnemyManager.Enemies.Remove(enemy);
                    // 所持金追加
                    var enemyData = enemy.CreatureData;
                    if (enemyData is EnemyData)
                    {
                        saveDataHolder.AddMoney(((EnemyData)enemyData).gold);
                    }
                    Destroy(enemy.gameObject);
                }
            }


            return enemyManager.Enemies.Count == 0;
        }

        /// <summary>
        /// プレイヤーにかかっているバフのリフレッシュ
        /// </summary>
        private void RefreshPlayerBuffs()
        {
            player.ResetBlock();
        }

        /// <summary>
        /// エネミーにかかっているバフのリフレッシュ
        /// </summary>
        private void RefreshEnemiesBuffs()
        {
            enemyManager.Enemies.ForEach(e => e.ResetBlock());
        }

        private int currentActionEnemy = 0;

        /// <summary>
        /// エネミーの行動処理
        /// </summary>
        /// <returns>エネミーの行動がすべて終わったか？</returns>
        private bool EnemyAction()
        {
            if (enemyManager.Enemies[currentActionEnemy].Hp == 0 || enemyManager.Enemies[currentActionEnemy].Action())
            {
                currentActionEnemy++;
                if (enemyManager.Enemies.Count <= currentActionEnemy)
                {
                    currentActionEnemy = 0;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ゲーム終了要因
        /// </summary>
        private enum EndBattleFactor
        {
            PlayerWin,
            PlayerLoss,
            GameClear,
            ERROR,
        }
        /// <summary>
        /// バトル終了処理
        /// </summary>
        /// <param name="clearFlag">バトル終了要因</param>
        private void EndBattle(EndBattleFactor clearFlag)
        {
            // ダンジョンマップファイルのパスを取得
            string mapSaveFilePath = Path.Combine(Application.persistentDataPath, $"{DungeonMapManager.saveDataFolderPath}/{DungeonMapManager.saveDataFileName}");

            // バトル終了要因によって分岐
            switch (clearFlag)
            {
                //////////////////////////////
                /// プレイヤーが勝利した時
                //////////////////////////////
                case EndBattleFactor.PlayerWin:
                    // ゲームデータの保存
                    saveDataHolder.WriteSaveData();

                    List<CardData> cardDatas = cardDataLibrary.GetRandomCards(3);

                    battleRewardManager?.EnableRewardView(cardDatas);

                    // デッキをリセット（全てのカードをデッキに戻す）
                    deckManager.ClearDeck();

                    // 一時的な情報としていたダンジョンマップデータを保存する
                    temporaryMapData.SaveDungeonMapData();

                    break;
                //////////////////////////////
                /// プレイヤーが敗北した時
                //////////////////////////////
                case EndBattleFactor.PlayerLoss:
                    // ゲームデータの破棄（はじめから）
                    DeleteGameData();

                    // ゲームオーバーシーンに移動
                    SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
                    break;
                //////////////////////////////
                /// プレイヤーが最終階層のボスを倒してクリアした時
                //////////////////////////////
                case EndBattleFactor.GameClear:
                    // ゲームデータの破棄（はじめから）
                    DeleteGameData();

                    // ゲームクリアシーンに移動
                    SceneManager.LoadScene("GameClearScene", LoadSceneMode.Additive);
                    break;
                //////////////////////////////
                /// エラー。適切なバトル終了フラグが割り振られていない
                //////////////////////////////
                case EndBattleFactor.ERROR:
                    Debug.LogError("想定外の理由によるゲームオーバーが発生しました。clearFlag: " + clearFlag);
                    SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
                    break;
            }
        }

        /// <summary>
        /// ゲームデータの削除
        /// </summary>
        private void DeleteGameData()
        {
            // プレイヤー情報の削除
            saveDataHolder.DeleteSaveDataFile();
            saveDataHolder.ClearSaveData();

            // ダンジョンマップファイルのパスを取得
            string mapSaveFilePath = Path.Combine(Application.persistentDataPath, $"{DungeonMapManager.saveDataFolderPath}/{DungeonMapManager.saveDataFileName}");

            // マップ情報の削除
            if (File.Exists(mapSaveFilePath))
            {
                // マップ進行データを破棄して、次回プレイ時に初めからゲームが開始される状態にする
                File.Delete(mapSaveFilePath);
            }
            // デッキデータファイルの削除
            deckManager.DeleteDeckData();
        }
    }
}
