using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ワイバーンクラス
/// 
/// ボスエネミー
/// </summary>
public class Wyvern : Enemy
{
    /// <summary>
    /// アニメーション経過時間
    /// </summary>
    private float animationCurrentTime = 0;

    /// <summary>
    /// アニメーション開始からファイアブレスが実際に放たれるまでの時間
    /// </summary>
    private float fireBreathTime = 0.5f;

    /// <summary>
    /// アニメーション開始から実際に飛行状態になるまでの時間
    /// </summary>
    private float flyTime = 0.5f;

    /// <summary>
    /// アニメーション開始から画面外まで高空飛行するまでの時間
    /// </summary>
    private float soarTime = 0.5f;

    /// <summary>
    /// アニメーション開始からテンペスト攻撃が発動するまでの時間
    /// </summary>
    private float tempestTime = 2.3f;

    /// <summary>
    /// 行動中かどうか
    /// </summary>
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

    /// <summary>
    /// 行動シーケンス
    /// </summary>
    private List<ActionPattern> actionSequence;

    /// <summary>
    /// 現在の行動シーケンス（インデックス）
    /// </summary>
    private int currentActionSequenceIndex;

    [SerializeField, Header("現在の行動パターン")]
    private ActionPattern currentActionPattern;

    [SerializeField, Header("ワイバーンのアニメータ")]
    private Animator animator;

    [SerializeField, Header("ファイアブレスダメージ")]
    private int fireBreathDamage = 15;

    [SerializeField, Header("ファイアブレス音")]
    private AudioClip fireBreathSE;

    [SerializeField, Header("ファイアブレス発射位置(ワイバーンの口)")]
    private Transform mouthPos;

    [SerializeField, Header("テンペストダメージ")]
    private int tempestDamage = 30;

    [SerializeField, Header("テンペスト音")]
    private AudioClip tempestSE;

    [SerializeField, Header("FlyからSoarに移行するまでのターン数")]
    private int turnsToSoarFromFly = 3;
    /// <summary>
    /// 現在の飛行ターン
    /// </summary>
    private int currentFlyTurns = 0;

    [SerializeField, Header("飛行時の音")]
    private AudioClip flySE;

    [SerializeField, Header("高空飛行時の音")]
    private AudioClip soarSE;

    [SerializeField, Header("気絶した時の音")]
    private AudioClip stunSE;

    [SerializeField, Header("飛行状態で何回攻撃を受けたらスタンするか")]
    private int hitsToStunWhileFlying = 3;
    /// <summary>
    /// 飛行状態に移行してからの被ダメージ回数
    /// </summary>
    private int currentHitsWhileFlying = 0;

    private void Awake()
    {
        // ダメージを受けたときの処理をイベント処理に登録
        RegisterTakeDamageEvent(ExecuteTakeDamageAnima);

        actionSequence = new List<ActionPattern>{ 
            ActionPattern.Fly,
            ActionPattern.FireBreath,
            ActionPattern.Soar,
            ActionPattern.Tempest,
        };
        currentActionPattern = actionSequence[currentActionSequenceIndex];
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
        // 現在の行動パターンがFireBreathだった場合（飛行状態移行後から高空飛行状態に移行するまでの行動パターン）
        // 被ダメージカウントを上げる（[hitsToStunWhileFlying]回以上攻撃を受けたらスタンする）
        if (currentActionPattern == ActionPattern.FireBreath)
        {
            currentHitsWhileFlying++;
        }
        if (currentHitsWhileFlying < hitsToStunWhileFlying)
        {
            // ダメージアニメーション
            animator.SetTrigger("TakeDamage");
        }
        else
        {
            // 墜落アニメーション
            animator.SetTrigger("Fall");
            currentActionPattern = ActionPattern.Stun; // 次のターン行動不能
            currentHitsWhileFlying = 0;
            RefreshActionIcon(); // 次の行動がリセットされるので行動アイコンもリセットする
        }
    }

    /// <summary>
    /// 次の行動を表すアイコンを更新する
    /// </summary>
    public override void RefreshActionIcon()
    {
        BatchManager.ResetActionBatches();

        IconBatch batch = null;
        switch (currentActionPattern)
        {
            case ActionPattern.Fly:
                // 飛行発動アイコン表示
                batch = BatchManager.GenerateActionBatch(4);
                break;
            case ActionPattern.FireBreath:
                // 斬撃発動アイコン表示
                batch = BatchManager.GenerateActionBatch(1);
                batch.effectCount = fireBreathDamage + Muscle;
                break;
            case ActionPattern.Soar:
                // 高空飛行発動アイコン表示
                batch = BatchManager.GenerateActionBatch(5);
                break;
            case ActionPattern.Tempest:
                // テンペスト発動アイコン表示
                batch = BatchManager.GenerateActionBatch(1);
                batch.effectCount = tempestDamage + Muscle;
                break;
            case ActionPattern.Stun:
                // スタン中は次の行動無し
                break;
            case ActionPattern.Idle:
                break;
        }
    }

