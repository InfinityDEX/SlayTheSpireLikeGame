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
        /// デッキ情報クラス
        /// </summary>
        private DeckInfomation deckInfo;

        public HandCount(DeckInfomation deckInfo)
        {
            this.deckInfo = deckInfo;
        }

        /// <summary>
        /// 自己複製メソッド
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override ValueExpressionComponent Spawn()
        {
            return new HandCount(deckInfo);
        }

        /// <summary>
        /// 手札の枚数を渡す
        /// </summary>
        /// <returns>手札枚数</returns>
        public override OneOf<float, string, CardActionComponent, ValueExpressionComponent> Evaluate()
        {
            float handCount = deckInfo.Hand.Count;
            var ret = new Number();
            ret.SetArgs(new List<OneOf<float, string, CardActionComponent, ValueExpressionComponent>>{
                handCount
            });
            return ret;
        }
    }
}