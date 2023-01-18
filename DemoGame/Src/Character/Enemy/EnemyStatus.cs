using System;

namespace Veronenger.Character.Enemy;

public class EnemyStatus {
    public float Health;
    public float MaxHealth;
    public float HealthPercent => Health / MaxHealth;

    public bool UnderAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.

    public EnemyStatus(float maxHealth, float health = int.MaxValue) {
        MaxHealth = maxHealth;
        Health = Math.Min(health, maxHealth);
    }

    public void Attacked(float damage) {
        Health -= damage;
    }

    public bool IsDead() => Health <= 0f;
}