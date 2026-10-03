using CardEffect.Expressions;

namespace CardEffect.CardActionModules
{
    /// <summary>
    /// 筋力追加クラス
    /// 
    /// 一つのターゲットを対象とした筋力追加効果を定義する
    /// </summary>
    public class AddMuscle : CardActionModule
    {    
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="muscleVal">筋力値</param>
        public AddMuscle(Number muscleVal)
        {
            // 設定する引数は1つ
            for (int i = 0; i < 1; i++)
            {
                args.Add(null);
            }
            args[0] = muscleVal;
        }

        /// <summary>
        /// カードの実行
        /// </summary>
        /// <param name="target">バフ対象</param>
        public override void Execute(Creature target)
        {
            target.AddMuscle((int)args[0].Evaluate());
        }
    }
}