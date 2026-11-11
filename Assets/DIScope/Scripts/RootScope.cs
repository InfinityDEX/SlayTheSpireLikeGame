using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootScope : LifetimeScope
{
    [field: SerializeField, Header("セーブデータホルダー(prefab)")]
    private SaveDataHolder saveDataHolder;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponentInNewPrefab(saveDataHolder, Lifetime.Singleton)
           .DontDestroyOnLoad();
    }
}

