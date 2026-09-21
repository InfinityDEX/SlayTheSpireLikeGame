/// <summary>
/// シングルターゲットブロッククラス
/// 
/// 一つのターゲットを対象としたブロック追加効果を定義する
/// </summary>
public class STBlock : CardEffect
{    
    /// <summary>
    /// 追加するブロック値
    /// </summary>
    private int shield;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_shield">追加するブロック値</param>
    public STBlock(int _shield)
    {
        shield = _shield;
    }

    /// <summary>
    /// カードの実行（バフ）
    /// </summary>
    /// <param name="target">バフ対象</param>
    public override void Play(Creature target)
    {
        target.AddShield(shield);
    }
}
