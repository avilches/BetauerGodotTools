using System;
using System.Collections.Generic;
using Betauer.Application.Persistent;
using Veronenger.Platform.Character.Player;
using Veronenger.Platform.Persistent;

namespace Veronenger.Platform.Managers;

public class PlatformSaveGameConsumer : SaveGameConsumer<MySaveGameMetadata> {
    public PlatformSaveGameConsumer(SaveGame<MySaveGameMetadata> saveGame) : base(saveGame) {
        ConsumePlayers();
    }

    public readonly List<PlayerSaveObject> Players = new ();
    public readonly List<InventorySaveObject> Inventories = new();

    public void ConsumePlayers() {
        Players.Add(ConsumeAlias<PlayerSaveObject>("Player0"));
        if (ConsumeAliasOrIgnore<PlayerSaveObject>("Player1") is PlayerSaveObject playerSaveObject) {
            Players.Add(playerSaveObject);;
        }
        Inventories.Add(ConsumeAlias<InventorySaveObject>("PlayerInventory0"));
        if (ConsumeAliasOrIgnore<InventorySaveObject>("PlayerInventory1") is InventorySaveObject inventorySaveObject) {
            Inventories.Add(inventorySaveObject);
        }
        ConsumeWhere<PlayerSaveObject>(p => throw new Exception($"Unknown player in the savegame: {p.Alias}/{p.Alias}"));
    }
}