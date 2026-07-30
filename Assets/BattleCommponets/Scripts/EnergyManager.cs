using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    public void RefreshEnergy()
    {
        currentEnergy = maxEnergy;
    }

    public bool UseEnergy(int cost)
    {
        if(currentEnergy < cost)
        {
            return false;
        }
        
        currentEnergy -= cost;
        return true;
    }

    public void AddEnergy(int energy)
    {
        currentEnergy += energy;
    }

    private void Update()
    {
        currentEnergyInspector = currentEnergy;
        if(energyPointGUI != null)
            energyPointGUI.text = $"{currentEnergy}/{maxEnergy}";
    }

    public void SetMaxEnergyPoint(int max)
    {
        maxEnergy = max;
    }
}
