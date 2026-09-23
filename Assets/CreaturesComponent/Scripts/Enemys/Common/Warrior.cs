using UnityEngine;

/// <summary>
/// 戦士クラス
/// 
/// ノーマルエネミー
/// </summary>
public class Warrior : Enemy
{
    /// <summary>
    /// アニメーション経過時間
    /// </summary>
    private float animationCurrentTime = 0;
    
    /// <summary>
    /// アニメーション開始から斬撃が当たるまでの時間
    /// </summary>
    private float slashTime = 0.5f;
    
    /// <summary>
    /// アニメーション開始から強化魔法が適用されるまでの時間
    /// </summary>
    private float pumpUpTime = 0.5f;

    /// <summary>
    /// 行動中かどうか
    /// </summary>
    private bool inAction = false;

    // 行動パターン
    private enum ActionPattern
    {
        Idle = 0, // 待機状態
        Slash = 1, // 斬撃
        PumpUp = 2, // 身体強化
        ConsecutiveSlash = 3, // 連続切り
    }

    [SerializeField, Header("現在の行動パターン")]
    private ActionPattern actionPattern;

    [SerializeField, Header("戦士のアニメータ")]
    private Animator animator;

    [SerializeField, Header("斬撃ダメージ")]
    private int slashDamage = 10;

    [SerializeField, Header("連撃斬撃の回数")]
    private int consecutiveSlashCount = 3;
    /// <summary>
    /// 現在連撃攻撃の何回目の斬撃中か
    /// </summary>
    private int currentConsecutiveSlashCount = 1;

    [SerializeField, Header("斬撃音")]
    private AudioClip slashSE;

    [SerializeField, Header("身体強化音")]
    private AudioClip pumpUpSE;

    [SerializeField, Header("バンプアップ時に上がる筋力")]
    private int muscleNum = 5;

    private void Awake()
    {
        // ダメージを受けたときの処理をイベント処理に登録
        RegisterTakeDamageEvent(ExecuteTakeDamageAnima);
        actionPattern = GetRandomNonIdleAction();
    }

    /// <summary>
    /// 被ダメージ時のアニメーション再生処理
    /// 
    /// damageとhpは本関数を登録するイベントハンドラ側が
    /// 指定している引数の為、使用していないが残す
    /// </summary>
    /// <param name="damage">ダメージ量</param>
    /// <param name="hp">体力</param>
    private void ExecuteTakeDamageAnima(int damage, int hp)
    {
        animator.SetTrigger("TakeDamage");
    }

    /// <summary>
    /// 次の行動を表すアイコンを更新する
    /// </summary>
    public override void RefreshActionIcon()
    {
        BatchManager.ResetActionBatches();

        IconBatch batch = null;
        switch (actionPattern)
        {
            case ActionPattern.Slash:
                // 斬撃発動アイコン表示
                batch = BatchManager.GenerateActionBatch(1);
                batch.effectCount = slashDamage + Muscle;
                break;
            case ActionPattern.ConsecutiveSlash:
                // 連続斬撃発動アイコン表示
                batch = BatchManager.GenerateActionBatch(1);
                batch.effectCount = slashDamage + Muscle;
                batch.ratioCount = consecutiveSlashCount;
                break;
            case ActionPattern.PumpUp:
                // 身体強化アイコン表示
                batch = BatchManager.GenerateActionBatch(3);
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

    /// <summary>
    /// 現在の行動パターンに合わせてアクション実行
    /// </summary>
    /// <returns>アクションは完了したか？（まだ行動中ならfalse。アニメーションの実行等全ての処理が終わったらtrue）</returns>
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
    /// <returns>行動完了したか？</returns>
    private bool Slash()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Slash");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= slashTime)
        {
            Debug.Log("斬撃発動");
            var bm = BattleManager.Instance;
            if (slashSE != null) AudioController.Instance?.PlaySE(slashSE);
            animationCurrentTime = 0;
            inAction = false;
            bm.Player.TakeDamage(slashDamage + Muscle);

            // ビジュアルエフェクトを生成
            GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(0);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.Player.transform.position;
            }

            actionPattern = ActionPattern.Idle;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 連続斬撃
    /// </summary>
    /// <returns>行動完了したか？</returns>
    private bool ConsecutiveSlash()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Slash");
            animator.SetBool("ConsecutiveSlash", true);
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= slashTime)
        {
            Debug.Log("斬撃発動");
            var bm = BattleManager.Instance;
            if (slashSE != null) AudioController.Instance?.PlaySE(slashSE);
            animationCurrentTime = 0;
            bm.Player.TakeDamage(slashDamage + Muscle);

            // ビジュアルエフェクトを生成
            GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(0);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.Player.transform.position;
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
    /// <returns>行動完了したか？</returns>
    private bool PumpUp()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("PumpUp");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= pumpUpTime)
        {
            Debug.Log("身体強化(PumpUp)を発動");
            animationCurrentTime = 0;
            inAction = false;

            // 筋力上昇等のパラメータ強化処理
            AddMuscle(muscleNum);
                
            var bm = BattleManager.Instance;
            GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(4);
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
