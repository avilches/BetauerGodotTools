using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Godot;
using Godot.Collections;
using Veronenger.Character.Player;
using Veronenger.Config;

namespace Veronenger.Persistent; 

public class PlayerGameObject : GameObject<PlayerNode> {
    public PlayerStatus Status;
    public PlayerConfig Config;

    public PlayerGameObject Configure(PlayerConfig config) {
        Config = config;
        Status = new PlayerStatus(config);
        return this;
    }


    public PlayerGameObject Load(PlayerConfig config, PlayerSaveObject saveObject) {
        Config = config;
        Status = new PlayerStatus(saveObject);
        return this;
    }

    public override SaveObject CreateSaveObject() => new PlayerSaveObject(this);
}

public class PlayerSaveObject : SaveObject<PlayerGameObject> {
    [JsonInclude] public float Health { get; set; }
    [JsonInclude] public float MaxHealth { get; set; }
    [JsonInclude] public Dictionary<AmmoType, int> Ammo { get; set; }
    [JsonInclude] public Vector2 GlobalPosition { get; set; }

    public PlayerSaveObject() {}

    public PlayerSaveObject(PlayerGameObject player) : base(player) {
        Health = player.Status.Health;
        MaxHealth = player.Status.MaxHealth;
        Ammo = new Dictionary<AmmoType, int>(player.Status.Ammo);
        GlobalPosition = player.Node!.GlobalPosition;
    }
}