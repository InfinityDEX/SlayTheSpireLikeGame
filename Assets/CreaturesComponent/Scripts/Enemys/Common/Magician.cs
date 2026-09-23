using UnityEngine;

/// <summary>
/// 魔法使いクラス
/// 
/// ノーマルエネミー
/// </summary>
public class Magician : Enemy
{
    /// <summary>
    /// アニメーション経過時間
    /// </summary>
    private float animationCurrentTime = 0;

    /// <summary>
    /// アニメーション開始から詠唱した魔法の効果が適応されるまでの時間
    /// </summary>
    private float castTime = 0.5f;

    /// <summary>
    /// 行動中かどうか
    /// </summary>
    private bool inAction = false;

    // 行動パターン
    private enum ActionPattern
    {
        Idle = 0,
        CastMagic = 1,
        Guard = 2,
    }
    [SerializeField, Header("現在の行動パターン")]
    private ActionPattern actionPattern;
    
    [SerializeField, Header("魔法使いのアニメータ")]
    private Animator animator;

    [SerializeField, Header("魔法発動音")]
    private AudioClip magicCastSE;

    [SerializeField, Header("シールド展開音")]
    private AudioClip activateShieldSE;

    [SerializeField, Header("魔法のダメージ")]
    private int magicDamage = 10;

    [SerializeField, Header("シールド展開で貼るシールド数")]
    private int activateShieldNum = 5;

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
        switch(actionPattern)
        {
            case ActionPattern.CastMagic:
                // 魔法発動アイコン表示
                batch = BatchManager.GenerateActionBatch(1);
                batch.effectCount = magicDamage;
                break;
            case ActionPattern.Guard:
                // シールド展開アイコン表示
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
        ActionPattern[] patterns = { ActionPattern.CastMagic, ActionPattern.Guard };
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
            case ActionPattern.CastMagic:
                if (CastMagic()) 
                {
                    actionPattern = GetRandomNonIdleAction();
                    return true;
                }
                break;
            case ActionPattern.Guard:
                if (ActivateShield()) 
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
    /// 魔法詠唱
    /// </summary>
    /// <returns>行動完了したか？</returns>
    private bool CastMagic()
    {
        // アニメーション起動
        if(animator != null && !inAction)
        {
            animator.SetTrigger("CastMagic");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if(animationCurrentTime >= castTime)
        {
            Debug.Log("魔法発動");
            if(magicCastSE != null) AudioController.Instance?.PlaySE(magicCastSE);
            animationCurrentTime = 0;
            // プレイヤーにダメージを与える
            BattleManager.Instance.Player.TakeDamage(magicDamage);
            inAction = false;
            if(BattleManager.Instance != null)
            {
                var bm = BattleManager.Instance;
                GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(2);
                // ビジュアルエフェクトを生成
                if (visualEffectPrefab != null)
                {
                    GameObject ve = Instantiate(visualEffectPrefab);
                    
                    ve.transform.position = bm.Player.transform.position;
                }
                actionPattern = ActionPattern.Idle;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// シールド展開
    /// </summary>
    /// <returns>行動完了したか？</returns>
    private bool ActivateShield()
    {
        // アニメーション起動
        if(animator != null && !inAction)
        {
            animator.SetTrigger("ActivateShield");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if(animationCurrentTime >= castTime)
        {
            Debug.Log("シールド展開");
            if(activateShieldSE != null) AudioController.Instance?.PlaySE(activateShieldSE);
            animationCurrentTime = 0;
            inAction = false;
            if(BattleManager.Instance != null)
            {
                // シールドを張る
                AddBlock(activateShieldNum);

                var bm = BattleManager.Instance;
                GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(3);
                if (visualEffectPrefab != null)
                {
                    var go = Instantiate(visualEffectPrefab);
                    go.transform.position = transform.position;
                }

                actionPattern = ActionPattern.Idle;
            }
            return true;
        }
        return false;
    }
}
