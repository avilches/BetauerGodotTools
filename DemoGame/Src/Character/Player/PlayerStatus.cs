using System;

namespace Veronenger.Character.Player;

public class PlayerStatus {
    public float Health;
    public float MaxHealth;
    public float HealthPercent => Health / MaxHealth;
    public bool Invincible { get; set; } = false; // true when the Hurt state starts. A timeout sets it to false later
    public bool UnderAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.

    public int AvailableHits { get; set; } = 0;

    public PlayerStatus(float maxHealth, float health = int.MaxValue) {
        MaxHealth = maxHealth;
        Health = Math.Min(health, maxHealth);
    }

    public void Hurt(float damage) {
        Health -= damage;
    }

    public bool IsDead() => Health <= 0f;
}