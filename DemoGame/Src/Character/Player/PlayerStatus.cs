using System;
using System.Collections.Generic;
using Betauer.Core;
using Veronenger.Config;
using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public class PlayerStatus {
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public float HealthPercent => Health / MaxHealth;
    public bool Invincible { get; set; } = false; // true when the Hurt state starts. A timeout sets it to false later
    public bool UnderAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.
    public int AvailableHits { get; set; } = 0;
    
    public Dictionary<AmmoType, int> Ammo { get; } = new();

    public event Action<PlayerHealthEvent> OnHealthUpdate;

    public PlayerStatus(PlayerConfig playerConfig) {
        MaxHealth = playerConfig.InitialMaxHealth;
        Health = Math.Clamp(playerConfig.InitialHealth, 0, playerConfig.InitialMaxHealth);
        Invincible = false;
        UnderAttack = false;
        AvailableHits = 0;
        OnHealthUpdate?.Invoke(new PlayerHealthEvent(Health, Health, MaxHealth));
        
        Enum.GetValues<AmmoType>().ForEach(ammoType => Ammo[ammoType] = 10);
    }

    public int GetAmmo(AmmoType ammoType) => Ammo[ammoType];
    public bool HasAmmo(AmmoType ammoType) => Ammo[ammoType] > 0;
    public void UpdateAmmo(AmmoType ammoType, int amount) => Ammo[ammoType] += amount;

    public void UpdateHealth(float update) => SetHealth(Health + update);
    public void UpdateMaxHealth(float update) => SetMaxHealth(MaxHealth + update);

    public void Reload(WeaponRangeGameObject weapon) {
        var needed = weapon.MagazineSize - weapon.Ammo;
        var available = GetAmmo(weapon.AmmoType);
        var toReload = Math.Min(needed, available);
        UpdateAmmo(weapon.AmmoType, -toReload);
        weapon.Ammo += toReload;
        Console.WriteLine($"{weapon.AmmoType} reloaded {toReload} ammo. Now have {GetAmmo(weapon.AmmoType)} ammo.");
    }

    public void SetHealth(float health) {
        var fromHealth = Health;
        Health = Math.Clamp(health, 0, MaxHealth);
        var toHealth = Health;
        OnHealthUpdate?.Invoke(new PlayerHealthEvent(fromHealth, toHealth, MaxHealth));
    }

    public void SetMaxHealth(float newMaxHealth) {
        var fromHealth = Health;
        MaxHealth = newMaxHealth;
        Health = Math.Clamp(Health, 0, MaxHealth);
        var toHealth = Health;
        OnHealthUpdate?.Invoke(new PlayerHealthEvent(fromHealth, toHealth, MaxHealth));
    }

    public bool IsDead() => Health <= 0f;

}