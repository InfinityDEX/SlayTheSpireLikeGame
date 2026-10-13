using CardEffect.Expressions;

namespace CardEffect.CardActionModules
{
    /// <summary>
    /// ブロック追加クラス
    ///
    /// 一つのターゲットを対象としたブロック追加効果を定義する
    /// </summary>
    public class AddBlock : CardActionComponent
    {
        /// <summary>
        /// 自己複製コンポーネント
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override CardActionComponent Spawn()
        {
            return new AddBlock();
        }

        /// <summary>
        /// カードの実行
        /// </summary>
        /// <param name="target">バフ対象</param>
        /// <returns>null</returns>
        public override ValueExpressionComponent Execute(Creature target)
        {
            // 型チェック
            if (!(Args.Count == 1 && Args[0].IsT1 == true && Args[0].AsT1 is Number))
            {
                throw new System.InvalidCastException("Args[0] is not of type Number.");
            }

            // 対象にブロック追加
            target.AddBlock((int)((Number)Args[0].AsT1).Evaluate().AsT0);

            return null;
        }
    }
}