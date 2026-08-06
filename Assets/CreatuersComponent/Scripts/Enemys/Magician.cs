using UnityEngine;

public class Magician : Enemy
{
    // 魔法の発動までの時間
    private float currentTime = 0;
    private float castTime = 0.5f;
    private bool inAction = false;

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

    [Header("シールド展開音")]
    [SerializeField]
    private AudioClip activateShieldSE;

    [Header("魔法のダメージ")]
    [SerializeField]
    private int magicDamage = 10;

    [Header("シールド展開で貼るシールド数")]
    [SerializeField]
    private int activateShieldNum = 5;

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
        switch(actionPattern)
        {
            case ActionPattern.CastMagic:
                // 魔法発動アイコン表示（仮想処理。UIやバッチ生成など任意の処理を入れてください）
                batch = batchManager.GenerateActionBatch(1);
                batch.effectCount = magicDamage;
                break;
            case ActionPattern.Guard:
                // シールド展開アイコン表示
                batch = batchManager.GenerateActionBatch(0);
                batch.effectCount = activateShieldNum;
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
    /// <returns>行動終了したか？</returns>
    private bool CastMagic()
    {
        // アニメーション起動
        if(animator != null && !inAction)
        {
            animator.SetTrigger("CastMagic");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if(currentTime >= castTime)
        {
            Debug.Log("魔法発動");
            if(magicCastSE != null) AudioController.Instance?.PlaySE(magicCastSE);
            currentTime = 0;
            // プレイヤーにダメージを与える
            BattleManager.Instance.player.TakeDamage(magicDamage);
            inAction = false;
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

    private bool ActivateShield()
    {
        // アニメーション起動
        if(animator != null && !inAction)
        {
            animator.SetTrigger("ActivateShield");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if(currentTime >= castTime)
        {
            Debug.Log("シールド展開");
            if(activateShieldSE != null) AudioController.Instance?.PlaySE(activateShieldSE);
            currentTime = 0;
            inAction = false;
            if(BattleManager.Instance != null)
            {
                // シールドを張る
                AddShield(activateShieldNum);

                var bm = BattleManager.Instance;
                GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(3);
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
