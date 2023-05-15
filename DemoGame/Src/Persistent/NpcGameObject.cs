using System;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Character.Npc;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class NpcGameObject : GameObject<NpcNode> {
    [Inject] private ConfigManager ConfigManager { get; set; }
    public NpcConfig Config { get; private set; }

    public float Health;
    public float MaxHealth;
    public float HealthPercent => Health / MaxHealth;
    public bool UnderMeleeAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.

    public override void New() {
        Config = ConfigManager.ZombieConfig;
        MaxHealth = Config.InitialMaxHealth;
        Health = Config.InitialHealth;
    }

    protected override Type SaveObjectType => typeof(NpcSaveObject);

    protected override void DoLoad(SaveObject s) {
        NpcSaveObject saveObject = (NpcSaveObject)s;
        Config = ConfigManager.ZombieConfig;
        MaxHealth = saveObject.MaxHealth;
        Health = saveObject.Health;
    }

    public override SaveObject CreateSaveObject() {
        return new NpcSaveObject(this);
    }

    public void UpdateHealth(float update) {
        Health += update;
    }

    public bool IsDead() => Health <= 0f;
}

public class NpcSaveObject : SaveObject<NpcGameObject> {
    [JsonInclude] public float Health { get; set; }
    [JsonInclude] public float MaxHealth { get; set; }
    [JsonInclude] public Vector2 GlobalPosition { get; set; }

    public NpcSaveObject() {
    }

    public NpcSaveObject(NpcGameObject npc) : base(npc) {
        Health = npc.Health;
        MaxHealth = npc.MaxHealth;
        GlobalPosition = npc.Node!.GlobalPosition;
    }
}