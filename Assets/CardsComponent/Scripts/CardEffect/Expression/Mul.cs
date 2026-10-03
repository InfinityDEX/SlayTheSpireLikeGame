namespace CardEffect.Expressions
{
    /// <summary>
    /// 数値乗算モジュール
    /// </summary>
    public class Mul : IValueExpression<int>
    {
        /// <summary>
        /// 左項
        /// </summary>
        private IValueExpression<int> left;
        
        /// <summary>
        /// 右項
        /// </summary>
        private IValueExpression<int> right;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Mul(int _left, int _right)
        {
            left = new Number(_left);
            right = new Number(_right);
        }

        /// <summary>
        /// 左項と右項の値をかけ合わせた値を渡す
        /// </summary>
        /// <returns>左項＊右項の結果</returns>
        public int Evaluate()
        {
            return left.Evaluate() * right.Evaluate();
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
}