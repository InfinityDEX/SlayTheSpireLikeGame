using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージデータクラス
/// 
/// ステージ（ダンジョン）の名前や再生するBGMのリスト、
/// 登場するエネミー等を記載する
/// </summary>
[CreateAssetMenu(fileName = "StageData", menuName = "Stage Data")]
public class StageData : ScriptableObject
{
    [field:SerializeField, Header("ステージ名")]
    public string stageName;

    [field:SerializeField, Header("このステージのバトルBGM候補")]
    public AudioClip[] battleBgmCandidates;

    [field:SerializeField, Header("ノーマルエネミーの組み合わせリスト")]
    public List<EnemyCombinationData> normalEnemies;

    [field:SerializeField, Header("エリートエネミーの組み合わせリスト")]
    public List<EnemyCombinationData> eliteEnemies;

    [field:SerializeField, Header("ボスエネミー（1種のみ）")]
    public EnemyCombinationData bossEnemy;
}