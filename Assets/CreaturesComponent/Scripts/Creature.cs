using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// クリーチャクラス
/// 
/// 全ての生物（プレイヤーキャラ・エネミー含む）のベースとなるクラス
/// </summary>
public class Creature : MonoBehaviour
{
    [SerializeField, Header("クリーチャデータ")]
    private CreatureData creatureData;
    
    [field:SerializeField, Header("クリーチャ画像表示オブジェクト")]
    public Image CreatureSprite { get; private set;}
    
    [SerializeField, Header("HPバー スライダー")]
    private Slider healthSlider;

    [SerializeField, Header("HPバー テキスト")]
    private TextMeshProUGUI healthText;

    [field:SerializeField, Header("体力値")]
    public int Hp { get; private set;}
    /// <summary>
    /// 最大体力
    /// </summary>
    public int maxHealth { get {return creatureData.maxHealth;} }
    
    [field:SerializeField, Header("ブロック数")]
    public int Block { get; private set; } = 0;

    [field:SerializeField, Header("筋力値")]
    public int Muscle { get; private set; } = 0;

    [field:SerializeField, Header("ダメージバッチ生成位置")]
    private Transform damageBatchGeneratePoint;

    [field:SerializeField, Header("バッチマネージャー")]
    public IconBatchManager BatchManager { get; private set; }

    /// <summary>
    /// ブロック値バッチ（エフェクト）
    /// </summary>
    private IconBatch blockBatch;

    /// <summary>
    /// 筋力値バッチ（エフェクト）
    /// </summary>
    private IconBatch muscleBatch;
    
    /// <summary>
    /// Startメソッドが終了したかどうか
    /// </summary>
    private bool endStartFunc = false;
    
    /// <summary>
    /// 被ダメージ処理ハンドラ（デリゲート）
    /// </summary>
    /// <param name="damage">受けたダメージ値</param>
    /// <param name="hp">現在の体力</param>
    public delegate void TakeDamageEventHandler(int damage, int hp);
    /// <summary>
    /// 被ダメージのイベント処理
    /// </summary>
    private event TakeDamageEventHandler TakeDamageEvents;

    private void Start()
    {
        Hp = creatureData.maxHealth;
        healthSlider.value = healthSlider.maxValue = creatureData.maxHealth;
        UpdateHealthText();
        CreatureSprite.sprite = creatureData.creatureSprite;
        CreatureSprite.color = new Color(1, 1, 1, 1);
        endStartFunc = true;
    }

    private void Update()
    {
        // ブロック値の更新処理
        if(Block == 0)
        {
            if(blockBatch != null)
            {
                blockBatch.gameObject.SetActive(false);
            }
        }
        else
        {
            if(blockBatch == null)
            {
                blockBatch = BatchManager.GenerateEffectBatch(0);
            }
            blockBatch.gameObject.SetActive(true);
            blockBatch.effectCount = Block;
        }

        // 筋力値の更新
        if(Muscle == 0)
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
                muscleBatch = BatchManager.GenerateEffectBatch(1);
            }
            muscleBatch.gameObject.SetActive(true);
            muscleBatch.effectCount = Muscle;
        }
    }

    /// <summary>
    /// このオブジェクトのStartメソッドの処理が完了しているか？
    /// </summary>
    /// <returns>trueなら完了している</returns>
    public bool IsEndStartFunc()
    {
        return endStartFunc;
    }

    /// <summary>
    /// ブロックを追加
    /// </summary>
    /// <param name="block">追加するブロック値</param>
    public void AddBlock(int block)
    {
        Block += block;
    }

    /// <summary>
    /// 筋力を追加
    /// </summary>
    /// <param name="muscle">追加する筋力値</param>
    public void AddMuscle(int muscle)
    {
        Muscle += muscle;
    }

    /// <summary>
    /// ブロック値のリセット
    /// </summary>
    public void ResetBlock()
    {
        Block = 0;
    }

    /// <summary>
    /// 被ダメージ処理イベントハンドラの登録
    /// </summary>
    /// <param name="e">このクリーチャがダメージを受けたときに実行するイベントハンドラ</param>
    public void RegisterTakeDamageEvent(TakeDamageEventHandler e)
    {
        TakeDamageEvents += e;
    }

    /// <summary>
    /// 被ダメージ処理イベントハンドラの
    /// </summary>
    /// <param name="e">削除したいイベントハンドラ</param>
    public void UnregisterTakeDamageEvent(TakeDamageEventHandler e)
    {
        TakeDamageEvents -= e;
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage">受けるダメージ値</param>
    public void TakeDamage(int damage)
    {
        int diff = damage;
        // まずブロックからダメージを受ける
        if(Block > 0)
        {
            Block -= damage;
            diff = -Block;
            Block = Mathf.Max(Block, 0);
        }

        // 余剰ダメージ
        if(diff > 0)
        {
            Hp = Mathf.Max(Hp - diff, 0);
        }
        healthSlider.value = Hp;
        UpdateHealthText();
        if(diff > 0 && damageBatchGeneratePoint != null)
        {
            BattleManager.Instance.DamageBatchGenerator.GenerateDamageBatch(damageBatchGeneratePoint.position, diff);
        }
        TakeDamageEvents?.Invoke(damage, Hp);
    }

    /// <summary>
    /// 体力値表示UIの更新
    /// </summary>
    private void UpdateHealthText()
    {
        healthText.text = $"{Hp}/{healthSlider.maxValue}";
        Debug.Log("体力を更新");
    }
}
