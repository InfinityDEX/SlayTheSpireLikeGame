using UnityEngine;
using UnityEngine.UI;

public class MapGirdFlicker : MonoBehaviour
{
    [Header("機能有効状態")]
    [SerializeField]
    public bool isEnabled = false; // デフォルトは無効

    [Header("点滅させる対象")]
    [SerializeField]
    private Graphic target;

    [Header("点滅周期[s]")]
    [SerializeField]
    private float cycle = 1;

    [Header("高速点滅モード")]
    [SerializeField]
    public bool fastBlinkMode = false; // 高速点滅モード

    [Header("高速点滅時の周期倍率（X倍速）")]
    [SerializeField]
    private float fastBlinkMultiplier = 4f; // 高速時はデフォルトで4倍速

    private double time;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<Graphic>();
    }

    private void Update()
    {
        if (target == null)
            return;
        if (isEnabled)
        {
            // 内部時刻を経過させる
            time += Time.deltaTime;

            // 高速点滅モード時は高速点滅周期で計算
            float currentCycle = fastBlinkMode ? cycle / fastBlinkMultiplier : cycle;
            var repeatValue = Mathf.Repeat((float)time, currentCycle);

            // 内部時刻timeにおける明滅状態を反映
            target.enabled = repeatValue >= currentCycle * 0.5f;
        }
        else
        {
            // 常に表示
            target.enabled = true;
        }
    }
}
