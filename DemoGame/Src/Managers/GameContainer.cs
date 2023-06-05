using Betauer.DI.Attributes;
using Betauer.DI.Factory;

namespace Veronenger.Managers;

[Singleton]
public class GameContainer {
    [Inject] private ITransient<Game> GameSceneFactory { get; set; }

    public Game CurrentGame { get; private set; }
	
    public void CreateGame() {
        CurrentGame = GameSceneFactory.Create();
    }

    public void RemoveGame() {
        CurrentGame = null!;
    }
}