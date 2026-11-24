using UnityEngine;
using VContainer;
using VContainer.Unity;
using CardsComponent;
using GameHUDComponent;
using StoreComponent;

namespace DIScope
{
    public class StoreSceneScope : LifetimeScope
    {
        [field: SerializeField, Header("ストアマネージャー")]
        private StoreManager storeManager;

        [field: SerializeField, Header("販売カードマネージャー")]
        private SalesCardManager saleCardManager;

        [field: SerializeField, Header("カードデータライブラリ")]
        private CardDataLibrary cardDataLibrary;

        [field: SerializeField, Header("デッキマネージャー")]
        private DeckManager deckManager;

        [field: SerializeField, Header("HUD 体力バーコントローラー")]
        private HUDHealthBarController hUDHealthBarController;

        [field: SerializeField, Header("HUD コインカウンター")]
        private HUDCoinCounter hUDCoinCounter;

        [field: SerializeField, Header("HUD フロアカウンター")]
        private HUDFloorNumberCounter hUDFloorNumberCounter;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<CardViewDataFactory>(Lifetime.Singleton).As<ICardDataFactory>();
            builder.RegisterComponent<StoreManager>(storeManager);
            builder.RegisterComponent<SalesCardManager>(saleCardManager);
            builder.RegisterComponent<CardDataLibrary>(cardDataLibrary);
            builder.RegisterComponent<DeckManager>(deckManager);
            builder.RegisterComponent<HUDHealthBarController>(hUDHealthBarController);
            builder.RegisterComponent<HUDCoinCounter>(hUDCoinCounter);
            builder.RegisterComponent<HUDFloorNumberCounter>(hUDFloorNumberCounter);
        }
    }
}
