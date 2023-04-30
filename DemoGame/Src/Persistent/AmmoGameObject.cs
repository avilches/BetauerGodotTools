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
}