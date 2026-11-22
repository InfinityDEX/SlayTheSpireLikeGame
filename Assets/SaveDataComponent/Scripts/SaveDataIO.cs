using System;
using System.IO;
using UnityEngine;

/// <summary>
/// セーブデータ読み書きクラス
/// </summary>
public class SaveDataIO
{
    /// <summary>
    /// 保存先フォルダパス
    /// </summary>
    private string folderPath;

    /// <summary>
    /// 保存ファイル名
    /// </summary>
    private string fileName;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="folderPath">セーブデータフォルダパス</param>
    /// <param name="fileName">セーブデータファイル名</param>
    public SaveDataIO(string folderPath, string fileName)
    {
        this.folderPath = folderPath;
        this.fileName = fileName;
    }

    /// <summary>
    /// セーブデータのロード
    /// </summary>
    /// <returns>セーブデータエンティティ</returns>
    public SaveDataEntity LoadData()
    {
        string saveFilePath = Path.Combine(folderPath, fileName);

        // ファイルがあれば読み込み、無ければ新規でセーブデータエンティティを作成する。(ファイル作成はしない)
        if (File.Exists(saveFilePath))
        {
            string jsonText = File.ReadAllText(saveFilePath);
            
            // ファイルの中身が空だった時も新規でセーブデータエンティティを作成する
            if (jsonText == "" || string.IsNullOrWhiteSpace(jsonText))
            {
                Debug.LogWarning($"ファイル[{saveFilePath}]の中身が空っぽです。新規セーブデータでゲームを開始します");
                return new();
            }

            // ファイルのデータをSaveDataEntityにコンバートして返す
            try
            {
               return JsonUtility.FromJson<SaveDataEntity>(jsonText);
            }
            catch (ArgumentException ex)
            {
                // ファイル破損していることを示すエラーを吐く
                throw new InvalidDataException($"{saveFilePath}は破損しています。", ex);
            }
        }
        else
        {
            return new();
        }
    }

    /// <summary>
    /// セーブデータ保存
    /// </summary>
    /// <param name="saveDataEntity">保存するセーブデータエンティティ</param>
    public void SaveData(SaveDataEntity saveDataEntity)
    {
        // ゲームデータの保存
        string json = JsonUtility.ToJson(saveDataEntity, true);
        string saveFilePath = Path.Combine(folderPath, fileName);

        //  フォルダが存在しない場合は作成する
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // ファイルに書き込む
        File.WriteAllText(saveFilePath, json);
    }
}