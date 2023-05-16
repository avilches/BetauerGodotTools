using System;
using Betauer.Application.Persistent;
using Veronenger.Persistent;

namespace Veronenger.Managers;

public class MySaveGameConsumer : SaveGameConsumer<MySaveGame> {
    public MySaveGameConsumer(MySaveGame saveGame) : base(saveGame) {
        ConsumePlayers();
    }

    public PlayerSaveObject Player0 { get; private set; }
    public PlayerSaveObject? Player1 { get; private set; }
    // public PlayerSaveObject Player2 { get; }

    public void ConsumePlayers() {
        Player0 = ConsumeAlias<PlayerSaveObject>("Player0");
        Player1 = ConsumeAliasOrIgnore<PlayerSaveObject>("Player1");
        ConsumeWhere<PlayerSaveObject>(p => throw new Exception($"Unknown player in the savegame: {p.Alias}/{p.Alias}"));
    }
}