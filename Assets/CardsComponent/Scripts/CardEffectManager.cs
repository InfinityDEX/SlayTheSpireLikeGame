using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public Dictionary<int, CardEffect> cardEffects;

    public static CardEffectManager instance{get; private set;}

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
            { 4, new STShield(5) }, // 盾
            { 5, new STShield(8) }, // 盾＋
            { 6, new STMuscle(3) }, // バンプアップ
            { 7, new STMuscle(6) }, // バンプアップ＋
        };

    }

    public void Play(int id, Creature target)
    {
        cardEffects[id].Play(target);
    }
}
