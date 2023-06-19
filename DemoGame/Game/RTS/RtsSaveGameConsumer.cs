using Betauer.Application.Persistent;
using Veronenger.Game.Worlds.RTS;

namespace Veronenger.Game.RTS;

public class RtsSaveGameConsumer : SaveGameConsumer<RtsSaveGameMetadata> {
    public RtsSaveGameConsumer(SaveGame<RtsSaveGameMetadata> saveGame) : base(saveGame) {
        ConsumePlayers();
    }

    public CameraSaveObject Camera { get; private set; }

    public void ConsumePlayers() {
        Camera = Consume<CameraSaveObject>();
    }
}