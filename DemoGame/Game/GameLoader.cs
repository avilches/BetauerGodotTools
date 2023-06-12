using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Veronenger.Game.UI;

namespace Veronenger.Game;

public class GameLoader : ResourceLoaderContainer {
    [Inject] private ILazy<BottomBar> BottomBarLazy { get; set; }
    [Inject] private ILazy<ProgressScreen> ProgressScreenLazy { get; set; }
	
    public Task LoadMainResources() => LoadResources("main");

    public async Task LoadGameResources() {
        LoadStart();
        await LoadResources("game");
        LoadEnd();
    }

    private void LoadStart() {
        BottomBarLazy.Get().Visible = false;
    }

    private void LoadEnd() {
        BottomBarLazy.Get().Visible = true;
        ProgressScreenLazy.Get().Hide();
    }

    public void UnloadGameResources() => UnloadResources("game");
}