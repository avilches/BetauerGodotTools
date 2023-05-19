using Godot;
using Betauer.DI.Attributes;

namespace Veronenger.Managers.Autoload; 

public partial class Global : Node {
    [Inject] private MainStateMachine MainStateMachine { get; set; }

    public bool IsPlayer(CharacterBody2D player) {
        return MainStateMachine.Game!.WorldScene.IsPlayer(player);
    }
}