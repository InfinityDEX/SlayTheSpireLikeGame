using System.IO;
using UnityEngine;

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

    public void Awake()
    {
        // 特定のタイミングを除き破壊不能
        DontDestroyOnLoad(this.gameObject);

        // ゲームデータの読み込み
        string folderPath = Path.Combine(Application.persistentDataPath, "SaveData");
        string saveFilePath = System.IO.Path.Combine(folderPath, "SaveData.json");

        //  フォルダが存在しない場合は作成する
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ファイルがあれば読み込み、無ければ新規でセーブデータオブジェクトを作成する。(ファイル作成はしない)
        if (System.IO.File.Exists(saveFilePath))
        {
            string jsonText = System.IO.File.ReadAllText(saveFilePath);
            SaveData = JsonUtility.FromJson<SaveDataEntity>(jsonText);
        }
        else
        {
            SaveData = new();
        }
    }

    /// <summary>
    /// 最大体力値の更新
    /// </summary>
    /// <param name="maxHealth">最大体力値</param>
    public void SetMaxHealth(int maxHealth)
    {
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
    /// <param name="money">追加する金額</param>
    public void AddMoney(int money)
    {
        SaveData.money += money;
    }

    /// <summary>
    /// 所持金の使用
    /// </summary>
    /// <param name="money">使用したい金額</param>
    /// <returns>所持金を消費できたか？</returns>
    public bool UseMoney(int money)
    {
        if (money <= SaveData.money)
        {
            SaveData.money -= money;
            return true;
        }
        else
        {
            return false;
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
        // ゲームデータの保存
        string json = JsonUtility.ToJson(SaveData, true);
        string folderPath = Path.Combine(Application.persistentDataPath, "SaveData");
        string saveFilePath = Path.Combine(folderPath, "SaveData.json");

        //  フォルダが存在しない場合は作成する
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ファイルに書き込む
        File.WriteAllText(saveFilePath, json);
    }

    /// <summary>
    /// セーブデータファイルを破棄する
    /// </summary>
    public void DeleteSaveDataFile()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "SaveData");
        string saveFilePath = Path.Combine(folderPath, "SaveData.json");

        // フォルダがないならそもそもセーブデータファイルもないので以降の処理をスキップする
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }
    }
}
