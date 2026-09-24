using UnityEngine;
using TMPro;

/// <summary>
/// ダメージポップアップクラス
/// 
/// 受けたダメージ値を視覚化する
/// </summary>
public class DamagePopup : MonoBehaviour
{
    [field:SerializeField, Header("ダメージ表示UI")]
    private TextMeshProUGUI damageText;

    /// <summary>
    /// ダメージ値
    /// </summary>
    private int damage;

    [field:SerializeField, Header("重力")]
    private float gravity = 0.3f;

    [field:SerializeField, Header("ダメージポップアップの寿命")]
    private float lifeTime = 3.0f;

    /// <summary>
    /// ダメージポップアップが生成されてからの累積時間
    /// </summary>
    private float elapsedTimeSinceSpawn;

    /// <summary>
    /// 現在ダメージポップアップオブジェクトにかかっている力のベクトル
    /// </summary>
    private Vector3 currentForce;

    private void Awake()
    {
        currentForce = new Vector3(
            UnityEngine.Random.Range(-.02f, .02f),
            UnityEngine.Random.Range(.01f, .02f)
        );
        elapsedTimeSinceSpawn = 0;
    }

    private void Update()
    {
        if (elapsedTimeSinceSpawn >= lifeTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            damageText.text = $"{damage}";
            transform.position += currentForce;
            currentForce += new Vector3(0, -gravity * Time.deltaTime);
            elapsedTimeSinceSpawn += Time.deltaTime;
        }
    }

    /// <summary>
    /// 表示するダメージ値を設定する
    /// </summary>
    /// <param name="value">ダメージ値</param>
    public void SetDamage(int value)
    {
        damage = value;
    }
}
