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

    public override SaveObject CreateSaveObject() {
        return new AmmoSaveObject(this);
    }
}

public class AmmoSaveObject : SaveObject<AmmoGameObject> {
    public AmmoType AmmoType { get; }
    public int Amount { get; }
    
    public AmmoSaveObject(AmmoGameObject ammo) : base(ammo) {
        AmmoType = ammo.AmmoType;
        Amount = ammo.Amount;
    }
}