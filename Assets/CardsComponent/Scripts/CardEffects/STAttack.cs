/// <summary>
/// シングルターゲットアタッククラス
/// 
/// 一つのターゲットを対象とした攻撃効果を定義する
/// </summary>
public class STAttack : CardEffect
{    
    /// <summary>
    /// ダメージ量
    /// </summary>
    private int damage;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_damage">ダメージ量</param>
    public STAttack(int _damage)
    {
        damage = _damage;
    }

    /// <summary>
    /// カードの実行（攻撃）
    /// </summary>
    /// <param name="target">攻撃対象</param>
    public override void Play(Creature target)
    {
        target.TakeDamage(damage + BattleManager.Instance.Player.Muscle);
    }
}
