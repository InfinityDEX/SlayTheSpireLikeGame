using System.Collections.Generic;
using OneOf;

namespace CardEffect.Expressions
{
    /// <summary>
    /// 減算モジュール
    /// </summary>
    public class Sub : MathExpressionBase
    {
        /// <summary>
        /// 自己複製コンポーネント
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override ValueExpressionComponent Spawn()
        {
            return new Sub();
        }

        /// <summary>
        /// 左項から右項の値を差し引いた値を渡す
        /// </summary>
        /// <returns>左項ー右項の結果</returns>
        public override OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate()
        {
            float leftVal = ExtractionArgValue(0); // 左辺（Args[0]）からfloat型の値を取得
            float rightVal = ExtractionArgValue(1); // 右辺（Args[1]）からfloat型の値を取得

            return new Number(leftVal - rightVal);
        }
    }
}