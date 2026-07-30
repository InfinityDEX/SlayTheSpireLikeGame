
// シングルターゲットブロッククラス
public class STShield : CardEffect
{    
    private int shield;

    public STShield(int shieldNum)
    {
        shield = shieldNum;
    }

    public override void Play(Creature target)
    {
        target.AddShield(shield);
    }
}
