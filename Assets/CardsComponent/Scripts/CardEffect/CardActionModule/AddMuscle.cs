using CardEffect.Expressions;

namespace CardEffect.CardActionModules
{
    /// <summary>
    /// 筋力追加クラス
    ///
    /// 一つのターゲットを対象とした筋力追加効果を定義する
    /// </summary>
    public class AddMuscle : CardActionComponent
    {
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

            // 対象に筋力追加
            target.AddMuscle((int)((Number)Args[0].AsT1).Evaluate().AsT0);

            return null;
        }
    }
}