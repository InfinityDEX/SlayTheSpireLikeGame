using UnityEngine;

public class Magician : Enemy
{
    // 魔法の発動までの時間
    private float currentCastTime = 0;
    private float castTime = 0.5f;
    private bool magicCasted = false;
    
    [Header("魔法使いのアニメータ")]
    [SerializeField]
    private Animator animator;

    [Header("魔法発動音")]
    [SerializeField]
    private AudioClip magicCastSE;

    private void Awake()
    {
        RegistTakeDamageEvent(ExcuteTakeDamageAnima);
    }

    private void ExcuteTakeDamageAnima(int damage, int hp)
    {
        animator.SetTrigger("TakeDamage");
    }

    public override bool Action()
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
            return true;
        }
        return false;
    }
}
