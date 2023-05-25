using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Betauer.Core;

namespace Betauer.Application.Persistent.Json;

public class JsonGameLoader<TSaveGame> : GameObjectLoader where TSaveGame : SaveGame {
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

    public virtual string GetSavegameFolder() => AppTools.GetUserFolder();

    public virtual FileInfo CreateFullPath(string saveName, string type) {
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) {
            var info = Directory.CreateDirectory(saveGameFolder);
            if (!info.Exists) throw new Exception($"Unable to create save game folder: {saveGameFolder}");
        }
        return new FileInfo(Path.Combine(saveGameFolder, Path.GetFileName($"{saveName}.{type}")));
    }

    private FileInfo GetMetadataInfo(string saveName) => CreateFullPath(saveName, MetadataExtension);
    private FileInfo GetSavegameInfo(string saveName) => CreateFullPath(saveName, SaveGameExtension);

    public async Task<List<TSaveGame>> GetSaveGames(params string[] saveNames) {
        var saveGames = new List<TSaveGame>();
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) return saveGames;
        foreach (var saveName in saveNames) {
            var saveGame = await LoadMetadata(saveName);
            saveGames.Add(saveGame);
        }
        return saveGames;
    }

    public async Task<List<TSaveGame>> ListSaveGames() {
        var saveGames = new List<TSaveGame>();
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) return saveGames;
        var files = Directory.GetFiles(saveGameFolder, "*." + MetadataExtension)
            .Concat(Directory.GetFiles(saveGameFolder, "*." + SaveGameExtension))
            .Select(Path.GetFileNameWithoutExtension)
            .Distinct();
        foreach (var saveName in files) {
            var saveGame = await LoadMetadata(saveName);
            saveGames.Add(saveGame);
        }
        return saveGames;
    }

    public async Task Save(TSaveGame saveGame, List<SaveObject> saveObjects, string saveName, Action<float>? progress = null, string? seed = null, bool compress = true) {
        saveGame.UpdateDate = DateTime.Now;
        if (!Directory.Exists(GetSavegameFolder())) Directory.CreateDirectory(GetSavegameFolder());
        var metadataInfo = GetMetadataInfo(saveName);
        var savegameInfo = GetSavegameInfo(saveName);

        // Save metadata
        await using FileStream metadataStream = File.Create(metadataInfo.FullName);
        await JsonSerializer.SerializeAsync(metadataStream, saveGame, JsonSerializerOptions());

        // Save savegame
        if (seed == null) {
            await using var stream = Compress(File.Create(savegameInfo.FullName), compress);
            await JsonSerializer.SerializeAsync(stream, CreateProgressList(saveObjects, progress), JsonSerializerOptions());
        } else {    
            using var encryptor = CreateEncryptor(seed);
            await using var cryptoStream = new CryptoStream(Compress(File.Create(savegameInfo.FullName), compress), encryptor, CryptoStreamMode.Write);
            await JsonSerializer.SerializeAsync(cryptoStream, CreateProgressList(saveObjects, progress), JsonSerializerOptions());
        }
    }

    private Stream Compress(Stream stream, bool compress) {
        return compress ? new GZipStream(stream, CompressionMode.Compress) : stream;
    }

    private Stream Decompress(string path, bool decompress) {
        var stream = File.OpenRead(path);
        return decompress ? new GZipStream(stream, CompressionMode.Decompress) : stream;
    }

    public async Task<TSaveGame> LoadMetadata(string saveName) {
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
                Logger.Error($"Error rebuilding metadata {metadataInfo.FullName}: {e.Message}");
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
            Logger.Error($"Error loading metadata: {metadataInfo.FullName}: {e.Message} (rebuilding)");
            return await LoadMissingMetadata(metadataInfo, savegameInfo);
        }
    }

    public async Task<TSaveGame> Load(string saveName, Action<float>? progress = null, string? seed = null, bool decompress = true) {
        var saveGame = await LoadMetadata(saveName);
        if (!saveGame.Ok) return saveGame;
        float total = saveGame.Size;

        try {
            if (seed == null) {
                await using var progressStream = new ProgressReadStream(Decompress(saveGame.SavegameFileName, decompress), (readPos) => progress?.Invoke(readPos / total));
                saveGame.GameObjects = await JsonSerializer.DeserializeAsync<List<SaveObject>>(progressStream, JsonSerializerOptions());
            } else {
                using var decryptor = CreateDecryptor(seed);
                var cryptoStream = new CryptoStream(Decompress(saveGame.SavegameFileName, decompress), decryptor, CryptoStreamMode.Read);
                await using var progressStream = new ProgressReadStream(cryptoStream, (readPos) => progress?.Invoke(readPos / total));
                saveGame.GameObjects = await JsonSerializer.DeserializeAsync<List<SaveObject>>(progressStream, JsonSerializerOptions());
            }
        } catch (Exception e) {
            Logger.Error($"Error loading savegame {saveGame.SavegameFileName}: {e.Message}");
            saveGame.LoadStatus = LoadStatus.SaveGameError;
        }
        return saveGame;
    }
    
    
    public static ICryptoTransform CreateDecryptor(string seed) {
        using var aes = Aes.Create();
        var (key, iv) = GenerateKeyAndIV(seed);
        return aes.CreateDecryptor(key, iv);
    }
        
    public static ICryptoTransform CreateEncryptor(string seed) {
        using var aes = Aes.Create();
        var (key, iv) = GenerateKeyAndIV(seed);
        return aes.CreateEncryptor(key, iv);
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
        if (progress == null) return saveObjects;
        progress.Invoke(0f);
        var current = 0f;
        var total = (float)saveObjects.Count;
        return saveObjects.Select(t => {
            current++;
            progress.Invoke(current / total);
            return t;
        });
    }
}