using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Godot;

namespace Veronenger.Game.Platform.Character.Npc;

public abstract class NpcGameObject : GameObject<NpcNode> {
    public abstract NpcConfig Config { get; }

    public float Health;
    public float MaxHealth;
    public float HealthPercent => Health / MaxHealth;
    public bool UnderMeleeAttack { get; set; } = false; // true when the first attack signal is emitted. false when Hurt state ends.

    public void UpdateHealth(float update) {
        Health += update;
    }

    public bool IsDead() => Health <= 0f;

}

public abstract class NpcSaveObject<T> : SaveObject<T> where T : GameObject {
    [JsonInclude] public float Health { get; set; }
    [JsonInclude] public float MaxHealth { get; set; }

    [JsonInclude] public Vector2 GlobalPosition { get; set; }
    [JsonInclude] public Vector2 Velocity { get; set; } // X Velocity doesn't matter because the MeleeAI state is not save, so it starts from Idle 
    [JsonInclude] public bool IsFacingRight { get; set; }

    protected NpcSaveObject() {
    }

    protected NpcSaveObject(NpcGameObject npc) : base(npc) {
        Health = npc.Health;
        MaxHealth = npc.MaxHealth;
        if (npc.Node != null) {
            GlobalPosition = npc.Node.GlobalPosition;
            Velocity = npc.Node.Velocity;
            IsFacingRight = npc.Node.IsFacingRight;
        }
    }
}