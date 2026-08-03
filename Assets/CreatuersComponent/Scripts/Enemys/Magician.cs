using UnityEngine;

public class Magician : Enemy
{
    // 魔法の発動までの時間
    private float currentCastTime = 0;
    private float castTime = 0.5f;
    private bool magicCasted = false;

    // 行動パターン
    private enum ActionPattern
    {
        Idle = 0,
        CastMagic = 1,
        Guard = 2,
    }
    [SerializeField]
    private ActionPattern actionPattern;
    
    [Header("魔法使いのアニメータ")]
    [SerializeField]
    private Animator animator;

    [Header("魔法発動音")]
    [SerializeField]
    private AudioClip magicCastSE;

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
    public override bool Action()
    {
        switch (actionPattern)
        {
            case ActionPattern.CastMagic:
                if (CastMagic()) {
                    actionPattern = GetRandomNonIdleAction();
                    return true;
                }
                break;
            case ActionPattern.Guard:
                // 5シールドを与える
                AddShield(5);
                actionPattern = GetRandomNonIdleAction();
                return true;
            case ActionPattern.Idle:
                Debug.LogError("本来であればここは通らないはず");
                Debug.Log("様子をうかがっている");
                return false;
        }
        Debug.LogError("本来であればここは通らないはず");
        return false;
    }

    /// <summary>
    /// 魔法詠唱
    /// </summary>
    /// <returns>行動終了したか？</returns>
    private bool CastMagic()
    {
        // アニメーション起動
        if(animator != null && !magicCasted)
        {
            animator.SetTrigger("CastMagic");
            magicCasted = true;
        }
        currentCastTime += Time.deltaTime;
        if(currentCastTime >= castTime)
        {
            Debug.Log("魔法発動");
            if(magicCastSE != null) AudioController.Instance?.PlaySE(magicCastSE);
            currentCastTime = 0;
            // プレイヤーに10点のダメージを与える
            BattleManager.Instance.player.TakeDamage(10);
            magicCasted = false;
            if(BattleManager.Instance != null)
            {
                var bm = BattleManager.Instance;
                GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(2);
                // ビジュアルエフェクトを生成
                if (visualEffectPrefab != null)
                {
                    GameObject ve = Instantiate(visualEffectPrefab);
                    
                    ve.transform.position = bm.player.transform.position;
                }
                actionPattern = ActionPattern.Idle;
            }
            return true;
        }
        return false;
    }
}
