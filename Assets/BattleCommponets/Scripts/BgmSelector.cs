using UnityEngine;

public static class BgmSelector
{
    public static AudioClip PickBattleBgm(StageData stage)
    {
        if (stage == null || stage.battleBgmCandidates.Length == 0)
            return null;

        int index = Random.Range(0, stage.battleBgmCandidates.Length);
        return stage.battleBgmCandidates[index];
    }
}