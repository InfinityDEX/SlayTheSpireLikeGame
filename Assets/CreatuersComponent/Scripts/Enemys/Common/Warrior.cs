using UnityEngine;

public class Warrior : Enemy
{
    private float currentTime = 0;
    // 斬撃までの時間
    private float slashTime = 0.5f;
    // 強化魔法発動までの時間
    private float pumpUpTime = 0.5f;
    private bool inAction = false;
    // 行動パターン
    private enum ActionPattern
    {
        Idle = 0, // 待機状態
        Slash = 1, // 斬撃
        PumpUp = 2, // 身体強化
        ConsecutiveSlash = 3, // 連続切り
    }

    [SerializeField]
    private ActionPattern actionPattern;

    [Header("戦士のアニメータ")]
    [SerializeField]
    private Animator animator;

    [Header("斬撃ダメージ")]
    [SerializeField]
    private int slashDamage = 10;

    [Header("連撃斬撃の回数")]
    [SerializeField]
    private int consecutiveSlashCount = 3;

    private int currentConsecutiveSlashCount = 1;

    [Header("斬撃音")]
    [SerializeField]
    private AudioClip slashSE;

    [Header("身体強化音")]
    [SerializeField]
    private AudioClip pumpUpSE;

    [Header("バンプアップ時に上がる筋力")]
    [SerializeField]
    private int muscleNum = 5;

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
            case ActionPattern.Slash:
                // 斬撃発動アイコン表示
                batch = batchManager.GenerateActionBatch(1);
                batch.effectCount = slashDamage + muscle;
                break;
            case ActionPattern.ConsecutiveSlash:
                // 連続斬撃発動アイコン表示
                batch = batchManager.GenerateActionBatch(1);
                batch.effectCount = slashDamage + muscle;
                batch.ratioCount = consecutiveSlashCount;
                break;
            case ActionPattern.PumpUp:
                // 身体強化アイコン表示
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
        ActionPattern[] patterns = { ActionPattern.Slash, ActionPattern.ConsecutiveSlash, ActionPattern.PumpUp };
        int idx = Random.Range(0, patterns.Length);
        return patterns[idx];
    }

    public override bool Action()
    {
        switch (actionPattern)
        {
            case ActionPattern.Slash:
                if (Slash())
                {
                    actionPattern = GetRandomNonIdleAction();
                    return true;
                }
                break;
            case ActionPattern.ConsecutiveSlash:
                if (ConsecutiveSlash())
                {
                    actionPattern = GetRandomNonIdleAction();
                    return true;
                }
                break;
            case ActionPattern.PumpUp:
                if (PumpUp())
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
    /// 斬撃
    /// </summary>
    /// <returns>行動終了したか？</returns>
    private bool Slash()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Slash");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= slashTime)
        {
            Debug.Log("斬撃発動");
            var bm = BattleManager.Instance;
            if (slashSE != null) AudioController.Instance?.PlaySE(slashSE);
            currentTime = 0;
            inAction = false;
            bm.player.TakeDamage(slashDamage + muscle);

            // ビジュアルエフェクトを生成
            GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(0);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.player.transform.position;
            }

            actionPattern = ActionPattern.Idle;
            return true;
        }
        return false;
    }

    private bool ConsecutiveSlash()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Slash");
            animator.SetBool("ConsecutiveSlash", true);
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= slashTime)
        {
            Debug.Log("斬撃発動");
            var bm = BattleManager.Instance;
            if (slashSE != null) AudioController.Instance?.PlaySE(slashSE);
            currentTime = 0;
            bm.player.TakeDamage(slashDamage + muscle);

            // ビジュアルエフェクトを生成
            GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(0);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.player.transform.position;
            }

            if (consecutiveSlashCount <= currentConsecutiveSlashCount)
            {
                inAction = false;
                currentConsecutiveSlashCount = 1;
                animator.SetBool("ConsecutiveSlash", false);
                actionPattern = ActionPattern.Idle;
                return true;
            }
            currentConsecutiveSlashCount++;
        }
        return false;
    }

    /// <summary>
    /// 身体強化（PumpUp）処理
    /// </summary>
    /// <returns>行動終了したか？</returns>
    private bool PumpUp()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("PumpUp");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= pumpUpTime)
        {
            Debug.Log("身体強化(PumpUp)を発動");
            currentTime = 0;
            inAction = false;

            // 筋力上昇等のパラメータ強化処理
            AddMuscle(muscleNum);
                
            var bm = BattleManager.Instance;
            GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(4);
            if (visualEffectPrefab != null)
            {
                var go = Instantiate(visualEffectPrefab);
                go.transform.position = transform.position;
            }


            if(pumpUpSE != null) AudioController.Instance?.PlaySE(pumpUpSE);

            actionPattern = ActionPattern.Idle;
            return true;
        }
        return false;
    }
}
