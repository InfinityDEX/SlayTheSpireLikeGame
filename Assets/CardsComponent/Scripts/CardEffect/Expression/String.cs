using System.Collections.Generic;
using OneOf;

namespace CardEffect.Expressions
{
    /// <summary>
    /// 数値モジュール
    /// </summary>
    public class String : PrimitiveValueExpressionComponent // プリミティブ型
    {
        /// <summary>
        /// 式の実行。
        /// </summary>
        /// <returns>このモジュール</returns>
        public override OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate()
        {
            var val = Args[0];

            if (!val.IsT1)
            {
                throw new System.InvalidOperationException("String型に文字列以外が登録されてます");
            }

            // 文字列情報を返す
            return Args[0].AsT1;
        }
    }
}