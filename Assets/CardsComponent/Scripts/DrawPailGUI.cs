using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DrawPailGUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI counter;
    [SerializeField]
    private DeckManager deckManager;

    private void Update()
    {
        counter.text = $"{deckManager.drawPile.Count}";
    }
}
