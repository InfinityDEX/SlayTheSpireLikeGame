using System.Collections.Generic;
using OneOf;

namespace CardEffect.Expressions
{
    /// <summary>
    /// 手札の枚数を出すモジュール
    /// </summary>
    public class HandCount : ValueExpressionComponent
    {
        /// <summary>
        /// 自己複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override ValueExpressionComponent Spawn()
        {
            return new HandCount();
        }

        /// <summary>
        /// 手札の枚数を渡す
        /// </summary>
        /// <returns>手札枚数</returns>
        public override OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate()
        {
            float handCount = DeckManager.Instance.Hand.Count;
            var ret = new Number();
            ret.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>>{
                handCount
            });
            return ret;
        }
    }
}