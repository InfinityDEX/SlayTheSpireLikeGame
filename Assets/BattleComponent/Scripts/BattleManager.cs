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

    [field:SerializeField, Header("ダメージバッチジェネレータ")]
    public DamageBatchGenerator DamageBatchGenerator { get; private set;}

    [SerializeField, Header("現在のフェーズ")]
    private BattlePhase currentPhase = BattlePhase.Idle;
    private bool isBattleActive = true;

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
    }

    private void Update()
    {
        Player = Player;
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
                if(EnemyManager.enemies.All(e => e.EndStart()))
                {
                    // 敵の次の行動を示す
                    EnemyManager.enemies.ForEach(e => e.RefreshActionIcon());
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
                EnemyManager.enemies.ForEach(e => e.RefreshActionIcon());

                currentPhase = BattlePhase.DrawPhase;
                break;

            case BattlePhase.EndBattle:
                // バトル終了処理
                isBattleActive = false;
                int result;
                bool playerDead = IsPlayerDead();
                bool enemiesDefeated = AreEnemiesDefeated();
                if (enemiesDefeated && !playerDead)
                {
                    result = 0;
                }
                else if (playerDead)
                {
                    result = 1;
                }
                else
                {
                    result = 2;
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
            deckManager.DrawCard();
            deckManager.DrawCard();
            deckManager.DrawCard();
            deckManager.DrawCard();
            deckManager.DrawCard();
        }

        // 手札のカードをすべて表示（現状全消し→再生成方式）
        if (handArea != null && cardTemplatePrefab != null)
        {
            // 既存の子オブジェクトを全削除
            foreach (Transform child in handArea)
            {
                Destroy(child.gameObject);
            }

            // 手札の配置（アーチ状）のパラメータ
            int cardCount = deckManager.hand.Count;
            float archRadius = 10f; // 弧の大きさ（調整可。単位はローカル座標）
            float archAngle = 50f; // 全体で使う角度の最大値(度)。180より小
            float angleStep = cardCount > 1 ? archAngle / (cardCount - 1) : 0;
            float startAngle = -archAngle / 2f;

            // 右から順に配置（handの先頭が右端に来る）
            for (int i = 0; i < cardCount; i++)
            {
                int reverseIndex = cardCount - 1 - i; // handの先頭が右端
                var handCard = deckManager.hand[reverseIndex];

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
                    cardScript.Setup(handCard.cardData, handCard.cardId);
                }
            }
        }
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
        Debug.Log($"プレイヤーのHP：{Player.hp}");
        return Player.hp <= 0;
    }

    /// <summary>
    /// エネミーの生存チェック
    /// </summary>
    /// <returns>全てのエネミーが死んでいるか？</returns>
    private bool AreEnemiesDefeated()
    {
        return EnemyManager.enemies.All(e => e.hp <= 0);
    }

    /// <summary>
    /// プレイヤーにかかっているバフのリフレッシュ
    /// </summary>
    private void RefreshPlayerBuffs()
    {
        Player.ResetBuff();
    }

    /// <summary>
    /// エネミーにかかっているバフのリフレッシュ
    /// </summary>
    private void RefreshEnemiesBuffs()
    {
        EnemyManager.enemies.ForEach(e => e.ResetBuff());
    }
    
    private int currentActionEnemy = 0;
    
    /// <summary>
    /// エネミーの行動処理
    /// </summary>
    /// <returns>エネミーの行動がすべて終わったか？</returns>
    private bool EnemyAction()
    {
        if (EnemyManager.enemies[currentActionEnemy].hp == 0 || EnemyManager.enemies[currentActionEnemy].Action())
        {
            currentActionEnemy++;
            if (EnemyManager.enemies.Count <= currentActionEnemy)
            {
                currentActionEnemy = 0;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// バトル終了処理
    /// </summary>
    /// <param name="clearFlag">バトル終了要因（０：プレイヤーの勝利　1：プレイヤー敗北）</param>
    private void EndBattle(int clearFlag)
    {
        // TODO：バトル終了アニメ・遷移など

        // ゲームデータの保存
        SaveData saveData = new();
        saveData.MaxHelth = Player.maxHealth;
        saveData.CurrentHelth = Player.hp;
        string json = JsonUtility.ToJson(saveData, true);
        string saveFilePath = System.IO.Path.Combine(Application.dataPath, "SaveData/SaveData.json");
        System.IO.File.WriteAllText(saveFilePath, json);

        // シーン遷移分岐
        if (clearFlag == 0)
        {
            // マップシーンに移動
            SceneManager.LoadScene("MapScene", LoadSceneMode.Single);
        }
        else if (clearFlag == 1)
        {
            // ゲームオーバーシーンに移動
            string mapSaveFilePath = System.IO.Path.Combine(Application.dataPath, MapManager.saveDataFilePath);
            if (System.IO.File.Exists(mapSaveFilePath))
            {
                // マップ進行データを破棄して、次回プレイ時に初めからゲームが開始される状態にする
                System.IO.File.Delete(mapSaveFilePath);
            }
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
        }
        else 
        {
            Debug.LogError("想定外の理由によるゲームオーバーが発生しました。clearFlag: " + clearFlag);
            SceneManager.LoadScene("GameOverScene", LoadSceneMode.Single);
        }
    }
}
