using CardEffect.Expressions;

namespace CardEffect.CardActionModules
{
    /// <summary>
    /// 脆弱追加クラス
    /// 
    /// 一つのターゲットを対象とした脆弱追加効果を定義する
    /// </summary>
    public class Weakened : CardActionModule
    {    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="weakenedVal">脆弱値</param>
        public Weakened(Number weakenedVal)
        {
            // 設定する引数は1つ
            for (int i = 0; i < 1; i++)
            {
                args.Add(null);
            }
            args[0] = weakenedVal;
        }

        /// <summary>
        /// カードの実行
        /// </summary>
        /// <param name="target">バフ対象</param>
        public override void Execute(Creature target)
        {
            target.AddWeakened((int)args[0].Evaluate());
        }
    }
}