using System;

namespace Veronenger.Character.Enemy;

public class EnemyStatus {
    public float Health;
    public float MaxHealth;
    public float HealthPercent => Health / MaxHealth;

    public EnemyStatus(float maxHealth, float health = int.MaxValue) {
        MaxHealth = maxHealth;
        Health = Math.Min(health, maxHealth);
    }

    public void Attacked(float damage) {
        Health -= damage;
    }

    public bool IsDead() => Health <= 0f;
}

public class PlayerStatus {
    public float Health;
    public float MaxHealth;
    public float HealthPercent => Health / MaxHealth;
    public bool Invincible { get; set; } = false;

    public PlayerStatus(float maxHealth, float health = int.MaxValue) {
        MaxHealth = maxHealth;
        Health = Math.Min(health, maxHealth);
    }

    public void Attacked(float damage) {
        Health -= damage;
    }

    public bool IsDead() => Health <= 0f;
}