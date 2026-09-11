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

            // 周期cycleで繰り返す値の取得
            // 0～cycleの範囲の値が得られる
            var repeatValue = Mathf.Repeat((float)time, cycle);

            // 内部時刻timeにおける明滅状態を反映
            target.enabled = repeatValue >= cycle * 0.5f;
        }
        else
        {
            // 常に表示
            target.enabled = true;
        }
    }
}
