using Godot;
using Betauer.DI.Attributes;

namespace Veronenger.Managers.Autoload; 

public partial class Global : Node {
    [Inject] private Game Game { get; set; }

    public bool IsPlayer(CharacterBody2D player) {
        return Game.WorldScene.IsPlayer(player);
    }
}