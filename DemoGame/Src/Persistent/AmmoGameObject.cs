using System;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Veronenger.Config;

namespace Veronenger.Persistent;

public class AmmoGameObject : PickableGameObject {
    public AmmoType AmmoType;
    public int Amount;

    public AmmoGameObject Configure(PickableConfig config, AmmoType ammoType, int amount) {
        Config = config;    
        AmmoType = ammoType;
        Amount = amount;
        return this;
    }

    public override void New() {
    }

    protected override Type SaveObjectType => typeof(AmmoSaveObject);
    
    protected override void DoLoad(SaveObject s) {
        AmmoSaveObject saveObject = (AmmoSaveObject) s;
    }

    public override SaveObject CreateSaveObject() => new AmmoSaveObject(this);
}

public class AmmoSaveObject : SaveObject<AmmoGameObject> {
    [JsonInclude] public AmmoType AmmoType { get; set; }
    [JsonInclude] public int Amount { get; set; }

    public AmmoSaveObject() {
    }

    public AmmoSaveObject(AmmoGameObject ammo) : base(ammo) {
        AmmoType = ammo.AmmoType;
        Amount = ammo.Amount;
    }
}