using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// マップグリッドクラス
/// </summary>
public class MapGrid : MonoBehaviour
{
    [field:SerializeField, Header("機能有効状態")]
    public bool isEnabled = false; // デフォルトは無効

    [field:SerializeField, Header("グリッド画像表示オブジェクト")]
    private Image gridImage;

    [field:SerializeField, Header("グリッド情報")]
    public MapGridInfo gridInfo;

    /// <summary>
    /// マップグリッド情報のID
    /// </summary>
    public int gridInfoId;

    [field:SerializeField, Header("隣接する上層")]
    public List<MapGrid> upperLayer;

    [field:SerializeField, Header("隣接する下層")]
    public List<MapGrid> lowerLayer;

    [field:SerializeField, Header("経路線のPrefab")]
    private GameObject pathLinePrefab;

    /// <summary>
    /// ダンジョンマップ上の本マップグリッドの座標
    /// </summary>
    public MapGridJsonEntity.MapGridPos pos;

    private struct DashedUILineRendererWithCol {
        public int col;
        public DashedUILineRenderer instance;
    }
    /// <summary>
    /// 経路線インスタンス(上層のみ)
    /// </summary>
    private List<DashedUILineRendererWithCol> upperLineInstances;
    
    /// <summary>
    /// デフォルトの経路線のスタイル
    /// </summary>
    private float defaultDashedLineDisplayLength;
    private float defaultDashedLineSpaceLength;
    private Color defaultDashedLineColor;


#if UNITY_EDITOR
    [Header("グリッド位置表示ディスプレイ(デバッグ用)")]
    [SerializeField]
    private TMPro.TextMeshProUGUI gridPosViewDisplay;
#endif

    private void Start()
    {
        var dulr = pathLinePrefab.GetComponent<DashedUILineRenderer>();
        defaultDashedLineDisplayLength = dulr.material.GetFloat("_Length");
        defaultDashedLineSpaceLength = dulr.material.GetFloat("_Space");
        defaultDashedLineColor = dulr.material.GetColor("_Color");
    }

    /// <summary>
    /// グリッド画像を点滅させる
    /// </summary>
    /// <param name="enable">点滅有効状態</param>
    /// <param name="fastBlinkMode">高速点滅モードか</param>
    public void FlickerGrid(bool enable, bool fastBlinkMode = false)
    {
        // 点滅機構はgridImageのオブジェクトが持っている
        var flicker = gridImage.gameObject.GetComponent<MapGirdFlicker>();
        if (flicker == null)
        {
            Debug.LogWarning("MapGirdFlicker コンポーネントが見つからなかったため、新規追加します。");

            // Flickerコンポーネントが存在しない場合はアタッチする
            flicker = gridImage.gameObject.AddComponent<MapGirdFlicker>();
        }
        flicker.isBlinkEnabled = enable;
        flicker.fastBlinkMode = fastBlinkMode;
    }

    /// <summary>
    /// 隣接する上層のグリッドにLineRendererで線分を引く
    /// </summary>
    /// <param name="routeLineCanvas">経路線を配置するCanvas</param>
    public void DrawLinesToUpperLayers(Canvas routeLineCanvas)
    {
        if (upperLayer == null || pathLinePrefab == null)
            return;

        foreach (var upper in upperLayer)
        {
            if (upper == null)
                continue;

            // routeLineCanvasを親として線インスタンスを生成
            Object lineIns = Instantiate(pathLinePrefab, routeLineCanvas.transform);

            // 頂点コネクタとラインレンダラを線オブジェクトのインスタンスから取得
            UILineConnector ulc = lineIns.GetComponent<UILineConnector>();
            DashedUILineRenderer dulr = lineIns.GetComponent<DashedUILineRenderer>();
            dulr.EnsureRuntimeMaterial();
            
            // 上層の経路線インスタンスを登録しておく。（進行経路の経路線のMaterialを変更をする時などに使用）
            if (upperLineInstances == null ) upperLineInstances = new();
            upperLineInstances.Add(new DashedUILineRendererWithCol { col = upper.pos.col, instance = dulr });    

            // 自身のgridImageとupperのgridImageのrectTransformを設定
            ulc.transforms = new RectTransform[] { this.gridImage.rectTransform, upper.gridImage.rectTransform };
        }
    }

