namespace SaveDataComponent.Tests.EditMode
{
    ////////////////////////////////////
    //// スタブ                         
    ////////////////////////////////////

    /// <summary>
    /// セーブデータIO（スタブ）
    /// </summary>
    class SimpleSaveDataIO : ISaveDataIO
    {
        /// <summary>
        /// ファイルに保存しているていのセーブデータ
        /// 実際には内部で保持してロードしたら送り替えす
        /// </summary>
        private SaveDataEntity saveData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="saveDataEntity">登録するセーブデータエンティティ</param>
        public SimpleSaveDataIO(SaveDataEntity saveDataEntity)
        {
            saveData = new SaveDataEntity();
            saveData.maxHealth = saveDataEntity.maxHealth;
            saveData.currentHealth = saveDataEntity.currentHealth;
            saveData.currentFloor = saveDataEntity.currentFloor;
            saveData.money = saveDataEntity.money;
        }

        /// <summary>
        /// ファイル削除
        /// </summary>
        public void DeleteSaveData()
        {
            // 何も行わない
        }

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <returns></returns>
        public SaveDataEntity LoadData()
        {
            return saveData;
        }

        /// <summary>
        /// ファイル保存
        /// </summary>
        /// <param name="saveDataEntity"></param>
        public void SaveData(SaveDataEntity saveDataEntity)
        {
            saveData = saveDataEntity;
        }
    }
    ////////////////////////////////////
}