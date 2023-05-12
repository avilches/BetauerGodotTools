using Betauer.Application.Persistent;
using Betauer.Core;
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
    public float Health { get; }
    public float MaxHealth { get; }
    public Dictionary<AmmoType, int> Ammo { get; } = new();

    public PlayerSaveObject(PlayerGameObject player) : base(player) {
        Health = player.Status.Health;
        MaxHealth = player.Status.MaxHealth;
        player.Status.Ammo.ForEach(ammo => Ammo[ammo.Key] = ammo.Value);
    }
}