using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カード効果マネージャクラス
/// 
/// カード効果を管理する
/// 一部のカードの処理は流用できる物がある為、（ダメージ量が違うだけの攻撃系カード等）
/// カードデータは違うが効果はほぼ同じというカードの
/// 効果処理を共有できるようにしている。
/// 
/// シングルトン
/// </summary>
public class CardEffectManager : MonoBehaviour
{
    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static CardEffectManager instance{get; private set;}


    /// <summary>
    /// カード効果リスト
    /// 
    /// IDとカード効果本体をセットで管理
    /// カードデータにはIDの方が登録されていて
    /// そのIDをもとに効果処理を呼び出す。
    /// </summary>
    public Dictionary<int, CardEffect> cardEffects;

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("CardEffectManagerが複数登録されようとしました。");
            return;
        }
        if(instance != this)
        {
            instance = this;
        }
   
        cardEffects = new Dictionary<int, CardEffect>
        {
            { 0, new STAttack(5) }, // パンチ
            { 1, new STAttack(7) }, // パンチ＋
            { 2, new STAttack(10) }, // スラッシュ
            { 3, new STAttack(12) }, // スラッシュ＋
            { 4, new STBlock(5) }, // 盾
            { 5, new STBlock(8) }, // 盾＋
            { 6, new STMuscle(3) }, // バンプアップ
            { 7, new STMuscle(6) }, // バンプアップ＋
        };
    }

    /// <summary>
    /// カード効果の実行
    /// </summary>
    /// <param name="id">呼び出したい効果のID</param>
    /// <param name="target">効果対象</param>
    public void Play(int id, Creature target)
    {
        cardEffects[id].Play(target);
    }
}
