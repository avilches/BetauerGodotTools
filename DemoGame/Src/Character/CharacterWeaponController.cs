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
            Current = null;
            
        } else if (weapon is WeaponMeleeItem melee) {
            _weaponSprite.Visible = true;
            var weaponMeleeModel = melee.Model;
            _weaponSprite.Texture = weaponMeleeModel.WeaponAnimation;
            for (var i = 0; i < _attackAreas.Length; i++) {
                var attackArea = _attackAreas[i];
                attackArea.Monitorable = false;
                attackArea.Monitoring = false;
                attackArea.GetChildren().ForEach(shape => {
                    var matches = weaponMeleeModel.ShapeName == shape.Name;
                    if (shape is CollisionShape2D collisionShape2D) collisionShape2D.Disabled = !matches;
                    if (shape is CollisionPolygon2D collisionPolygon2D) collisionPolygon2D.Disabled = !matches;
                });
            }
            Current = weapon;
        }
    }
}