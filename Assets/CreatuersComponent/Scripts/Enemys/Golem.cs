using UnityEngine;

public class Golem : Enemy
{
    private float currentTime = 0;
    // パンチまでの時間
    private float punchTime = 0.5f;
    private float defenseTime = 0.5f;
    private bool inAction = false;

    // 行動パターン
    private enum ActionPattern
    {
        Idle = 0, // 待機状態
        Punch = 1, // パンチ
        Defense = 2, // 防御魔法(全体防御)
    }

    [SerializeField]
    private ActionPattern actionPattern;

    [Header("ゴーレムのアニメータ")]
    [SerializeField]
    private Animator animator;

    [Header("パンチダメージ")]
    [SerializeField]
    private int punchDamage = 10;

    [Header("連撃斬撃の回数")]
    [SerializeField]
    private int consecutiveSlashCount = 3;

    private int currentConsecutiveSlashCount = 1;

    [Header("パンチ音")]
    [SerializeField]
    private AudioClip punchSE;

    [Header("防御魔法音")]
    [SerializeField]
    private AudioClip defenseSE;

    [Header("防御魔法で全体に振りまくShield値")]
    [SerializeField]
    private int defenceShieldNum = 10;

    private void Awake()
    {
        // ダメージを受けたときの処理をイベント処理に登録
        RegistTakeDamageEvent(ExcuteTakeDamageAnima);
        actionPattern = GetRandomNonIdleAction();
    }

    private void ExcuteTakeDamageAnima(int damage, int hp)
    {
        animator.SetTrigger("TakeDamage");
    }

    public override void RefreshActionIcon()
    {
        batchManager.DeleteActionBatches();

        IconBatch batch = null;
        switch (actionPattern)
        {
            case ActionPattern.Punch:
                // 斬撃発動アイコン表示
                batch = batchManager.GenerateActionBatch(1);
                batch.effectCount = punchDamage + muscle;
                break;
            case ActionPattern.Defense:
                // 連続斬撃発動アイコン表示
                batch = batchManager.GenerateActionBatch(3);
                break;
            case ActionPattern.Idle:
                break;
        }
    }

    /// <summary>
    /// Idle以外の行動パターンをランダムで返す
    /// </summary>
    /// <returns>Idle以外のActionPattern</returns>
    private ActionPattern GetRandomNonIdleAction()
    {
        ActionPattern[] patterns = { ActionPattern.Punch, ActionPattern.Defense };
        int idx = Random.Range(0, patterns.Length);
        return patterns[idx];
    }

    public override bool Action()
    {
        switch (actionPattern)
        {
            case ActionPattern.Punch:
                if (Punch())
                {
                    actionPattern = GetRandomNonIdleAction();
                    return true;
                }
                break;
            case ActionPattern.Defense:
                if (Defense())
                {
                    actionPattern = GetRandomNonIdleAction();
                    return true;
                }
                break;
            case ActionPattern.Idle:
                Debug.LogError("本来であればここは通らないはず");
                Debug.Log("様子をうかがっている");
                return false;
        }
        return false;
    }

    /// <summary>
    /// パンチ
    /// </summary>
    /// <returns>行動終了したか？</returns>
    private bool Punch()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Punch");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= punchTime)
        {
            Debug.Log("パンチ発動");
            var bm = BattleManager.Instance;
            if (punchSE != null) AudioController.Instance?.PlaySE(punchSE);
            currentTime = 0;
            inAction = false;
            bm.player.TakeDamage(punchDamage + muscle);

            // ビジュアルエフェクトを生成
            GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(1);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.player.transform.position;
                ve.transform.Rotate(Vector3.forward, -90);
            }

            actionPattern = ActionPattern.Idle;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 防御魔法
    /// </summary>
    /// <returns>行動終了したか？</returns>
    private bool Defense()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Defense");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= defenseTime)
        {
            Debug.Log("防御魔法を発動");
            currentTime = 0;
            inAction = false;
            var bm = BattleManager.Instance;

            // 全ての敵キャラにShieldを付与する
            var enemyMG = bm.enemyManager;
            foreach(var enemy in enemyMG.enemies)
            {
                // Shieldを付与
                enemy.AddShield(defenceShieldNum);

                GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(3);
                if (visualEffectPrefab != null)
                {
                    var go = Instantiate(visualEffectPrefab);
                    go.transform.position = enemy.transform.position;
                }   
            }

            if(defenseSE != null) AudioController.Instance?.PlaySE(defenseSE);

            actionPattern = ActionPattern.Idle;
            return true;
        }
        return false;
    }
}
