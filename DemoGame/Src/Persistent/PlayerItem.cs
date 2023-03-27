using Veronenger.Character.Player;
using Veronenger.Config;

namespace Veronenger.Persistent; 

public class PlayerItem : Item<PlayerNode> {
    public PlayerStatus Status;
    public PlayerConfig Config;

    public PlayerItem Configure(PlayerConfig config) {
        Config = config;
        Status = new PlayerStatus(config);
        return this;
    }
}