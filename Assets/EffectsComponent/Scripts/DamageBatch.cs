using UnityEngine;
using TMPro;

public class DamageBatch : MonoBehaviour
{
    [Header("ダメージ表示UI")]
    [SerializeField]
    private TextMeshProUGUI damageText;

    private int damage;

 [Header("重力")]
    [SerializeField]
    private float gravity = 0.3f;

    [Header("ダメージバッチの寿命")]
    [SerializeField]
    private float lifeTime = 3.0f;

    private float currentTime;
    private Vector3 currentForce;

    private void Awake()
    {
        currentForce = new Vector3(
            UnityEngine.Random.Range(-.02f, .02f),
            UnityEngine.Random.Range(.01f, .02f)
        );
        currentTime = 0;
    }

    private void Update()
    {
        if (currentTime >= lifeTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            damageText.text = $"{damage}";
            transform.position += currentForce;
            currentForce += new Vector3(0, -gravity * Time.deltaTime);
            currentTime += Time.deltaTime;
        }
    }

    public void SetDamage(int value)
    {
        damage = value;
    }
}
