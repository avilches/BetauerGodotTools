using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Betauer.Application.Lifecycle.Attributes;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.DI.Attributes;
using Veronenger.Game.Dungeon;

namespace Veronenger.Game.Dungeon;

[Configuration]
[Loader("GameLoader", Tag = MainResources.GameLoaderTag)]
public class DungeonMainResources {
    [Transient<DungeonGameView>(Name = "DungeonGameView")]
    public DungeonGameView GameView => new DungeonGameView();
}

[Configuration]
[Loader("GameLoader", Tag = GameLoaderTag)]
[Scene.Transient<DungeonMap>(Name="DungeonMapFactory", Path="res://Game/Dungeon/DungeonMap.tscn")]
public class DungeonGameResources {
    public const string GameLoaderTag = "dungeon";
}


public interface IDungeonSaveObject : ISaveObject {
}

[Configuration]
public class DungeonGameConfig {
    [Singleton] public JsonGameLoader<DungeonSaveGameMetadata> DungeonGameObjectLoader() {
        var loader = new JsonGameLoader<DungeonSaveGameMetadata>();
        loader.WithJsonSerializerOptions(options => {
            options.AllowTrailingCommas = true;
            options.WriteIndented = true;
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
        loader.Scan<IDungeonSaveObject>();
        return loader;
    }
}

[Singleton]
public class DungeonConfig {
}
