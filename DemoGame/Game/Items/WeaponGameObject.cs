using Betauer.DI.Attributes;
using Veronenger.Game.Config;

namespace Veronenger.Game.Items;

public abstract class WeaponGameObject : PickableGameObject {
    public override WeaponConfig Config => (WeaponConfig)base.Config;
    
    [Inject] protected ConfigManager ConfigManager { get; set; }

    public string GetConfigName() => ConfigManager.GetConfigName(Config);
    public virtual WeaponConfig GetConfig(string name) => (WeaponConfig)ConfigManager.GetConfig(name);

    public float DamageBase;
    public float DamageFactor = 1f;
    public float Damage => DamageBase * DamageFactor;
}