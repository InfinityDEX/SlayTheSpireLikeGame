using System;
using System.Collections.Generic;

/// <summary>
/// カード効果処理クラス登録クラス
/// </summary>
/// <typeparam name="T">登録するクラス種別</typeparam>
public class CardEffectRegistry<T> where T: class, IPrototype<T>
{
    /// <summary>
    /// レジストリに登録される要素の構造
    /// </summary>
    private struct Entry
    {
        public int ID; // 登録要素のID
        public T Body; // 登録しているインスタンス本体
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
            throw new ArgumentException($"レジストリに重複するID：[{id}]を登録しようとしました");
        }

        if (entries.Exists(entry => { return entry.Body.GetType() == instance.GetType(); }))
        {
            throw new ArgumentException($"レジストリに重複するクラス[{instance.GetType().Name}]のインスタンスを登録しようとしました");
        }

        entries.Add(new Entry
        {
            ID = id,
            Body = instance
        });
    }

    /// <summary>
    /// 登録されている要素を取得
    /// 
    /// IDを指定し、もしも見つかればinstanceに
    /// 対応する要素の複製を返す。
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="instance">登録されている要素の複製インスタンス</param>
    /// <returns>取得に成功したか？</returns>
    public bool TryGet(int id, out T instance)
    {
        // IDで登録されているEntryを検索して、中に入っているインスタンス(value)を返す
        var searchResult = entries.Find(entry => entry.ID == id);
        instance = searchResult.Body?.Spawn();
        
        // nullは例外処理で登録できないようにしているので、valueがnullのデータが返ってきたら
        // 見つからなかった物として判断する。
        return searchResult.Body != null;
    }
}
