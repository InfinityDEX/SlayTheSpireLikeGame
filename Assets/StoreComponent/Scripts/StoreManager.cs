using UnityEngine;

public class StoreManager : MonoBehaviour
{
    [field: SerializeField, Header("体力回復料金")]
    private int recoveryPrice;


    public void FullRecovery()
    {

        var saveDataHolder = SaveDataHolder.Instance;
        if (saveDataHolder != null)
        {
            if (saveDataHolder.UseMoney(recoveryPrice) == true)
            {
                saveDataHolder.FullRecovery();
            }
        }
    }
}
