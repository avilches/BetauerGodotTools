using Godot;
using Betauer.DI;
using Veronenger.Persistent;

namespace Veronenger.Managers.Autoload; 

public partial class Global : Node {
    [Inject] private World World { get; set; }

    public bool IsPlayer(CharacterBody2D player) {
        return World.IsPlayer(player);
    }

        
}