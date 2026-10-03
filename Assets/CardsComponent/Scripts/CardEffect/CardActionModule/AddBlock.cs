/// <summary>
/// ブロック追加クラス
/// 
/// 一つのターゲットを対象としたブロック追加効果を定義する
/// </summary>
public class AddBlock : CardActionModule
{    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="blockVal">ブロック値</param>
    public AddBlock(Number blockVal)
    {
        // 設定する引数は1つ
        for (int i = 0; i < 1; i++)
        {
            args.Add(null);
        }
        args[0] = blockVal;
    }

    /// <summary>
    /// カードの実行
    /// </summary>
    /// <param name="target">バフ対象</param>
    public override void Execute(Creature target)
    {
        target.AddBlock((int)args[0].Evaluate());
    }
}
