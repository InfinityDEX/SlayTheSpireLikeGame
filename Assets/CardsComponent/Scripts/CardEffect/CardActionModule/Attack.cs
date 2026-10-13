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
        /// 自己複製コンポーネント
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override CardActionComponent Spawn()
        {
            return new Attack();
        }

        /// <summary>
        /// カードの実行
        /// </summary>
        /// <param name="target">攻撃対象</param>
        /// <returns>null</returns>
        public override ValueExpressionComponent Execute(Creature target)
        {
            float damageVal = 0;
            if (Args[0].IsT0)
            {
                var preVal = Args[0].AsT0.Execute(null).EvaluateToValue();
                if (preVal.IsT1)
                {
                    // string型は非対応
                    throw new System.InvalidOperationException("Attackモジュールに再帰的な計算を実施すると最終的にstring型が出力されるArgsが渡されました。");
                }
                else if (preVal.IsT0)
                {
                    damageVal = preVal.AsT0;
                }
            }
            else if (Args[0].IsT1)
            {
                var preVal = Args[0].AsT1.EvaluateToValue();
                if (preVal.IsT1)
                {
                    // string型は非対応
                    throw new System.InvalidOperationException("Attackモジュールに再帰的な計算を実施すると最終的にstring型が出力されるArgsが渡されました。");
                }
                else if (preVal.IsT0)
                {
                    damageVal = preVal.AsT0;
                }
            }

            // 対象にダメージを与える
            target.TakeDamage((int)damageVal + BattleManager.Instance.Player.Muscle);

            return null;
        }
    }
}