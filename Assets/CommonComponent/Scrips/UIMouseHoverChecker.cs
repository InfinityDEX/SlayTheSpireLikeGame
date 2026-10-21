using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// マウスホバーチェッカー(UI用)
/// </summary>
public class UIMouseHoverChecker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsHovered { get; private set; }

    // マウスが乗ったとき
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovered = true;
    }

    // マウスが離れたとき
    public void OnPointerExit(PointerEventData eventData)
    {
        IsHovered = false;
    }

    // 非アクティブになったときは安全のためにフラグをリセット
    private void OnDisable()
    {
        IsHovered = false;
    }
}