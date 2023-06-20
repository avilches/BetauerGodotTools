using Veronenger.Game.Platform.Character.Npc;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.Character.Player;

public class PlayerAttackEvent {
    // public PlayerNode Player { get; }
    public NpcNode NpcNode { get; }
    public WeaponGameObject Weapon { get; }

    public PlayerAttackEvent(PlayerNode player, NpcNode npcNode, WeaponGameObject weapon) {
        // Player = player;
        NpcNode = npcNode;
        Weapon = weapon;
    }
}