    /// <summary>
    /// 現在の行動パターンに合わせてアクション実行
    /// </summary>
    /// <returns>アクションは完了したか？（まだ行動中ならfalse。アニメーションの実行等全ての処理が終わったらtrue）</returns>
    public override bool Action()
    {
        bool ret = false;
        switch (currentActionPattern)
        {
            case ActionPattern.FireBreath:
                // ファイアブレス攻撃実行
                ret = FireBreath();
                currentActionPattern = actionSequence[currentActionSequenceIndex];
                return ret;
            case ActionPattern.Fly:
                // 飛び上がり実行
                ret = Fly();
                currentActionPattern = actionSequence[currentActionSequenceIndex];
                return ret;
            case ActionPattern.Soar:
                // 急上昇実行
                ret = Soar();
                currentActionPattern = actionSequence[currentActionSequenceIndex];
                return ret;
            case ActionPattern.Tempest:
                // テンペスト攻撃実行
                ret = Tempest();
                currentActionPattern = actionSequence[currentActionSequenceIndex];
                return ret;
            case ActionPattern.Stun:
                // スタン状態。行動シーケンスをリセットしてすぐにアクション終了
                currentActionSequenceIndex = 0;
                currentActionPattern = actionSequence[currentActionSequenceIndex];
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
    /// <returns>行動完了したか？</returns>
    private bool FireBreath()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("FireBreath");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= fireBreathTime)
        {
            Debug.Log("ファイアブレス発動");
            var bm = BattleManager.Instance;
            if (fireBreathSE != null) AudioController.Instance?.PlaySE(fireBreathSE);
            animationCurrentTime = 0;
            inAction = false;
            bm.Player.TakeDamage(fireBreathDamage + Muscle);

            // ファイアブレスエフェクトを生成
            GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(5);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.parent = mouthPos;
                ve.transform.localPosition = Vector3.zero;
                ve.transform.localScale = new Vector3(-1 * ve.transform.localScale.x, ve.transform.localScale.y, ve.transform.localScale.z);
           
            }
            currentFlyTurns++;
            if (currentFlyTurns == turnsToSoarFromFly)
            {
                currentFlyTurns = 0;
                currentActionSequenceIndex++;
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
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= tempestTime)
        {
            Debug.Log("テンペスト発動");
            var bm = BattleManager.Instance;
            if (tempestSE != null) AudioController.Instance?.PlaySE(tempestSE);
            bm.Player.TakeDamage(tempestDamage + Muscle);
            animationCurrentTime = 0;
            inAction = false;

            // テンペスト攻撃のエフェクトを生成
            GameObject visualEffectPrefab = bm.VisualEffectLibrary.GetEffectById(1);
            if (visualEffectPrefab != null)
            {
                GameObject ve = Instantiate(visualEffectPrefab);

                ve.transform.position = bm.Player.transform.position;
                ve.transform.Rotate(Vector3.forward, -90);
            }

            currentActionSequenceIndex = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 飛行する
    /// </summary>
    /// <returns>行動完了したか？</returns>
    private bool Fly()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Fly");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= flyTime)
        {
            Debug.Log("飛行発動");
            var bm = BattleManager.Instance;
            if (flySE != null) AudioController.Instance?.PlaySE(flySE);
            animationCurrentTime = 0;
            inAction = false;

            currentActionSequenceIndex++;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 高く飛び上がる
    /// </summary>
    /// <returns>行動完了したか？</returns>
    private bool Soar()
    {
        // アニメーション起動
        if (animator != null && !inAction)
        {
            animator.SetTrigger("Soar");
            inAction = true;
        }
        animationCurrentTime += Time.deltaTime;
        if (animationCurrentTime >= soarTime)
        {
            Debug.Log("高空飛行発動");
            var bm = BattleManager.Instance;
            if (soarSE != null) AudioController.Instance?.PlaySE(soarSE);
            animationCurrentTime = 0;
            inAction = false;

            currentActionSequenceIndex++;
            return true;
        }
        return false;
    }
}
