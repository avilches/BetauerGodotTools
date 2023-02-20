using Betauer.Core;
using Godot;
using Veronenger.Items;

namespace Veronenger.Character;

public class CharacterWeaponController {
    private readonly Area2D[] _attackAreas;
    private readonly Sprite2D _weaponSprite;

    public WeaponItem? Current { get; private set; }

    public CharacterWeaponController(Area2D[] attackAreas, Sprite2D weaponSprite) {
        _attackAreas = attackAreas;
        _weaponSprite = weaponSprite;
        Unequip();
    }

    public void Unequip() => Equip(null);
		
    public void Equip(WeaponItem? weapon) {
        if (weapon == null) {
            _weaponSprite.Visible = false;
        } else if (weapon is WeaponRangeItem range) {
            _weaponSprite.Visible = true;
            _weaponSprite.Texture = range.Config.WeaponAnimation;
            for (var i = 0; i < _attackAreas.Length; i++) {
                var attackArea = _attackAreas[i];
                attackArea.Monitorable = false;
                attackArea.Monitoring = false;
            }
        } else if (weapon is WeaponMeleeItem melee) {
            _weaponSprite.Visible = true;
            _weaponSprite.Texture = melee.Config.WeaponAnimation;
            for (var i = 0; i < _attackAreas.Length; i++) {
                var attackArea = _attackAreas[i];
                attackArea.Monitorable = false;
                attackArea.Monitoring = false;
                attackArea.GetChildren().ForEach(shape => {
                    var matches = melee.Config.ShapeName == shape.Name;
                    if (shape is CollisionShape2D collisionShape2D) collisionShape2D.Disabled = !matches;
                    if (shape is CollisionPolygon2D collisionPolygon2D) collisionPolygon2D.Disabled = !matches;
                });
            }
        }
        Current = weapon;
    }
}