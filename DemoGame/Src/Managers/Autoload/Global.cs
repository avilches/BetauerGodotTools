using Godot;
using Betauer.DI;
using Betauer.DI.Attributes;
using Veronenger.Persistent;

namespace Veronenger.Managers.Autoload; 

public partial class Global : Node {
    [Inject] private ItemRepository ItemRepository { get; set; }

    public bool IsPlayer(CharacterBody2D player) {
        return ItemRepository.IsPlayer(player);
    }

        
}