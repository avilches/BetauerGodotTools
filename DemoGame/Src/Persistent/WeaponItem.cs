
namespace Veronenger.Persistent;

public abstract class WeaponItem : PickableItem {
    public float DamageBase;
    public float DamageFactor = 1f;
    public float Damage => DamageBase * DamageFactor;
}