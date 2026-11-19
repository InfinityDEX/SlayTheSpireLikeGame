using System.Collections.Generic;
using OneOf;

namespace CardEffect.Expressions
{
    /// <summary>
    /// 数値演算系モジュールの抽象クラス
    /// </summary>
    public abstract class MathExpressionBase : ValueExpressionComponent
    {
        /// <summary>
        /// Argsの要素からfloat型の値を抽出するメソッド
        /// </summary>
        /// <param name="argIdx">値抽出したいArgsの要素のインデックス</param>
        /// <returns>argの計算結果（float型）</returns>
        /// <exception cref="System.InvalidOperationException">argの計算結果がstring型（数式演算では使用不能）になった</exception>
        protected float ExtractionArgValue(int argIdx)
        {
            var arg = Args[argIdx];
            float ret = 0;

            if (arg.IsT2)
            {
                var tmp = arg.AsT2.Execute(null)?.EvaluateToValue();
                if (tmp?.IsT1 == true)
                {
                    throw new System.InvalidOperationException("argの計算結果がstring型（数式演算では使用不能）になった。");
                }
                if (tmp?.IsT0 == true)
                {
                    ret = (float)(tmp?.AsT0);
                }
            }

            if (arg.IsT3)
            {
                var tmp = arg.AsT3.EvaluateToValue();
                if (tmp.IsT1)
                {
                    throw new System.InvalidOperationException("argの計算結果がstring型（数式演算では使用不能）になった。");
                }

                ret = tmp.AsT0;
            }

            return ret;
        }
    }
}