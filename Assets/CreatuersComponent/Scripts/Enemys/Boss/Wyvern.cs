using System;
using System.Collections.Generic;
using UnityEngine;

public class Wyvern : Enemy
{
    private float currentTime = 0;
    // ファイアブレスまでの時間
    private float fireBreathTime = 0.5f;
    // 飛行までの時間
    private float flyTime = 0.5f;
    // 高空飛行までの時間
    private float soarTime = 0.5f;
    // テンペストまでの時間
    private float tempestTime = 2.3f;
    private bool inAction = false;

    // 行動パターン
    private enum ActionPattern
    {
        Idle = 0, // 待機状態
        FireBreath = 1, // ファイアブレス
        Fly = 2, // 飛行
        Soar = 3, // 高空飛行(大技待機。無敵状態)
        Tempest = 4, // テンペスト(大技)
        Stun = 5, // 気絶
    }

    [SerializeField]
    private ActionPattern actionPattern;

    [Header("ワイバーンのアニメータ")]
    [SerializeField]
    private Animator animator;

    [Header("ファイアブレスダメージ")]
    [SerializeField]
    private int fireBreathDamage = 15;

    [Header("ファイアブレス音")]
    [SerializeField]
    private AudioClip fireBreathSE;

    [Header("テンペストダメージ")]
    [SerializeField]
    private int tempestDamage = 30;

    [Header("テンペスト音")]
    [SerializeField]
    private AudioClip tempestSE;

    [Header("FlyからSoarに移行するまでのターン数")]
    [SerializeField]
    private int turnsToSoarFromFly = 3;
    private int currentFlyTurns = 0;

    [Header("飛行時の音")]
    [SerializeField]
    private AudioClip flySE;

    [Header("高空飛行時の音")]
    [SerializeField]
    private AudioClip soarSE;

    [Header("気絶した時の音")]
    [SerializeField]
    private AudioClip stunSE;

    // どのような順序で行動を移すか
    private List<ActionPattern> actionSequence;

    [Header("飛行状態で何回攻撃を受けたらスタンするか")]
    [SerializeField]
    private int hitsToStunWhileFlying = 3;
    private int currentHitsWhileFlying = 0;
  
    // 現在のシーケンス位置
    private int currentSequence;

    private void Awake()
    {
        // ダメージを受けたときの処理をイベント処理に登録
        RegistTakeDamageEvent(ExcuteTakeDamageAnima);

        actionSequence = new List<ActionPattern>{ 
            ActionPattern.Fly,
            ActionPattern.FireBreath,
            ActionPattern.Soar,
            ActionPattern.Tempest,
        };
        actionPattern = actionSequence[currentSequence];
    }

    private void ExcuteTakeDamageAnima(int damage, int hp)
    {
        if (actionPattern == ActionPattern.FireBreath)
        {
            currentHitsWhileFlying++;
        }
        if (currentHitsWhileFlying < hitsToStunWhileFlying)
        {
            animator.SetTrigger("TakeDamage");
        }
        else
        {
            animator.SetTrigger("Fall");
            actionPattern = ActionPattern.Stun;
            currentHitsWhileFlying = 0;
            RefreshActionIcon(); // 次の行動がリセットされるので行動アイコンもリセットする
        }
    }

    public override void RefreshActionIcon()
    {
        batchManager.DeleteActionBatches();

        IconBatch batch = null;
        switch (actionPattern)
        {
            case ActionPattern.Fly:
                // 飛行発動アイコン表示
                batch = batchManager.GenerateActionBatch(4);
                break;
            case ActionPattern.FireBreath:
                // 斬撃発動アイコン表示
                batch = batchManager.GenerateActionBatch(1);
                batch.effectCount = fireBreathDamage + muscle;
                break;
            case ActionPattern.Soar:
                // 高空飛行発動アイコン表示
                batch = batchManager.GenerateActionBatch(5);
                break;
            case ActionPattern.Tempest:
                // テンペスト発動アイコン表示
                batch = batchManager.GenerateActionBatch(1);
                batch.effectCount = tempestDamage + muscle;
                break;
            case ActionPattern.Stun:
                // スタン中は次の行動無し
                break;
            case ActionPattern.Idle:
                break;
        }
    }

