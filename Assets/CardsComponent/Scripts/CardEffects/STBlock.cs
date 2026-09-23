/// <summary>
/// シングルブロッククラス
/// 
/// 一つのブロックを対象としたブロック追加効果を定義する
/// </summary>
public class STBlock : CardEffect
{    
    /// <summary>
    /// 追加するブロック値
    /// </summary>
    private int block;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_block">追加するブロック値</param>
    public STBlock(int _block)
    {
        block = _block;
    }

    /// <summary>
    /// カードの実行（バフ）
    /// </summary>
    /// <param name="target">バフ対象</param>
    public override void Play(Creature target)
    {
        target.AddBlock(block);
    }
}
