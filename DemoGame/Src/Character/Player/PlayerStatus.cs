using System;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Veronenger.Managers;

namespace Veronenger.Character.Player;

[Service(Lifetime.Transient)]
public class PlayerStatus {
    [Inject] private EventBus EventBus { get; set; }

    public PlayerNode PlayerNode { get; private set; }
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public float HealthPercent => Health / MaxHealth;
    public bool Invincible { get; set; } = false; // true when the Hurt state starts. A timeout sets it to false later
    public bool UnderAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.
    public int AvailableHits { get; set; } = 0;

    public event Action<PlayerUpdateHealthEvent> OnHealthUpdate;

    public void Configure(PlayerNode playerNode, float maxHealth, float health) {
        PlayerNode = playerNode;
        MaxHealth = maxHealth;
        Health = Math.Clamp(health, 0, maxHealth);
        Invincible = false;
        UnderAttack = false;
        AvailableHits = 0;
        OnHealthUpdate?.Invoke(new PlayerUpdateHealthEvent(PlayerNode, Health, Health, MaxHealth));
    }

    public void UpdateHealth(float update) => SetHealth(Health + update);
    
    public void UpdateMaxHealth(float update) => SetMaxHealth(MaxHealth + update);

    public void SetHealth(float health) {
        var fromHealth = Health;
        Health = Math.Clamp(health, 0, MaxHealth);
        var toHealth = Health;
        OnHealthUpdate?.Invoke(new PlayerUpdateHealthEvent(PlayerNode, fromHealth, toHealth, MaxHealth));
    }

    public void SetMaxHealth(float newMaxHealth) {
        var fromHealth = Health;
        MaxHealth = newMaxHealth;
        Health = Math.Clamp(Health, 0, MaxHealth);
        var toHealth = Health;
        OnHealthUpdate?.Invoke(new PlayerUpdateHealthEvent(PlayerNode, fromHealth, toHealth, MaxHealth));
    }

    public bool IsDead() => Health <= 0f;
}