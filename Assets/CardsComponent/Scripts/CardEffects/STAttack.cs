// シングルターゲットアタッククラス
public class STAttack : CardEffect
{    
    private int damage;

    public STAttack(int attackDamage)
    {
        damage = attackDamage;
    }

    public override void Play(Creature target)
    {
        if(BattleManager.Instance.player.muscle > 0)
            target.TakeDamage((int)((float)damage * 1.25f));
        else
            target.TakeDamage(damage);
    }
}
