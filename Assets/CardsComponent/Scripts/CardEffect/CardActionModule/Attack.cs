/// <summary>
/// 攻撃モジュール
/// 
/// 一つのターゲッを対象とした攻撃効果を定義する
/// </summary>
public class Attack : CardActionModule
{    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="damageVal">ダメージ値</param>
    public Attack(Number damageVal)
    {
        // 設定する引数は1つ
        for (int i = 0; i < 1; i++)
        {
            args.Add(null);
        }
        args[0] = damageVal;
    }

    /// <summary>
    /// カードの実行
    /// </summary>
    /// <param name="target">攻撃対象</param>
    public override void Execute(Creature target)
    {
        // args[0]にはコンストラクタで渡されたNumberモジュールが登録されていて、
        // 確実にEvaluateメソッドでint型の値を返すのでエラーチェックせずに明示的にキャストできる。
        target.TakeDamage((int)args[0].Evaluate() + BattleManager.Instance.Player.Muscle);
    }
}
