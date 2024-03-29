using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Betauer.Core;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.Character.Player;

public class PlayerGameObject : GameObject<PlayerNode> {
    [Inject] public PlayerConfig PlayerConfig { get; private set; }

    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public float HealthPercent => Health / MaxHealth;
    public bool Invincible { get; set; } = false; // true when the Hurt state starts. A timeout sets it to false later
    public bool UnderAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.
    public int AvailableHits { get; set; } = 0;

    public Dictionary<AmmoType, int> Ammo { get; } = new();

    public event Action<PlayerHealthEvent> OnHealthUpdate;

    public PlayerGameObject() {
        Enum.GetValues<AmmoType>().ForEach(ammoType => Ammo[ammoType] = 0);
    }

    public override void OnInitialize() {
        MaxHealth = PlayerConfig.InitialMaxHealth;
        Health = Math.Clamp(PlayerConfig.InitialHealth, 0, PlayerConfig.InitialMaxHealth);
    }

    public override void OnLoad(SaveObject s) {
        PlayerSaveObject saveObject = (PlayerSaveObject)s;
        MaxHealth = saveObject.MaxHealth;
        Health = saveObject.Health;
        Ammo.ForEach(ammo => saveObject.Ammo[ammo.Key] = ammo.Value);
    }

    public override PlayerSaveObject CreateSaveObject() => new PlayerSaveObject(this);

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
        TriggerRefresh(fromHealth, toHealth, MaxHealth);
    }

    public void SetMaxHealth(float newMaxHealth) {
        var fromHealth = Health;
        MaxHealth = newMaxHealth;
        Health = Math.Clamp(Health, 0, MaxHealth);
        var toHealth = Health;
        TriggerRefresh(fromHealth, toHealth, MaxHealth);
    }

    public void TriggerRefresh() {
        TriggerRefresh(Health, Health, MaxHealth);
    }

    public void TriggerRefresh(float fromHealth, float toHealth, float maxHealth) {
        OnHealthUpdate?.Invoke(new PlayerHealthEvent(Node, fromHealth, toHealth, maxHealth));
    }

    public bool IsDead() => Health <= 0f;
}

public class PlayerSaveObject : SaveObject<PlayerGameObject>, IPlatformSaveObject {
    [JsonInclude] public float Health { get; set; }
    [JsonInclude] public float MaxHealth { get; set; }
    [JsonInclude] public Dictionary<AmmoType, int> Ammo { get; set; }
    
    [JsonInclude] public Vector2 GlobalPosition { get; set; }
    [JsonInclude] public Vector2 Velocity { get; set; }
    [JsonInclude] public bool IsFacingRight { get; set; }

    public override string Discriminator() => "Player";

    public PlayerSaveObject() {}

    public PlayerSaveObject(PlayerGameObject player) : base(player) {
        Health = player.Health;
        MaxHealth = player.MaxHealth;
        Ammo = new Dictionary<AmmoType, int>(player.Ammo);
        GlobalPosition = player.Node!.GlobalPosition;
        IsFacingRight = player.Node!.LateralState.IsFacingRight;
        Velocity = player.Node!.CharacterBody2D.Velocity;
    }
}