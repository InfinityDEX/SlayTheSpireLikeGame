using CardEffect.Expressions;

namespace CardEffect.CardActionModules
{
    /// <summary>
    /// 攻撃モジュール
    ///
    /// 一つのターゲッを対象とした攻撃効果を定義する
    /// </summary>
    public class Attack : CardActionComponent
    {
        /// <summary>
        /// カードの実行
        /// </summary>
        /// <param name="target">攻撃対象</param>
        /// <returns>null</returns>
        public override ValueExpressionComponent Execute(Creature target)
        {
            // 型チェック
            if (!(Args.Count == 1 && Args[0].IsT1 == true && Args[0].AsT1 is Number))
            {
                throw new System.InvalidCastException("Args[0] is not of type Number.");
            }

            // 対象にダメージを与える
            target.TakeDamage((int)((Number)Args[0].AsT1).Evaluate().AsT0 + BattleManager.Instance.Player.Muscle);

            return null;
        }
    }
}