    public override bool Action()
    {
        bool ret = false;
        switch (actionPattern)
        {
            case ActionPattern.FireBreath:
                ret = FireBreath();
                actionPattern = actionSequence[currentSequence];
                return ret;
            case ActionPattern.Fly:
                ret = Fly();
                actionPattern = actionSequence[currentSequence];
                return ret;
            case ActionPattern.Soar:
                ret = Soar();
                actionPattern = actionSequence[currentSequence];
                return ret;
            case ActionPattern.Tempest:
                ret = Tempest();
                actionPattern = actionSequence[currentSequence];
                return ret;
            case ActionPattern.Stun:
                currentSequence = 0;
                actionPattern = actionSequence[currentSequence];
                animator.SetTrigger("StunEnd");
                return true; // 何もしない
            case ActionPattern.Idle:
                Debug.LogError("本来であればここは通らないはず");
                Debug.Log("様子をうかがっている");
                return false;
        }
        return false;
    }

    /// <summary>
    /// ファイアブレス
    /// </summary>
    /// <returns>行動終了したか？</returns>
    private bool FireBreath()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("FireBreath");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= fireBreathTime)
        {
            Debug.Log("ファイアブレス発動");
            var bm = BattleManager.Instance;
            if (fireBreathSE != null) AudioController.Instance?.PlaySE(fireBreathSE);
            currentTime = 0;
            inAction = false;
            bm.player.TakeDamage(fireBreathDamage + muscle);

            // ビジュアルエフェクトを生成
            GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(1);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.player.transform.position;
                ve.transform.Rotate(Vector3.forward, -90);
            }
            currentFlyTurns++;
            if (currentFlyTurns == turnsToSoarFromFly)
            {
                currentFlyTurns = 0;
                currentSequence++;
            }
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// テンペスト
    /// </summary>
    /// <returns>行動終了したか？</returns>
    private bool Tempest()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Tempest");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= tempestTime)
        {
            Debug.Log("テンペスト発動");
            var bm = BattleManager.Instance;
            if (tempestSE != null) AudioController.Instance?.PlaySE(tempestSE);
            bm.player.TakeDamage(tempestDamage + muscle);
            currentTime = 0;
            inAction = false;

            // ビジュアルエフェクトを生成
            GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(1);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.player.transform.position;
                ve.transform.Rotate(Vector3.forward, -90);
            }

            currentSequence = 0;
            return true;
        }
        return false;
    }

    private bool Fly()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Fly");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= flyTime)
        {
            Debug.Log("飛行発動");
            var bm = BattleManager.Instance;
            if (flySE != null) AudioController.Instance?.PlaySE(flySE);
            currentTime = 0;
            inAction = false;

            // // ビジュアルエフェクトを生成
            // GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(1);
            // if (visualEffectPrefab != null)
            // {
            //     GameObject ve = Instantiate(visualEffectPrefab);

            //     ve.transform.position = bm.player.transform.position;
            //     ve.transform.Rotate(Vector3.forward, -90);
            // }

            currentSequence++;
            return true;
        }
        return false;
    }

    private bool Soar()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Soar");
            inAction = true;
        }
        currentTime += Time.deltaTime;
        if (currentTime >= soarTime)
        {
            Debug.Log("高空飛行発動");
            var bm = BattleManager.Instance;
            if (soarSE != null) AudioController.Instance?.PlaySE(soarSE);
            currentTime = 0;
            inAction = false;

            // // ビジュアルエフェクトを生成
            // GameObject visualEffectPrefab = bm.visualEffectLibrary.GetEffectById(1);
            // if (visualEffectPrefab != null)
            // {
            //     GameObject ve = Instantiate(visualEffectPrefab);

            //     ve.transform.position = bm.player.transform.position;
            //     ve.transform.Rotate(Vector3.forward, -90);
            // }

            currentSequence++;
            return true;
        }
        return false;
    }
}
