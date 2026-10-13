using System.Collections.Generic;
using OneOf;

namespace CardEffect.Expressions
{
    /// <summary>
    /// 数値除算モジュール
    /// </summary>
    public class Div : ValueExpressionComponent
    {
        /// <summary>
        /// 自己複製コンポーネント
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override ValueExpressionComponent Spawn()
        {
            return new Div();
        }

        /// <summary>
        /// 左項から右項の値を割った値を渡す
        /// </summary>
        /// <returns>左項＋右項の結果</returns>
        public override OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate()
        {
            var left = Args[0];
            var right = Args[1];
            float leftVal = 0, rightVal = 0;

            if (left.IsT2)
            {
                var tmp = left.AsT2.Execute(null)?.EvaluateToValue();
                if (tmp?.IsT1 == true)
                {
                    throw new System.InvalidOperationException("Div演算子では文字列型を受け付けません。");
                }
                if (tmp?.IsT0 == true)
                {
                    leftVal = (float)(tmp?.AsT0);
                }
            }
            if (right.IsT2)
            {
                var tmp = left.AsT2.Execute(null)?.EvaluateToValue();
                if (tmp?.IsT1 == true)
                {
                    throw new System.InvalidOperationException("Div演算子では文字列型を受け付けません。");
                }

                if (tmp?.IsT0 == true)
                {
                    if (tmp?.AsT0 == 0)
                    {
                        throw new System.DivideByZeroException("ゼロ除算");
                    }

                    rightVal = (float)(tmp?.AsT0);
                }
            }

            if (left.IsT3)
            {
                var tmp = left.AsT3.EvaluateToValue();
                if (tmp.IsT1)
                {
                    throw new System.InvalidOperationException("Div演算子では文字列型を受け付けません。");
                }

                leftVal = tmp.AsT0;
            }
            if (right.IsT3)
            {
                var tmp = right.AsT3.EvaluateToValue();
                if (tmp.IsT1)
                {
                    throw new System.InvalidOperationException("Div演算子では文字列型を受け付けません。");
                }

                if (tmp.AsT0 == 0)
                {
                    throw new System.DivideByZeroException("ゼロ除算");
                }

                rightVal = tmp.AsT0;
            }
            var ret = new Number();
            ret.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>>{
                leftVal / rightVal
            });

            return ret;
        }
    }
}