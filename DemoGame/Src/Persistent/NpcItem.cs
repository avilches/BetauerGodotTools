using System;
using Veronenger.Config;
using Veronenger.Persistent.Node;

namespace Veronenger.Persistent;

public class NpcItem : Item<INpcItemNode>  {
    public NpcStatus Status { get; private set; }
    
    public NpcConfig Config { get; private set; }

    public NpcItem Configure(NpcConfig config) {
        Config = config;
        Status = new NpcStatus(config.InitialMaxHealth, config.InitialHealth);
        return this;
    }

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
}