// シングルターゲット筋力クラス
public class STMuscle : CardEffect
{    
    private int muscle;

    public STMuscle(int muscleNum)
    {
        muscle = muscleNum;
    }

    public override void Play(Creature target)
    {
        target.AddMuscle(muscle);
    }
}