    /// <summary>
    /// 経路線を指定したパスとフロアに基づいて色分けする
    /// </summary>
    /// <param name="movePath">移動経路となるグリッドの座標リスト</param>
    public void ColoringRouteLine(in List<MapGridJsonEntity.MapGridPos> movePath)
    {
        // 進行先の列数を確認する為、0(スタートマス)ではなく1(1階層目)をcurrentCheckFloorに渡す
        ColoringRouteLine(movePath, 1); 
    }

    /// <summary>
    /// 経路線を指定したパスとフロアに基づいて色分けする(再帰処理用)
    /// </summary>
    /// <param name="movePath">移動経路となるグリッドの座標リスト</param>
    /// <param name="currentCheckFloor">現在確認しているフロアのインデックス（デフォルトは0）</param>
    private void ColoringRouteLine(in List<MapGridJsonEntity.MapGridPos> movePath , int currentCheckFloor)
    {
        // 次に進むマスへの道を列数から検索する
        var nextRouteCol = movePath[currentCheckFloor].col;
        var coloringLine = upperLineInstances.Find( upperLineInfo => {return upperLineInfo.col == nextRouteCol;}).instance;
        
        coloringLine.SetDashStyle(1, 0, Color.black);

        // 移動経路を全て着色するまで進行先のMapGridのColoringRouteLineを呼び出し再帰処理を行う。
        if (currentCheckFloor + 1 < movePath.Count)
        {
            upperLayer.Find( currentMapGrid => currentMapGrid.pos.col == nextRouteCol).ColoringRouteLine(movePath, currentCheckFloor + 1);
        }
    }

    /// <summary>
    /// 自身のマスから進行が可能な全ての経路を脱色する（最初のマスが呼び出すようにしてください）
    /// </summary>
    public void ResetAllLinkLineMaterial()
    {
        // 最上層なら以降の処理をスキップ
        if (upperLineInstances == null) return;

        // 全ての進行可能経路インスタンスのマテリアルをデフォルトに戻す
        foreach (var lineIns in upperLineInstances)
        {
            lineIns.instance.SetDashStyle(
                defaultDashedLineDisplayLength,
                defaultDashedLineSpaceLength,
                defaultDashedLineColor
            );
        }
        
        // 移動経路インスタンスのマテリアルを全てデフォルトにするまで再帰処理を行う。
        foreach (var mapGridIns in upperLayer)
        {
            mapGridIns?.ResetAllLinkLineMaterial();
        }
    }

    /// <summary>
    /// グリッド情報を登録
    /// </summary>
    /// <param name="info">グリッド情報</param>
    public void SetInfo(MapGridInfo info)
    { 
        gridInfoId = info.id;
        gridInfo = info;
        // グリッド情報の登録と同時にImageに画像データを登録する
        gridImage.sprite = gridInfo.gridSprite;
    }

    /// <summary>
    /// 隣接する下層フロアを追加
    /// </summary>
    /// <param name="grid">フロアグリッド</param>
    public void SetUnderLayer(MapGrid grid)
    {
        lowerLayer.Add(grid);
    }

    /// <summary>
    /// 隣接する上層フロアを追加
    /// </summary>
    /// <param name="grid">フロアグリッド</param>
    public void SetUpperLayer(MapGrid grid)
    {
        upperLayer.Add(grid);
    }
#if UNITY_EDITOR
    /// <summary>
    /// ダンジョンマップ上の座標をマップグリッドアイコンの前に表示（デバッグ用）
    /// </summary>
    public void SetDebugView_GridPosition()
    {
        if(gridPosViewDisplay != null)
        {
            gridPosViewDisplay.text = $"Col:{pos.col}\nRow:{pos.row}";
        }
    }
#endif
}
