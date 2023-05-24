using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Betauer.Core;

namespace Betauer.Application.Persistent.Json;

public class JsonGameLoader<TSaveGame> : GameObjectLoader<TSaveGame> where TSaveGame : SaveGame {
    private JsonSerializerOptions? _jsonSerializerOptions;
    public JsonSerializerOptions JsonSerializerOptions() => _jsonSerializerOptions ??= BuildJsonSerializerOptions();

    public static string MetadataExtension = "metadata";
    public static string SaveGameExtension = "data";
    
    public void WithJsonSerializerOptions(Action<JsonSerializerOptions> action) => action(JsonSerializerOptions());

    protected virtual JsonSerializerOptions BuildJsonSerializerOptions() {
        // more info: https://devblogs.microsoft.com/dotnet/system-text-json-in-dotnet-7/
        var options = new JsonSerializerOptions {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver {
                Modifiers = { ConfigureSaveObjectSerializer }
            },
            Converters = {
                new Rect2Converter(),
                new Vector2Converter(),
                new Vector3Converter(),
                new Rect2IConverter(),
                new Vector2IConverter(),
                new Vector3IConverter(),
                new ColorConverter(),
            }
        };
        return options;
    }

    private FileInfo GetMetadataInfo(string saveName) => CreateFullPath(saveName, MetadataExtension);
    private FileInfo GetSavegameInfo(string saveName) => CreateFullPath(saveName, SaveGameExtension);

    public override async Task<List<TSaveGame>> GetSaveGames(params string[] saveNames) {
        var saveGames = new List<TSaveGame>();
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) return saveGames;
        foreach (var saveName in saveNames) {
            var saveGame = await LoadMetadataFile(saveName);
            saveGames.Add(saveGame);
        }
        return saveGames;
    }

    public override async Task<List<TSaveGame>> ListSaveGames() {
        var saveGames = new List<TSaveGame>();
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) return saveGames;
        var files = Directory.GetFiles(saveGameFolder, "*."+MetadataExtension)
            .Concat(Directory.GetFiles(saveGameFolder, "*."+SaveGameExtension))
            .Select(Path.GetFileNameWithoutExtension)
            .Distinct();
        foreach (var saveName in files) {
            var saveGame = await LoadMetadataFile(saveName);
            saveGames.Add(saveGame);
        }
        return saveGames;
    }

    public override async Task Save(TSaveGame saveGame, List<SaveObject> saveObjects, string saveName, Action<float>? progress) {
        saveGame.UpdateDate = DateTime.Now;
        if (!Directory.Exists(GetSavegameFolder())) Directory.CreateDirectory(GetSavegameFolder());
        await using FileStream createStreamMetadata = File.Create(GetMetadataInfo(saveName).FullName);
        await using FileStream createStreamSaveObjects = File.Create(GetSavegameInfo(saveName).FullName);
        await JsonSerializer.SerializeAsync(createStreamMetadata, saveGame, JsonSerializerOptions());
        await JsonSerializer.SerializeAsync(createStreamSaveObjects, CreateProgressList(saveObjects, progress), JsonSerializerOptions());
    }

    public override async Task<TSaveGame> LoadMetadataFile(string saveName) {
        var metadataInfo = GetMetadataInfo(saveName);
        var savegameInfo = GetSavegameInfo(saveName);
        TSaveGame saveGame = null!;
        if (metadataInfo.Exists) {
            saveGame = await LoadMetadataFile(metadataInfo, savegameInfo);
            if (savegameInfo.Exists) {
                saveGame.Size = savegameInfo.Length;
            } else {
                saveGame.LoadStatus = LoadStatus.SavegameNotFound;
            }
        } else {
            saveGame = await LoadMissingMetadata(metadataInfo, savegameInfo);
        }
        saveGame.Name = saveName;
        saveGame.ReadDate = DateTime.Now;
        saveGame.MetadataFileName = metadataInfo.FullName;
        saveGame.SavegameFileName = savegameInfo.FullName;
        return saveGame;
    }

    private async Task<TSaveGame> LoadMissingMetadata(FileInfo metadataInfo, FileInfo savegameInfo) {
        var saveGame = Activator.CreateInstance<TSaveGame>();
        if (savegameInfo.Exists) {
            // No metadata but savegame, create metadata
            saveGame.LoadStatus = LoadStatus.Ok;
            saveGame.Size = savegameInfo.Length;
            saveGame.CreateDate = savegameInfo.CreationTime;
            saveGame.UpdateDate = savegameInfo.LastWriteTime;
            try {
                await using FileStream createStreamMetadata = File.Create(metadataInfo.FullName);
                await JsonSerializer.SerializeAsync(createStreamMetadata, saveGame, JsonSerializerOptions());
            } catch (Exception e) {
                Logger.Error($"Error rebuilding metadata: {metadataInfo.FullName}\n{e}");
                saveGame.LoadStatus = LoadStatus.MetadataError;
            }
        } else {
            saveGame.LoadStatus = LoadStatus.SavegameNotFound;
        }
        return saveGame;
    }

    private async Task<TSaveGame> LoadMetadataFile(FileInfo metadataInfo, FileInfo savegameInfo) {
        try {
            await using FileStream openStreamMetadata = File.OpenRead(metadataInfo.FullName);
            return (await JsonSerializer.DeserializeAsync<TSaveGame>(openStreamMetadata, JsonSerializerOptions()))!;
        } catch (Exception e) {
            Logger.Error($"Error loading metadata: {metadataInfo.FullName}. Rebuilding. {e.Message}");
            return await LoadMissingMetadata(metadataInfo, savegameInfo);
        }
    }

    public override async Task<TSaveGame> Load(string saveName, Action<float>? progress) {
        var saveGame = await LoadMetadataFile(saveName);
        if (!saveGame.Ok) return saveGame;

        try {
            await using FileStream openStreamSaveObjects = File.OpenRead(saveGame.SavegameFileName);
            float total = saveGame.Size;
            var progressReadStream = new ProgressReadStream(openStreamSaveObjects, (readPos) => progress?.Invoke(readPos / total));
            var saveObjects = (await JsonSerializer.DeserializeAsync<List<SaveObject>>(progressReadStream,
            JsonSerializerOptions()))!;
            saveGame.GameObjects = saveObjects;
        } catch (Exception e) {
            Logger.Error($"Error loading savegame: {saveGame.SavegameFileName}\n{e}");
            saveGame.LoadStatus = LoadStatus.SaveGameError;
        }
        return saveGame;
    }

    private static void ConfigureSaveObjectSerializer(JsonTypeInfo jsonTypeInfo) {
        if (jsonTypeInfo.Type == typeof(SaveObject)) {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
                // TypeDiscriminatorPropertyName = "_case",
                // UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                // DerivedTypes = JsonDerivedTypes
            };
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList()
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(SaveObject).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<SaveObject>()
                .Select(t => new JsonDerivedType(t.GetType(), t.Discriminator()))
                .ForEach(dt => jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(dt));
        }
    }

    private static IEnumerable<SaveObject> CreateProgressList(IReadOnlyCollection<SaveObject> saveObjects, Action<float>? progress) {
        progress?.Invoke(0f);
        var current = 0f;
        var total = (float)saveObjects.Count;
        return saveObjects.Select(t => {
            current++;
            progress?.Invoke(current / total);
            return t;
        });
    }
}