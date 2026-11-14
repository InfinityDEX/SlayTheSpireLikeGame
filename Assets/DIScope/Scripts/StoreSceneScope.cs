using UnityEngine;
using VContainer;
using VContainer.Unity;

public class StoreSceneScope : LifetimeScope
{
    [field: SerializeField, Header("ストアマネージャー")]
    private StoreManager storeManager;

    [field: SerializeField, Header("販売カードマネージャー")]
    private SalesCardManager saleCardManager;

    [field: SerializeField, Header("HUD 体力バーコントローラー")]
    private HUDHealthBarController hUDHealthBarController;

    [field: SerializeField, Header("HUD コインカウンター")]
    private HUDCoinCounter hUDCoinCounter;

    [field: SerializeField, Header("HUD フロアカウンター")]
    private HUDFloorNumberCounter hUDFloorNumberCounter;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<StoreManager>(storeManager);
        builder.RegisterComponent<SalesCardManager>(saleCardManager);
        builder.RegisterComponent<HUDHealthBarController>(hUDHealthBarController);
        builder.RegisterComponent<HUDCoinCounter>(hUDCoinCounter);
        builder.RegisterComponent<HUDFloorNumberCounter>(hUDFloorNumberCounter);
    }
}