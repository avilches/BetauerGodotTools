using Veronenger.Character.Items;

namespace Veronenger.Character.Player;

public class PlayerAttackEvent {
    public PlayerNode Player { get; }
    public EnemyItem Enemy { get; }
    public BaseWeaponItem Weapon { get; }

    public PlayerAttackEvent(PlayerNode player, EnemyItem enemy, BaseWeaponItem weapon) {
        Player = player;
        Enemy = enemy;
        Weapon = weapon;
    }
}