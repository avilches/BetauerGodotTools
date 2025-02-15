using System;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Veronenger.Game.UI;

namespace Veronenger.Game;

public class GameLoader : ResourceLoaderContainer {
    [Inject] private ILazy<BottomBar> BottomBarLazy { get; set; }
    [Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }

    public bool MultiThreaded { get; set; } = true;
	
    public async Task Load(string tag, Action<LoadProgress>? progressAction = null) {
        LoadStart();
        await LoadResources(tag, MultiThreaded, progressAction);
        LoadEnd();
    }

    public async Task LoadMainResources(Action<LoadProgress>? progressAction = null) {
        // Main resources don't use LoadStart/LoadEnd because BottomBarLazy is not loaded yet!
        await LoadResources(BottomBarResources.GameLoaderTag, MultiThreaded);
        await Load(MainResources.GameLoaderTag, progressAction);

    }

    private void LoadStart() {
        BottomBarLazy.Get().Visible = false;
    }

    private void LoadEnd() {
        BottomBarLazy.Get().Visible = true;
        ProgressScreenLazy.Get().Hide();
    }
}