using System;
using UnityEngine;
using VContainer;

/// <summary>
/// セーブデータホルダー
///
/// ゲームプレイ中常に破壊されず保持され、SaveDataEntityの値を保持する。
/// 現在のセーブデータの共有と、セーブデータの更新、Jsonファイルへの出力する機能を持つ。
/// セーブデータの設定値は常にゲーム内の最新の情報と同期されていること。（プレイヤーの現在の体力値等）
/// </summary>
public class SaveDataHolder : MonoBehaviour
{
    /// <summary>
    /// セーブデータエンティティ
    /// </summary>
    public SaveDataEntity SaveData { get; private set; }

    /// <summary>
    /// セーブデータIO
    /// </summary>
    private ISaveDataIO saveDataIO;

    /// <summary>
    /// 依存性注入
    /// </summary>
    /// <param name="saveDataIO">セーブデータIO</param>
    [Inject]
    public void Init(ISaveDataIO saveDataIO)
    {
        this.saveDataIO = saveDataIO;
    }

    /// <summary>
    /// セーブデータのロード
    /// </summary>
    public void LoadSaveData()
    {
        // すでにセーブデータが存在する場合はロード処理をスキップする
        if (SaveData != null)
        {
            return;
        }

        SaveData = saveDataIO.LoadData();
    }

    /// <summary>
    /// 最大体力値の更新
    /// </summary>
    /// <param name="maxHealth">最大体力値</param>
    public void SetMaxHealth(int maxHealth)
    {
        // 処理のタイミングによりゲームオーバー等でセーブデータインスタンスが破棄済みの場合があるので、nullチェックを行い
        // nullの場合は処理をスキップする
        if (SaveData == null)
            return;

        // maxHealthが0以上であれば更新
        if (maxHealth >= 0)
        {
            SaveData.maxHealth = maxHealth;

            // もしも最大体力が現在の体力を下回ってしまった場合、現在の体力を最大体力に更新する
            if (SaveData.maxHealth < SaveData.currentHealth)
                SaveData.currentHealth = SaveData.maxHealth;
        }
    }

    /// <summary>
    /// 現在の体力値の更新
    /// </summary>
    /// <param name="currentHealth">現在の体力値</param>
    public void SetCurrentHealth(int currentHealth)
    {
        // 処理のタイミングによりゲームオーバー等でセーブデータインスタンスが破棄済みの場合があるので、nullチェックを行い
        // nullの場合は処理をスキップする
        if (SaveData == null)
            return;

        // currentHealthが0以上で、かつMaxHealthを超えていなければ更新
        if (currentHealth >= 0 && currentHealth <= SaveData.maxHealth)
            SaveData.currentHealth = currentHealth;
    }

    /// <summary>
    /// 体力全回復
    /// </summary>
    public void FullRecovery()
    {
        SaveData.currentHealth = SaveData.maxHealth;
    }

    /// <summary>
    /// 所持金の追加
    /// </summary>
    /// <param name="amount">追加する金額</param>
    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), $"追加したい金額として[{amount}]が指定されました。負の数は使用できません。");
        }
        SaveData.money += amount;
    }

    /// <summary>
    /// 支払い可能か確認
    /// </summary>
    /// <param name="cost">支払いたい金額</param>
    /// <returns>支払い可能か？</returns>
    public bool CanAfford(int cost)
    {
        if (cost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cost), $"消費できるか確認したい金額として[{cost}]が指定されました。負の数は使用できません。");
        }

        return cost <= SaveData.money; // costが現在の所持金以下ならtrue
    }

    /// <summary>
    /// 所持金の使用
    /// </summary>
    /// <param name="cost">使用したい金額</param>
    /// <returns>所持金を消費できたか？</returns>
    public bool UseMoney(int cost)
    {
        if (cost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cost), $"消費したい金額として[{cost}]が指定されました。負の数は使用できません。");
        }

        if (CanAfford(cost))
        {
            SaveData.money -= cost;
            return true;
        }
        else
        {
            throw new InvalidOperationException($"Moneyを[{cost}]消費しようとしました。現在の所持金は[{SaveData.money}]でした。");
        }
    }

    /// <summary>
    /// 現在の階層数情報を更新
    /// </summary>
    /// <param name="floorNum">現在の階層</param>
    public void SetCurrentFloor(int floorNum)
    {
        SaveData.currentFloor = floorNum;
    }

    /// <summary>
    /// セーブデータを保存する
    /// </summary>
    public void WriteSaveData()
    {
        // セーブデータの保存
        saveDataIO.SaveData(SaveData);
    }

    /// <summary>
    /// セーブデータファイルを破棄する
    /// </summary>
    public void DeleteSaveDataFile()
    {
        // セーブデータの削除
        saveDataIO.DeleteSaveData();
    }

    /// <summary>
    /// セーブデータインスタンスの初期化
    /// </summary>
    public void ClearSaveData()
    {
        SaveData = null;
    }
}
