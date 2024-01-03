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
	
    public Task LoadMainResources(Action<ResourceProgress>? progressAction = null) => LoadResources("main", progressAction);

    public async Task LoadPlatformGameResources() {
        LoadStart();
        await LoadResources("platform");
        LoadEnd();
    }

    public async Task LoadRtsGameResources() {
        LoadStart();
        await LoadResources("rts");
        LoadEnd();
    }

    private void LoadStart() {
        BottomBarLazy.Get().Visible = false;
    }

    private void LoadEnd() {
        BottomBarLazy.Get().Visible = true;
        ProgressScreenLazy.Get().Hide();
    }

    public void UnloadPlatformGameResources() => UnloadResources("platform");
    public void UnloadRtsGameResources() => UnloadResources("rts");
}