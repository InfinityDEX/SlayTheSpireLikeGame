using TMPro;
using UnityEngine;

/// <summary>
/// エナジー管理クラス
/// 
/// カードをプレイする際に支払うエナジーを管理するクラス
/// </summary>
public class EnergyManager : MonoBehaviour
{
    [Header("コスト値表示GUI")]
    [SerializeField]
    private TextMeshProUGUI energyPointGUI;

    [Header("エナジー最大値")]
    [SerializeField]
    private int maxEnergy = 3;
    
    [Header("エナジー残量")]
    [SerializeField]
    private int currentEnergyInspector;
    public int currentEnergy{ get; private set;}

    private void Update()
    {
        currentEnergyInspector = currentEnergy;
        if(energyPointGUI != null)
            energyPointGUI.text = $"{currentEnergy}/{maxEnergy}";
    }

    /// <summary>
    /// エナジーの全回復処理
    /// </summary>
    public void RefreshEnergy()
    {
        currentEnergy = maxEnergy;
    }

    /// <summary>
    /// エナジー消費処理
    /// </summary>
    /// <param name="cost">支払いたいエナジー量</param>
    /// <returns>実際に支払えたか？（支払えないエナジー量を提示された場合は、エナジーを変更せずfalseを返す）</returns>
    public bool UseEnergy(int cost)
    {
        if(currentEnergy < cost)
        {
            return false;
        }
        
        currentEnergy -= cost;
        return true;
    }

    /// <summary>
    /// エナジー回復処理
    /// </summary>
    /// <param name="energy"></param>
    public void RecoveryEnergy(int energy)
    {
        currentEnergy += energy;
    }

    public void SetMaxEnergyPoint(int max)
    {
        maxEnergy = max;
    }
}
