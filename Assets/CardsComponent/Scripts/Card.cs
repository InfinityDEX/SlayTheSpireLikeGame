using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public CardData data;
    public int id;

    public CardEffectManager cardEffectManager;

    //////////////////////////////////////////////////////////////
    /// MonoBehaviourの関数
    //////////////////////////////////////////////////////////////

    private void Start()
    {
        InitializeCardUI();
    }

    //////////////////////////////////////////////////////////////
    /// カードの効果に関する処理
    //////////////////////////////////////////////////////////////
    public void Play(Creature creature)
    {
        CardEffectManager.instance.Play(data.cardEffectId, creature);
        Debug.Log(data.cardName + "を使用");
    }

    //////////////////////////////////////////////////////////////
    // カードデータを受け取ってカードを初期化する処理
    //////////////////////////////////////////////////////////////
    public void Setup(CardData cardData, int cardId)
    {
        data = cardData;
        id = cardId;
        InitializeCardUI();
    }


    //////////////////////////////////////////////////////////////
    /// カードのUIに関する処理
    //////////////////////////////////////////////////////////////
    
    [SerializeField]
    private TextMeshProUGUI cardNameText;
    [SerializeField]
    private TextMeshProUGUI cardDescriptionText;
    [SerializeField]
    private TextMeshProUGUI cardCostText;
    [SerializeField]
    private TextMeshProUGUI cardTypeText;
    [SerializeField]
    private UnityEngine.UI.Image cardImage;

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
