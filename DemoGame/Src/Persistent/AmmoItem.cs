using Veronenger.Config;

namespace Veronenger.Persistent;

public class AmmoItem : PickableItem {
    public AmmoType AmmoType;
    public int Amount;

    public AmmoItem Configure(PickableConfig config, AmmoType ammoType, int amount) {
        Config = config;    
        AmmoType = ammoType;
        Amount = amount;
        return this;
    }
}