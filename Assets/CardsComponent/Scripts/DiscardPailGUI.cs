using UnityEngine;
using TMPro;

/// <summary>
/// 捨て札UIクラス
/// </summary>
public class DiscardPailGUI : MonoBehaviour
{
    [SerializeField, Header("捨て札数")]
    private TextMeshProUGUI counter;
    
    [SerializeField, Header("デッキマネージャ")]
    private DeckManager deckManager;

    private void Update()
    {
        counter.text = $"{deckManager.discardPile.Count}";
    }
}
