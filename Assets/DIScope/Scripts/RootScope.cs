using CardEffect;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootScope : LifetimeScope
{
    [field: SerializeField, Header("セーブデータホルダー(prefab)")]
    private SaveDataHolder saveDataHolder;

    [field: SerializeField, Header("カードデータライブラリ(prefab)")]
    private CardDataLibrary cardDataLibrary;

    [field: SerializeField, Header("仮のマップデータ(prefab)")]
    private TemporaryMapData temporaryMapData;

    [field: SerializeField, Header("デッキマネージャー(prefab)")]
    private DeckManager deckManager;

    [field: SerializeField, Header("カードビュワー(prefab)")]
    private CardViewer cardViewer;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<DeckInfomation>(Lifetime.Singleton);
        builder.Register<CardDataFactory>(Lifetime.Singleton);
        builder.RegisterComponentInNewPrefab(saveDataHolder, Lifetime.Singleton)
           .DontDestroyOnLoad();
        builder.RegisterComponentInNewPrefab(cardDataLibrary, Lifetime.Singleton)
           .DontDestroyOnLoad();
        builder.RegisterComponentInNewPrefab(temporaryMapData, Lifetime.Singleton)
           .DontDestroyOnLoad();
        builder.RegisterComponentInNewPrefab(deckManager, Lifetime.Singleton)
           .DontDestroyOnLoad();
        builder.RegisterComponentInNewPrefab(cardViewer, Lifetime.Singleton)
            .DontDestroyOnLoad();
    }
}

