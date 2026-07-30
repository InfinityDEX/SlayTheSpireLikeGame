using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    public string stageName;
    public EnemyData[] enemies;

    [Header("このステージのバトルBGM候補")]
    public AudioClip[] battleBgmCandidates;
}