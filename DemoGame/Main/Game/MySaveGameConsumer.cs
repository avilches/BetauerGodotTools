using System;
using Betauer.Application.Persistent;
using Veronenger.Character.Player;
using Veronenger.Persistent;

namespace Veronenger.Main.Game;

public class MySaveGameConsumer : SaveGameConsumer<MySaveGameMetadata> {
    public MySaveGameConsumer(SaveGame<MySaveGameMetadata> saveGame) : base(saveGame) {
        ConsumePlayers();
    }

    public PlayerSaveObject Player0 { get; private set; }
    public PlayerSaveObject? Player1 { get; private set; }

    public InventorySaveObject Inventory0 { get; private set; }
    public InventorySaveObject? Inventory1 { get; private set; }
    // public PlayerSaveObject Player2 { get; }

    public void ConsumePlayers() {
        Player0 = ConsumeAlias<PlayerSaveObject>("Player0");
        Player1 = ConsumeAliasOrIgnore<PlayerSaveObject>("Player1");
        Inventory0 = ConsumeAlias<InventorySaveObject>("PlayerInventory0");
        Inventory1 = ConsumeAliasOrIgnore<InventorySaveObject>("PlayerInventory1");
        ConsumeWhere<PlayerSaveObject>(p => throw new Exception($"Unknown player in the savegame: {p.Alias}/{p.Alias}"));
    }
}