using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ダンジョンマップグリッドを点滅させるクラス
/// </summary>
public class MapGirdFlicker : MonoBehaviour
{
    [field:SerializeField, Header("機能有効状態")]
    public bool isBlinkEnabled = false; // デフォルトは無効

    [field:SerializeField, Header("点滅させる対象")]
    private Graphic target;

    [field:SerializeField, Header("点滅周期[s]")]
    private float blinkCycle = 1;

    [field:SerializeField, Header("高速点滅モード")]
    public bool fastBlinkMode = false;

    [field:SerializeField, Header("高速点滅時の周期倍率（X倍速）")]
    private float fastBlinkMultiplier = 4f; // 高速時はデフォルトで4倍速

    /// <summary>
    /// 点滅してからの経過時間
    /// </summary>
    private double elapsedBlinkTime;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<Graphic>();
    }

    private void Update()
    {
        if (target == null)
            return;

        if (isBlinkEnabled)
        {
            // 内部時刻を経過させる
            elapsedBlinkTime += Time.deltaTime;

            // 高速点滅モード時は高速点滅周期で計算
            float currentCycle = fastBlinkMode ? blinkCycle / fastBlinkMultiplier : blinkCycle;
            var repeatValue = Mathf.Repeat((float)elapsedBlinkTime, currentCycle);

            // 内部時刻timeにおける明滅状態を反映
            target.enabled = repeatValue >= currentCycle * 0.5f;
        }
        else
        {
            // 常に表示
            target.enabled = true;

            // 明滅してからの経過時間を0にする
            elapsedBlinkTime = 0;
        }
    }
}
