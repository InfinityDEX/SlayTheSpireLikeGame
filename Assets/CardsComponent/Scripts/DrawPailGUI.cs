using UnityEngine;
using TMPro;

/// <summary>
/// 山札GUIクラス
/// </summary>
public class DrawPailGUI : MonoBehaviour
{
    [SerializeField, Header("山札残り枚数")]
    private TextMeshProUGUI counter;
    
    [SerializeField, Header("デッキマネージャ")]
    private DeckManager deckManager;

    private void Update()
    {
        counter.text = $"{deckManager.drawPile.Count}";
    }
}
