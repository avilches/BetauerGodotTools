using System;
using Betauer.Application.Persistent;
using Betauer.DI.Attributes;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class ZombieGameObject : NpcGameObject {
    [Inject] public ZombieConfig ZombieConfig { get; set; }
    public override NpcConfig Config => ZombieConfig;

    public override void New() {
        MaxHealth = Config.InitialMaxHealth;
        Health = Config.InitialHealth;
    }

    protected override Type SaveObjectType => typeof(ZombieSaveObject);

    protected override void DoLoad(SaveObject s) {
        ZombieSaveObject saveObject = (ZombieSaveObject)s;
        MaxHealth = saveObject.MaxHealth;
        Health = saveObject.Health;
    }
    public override SaveObject CreateSaveObject() => new ZombieSaveObject(this);
}


public class ZombieSaveObject : NpcSaveObject<ZombieGameObject> {
    public ZombieSaveObject() { }
    public ZombieSaveObject(NpcGameObject gameObject) : base(gameObject) { }
    public override string Discriminator() => "Npc.Zombie";
}
