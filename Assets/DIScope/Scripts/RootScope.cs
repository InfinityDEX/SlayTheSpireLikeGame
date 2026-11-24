using CardEffect;
using System.IO;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using CardsComponent;
using CreaturesComponent;
using DungeonMapComponent;
using SaveDataComponent;

namespace DIScope
{
    public class RootScope : LifetimeScope
    {
        [field: SerializeField, Header("セーブデータホルダー(prefab)")]
        private SaveDataHolder saveDataHolder;

        [field: SerializeField, Header("仮のマップデータ(prefab)")]
        private TemporaryMapData temporaryMapData;

        [field: SerializeField, Header("カードビュワー(prefab)")]
        private CardViewer cardViewer;

        [field: SerializeField, Header("エネミー組み合わせデータマネージャ(prefab)")]
        private EnemyCombinationsDataManager enemyCombinationsDataManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<DeckInfomation>(Lifetime.Singleton);

            string folderPath = Path.Combine(Application.persistentDataPath, "SaveData");
            string saveFileName = "SaveData.json";
            builder.RegisterInstance<SaveDataIO>(new (folderPath, saveFileName)).As<ISaveDataIO>();

            builder.RegisterComponentInNewPrefab(saveDataHolder, Lifetime.Singleton)
               .DontDestroyOnLoad();
            builder.RegisterComponentInNewPrefab(temporaryMapData, Lifetime.Singleton)
               .DontDestroyOnLoad();
            builder.RegisterComponentInNewPrefab(cardViewer, Lifetime.Singleton)
                .DontDestroyOnLoad();
            builder.RegisterBuildCallback(container =>
            {
                var saveDataHolderInstance = container.Resolve<CardViewer>();
                saveDataHolderInstance.gameObject.SetActive(false);
            });
            builder.RegisterComponentInNewPrefab(enemyCombinationsDataManager, Lifetime.Singleton)
                .DontDestroyOnLoad();
        }
    }
}
