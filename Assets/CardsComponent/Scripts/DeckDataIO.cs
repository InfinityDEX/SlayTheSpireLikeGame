using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VContainer;
using static CardsComponent.DeckInfomation;

namespace CardsComponent
{
    /// <summary>
    /// デッキデータIOインターフェース
    /// </summary>
    public interface IDeckDataIO
    {
        /// <summary>
        /// Jsonファイルからデッキデータ読み取る
        /// </summary>
        /// <returns>Jsonファイルから読み取ったデッキデータ</returns>
        public DeckDataJson LoadDeckDataFromJsonFile();

        /// <summary>
        /// デッキデータをJsonファイルに保存する
        /// </summary>
        /// <param name="deckDataJson">保存したいデッキデータ</param>
        public void SaveDeckDataToJsonFile(DeckDataJson deckDataJson);

        /// <summary>
        /// デッキデータJsonファイルを削除する
        /// </summary>
        public void DeleteDeckDataJsonFile();
    }

    /// <summary>
    /// デッキデータIOクラス
    /// </summary>
    public class DeckDataIO : IDeckDataIO
    {
        /// <summary>
        /// デフォルトデッキのファイルパス(Resourcesフォルダ内)
        /// </summary>
        private const string DefaultDeckDataPath = "MetaData/DefaultDeckData.json";

        /// <summary>
        /// デッキデータを保存するJsonファイルの保存フォルダパス
        /// </summary>
        private string deckDataJsonFolderPath;

        /// <summary>
        /// デッキデータJsonファイル名
        /// </summary>
        private string deckDataJsonFileName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="deckDataJsonFolderPath">デッキデータファイル保存パス</param>
        /// <param name="deckDataJsonFileName">デッキデータファイル名</param>
        public DeckDataIO(string deckDataJsonFolderPath, string deckDataJsonFileName)
        {
            this.deckDataJsonFolderPath = deckDataJsonFolderPath;
            this.deckDataJsonFileName = deckDataJsonFileName;
        }

        /// <summary>
        /// Jsonファイルからデッキデータ読み取る
        /// </summary>
        /// <returns>Jsonファイルから読み取ったデッキデータ</returns>
        public DeckDataJson LoadDeckDataFromJsonFile()
        {
            // DeckData.jsonを読み込む
            string filePath = Path.Combine(deckDataJsonFolderPath, deckDataJsonFileName);

            //  フォルダが存在しない場合は作成する
            if (!Directory.Exists(deckDataJsonFolderPath))
            {
                Directory.CreateDirectory(deckDataJsonFolderPath);
            }

            // ファイルがない場合はデフォルトデッキデータをコピーして新規作成する。
            if (!File.Exists(filePath))
            {
                Debug.Log($"{filePath}が見つかりません。Resourcesフォルダ内の{DefaultDeckDataPath}を複製して新規作成します: ");

                // デフォルトデッキデータのパス
                TextAsset defaultDeckTextAsset = Resources.Load<TextAsset>(DefaultDeckDataPath.Replace(".json", ""));
                if (defaultDeckTextAsset != null)
                {
                    // DefaultDeckData.jsonの内容をDeckData.jsonに新規作成
                    File.WriteAllText(filePath, defaultDeckTextAsset.text);
                    Debug.Log("DefaultDeckData.jsonをDeckData.jsonとしてコピーしました。");
                }
                else
                {
                    throw new FileNotFoundException("DefaultDeckData.jsonがビルドデータ内にありません。");
                }
            }

            string json = System.IO.File.ReadAllText(filePath);

            return JsonUtility.FromJson<DeckDataJson>(json);
        }

        /// <summary>
        /// デッキデータをJsonファイルに保存する
        /// </summary>
        /// <param name="deckDataJson">保存したいデッキデータ</param>
        public void SaveDeckDataToJsonFile(DeckDataJson deckDataJson)
        {
            // デッキデータの書き込み
            string json = JsonUtility.ToJson(deckDataJson, true);
            string folderPath = Path.Combine(Application.persistentDataPath, deckDataJsonFolderPath);
            string filePath = Path.Combine(folderPath, deckDataJsonFileName);

            //  フォルダが存在しない場合は作成する
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // ファイルに書き込む
            File.WriteAllText(filePath, json);
        }
    
        /// <summary>
        /// デッキデータJsonファイルを削除する
        /// </summary>
        public void DeleteDeckDataJsonFile()
        {
            string folderPath = Path.Combine(Application.persistentDataPath, deckDataJsonFolderPath);
            string filePath = Path.Combine(folderPath, deckDataJsonFileName);

            //  フォルダが存在しない場合は削除することはできないので以降の処理をスキップする
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Deckデータファイルを削除しました: " + filePath);
            }
            else
            {
                Debug.LogWarning("Deckデータファイルが存在しません: " + filePath);
            }
        }
    }
}
