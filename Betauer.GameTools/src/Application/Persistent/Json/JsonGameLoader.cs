using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Betauer.Core;

namespace Betauer.Application.Persistent.Json;

public class JsonGameLoader<TSaveGame> : IGameObjectLoader<TSaveGame> where TSaveGame : SaveGame {
    private JsonSerializerOptions? _jsonSerializerOptions;
    public JsonSerializerOptions JsonSerializerOptions() => _jsonSerializerOptions ??= BuildJsonSerializerOptions();
    
    public int Version { get; }

    public JsonGameLoader(int version) {
        Version = version;
    }

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

    public virtual string GetSavegameFolder() => AppTools.GetUserFolder();

    public virtual string GetSavegameFullPath(string saveName, string type) {
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) {
            var info = Directory.CreateDirectory(saveGameFolder);
            if (!info.Exists) throw new Exception($"Unable to create save game folder: {saveGameFolder}");
        }
        return Path.Combine(saveGameFolder, Path.GetFileName($"{saveName}.{type}"));
    }

    protected FileStream OpenMetadata(string saveName) => File.OpenRead(GetSavegameFullPath(saveName, "metadata"));
    protected FileStream WriteMetadata(string saveName) => File.Create(GetSavegameFullPath(saveName, "metadata"));
    protected FileStream OpenData(string saveName) => File.OpenRead(GetSavegameFullPath(saveName, "data"));
    protected FileStream WriteData(string saveName) => File.Create(GetSavegameFullPath(saveName, "data"));

    public async Task<List<TSaveGame>> ListSaveGames() {
        var saveGames = new List<TSaveGame>();
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) return saveGames;
        var files = Directory.GetFiles(saveGameFolder, "*.metadata");
        foreach (var file in files) {
            var saveName = Path.GetFileNameWithoutExtension(file);
            try {
                var saveGame = await LoadHeader(saveName);
                saveGames.Add(saveGame);
            } catch (Exception e) {
                saveGames.Add(CreateFaultedSaveGame(file, e.Message));
            }
        }
        return saveGames;
    }

    public async Task Save(TSaveGame saveGame, List<SaveObject> saveObjects, string saveName) {
        saveGame.Version = Version;
        saveGame.UpdateDate = DateTime.Now;
        
        await using FileStream createStreamMetadata = WriteMetadata(saveName);
        await using FileStream createStreamSaveObjects = WriteData(saveName);
        await JsonSerializer.SerializeAsync(createStreamMetadata, saveGame, JsonSerializerOptions());
        await JsonSerializer.SerializeAsync(createStreamSaveObjects, saveObjects, JsonSerializerOptions());
    }

    public async Task<TSaveGame> LoadHeader(string saveName) {
        await using FileStream openStreamMetadata = OpenMetadata(saveName);
        var saveGame = (await JsonSerializer.DeserializeAsync<TSaveGame>(openStreamMetadata, JsonSerializerOptions()))!;
        saveGame.Name = saveName;
        saveGame.ReadDate = DateTime.Now;
        saveGame.ErrorMessage = null;
        return saveGame;
    }

    public async Task<TSaveGame> Load(string saveName) {
        var saveGame = await LoadHeader(saveName);
        await using FileStream openStreamSaveObjects = OpenData(saveName);
        var saveObjects = (await JsonSerializer.DeserializeAsync<List<SaveObject>>(openStreamSaveObjects, JsonSerializerOptions()))!;
        saveGame.GameObjects = saveObjects;
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

    private static TSaveGame CreateFaultedSaveGame(string file, string errorMessage) {
        var saveName = Path.GetFileNameWithoutExtension(file);
        var saveGame = Activator.CreateInstance<TSaveGame>();
        saveGame.Name = saveName;
        saveGame.ReadDate = DateTime.Now;
        saveGame.ErrorMessage = errorMessage;
        try {
            saveGame.CreateDate = File.GetCreationTime(file);
            saveGame.UpdateDate = File.GetLastWriteTime(file);
        } catch (Exception exception) {
            Console.WriteLine(exception);
        }
        return saveGame;
    }
}