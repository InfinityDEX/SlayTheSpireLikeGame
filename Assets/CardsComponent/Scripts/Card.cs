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
    public CardData data;

    /// <summary>
    /// カードID
    /// </summary>
    public int id;

    [SerializeField, Header("カード名表示GUI")]
    private TextMeshProUGUI cardNameText;
    
    [SerializeField, Header("カード効果テキスト表示GUI")]
    private TextMeshProUGUI cardDescriptionText;
    
    [SerializeField, Header("カード効果表示GUI")]
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
        CardEffectManager.instance.Play(data.cardEffectId, creature);
        Debug.Log(data.cardName + "を使用");
    }

    /// <summary>
    /// カードデータ初期化処理
    /// </summary>
    /// <param name="cardData">カードデータ</param>
    /// <param name="cardId">カードID</param>
    public void Setup(CardData cardData, int cardId)
    {
        data = cardData;
        id = cardId;
        InitializeCardUI();
    }

    /// <summary>
    /// カードの見た目初期化処理
    /// </summary>
    public void InitializeCardUI()
    {
        cardNameText.text = data.cardName;
        cardDescriptionText.text = data.cardDescription;
        cardCostText.text = data.cost.ToString();
        cardTypeText.text = data.type.ToString();
        cardImage.sprite = data.cardImage;
        cardImage.color = new UnityEngine.Color(1, 1, 1, 1);
    }
}
