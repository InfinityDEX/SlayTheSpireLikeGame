using UnityEngine;
using TMPro;

/// <summary>
/// カード実体クラス
///
/// カードの効果や見た目の定義を管理する
/// </summary>
public class Card : MonoBehaviour
{
    /// <summary>
    /// カードデータ
    /// </summary>
    public CardData ThisCardData { get; private set;}

    /// <summary>
    /// カードID
    /// </summary>
    public int ID { get {return ThisCardData.CardID;} }

    /// <summary>
    /// 手札の何番目か
    /// </summary>
    public int HandIdx { get; private set; }

    [SerializeField, Header("カード名表示GUI")]
    private TextMeshProUGUI cardNameText;

    [SerializeField, Header("カード効果テキスト表示GUI")]
    private TextMeshProUGUI cardDescriptionText;

    [SerializeField, Header("カードコスト表示GUI")]
    private TextMeshProUGUI cardCostText;

    [SerializeField, Header("カード種別表示GUI")]
    private TextMeshProUGUI cardTypeText;

    [SerializeField, Header("カード画像表示GUI")]
    private UnityEngine.UI.Image cardImage;

    private void Start()
    {
        InitializeCardUI();
    }

    /// <summary>
    /// カード効果の実行
    /// </summary>
    /// <param name="creature">ターゲット生物</param>
    public void Play(Creature creature)
    {
        ThisCardData.Play(creature);
        Debug.Log(ThisCardData.CardName + "を使用");
    }

    /// <summary>
    /// カードデータ初期化処理
    /// </summary>
    /// <param name="cardData">カードデータ</param>
    /// <param name="idx">インデックス(手札の何番目か)</param>
    public void Setup(CardData cardData, int idx)
    {
        if (idx < 0)
        {
            if (idx == -1)
            {
                Debug.Log("手札インデックスが-1 (手札として本オブジェクトを運用していない時の値): " + idx);
            }
            else
            {
                Debug.LogError("手札インデックスが0未満です: " + idx);
                return;
            }
        }
        HandIdx = idx;
        if (cardData == null)
        {
            Debug.LogError("カードデータがnullです。");
            return;
        }
        ThisCardData = cardData;
        InitializeCardUI();
    }

    /// <summary>
    /// カードの見た目初期化処理
    /// </summary>
    public void InitializeCardUI()
    {
        cardNameText.text = ThisCardData.CardName;
        cardDescriptionText.text = ThisCardData.CardDescription;
        cardCostText.text = ThisCardData.Cost.ToString();
        cardTypeText.text = ThisCardData.Type.ToString();
        cardImage.sprite = ThisCardData.CardImage;
        cardImage.color = new UnityEngine.Color(1, 1, 1, 1);
    }
}
