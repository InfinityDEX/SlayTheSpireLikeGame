using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Creature : MonoBehaviour
{
    [SerializeField]
    private CreatureData creatureData;
    [SerializeField]
    private Image creatureSpriteInspector;
    public Image creatureSprite { get; private set;}
    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private TextMeshProUGUI healthText;
    [SerializeField]
    private int hpInspector;
    public int hp { get; private set;}
    [SerializeField]
    private int shieldInspector = 0;
    public int shield { get; private set; }
    [SerializeField]
    private int muscleInspector = 0;
    public int muscle { get; private set; }

    [Header("エフェクトバッチマネージャー")]
    [SerializeField]
    private EffectBatchManager effectBatchManager;
    private EffectBatch shieldBatch;
    private EffectBatch muscleBatch;

    private void Start()
    {
        hp = creatureData.maxHealth;
        creatureSprite = creatureSpriteInspector;
        healthSlider.value = healthSlider.maxValue = creatureData.maxHealth;
        UpdateHealthText();
        creatureSpriteInspector.sprite = creatureData.creatureSprite;
        creatureSpriteInspector.color = new Color(1, 1, 1, 1);

        hpInspector = hp;
        shieldInspector = shield;
        muscleInspector = muscle;
    }

    private void Update()
    {
        hpInspector = hp;
        shieldInspector = shield;
        muscleInspector = muscle;
        if(shield == 0)
        {
            if(shieldBatch != null)
            {
                shieldBatch.gameObject.SetActive(false);
            }
        }
        else
        {
            if(shieldBatch == null)
            {
                shieldBatch = effectBatchManager.GenerateEffectBatch(0);
            }
            shieldBatch.gameObject.SetActive(true);
            shieldBatch.effectCount = shield;
        }

        if(muscle == 0)
        {
            if(muscleBatch != null)
            {
                muscleBatch.gameObject.SetActive(false);
            }
        }
        else
        {
            if(muscleBatch == null)
            {
                muscleBatch = effectBatchManager.GenerateEffectBatch(1);
            }
            muscleBatch.gameObject.SetActive(true);
            muscleBatch.effectCount = muscle;
        }
    }


    public void AddShield(int block)
    {
        shield += block;
    }

    public void AddMuscle(int power)
    {
        muscle += power;
    }

    public void ResetBuff()
    {
        shield = 0;
        muscle = Mathf.Max(muscle - 1 , 0);
    }

    public delegate void TakeDamageEventHandler(int damage, int hp);
    private event TakeDamageEventHandler takeDamageEvents;

    public void RegistTakeDamageEvent(TakeDamageEventHandler e)
    {
        takeDamageEvents += e;
    }

    public void UnregistTakeDamageEvent(TakeDamageEventHandler e)
    {
        takeDamageEvents -= e;
    }

    public void TakeDamage(int damage)
    {
        int diff = damage;
        // まずシールドからダメージを受ける
        if(shield > 0)
        {
            shield -= damage;
            diff = -shield;
            shield = Mathf.Max(shield, 0);
        }

        // 余剰ダメージ
        if(diff > 0)
        {
            hp = Mathf.Max(hp - diff, 0);
        }
        healthSlider.value = hp;
        UpdateHealthText();

        takeDamageEvents?.Invoke(damage, hp);
    }

    private void UpdateHealthText()
    {
        healthText.text = $"{hp}/{healthSlider.maxValue}";
        Debug.Log("体力を更新");
    }
}
