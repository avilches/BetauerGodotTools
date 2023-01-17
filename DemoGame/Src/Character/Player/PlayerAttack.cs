using Godot;
using Veronenger.Character.Items;

namespace Veronenger.Character.Player;

public class PlayerAttack {
    public readonly PlayerNode Player;
    public readonly Area2D EnemyAttackArea;
    public readonly WeaponItem Weapon;

    public PlayerAttack(PlayerNode player, Area2D enemyAttackArea, WeaponItem weapon) {
        Player = player;
        EnemyAttackArea = enemyAttackArea;
        Weapon = weapon;
    }
}