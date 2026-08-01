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
    private EnemyManager enemyManager;

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
        player = playerInspector;
        visualEffectLibrary = visualEffectLibraryInspector;
        energyManager = energyManagerInspector; 
        energyManager.RefreshEnergy();
        AudioClip bgm = BgmSelector.PickBattleBgm(currentStage);
        AudioController.Instance?.PlayBGM(bgm);
        currentPhase = BattlePhase.DrawPhase;
    }


    // バトルのフローチャートに基づき、バトル進行用の状態管理・更新
    // ターン管理や進行をUpdate内から管理（例：状態遷移やループ）
    // 実際はコルーチンやイベントで処理を書いたほうが良いが、ここではシンプルに状態のみ用意

    // 簡易的なバトルステート列挙体
    enum BattlePhase
    {
        DrawPhase,
        UseCardPhase,
        PlayerEndPhase,
        BuffRefreshPhase,
        EnemyActionPhase,
        EndBattle,
        Idle
    }

    [SerializeField, ReadOnly(true)]
    private BattlePhase currentPhase = BattlePhase.Idle;
    private bool isBattleActive = true;

    private void BattleLoop()
    {
        switch (currentPhase)
        {
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
                    currentPhase = BattlePhase.BuffRefreshPhase;
                }
                break;

            case BattlePhase.BuffRefreshPhase:
                // バフや状態異常のリフレッシュ処理
                RefreshBuffs();
                energyManager.RefreshEnergy();
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
        return enemyManager.enemies.All(e => e.hp <= 0);
    }

    // バフのリフレッシュ
    private void RefreshBuffs()
    {
        player.ResetBuff();
        enemyManager.enemies.ForEach(e => e.ResetBuff());
    }

    private bool EnemyAction()
    {
        return enemyManager.enemies.All(e => e.Action() == true);
        // foreach(var enemy in enemyManager.enemies)
        // {
        //     enemy.Action();
        // }
    }

    private void EndBattle()
    {
        // バトル終了アニメ・遷移など

        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
    }
}
