using CardEffect;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
        builder.RegisterComponentInNewPrefab(saveDataHolder, Lifetime.Singleton)
           .DontDestroyOnLoad();
        builder.RegisterComponentInNewPrefab(temporaryMapData, Lifetime.Singleton)
           .DontDestroyOnLoad();
        builder.RegisterComponentInNewPrefab(cardViewer, Lifetime.Singleton)
            .DontDestroyOnLoad();
        builder.RegisterComponentInNewPrefab(enemyCombinationsDataManager, Lifetime.Singleton)
            .DontDestroyOnLoad();
    }
}

