// このファイルはAIで作成。(Claudeを使用)
using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

/// <summary>
/// エディタ外（CLI・外部エディタ等）でのファイル変更を検知し、
/// Unity がフォーカスされていなくても自動でアセットの再読み込みを行うエディタ拡張
///
/// ・Assets 以下の変更      → AssetDatabase.Refresh()（スクリプトなら再コンパイルまで走る）
/// ・Packages 以下の変更    → Client.Resolve()（manifest.json の変更を反映）
/// ・Play 中 / コンパイル中 / インポート中は、終わるまで待ってから実行する
/// ・メニュー「Tools/外部変更の自動リロード」で有効/無効を切り替え可能
/// </summary>
[InitializeOnLoad]
internal static class ExternalChangeAutoRefresh
{
    private const string MenuPath = "Tools/外部変更の自動リロード";
    private const string PrefKey = "ExternalChangeAutoRefresh.Enabled";

    /// <summary>最後の変更からこの秒数だけ変更が止まったらリロードする（連続した書き込みをまとめるため）</summary>
    private const double DebounceSeconds = 1.0;

    private static FileSystemWatcher[] watchers;

    // FileSystemWatcher のイベントはメインスレッド外で呼ばれるため Interlocked で受け渡す
    private static long lastChangeTicks;
    private static int assetsDirty;
    private static int packagesDirty;

    private static bool Enabled
    {
        get => EditorPrefs.GetBool(PrefKey, true);
        set => EditorPrefs.SetBool(PrefKey, value);
    }

    static ExternalChangeAutoRefresh()
    {
        if (Application.isBatchMode)
            return;

        StartWatching();
        EditorApplication.update += OnUpdate;
        AssemblyReloadEvents.beforeAssemblyReload += StopWatching;
        EditorApplication.quitting += StopWatching;
    }

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        Enabled = !Enabled;
        Debug.Log($"[外部変更の自動リロード] {(Enabled ? "有効" : "無効")}にしました");
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, Enabled);
        return true;
    }

    private static void StartWatching()
    {
        StopWatching();

        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        watchers = new[]
        {
            CreateWatcher(Path.Combine(projectRoot, "Assets"), () => Interlocked.Exchange(ref assetsDirty, 1)),
            CreateWatcher(Path.Combine(projectRoot, "Packages"), () => Interlocked.Exchange(ref packagesDirty, 1)),
        };
    }

    private static void StopWatching()
    {
        if (watchers == null)
            return;

        foreach (var watcher in watchers)
        {
            if (watcher == null)
                continue;
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        watchers = null;
    }

    private static FileSystemWatcher CreateWatcher(string path, Action markDirty)
    {
        if (!Directory.Exists(path))
            return null;

        var watcher = new FileSystemWatcher(path)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size,
            InternalBufferSize = 64 * 1024,
        };

        void OnChanged()
        {
            markDirty();
            Interlocked.Exchange(ref lastChangeTicks, DateTime.UtcNow.Ticks);
        }

        watcher.Changed += (_, _) => OnChanged();
        watcher.Created += (_, _) => OnChanged();
        watcher.Deleted += (_, _) => OnChanged();
        watcher.Renamed += (_, _) => OnChanged();
        // バッファあふれ等でイベントを取りこぼした場合も念のためリロードする
        watcher.Error += (_, _) => OnChanged();

        watcher.EnableRaisingEvents = true;
        return watcher;
    }

    private static void OnUpdate()
    {
        if (!Enabled)
            return;

        long changedAt = Interlocked.Read(ref lastChangeTicks);
        if (changedAt == 0)
            return;

        if (DateTime.UtcNow.Ticks - changedAt < TimeSpan.FromSeconds(DebounceSeconds).Ticks)
            return;

        // Play 中やコンパイル・インポート中は終わるまで待つ
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            return;

        Interlocked.Exchange(ref lastChangeTicks, 0);
        bool packagesChanged = Interlocked.Exchange(ref packagesDirty, 0) == 1;
        bool assetsChanged = Interlocked.Exchange(ref assetsDirty, 0) == 1;

        if (packagesChanged)
            Client.Resolve();

        if (assetsChanged)
            AssetDatabase.Refresh();
    }
}
