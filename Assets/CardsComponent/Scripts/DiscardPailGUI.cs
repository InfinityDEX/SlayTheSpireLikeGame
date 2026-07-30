using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiscardPailGUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI counter;
    [SerializeField]
    private DeckManager deckManager;

    private void Update()
    {
        counter.text = $"{deckManager.discardPile.Count}";
    }
}
