using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Veronenger.Game.UI;

namespace Veronenger.Game;

public class GameLoader : ResourceLoaderContainer {
    [Inject] private ILazy<BottomBar> BottomBarSceneFactory { get; set; }
    [Inject] private ILazy<ProgressScreen> ProgressScreenFactory { get; set; }
	
    public Task LoadMainResources() => LoadResources("main");

    public async Task LoadGameResources() {
        LoadStart();
        await LoadResources("game");
        LoadEnd();
    }

    private void LoadStart() {
        BottomBarSceneFactory.Get().Visible = false;
    }

    private void LoadEnd() {
        BottomBarSceneFactory.Get().Visible = true;
        ProgressScreenFactory.Get().Hide();
    }

    public void UnloadGameResources() => UnloadResources("game");
}