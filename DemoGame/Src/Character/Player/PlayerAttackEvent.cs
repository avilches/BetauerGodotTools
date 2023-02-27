using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public class PlayerAttackEvent {
    // public PlayerNode Player { get; }
    public NpcItem Npc { get; }
    public WeaponItem Weapon { get; }

    public PlayerAttackEvent(PlayerNode player, NpcItem npc, WeaponItem weapon) {
        // Player = player;
        Npc = npc;
        Weapon = weapon;
    }
}