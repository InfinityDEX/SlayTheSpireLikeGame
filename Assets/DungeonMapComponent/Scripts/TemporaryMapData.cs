
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// バトル画面で使用する為の一時的なマップデータ管理クラス
/// </summary>
public class TemporaryMapData : MonoBehaviour
{
    /// <summary>
    /// マップのマスオブジェクトリスト
    /// </summary>
    private List<List<MapGrid>> mapGrids;

    /// <summary>
    /// 移動経路
    /// </summary>
    private List<MapGridJsonEntity.MapGridPos> movePath;

    /// <summary>
    /// 現在の位置
    /// </summary>
    private MapGridJsonEntity.MapGridPos currentGridPos;

    /// <summary>
    /// セーブデータの保存パス
    /// </summary>
    private string saveDataFilePath;

    /// <summary>
    /// 最終フロアか？
    /// </summary>
    public bool CurrentFloorIsFinal { get { return currentGridPos.Row + 1 == mapGrids.Count;}}

    /// <summary>
    /// ダンジョンマップデータ初期化
    /// </summary>
    /// <param name="mapGrids">マップグリッド</param>
    /// <param name="movePath">移動経路</param>
    /// <param name="currentGridPos">現在位置</param>
    /// <param name="saveDataFilePath">ダンジョンマップセーブデータファイルのパス</param>
    public void Init(
        List<List<MapGrid>> mapGrids,
        List<MapGridJsonEntity.MapGridPos> movePath,
        MapGridJsonEntity.MapGridPos currentGridPos,
        string saveDataFilePath
        )
    {
        this.mapGrids = mapGrids;
        this.movePath = movePath;
        this.currentGridPos = currentGridPos;
        this.saveDataFilePath = saveDataFilePath;
    }

    // TODO:↓DungeonMapManagerクラスにまったく同じ関数が書いてあるのでどうにかして一緒にしたい。

    /// <summary>
    /// ダンジョンマップ情報の永続化（セーブ）
    /// </summary>
    public void SaveDungeonMapData()
    {
        DungeonMapJson dungeonMapJson = new ();

        for (int row = 0; row < mapGrids.Count; row++)
        {
            DungeonFloorJson floor = new();
            for (int column = 0; column < mapGrids[row].Count; column++)
            {
                MapGridJsonEntity mapGridJson = new()
                {
                    mapGridInfoID = mapGrids[row][column].gridInfoId,
                    Pos = mapGrids[row][column].pos
                };
                foreach (var item in mapGrids[row][column].upperLayer)
                {
                    mapGridJson.UpperMapGridPos.Add(item.pos);
                }
                foreach (var item in mapGrids[row][column].lowerLayer)
                {
                    mapGridJson.LowerMapGridPos.Add(item.pos);
                }

                floor.FloorGrids.Add(mapGridJson);
            }
            dungeonMapJson.DungeonMapGrid.Add(floor);
        }

        dungeonMapJson.MovePath = movePath;

        dungeonMapJson.CurrentGridPos = currentGridPos;

        var jsonText = JsonUtility.ToJson(dungeonMapJson, true);
        string saveFilePath = System.IO.Path.Combine(Application.dataPath, saveDataFilePath);
        System.IO.File.WriteAllText(saveFilePath, jsonText, Encoding.UTF8);

        // セーブデータの書き換え・保存
        var saveDataHolder = SaveDataHolder.Instance;
        saveDataHolder.SetCurrentFloor(currentGridPos.Row);
        saveDataHolder.WriteSaveData();
    }
}