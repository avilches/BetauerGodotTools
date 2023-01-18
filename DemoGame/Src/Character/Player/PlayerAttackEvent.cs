using Veronenger.Character.Items;

namespace Veronenger.Character.Player;

public class PlayerAttackEvent {
    public readonly PlayerNode Player;
    public readonly EnemyItem Enemy;
    public readonly WeaponItem Weapon;

    public PlayerAttackEvent(PlayerNode player, EnemyItem enemy, WeaponItem weapon) {
        Player = player;
        Enemy = enemy;
        Weapon = weapon;
    }
}