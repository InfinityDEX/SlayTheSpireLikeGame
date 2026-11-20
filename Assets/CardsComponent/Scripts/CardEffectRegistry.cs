using System;
using System.Collections.Generic;

/// <summary>
/// カード効果処理クラス登録クラス
/// </summary>
/// <typeparam name="T">登録したいクラス種別</typeparam>
public class CardEffectRegistry<T>
{
    /// <summary>
    /// レジストリに登録される要素の構造
    /// </summary>
    private struct Entry
    {
        public int ID;
        public T Value;
    }

    /// <summary>
    /// 登録されているインスタンスのリスト
    /// </summary>
    private List<Entry> entries;

    /// <summary>
    /// 登録されているインスタンスの数
    /// </summary>
    public int EntriesCount { get { return entries.Count; } }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CardEffectRegistry()
    {
        entries = new();
    }

    /// <summary>
    /// レジストリにインスタンス登録
    /// </summary>
    /// <param name="id">登録する要素のID</param>
    /// <param name="instance">インスタンス本体</param>
    public void Register(int id, T instance)
    {
        if (instance == null)
        {
            throw new ArgumentNullException(nameof(instance), "nullを登録しようとしました");
        }

        if (entries.Exists(entry => { return entry.ID == id; }))
        {
            throw new ArgumentException($"レジストリに重複するID：{id}を登録しようとしました");
        }

        if (entries.Exists(entry => { return entry.Value.GetType() == instance.GetType(); }))
        {
            throw new ArgumentException($"レジストリに重複するクラス[{instance.GetType().Name}]のインスタンスを登録しようとしました");
        }

        entries.Add(new Entry
        {
            ID = id,
            Value = instance
        });
    }
}
