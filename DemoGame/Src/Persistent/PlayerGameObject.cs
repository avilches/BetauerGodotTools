using Betauer.Application.Persistent;
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
}