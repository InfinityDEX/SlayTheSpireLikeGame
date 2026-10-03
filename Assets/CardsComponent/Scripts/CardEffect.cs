using System.Collections.Generic;

/// <summary>
/// ゲームの状態を変化させるモジュール
/// </summary>
public abstract class CardActionModule
{
    /// <summary>
    /// カード処理で使用する引数
    /// 扱う値は処理によって異なるので、
    /// object型を返す非ジェネリックのIValueExpressionを使う。
    /// 
    /// 派生クラスではコンストラクタで設定するように記述し、
    /// コンストラクタで渡す引数ではジェネリックを使ったIValueExpressionを渡すように指定することで、
    /// そのモジュールの外から想定外の型が渡されるのを防ぐように実装する。
    /// 
    /// 例：整数情報を管理する「A」というクラスのコンストラクタの場合は、「public A(IValueExpression<int> val)」のように渡して
    /// valをargsの0番目に設定する。
    /// </summary>
    protected List<IValueExpression> args { get; set; } = new();

    /// <summary>
    /// 効果の実行
    /// </summary>
    /// <param name="target">カード効果の適用対象</param>
    public abstract void Execute(Creature target);
}

/// <summary>
/// ゲームの状態から「値」を計算して返すモジュールのインターフェース（例：手札の枚数、敵の数、かかっているバフデバフの値等）
/// </summary>
public interface IValueExpression
{
    object Evaluate();
}

/// <summary>
/// ジェネリック版
/// </summary>
/// <typeparam name="T">Evaluateメソッドで返す型</typeparam>
public interface IValueExpression<T> : IValueExpression
{
    new T Evaluate();
}