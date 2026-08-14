using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("ステージ情報")]
    [SerializeField] 
    private StageData currentStage;

    [Header("デッキマネージャー")]
    [SerializeField] 
    private DeckManager deckManager;

    [Header("手札表示エリア(親オブジェクト)")]
    [SerializeField]
    private Transform handArea;

    [Header("敵マネージャー")]
    [SerializeField]
    private EnemyManager enemyManagerInspector;
    public EnemyManager enemyManager { get; private set;}

    [Header("カードプレハブ")]
    [SerializeField]
    private GameObject cardTemplatePrefab;

    [Header("プレイヤー")]
    [SerializeField]
    private Creature playerInspector;
    public Creature player { get; private set;}

    [Header("エナジー管理オブジェクト")]
    [SerializeField]
    private EnergyManager energyManagerInspector;
    public EnergyManager energyManager { get; private set;}

    [Header("ビジュアルエフェクト一覧")]
    [SerializeField]
    private VisualEffectLibrary visualEffectLibraryInspector;
    public VisualEffectLibrary visualEffectLibrary { get; private set;}

    [Header("ダメージバッチジェネレータ")]
    [SerializeField]
    private DamageBatchGenerator damageBatchGeneratorInspector;
    public DamageBatchGenerator damageBatchGenerator{ get; private set;}

    // [Header("生成する敵情報")]
    // [SerializeField]
    // private EnemyCombinationData enemyCombinationData;

    private void Awake()
    {
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
        damageBatchGenerator = damageBatchGeneratorInspector;
        player = playerInspector;
        visualEffectLibrary = visualEffectLibraryInspector;
        energyManager = energyManagerInspector; 
        enemyManager = enemyManagerInspector;
        energyManager.RefreshEnergy();
        AudioClip bgm = BgmSelector.PickBattleBgm(currentStage);
        AudioController.Instance?.PlayBGM(bgm);
        enemyManagerInspector.SetEnemy(EnemyCombinationsDataManager.Instance.GetData());
        currentPhase = BattlePhase.InitializePhase;
    }

    // バトルのフローチャートに基づき、バトル進行用の状態管理・更新
    // ターン管理や進行をUpdate内から管理（例：状態遷移やループ）
    // 実際はコルーチンやイベントで処理を書いたほうが良いが、ここではシンプルに状態のみ用意

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

    [Header("現在のフェーズ")]
    [SerializeField, ReadOnly(true)]
    private BattlePhase currentPhase = BattlePhase.Idle;
    private bool isBattleActive = true;

    private void BattleLoop()
    {
        switch (currentPhase)
        {
            case BattlePhase.InitializePhase:
                // 全ての敵クリーチャーがStart処理を終えるまで待つ
                if(enemyManagerInspector.enemies.All(e => e.EndStart()))
                {
                    // 敵の次の行動を示す
                    enemyManagerInspector.enemies.ForEach(e => e.RefreshActionIcon());
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
                if (deckManager != null)
                {
                    deckManager.DiscardCardAll(); // 手札全捨て処理関数
                }
                // 敵の行動フェーズへ
                currentPhase = BattlePhase.EnemyBuffRefreshPhase;
                break;
            case BattlePhase.EnemyBuffRefreshPhase:
                RefreshEnemysBuffs();
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
                enemyManagerInspector.enemies.ForEach(e => e.RefreshActionIcon());

                currentPhase = BattlePhase.DrawPhase;
                break;

            case BattlePhase.EndBattle:
                // バトル終了処理
                isBattleActive = false;
                EndBattle();
                currentPhase = BattlePhase.Idle;
                break;

            case BattlePhase.Idle:
            default:
                // 何もしない
                break;
        }
    }

    private void Update()
    {
        player = playerInspector;
        if (isBattleActive)
        {
            BattleLoop();
        }
    }

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

            // アーチのパラメータ
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

                cardObj.transform.localScale = new Vector3(1.5f, 1.5f, 1);

                cardObj.transform.localPosition = localPos;

                // カードをアーチの接線方向に少し傾け、自然なファンに
                cardObj.transform.localRotation = Quaternion.Euler(0f, 0f, angle * -0.5f);

                // Cardスクリプトの取得
                Card cardScript = cardObj.GetComponent<Card>();

                // カード情報を渡す
                if (cardScript != null)
                {
                    cardScript.Setup(handCard.cardData, handCard.cardId);
                }
            }
        }
        // 今後、他のプレイヤーターン開始処理をここに追加予定
    }

    // プレイヤーターンを終了する
    public void EndPlayerTurn(){
        Debug.Log("ターン終了");
        if(currentPhase == BattlePhase.UseCardPhase)
        {
            currentPhase = BattlePhase.PlayerEndPhase;
        }
    }

    // バトル終了判定／敵全滅判定など
    private bool IsPlayerDead()
    {
        Debug.Log($"プレイヤーのHP：{player.hp}");
        return player.hp <= 0;
    }

    // 全敵の生存チェック
    private bool AreEnemiesDefeated()
    {
        return enemyManagerInspector.enemies.All(e => e.hp <= 0);
    }

    // バフのリフレッシュ
    private void RefreshPlayerBuffs()
    {
        player.ResetBuff();
    }
    private void RefreshEnemysBuffs()
    {
        enemyManagerInspector.enemies.ForEach(e => e.ResetBuff());
    }
    
    private int currentActionEnemy = 0;
    private bool EnemyAction()
    {
        if (enemyManagerInspector.enemies[currentActionEnemy].hp == 0 || enemyManagerInspector.enemies[currentActionEnemy].Action())
        {
            currentActionEnemy++;
            if (enemyManagerInspector.enemies.Count <= currentActionEnemy)
            {
                currentActionEnemy = 0;
                return true;
            }
        }
        return false;
    }

    private void EndBattle()
    {
        // バトル終了アニメ・遷移など

        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
    }
}
