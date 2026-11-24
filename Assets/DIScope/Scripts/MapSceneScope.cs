using CardEffect.CardActionModules;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using AudioComponent;
using CardsComponent;
using CreaturesComponent;
using DungeonMapComponent;
using GameHUDComponent;

namespace DIScope
{
    public class MapSceneScope : LifetimeScope
    {
        [field: SerializeField, Header("ダンジョンマップマネージャー")]
        private DungeonMapManager mapManager;

        [field: SerializeField, Header("オーディオコントローラー")]
        private AudioController audioController;

        [field: SerializeField, Header("カードデータライブラリ")]
        private CardDataLibrary cardDataLibrary;

        [field: SerializeField, Header("デッキマネージャー")]
        private DeckManager deckManager;

        [field: SerializeField, Header("エネミー組み合わせデータジェネレーター")]
        private EnemyCombinationsDataGenerator enemyCombinationsDataGenerator;

        [field: SerializeField, Header("HUD 体力バーコントローラー")]
        private HUDHealthBarController hUDHealthBarController;

        [field: SerializeField, Header("HUD コインカウンター")]
        private HUDCoinCounter hUDCoinCounter;

        [field: SerializeField, Header("HUD フロアカウンター")]
        private HUDFloorNumberCounter hUDFloorNumberCounter;

        [field: SerializeField, Header("山札カウンター")]
        private DrawPailGUI drawCard;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<CardViewDataFactory>(Lifetime.Singleton).As<ICardDataFactory>();
            builder.RegisterComponent<DungeonMapManager>(mapManager);
            builder.RegisterComponent<AudioController>(audioController);
            builder.RegisterComponent<DrawPailGUI>(drawCard);
            builder.RegisterComponent<CardDataLibrary>(cardDataLibrary);
            builder.RegisterComponent<DeckManager>(deckManager);
            builder.RegisterComponent<EnemyCombinationsDataGenerator>(enemyCombinationsDataGenerator);
            builder.RegisterComponent<HUDHealthBarController>(hUDHealthBarController);
            builder.RegisterComponent<HUDCoinCounter>(hUDCoinCounter);
            builder.RegisterComponent<HUDFloorNumberCounter>(hUDFloorNumberCounter);
        }
    }
}
