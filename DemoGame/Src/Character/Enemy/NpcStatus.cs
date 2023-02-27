using System;

namespace Veronenger.Character.Enemy;

public class NpcStatus {
    public float Health;
    public float MaxHealth;
    public float HealthPercent => Health / MaxHealth;

    public bool UnderMeleeAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.

    public NpcStatus(float maxHealth, float health = int.MaxValue) {
        MaxHealth = maxHealth;
        Health = Math.Min(health, maxHealth);
    }

    public void UpdateHealth(float update) {
        Health += update;
    }

    public bool IsDead() => Health <= 0f;
}