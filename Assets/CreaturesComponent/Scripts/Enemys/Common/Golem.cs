using UnityEngine;

/// <summary>
/// ゴーレム（ロボット）のクラス
/// 
/// エリートエネミー
/// </summary>
public class Golem : Enemy
{
    /// <summary>
    /// アニメーション経過時間
    /// </summary>
    private float animationCurrentTime = 0;
    
    /// <summary>
    /// アニメーション開始からパンチ攻撃が実際に放たれるまでの時間
    /// </summary>
    private float punchTime = 0.5f;

    /// <summary>
    /// アニメーション開始から防御魔法が実際に適用されるまでの時間
    /// </summary>
    private float defenseTime = 0.5f;

    /// <summary>
    /// 行動中かどうか
    /// </summary>
    private bool inAction = false;

    // 行動パターン
    private enum ActionPattern
    {
        Idle = 0, // 待機状態
        Punch = 1, // パンチ
        Defense = 2, // 防御魔法(全体防御)
    }

    [SerializeField, Header("現在の行動パターン")]
    private ActionPattern actionPattern;

    [SerializeField, Header("ゴーレムのアニメータ")]
    private Animator animator;

    [SerializeField, Header("パンチダメージ")]
    private int punchDamage = 10;

    [SerializeField, Header("パンチ音")]
    private AudioClip punchSE;

    [SerializeField, Header("防御魔法音")]
    private AudioClip defenseSE;

    [SerializeField, Header("防御魔法で全体に振りまくShield値")]
    private int defenceShieldNum = 10;

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
            case ActionPattern.Punch:
                // 斬撃発動アイコン表示
                batch = BatchManager.GenerateActionBatch(1);
                batch.effectCount = punchDamage + Muscle;
                break;
            case ActionPattern.Defense:
                // 防御魔法発動アイコン表示
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
        ActionPattern[] patterns = { ActionPattern.Punch, ActionPattern.Defense };
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
    /// <returns>行動完了したか？</returns>
    private bool Punch()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Punch");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= punchTime)
        {
            Debug.Log("パンチ発動");
            var bm = BattleManager.Instance;
            if (punchSE != null) AudioController.Instance?.PlaySE(punchSE);
            animationCurrentTime = 0;
            inAction = false;
            bm.Player.TakeDamage(punchDamage + Muscle);

            // パンチエフェクトを生成
            GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(1);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.Player.transform.position;
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
    /// <returns>行動完了したか？</returns>
    private bool Defense()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Defense");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= defenseTime)
        {
            Debug.Log("防御魔法を発動");
            animationCurrentTime = 0;
            inAction = false;
            var bm = BattleManager.Instance;

            // 全ての敵キャラ（ゴーレムにとっては味方）にShieldを付与する
            var enemyMG = bm.EnemyManager;
            foreach(var enemy in enemyMG.Enemies)
            {
                // Shieldを付与
                enemy.AddBlock(defenceShieldNum);

                GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(3);
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
