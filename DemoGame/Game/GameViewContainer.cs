using System;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Game.Worlds.Platform;

namespace Veronenger.Game;

[Singleton]
public class GameViewContainer {
    [Inject] private ITransient<GameView> GameSceneFactory { get; set; }
    [Inject] private SceneTree SceneTree { get; set; }

    public IGameView CurrentGame { get; private set; }
	
    public void CreateGame() {
        CurrentGame = GameSceneFactory.Create();
        SceneTree.Root.AddChild((Node)CurrentGame);
    }

    public void RemoveGame() {
        ((Node)CurrentGame).Free();
        GC.GetTotalMemory(true);
        // PrintOrphanNodes();
        CurrentGame = null!;
    }
}