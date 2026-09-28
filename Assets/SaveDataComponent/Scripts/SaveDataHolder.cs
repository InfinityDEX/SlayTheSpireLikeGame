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
    /// シングルトンインスタンス
    /// </summary>
    public static SaveDataHolder Instance { get; private set;}

    /// <summary>
    /// セーブデータエンティティ
    /// </summary>
    public SaveDataEntity SaveData { get; private set; }

    public void Awake()
    {
        // セーブデータホルダーは複数個存在できない
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // 特定のタイミングを除き破壊不能
        DontDestroyOnLoad(this.gameObject);

        // ゲームデータの読み込み
        string saveFilePath = System.IO.Path.Combine(Application.dataPath, "SaveData/SaveData.json");
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
    /// セーブデータを保存する
    /// </summary>
    public void WriteSaveData()
    {
        // ゲームデータの保存
        string json = JsonUtility.ToJson(SaveData, true);
        string saveFilePath = System.IO.Path.Combine(Application.dataPath, "SaveData/SaveData.json");
        System.IO.File.WriteAllText(saveFilePath, json);
    }

    /// <summary>
    /// セーブデータファイルを破棄する
    /// </summary>
    public void DeleteSaveDataFile()
    {
        string saveFilePath = System.IO.Path.Combine(Application.dataPath, "SaveData/SaveData.json");
        if (System.IO.File.Exists(saveFilePath))
        {
            System.IO.File.Delete(saveFilePath);
        }
    }
}
