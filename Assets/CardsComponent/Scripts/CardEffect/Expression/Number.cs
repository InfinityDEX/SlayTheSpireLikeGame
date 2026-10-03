/// <summary>
/// カードの数値情報を管理するモジュール
/// </summary>
public class Number : IValueExpression<int>
{    
    /// <summary>
    /// 数値
    /// </summary>
    private int value;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_value">このモジュールに登録する数値情報</param>
    public Number(int _value)
    {
        value = _value;
    }

    /// <summary>
    /// このモジュールが持っている値を渡す
    /// </summary>
    /// <returns>valueの値</returns>
    public int Evaluate()
    {
        return value;
    }

    /// <summary>
    /// IValueExpressionの非ジェネリックなEvaluate実装。
    /// int型で返される値をobject型で返すためBox化する。
    /// 基本的に使用しない
    /// </summary>
    /// <returns>valueの値（object型）</returns>
    object IValueExpression.Evaluate()
    {
        // 上の本命メソッド（intを返す）を呼び出し、自動でobjectにボックス化する
        return this.Evaluate();
    }
}
