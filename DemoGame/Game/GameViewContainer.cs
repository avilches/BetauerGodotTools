using Betauer.DI.Attributes;
using Betauer.DI.Factory;

namespace Veronenger.Game;

[Singleton]
public class GameViewContainer {
    [Inject] private ITransient<GameView> GameSceneFactory { get; set; }

    public GameView CurrentGame { get; private set; }
	
    public void CreateGame() {
        CurrentGame = GameSceneFactory.Create();
    }

    public void RemoveGame() {
        CurrentGame = null!;
    }
}