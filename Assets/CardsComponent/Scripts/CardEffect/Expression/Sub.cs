namespace CardEffect.Expressions
{
    /// <summary>
    /// 数値減算モジュール
    /// </summary>
    public class Sub : IValueExpression<int>
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
        public Sub(int _left, int _right)
        {
            left = new Number(_left);
            right = new Number(_right);
        }

        /// <summary>
        /// 左項から右項の値を差し引いた値を渡す
        /// </summary>
        /// <returns>左項ー右項の結果</returns>
        public int Evaluate()
        {
            return left.Evaluate() - right.Evaluate();
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