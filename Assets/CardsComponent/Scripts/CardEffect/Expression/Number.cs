using System.Collections.Generic;
using OneOf;

namespace CardEffect.Expressions
{
    /// <summary>
    /// 数値モジュール
    /// </summary>
    public class Number : PrimitiveValueExpressionComponent // プリミティブ型
    {
        /// <summary>
        /// 式の実行。
        /// </summary>
        /// <returns>このモジュール</returns>
        public override OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate()
        {
            var val = Args[0];

            if (!val.IsT0)
            {
                throw new System.InvalidOperationException("Number型に数値以外が登録されてます");
            }

            // 数値情報を返す
            return Args[0].AsT0;
        }
    }
}