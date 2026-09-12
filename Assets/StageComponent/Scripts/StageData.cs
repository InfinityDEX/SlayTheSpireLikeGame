using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    [Header("ステージ名")]
    public string stageName;

    [Header("このステージのバトルBGM候補")]
    public AudioClip[] battleBgmCandidates;

    [Header("ノーマルエネミーの組み合わせリスト")]
    public List<EnemyCombinationData> normalEnemies;

    [Header("エリートエネミーの組み合わせリスト")]
    public List<EnemyCombinationData> eliteEnemies;

    [Header("ボスエネミー（1種のみ）")]
    public EnemyCombinationData bossEnemy;
}