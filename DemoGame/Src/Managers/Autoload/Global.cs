using Godot;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;

namespace Veronenger.Managers.Autoload; 

public partial class Global : Node {
    [Inject] private ILazy<Game> Game { get; set; }

    public bool IsPlayer(CharacterBody2D player) {
        return Game.Get().WorldScene.IsPlayer(player);
    }
}