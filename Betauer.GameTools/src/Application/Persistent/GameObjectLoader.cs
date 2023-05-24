using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Betauer.Tools.Logging;

namespace Betauer.Application.Persistent;

public enum LoadStatus {
    Ok,
    SavegameNotFound,
    MetadataError,
    SaveGameError,
}

public abstract class GameObjectLoader {
    public static Logger Logger = LoggerFactory.GetLogger<GameObjectLoader>();
}

public abstract class GameObjectLoader<TSaveGame> : GameObjectLoader where TSaveGame : SaveGame {
    public abstract Task Save(TSaveGame saveGame, List<SaveObject> saveObjects, string saveName, Action<float>? progress);
    public abstract Task<List<TSaveGame>> GetSaveGames(params string[] saveNames);
    public abstract Task<List<TSaveGame>> ListSaveGames();
    public abstract Task<TSaveGame> Load(string saveName, Action<float>? progress);
    public abstract Task<TSaveGame> LoadMetadataFile(string saveName);

    public virtual string GetSavegameFolder() => AppTools.GetUserFolder();

    public virtual FileInfo CreateFullPath(string saveName, string type) {
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) {
            var info = Directory.CreateDirectory(saveGameFolder);
            if (!info.Exists) throw new Exception($"Unable to create save game folder: {saveGameFolder}");
        }
        return new FileInfo(Path.Combine(saveGameFolder, Path.GetFileName($"{saveName}.{type}")));
    }
}