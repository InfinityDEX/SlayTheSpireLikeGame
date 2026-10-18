using CardEffect.Expressions;

namespace CardEffect.CardActionModules
{
    /// <summary>
    /// ドローモジュール
    ///
    /// カードのドロー効果を定義する
    /// </summary>
    public class DrawCard : CardActionComponent
    {
        /// <summary>
        /// 自己複製コンポーネント
        /// </summary>
        /// <returns>複製したコンポーネント</returns>
        public override CardActionComponent Spawn()
        {
            return new DrawCard();
        }

        /// <summary>
        /// カードの実行
        /// </summary>
        /// <param name="target">攻撃対象</param>
        /// <returns>null</returns>
        public override ValueExpressionComponent Execute(Creature target)
        {
            float drawCardNum = 0;
            if (Args[0].IsT0)
            {
                var preVal = Args[0].AsT0.Execute(null).EvaluateToValue();
                if (preVal.IsT1)
                {
                    // string型は非対応
                    throw new System.InvalidOperationException("DrawCardモジュールに再帰的な計算を実施すると最終的にstring型が出力されるArgsが渡されました。");
                }
                else if (preVal.IsT0)
                {
                    drawCardNum = preVal.AsT0;
                }
            }
            else if (Args[0].IsT1)
            {
                var preVal = Args[0].AsT1.EvaluateToValue();
                if (preVal.IsT1)
                {
                    // string型は非対応
                    throw new System.InvalidOperationException("DrawCardモジュールに再帰的な計算を実施すると最終的にstring型が出力されるArgsが渡されました。");
                }
                else if (preVal.IsT0)
                {
                    drawCardNum = preVal.AsT0;
                }
            }

            // パラメータで設定された数だけカードを引く
            for (int i = 0; i < drawCardNum; i++)
            {
                DeckManager.Instance.DrawCard();
            }
            // 手札の見た目を修整(ゲーム中に表示するカードオブジェクトは現在バトルマネージャが管理している)
            BattleManager.Instance.ResetHandCardView();
            return null;
        }
    }
}