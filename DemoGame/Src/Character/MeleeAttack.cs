using Betauer.Core;
using Godot;

namespace Veronenger.Character;

public class MeleeAttack {
    private readonly Area2D _attackArea;
    private readonly Sprite2D _weaponSprite;

    public MeleeAttack(Area2D attackArea, Sprite2D weaponSprite) {
        _attackArea = attackArea;
        _weaponSprite = weaponSprite;
        Unequip();
    }

    public void Unequip() => Equip(null);
		
    public void Equip(MeleeWeapon? weapon) {
        _attackArea.Monitorable = false;
        _attackArea.Monitoring = false;
        if (weapon != null) {
            _weaponSprite.Visible = true;
            _weaponSprite.Texture = weapon.Resource;
        } else {
            _weaponSprite.Visible = false;
        }
        _attackArea.GetChildren().ForEach((shape) => {
            var matches = weapon != null && weapon.ShapeName == shape.Name;
            if (shape is CollisionShape2D collisionShape2D) collisionShape2D.Disabled = !matches;
            if (shape is CollisionPolygon2D collisionPolygon2D) collisionPolygon2D.Disabled = !matches;
        });
    }
}