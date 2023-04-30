using Betauer.Application.Persistent;
using Godot;
using Betauer.DI.Attributes;

namespace Veronenger.Managers.Autoload; 

public partial class Global : Node {
    [Inject] private GameObjectRepository GameObjectRepository { get; set; }

    public bool IsPlayer(CharacterBody2D player) {
        return GameObjectRepository.IsPlayer(player);
    }

        
}