using System;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class NpcGameObject : GameObject<NpcNode>  {
    public NpcStatus Status { get; private set; }
    
    public NpcConfig Config { get; private set; }

    public NpcGameObject Configure(NpcConfig config) {
        Config = config;
        Status = new NpcStatus(config.InitialMaxHealth, config.InitialHealth);
        return this;
    }

    public NpcGameObject Load(NpcConfig config, NpcSaveObject npcSaveObject) {
        Config = config;
        Status = new NpcStatus(npcSaveObject);
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

        public NpcStatus(NpcSaveObject npcSaveObject) {
            Health = npcSaveObject.Health;
            MaxHealth = npcSaveObject.MaxHealth;
        }

        public void UpdateHealth(float update) {
            Health += update;
        }

        public bool IsDead() => Health <= 0f;
    }

    public override SaveObject CreateSaveObject() {
        return new NpcSaveObject(this);
    }
}

public class NpcSaveObject : SaveObject<NpcGameObject> {
    [JsonInclude] public float Health { get; set; }
    [JsonInclude] public float MaxHealth { get; set; }
    [JsonInclude] public Vector2 GlobalPosition { get; set; }

    public NpcSaveObject() {
    }

    public NpcSaveObject(NpcGameObject npc) : base(npc) {
        Health = npc.Status.Health;
        MaxHealth = npc.Status.MaxHealth;
        GlobalPosition =  npc.Node.GlobalPosition;
    }
}