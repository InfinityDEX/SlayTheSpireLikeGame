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
        target.TakeDamage(damage + BattleManager.Instance.player.muscle);
    }
}
