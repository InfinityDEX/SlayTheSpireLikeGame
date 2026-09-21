/// <summary>
/// シングルターゲット筋力追加クラス
/// 
/// 一つのターゲットを対象とした筋力追加効果を定義する
/// </summary>
public class STMuscle : CardEffect
{    
    /// <summary>
    /// 追加する筋力値
    /// </summary>
    private int muscle;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="_muscle">追加する筋力値</param>
    public STMuscle(int _muscle)
    {
        muscle = _muscle;
    }

    /// <summary>
    /// カードの実行（バフ）
    /// </summary>
    /// <param name="target">バフ対象</param>
    public override void Play(Creature target)
    {
        target.AddMuscle(muscle);
    }
}