using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// バトルマネージャクラス
///
/// バトルの進行や、プレイヤーおよびエネミーの死亡判定を行う。
/// シングルトン。
/// </summary>
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [field:SerializeField, Header("デッキマネージャー")]
    private DeckManager deckManager;

    [field:SerializeField, Header("手札表示エリア(親オブジェクト)")]
    private Transform handArea;

    [field:SerializeField, Header("敵マネージャー")]
    public EnemyManager EnemyManager { get; private set;}

    [field:SerializeField, Header("カードプレハブ")]
    private GameObject cardTemplatePrefab;

    [field:SerializeField, Header("プレイヤー")]
    public Creature Player { get; private set;}

    [field:SerializeField, Header("エナジー管理オブジェクト")]
    public EnergyManager EnergyManager { get; private set;}

    [field:SerializeField, Header("ビジュアルエフェクト一覧")]
    public VisualEffectLibrary VisualEffectLibrary { get; private set;}

    [field:SerializeField, Header("ダメージポップアップジェネレータ")]
    public DamagePopupGenerator DamagePopupGenerator { get; private set;}

    [field:SerializeField, Header("リワード画面マネージャ")]
    public BattleRewardManager battleRewardManager { get; private set;}

    [SerializeField, Header("現在のフェーズ")]
    private BattlePhase currentPhase = BattlePhase.Idle;

    /// <summary>
    /// 一時的なダンジョンデータ
    /// </summary>
    private TemporaryMapData temporaryMapData;

    /// <summary>
    /// バトル中かどうか
    /// </summary>
    private bool isBattleActive = true;

    /// <summary>
    /// 最初のアップデート処理か？
    /// </summary>
    private bool isFirstUpdate = true;

    private void Awake()
    {
        // シングルトン違反判定
        if (Instance != null && Instance != this)
        {
            Debug.LogError("BattleManagerが複数存在しようとしています。");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // エナジーの初期化（回復）
        EnergyManager.RefreshEnergy();

        // EnemyCombinationDataManagerから今回生成するエネミー群データを取得してEnemyManagerもセットする
        EnemyManager.SetEnemy(EnemyCombinationsDataManager.Instance.GetData());

        // バトルフェーズを初期化フェーズにする
        currentPhase = BattlePhase.InitializePhase;

        // 一時的なダンジョンデータを取得する。
        temporaryMapData = GameObject.Find("TemporaryDungeonMapData")?.GetComponent<TemporaryMapData>();
    }

    private void Update()
    {
        // プレイヤーの体力値をセーブデータの値で更新する際、
        // PlayerインスタンスのStart処理の完了を待たないといけない為、
        // ここで一回だけＨＰの書き換え処理を行う。
        if (isFirstUpdate)
        {
            // プレイヤーの体力値をセーブデータから取得
            if (SaveDataHolder.Instance != null)
                Player.SetHealth(SaveDataHolder.Instance.SaveData.currentHealth);
            isFirstUpdate = false;
        }

        // セーブデータエンティティの更新
        SaveDataHolder.Instance?.SetMaxHealth(Player.MaxHealth);
        SaveDataHolder.Instance?.SetCurrentHealth(Player.Hp);

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
                if(EnemyManager.Enemies.All(e => e.IsEndStartFunc()))
                {
                    // 敵の次の行動を示す
                    EnemyManager.Enemies.ForEach(e => e.RefreshActionIcon());
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
                deckManager?.DiscardCardAll(); // 手札全捨て処理関数
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
                EnergyManager.RefreshEnergy();

                // 敵の次の行動を示す
                EnemyManager.Enemies.ForEach(e => e.RefreshActionIcon());

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
        if (deckManager != null)
        {
            for (int i = 0; i < 5; i++)
            deckManager.DrawCard();
        }

        // 手札の見た目を修整
        ResetHandCardView();

        // TODO：今後、他のプレイヤーターン開始処理をここに追加予定
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
            int cardCount = deckManager.Hand.Count;
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
        Debug.Log($"プレイヤーのHP：{Player.Hp}");
        return Player.Hp <= 0;
    }

    /// <summary>
    /// エネミーの生存チェック
    /// </summary>
    /// <returns>全てのエネミーが死んでいるか？</returns>
    private bool AreEnemiesDefeated()
    {
        EnemyManager.Enemies.RemoveAll(enemy => enemy == null);

        // 体力が0ならクリーチャを破壊する
        foreach (var enemy in EnemyManager.Enemies)
        {
            if (enemy.Hp <= 0)
            {
                // EnemyManager.Enemies.Remove(enemy);
                // 所持金追加
                var enemyData = enemy.CreatureData;
                if (enemyData is EnemyData)
                {
                    SaveDataHolder.Instance.AddMoney(((EnemyData)enemyData).gold);
                }
                Destroy(enemy.gameObject);
            }
        }


        return EnemyManager.Enemies.Count == 0;
    }

    /// <summary>
    /// プレイヤーにかかっているバフのリフレッシュ
    /// </summary>
    private void RefreshPlayerBuffs()
    {
        Player.ResetBlock();
    }

    /// <summary>
    /// エネミーにかかっているバフのリフレッシュ
    /// </summary>
    private void RefreshEnemiesBuffs()
    {
        EnemyManager.Enemies.ForEach(e => e.ResetBlock());
    }

    private int currentActionEnemy = 0;

    /// <summary>
    /// エネミーの行動処理
    /// </summary>
    /// <returns>エネミーの行動がすべて終わったか？</returns>
    private bool EnemyAction()
    {
        if (EnemyManager.Enemies[currentActionEnemy].Hp == 0 || EnemyManager.Enemies[currentActionEnemy].Action())
        {
            currentActionEnemy++;
            if (EnemyManager.Enemies.Count <= currentActionEnemy)
            {
                currentActionEnemy = 0;
                return true;
            }
        }
        return false;
    }

    // ゲーム終了要因
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
        string mapSaveFilePath = System.IO.Path.Combine(Application.dataPath, DungeonMapManager.saveDataFilePath);
        switch (clearFlag)
        {
            case EndBattleFactor.PlayerWin:
                // ゲームデータの保存
                SaveDataHolder.Instance?.WriteSaveData();

                List<CardData> cardDatas = CardDataLibrary.Instance.GetRandomCards(3);

                battleRewardManager?.EnableRewardView(cardDatas);

                // 一時的な情報としていたダンジョンマップデータを保存する
                temporaryMapData.SaveDungeonMapData();

                break;
            case EndBattleFactor.PlayerLoss:
                // ゲームデータの破棄（はじめから）

                // 進行情報削除
                SaveDataHolder.Instance?.DeleteSaveDataFile();
                // マップ情報の削除
                if (System.IO.File.Exists(mapSaveFilePath))
                {
                    // マップ進行データを破棄して、次回プレイ時に初めからゲームが開始される状態にする
                    System.IO.File.Delete(mapSaveFilePath);
                }
                // デッキ情報の削除
                DeckManager.Instance.DeleteDeckData();

                // ゲームオーバーシーンに移動
                SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
                break;
            case EndBattleFactor.GameClear:
                // ゲームデータの破棄（はじめから）

                // 進行情報削除
                SaveDataHolder.Instance?.DeleteSaveDataFile();
                // マップ情報の削除
                if (System.IO.File.Exists(mapSaveFilePath))
                {
                    // マップ進行データを破棄して、次回プレイ時に初めからゲームが開始される状態にする
                    System.IO.File.Delete(mapSaveFilePath);
                }
                // デッキ情報の削除
                DeckManager.Instance.DeleteDeckData();

                // ゲームクリアシーンに移動
                SceneManager.LoadScene("GameClearScene", LoadSceneMode.Additive);
                break;
            case EndBattleFactor.ERROR:
                Debug.LogError("想定外の理由によるゲームオーバーが発生しました。clearFlag: " + clearFlag);
                SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
                break;
        }
        // このままだとシーンを跨いでも残り続けてしまうので破壊する
        SceneManager.MoveGameObjectToScene(temporaryMapData.gameObject, SceneManager.GetActiveScene());
        Destroy(temporaryMapData.gameObject);
    }
}
