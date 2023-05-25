using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Betauer.Core;

namespace Betauer.Application.Persistent.Json;

public class JsonGameLoader<TMetadata> : GameObjectLoader where TMetadata : Metadata {
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

    public async Task<List<TMetadata>> GetMetadatas(string? seed, params string[] saveNames) {
        var metadatas = new List<TMetadata>();
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) return metadatas;
        foreach (var saveName in saveNames) {
            try {
                var metadata = await LoadMetadata(saveName, seed);
                metadatas.Add(metadata);
            } catch (Exception) {
            }
        }
        return metadatas;
    }

    public async Task<List<TMetadata>> ListMetadatas(string? seed = null) {
        var saveGames = new List<TMetadata>();
        var saveGameFolder = GetSavegameFolder();
        if (!Directory.Exists(saveGameFolder)) return saveGames;
        var files = Directory.GetFiles(saveGameFolder, "*." + MetadataExtension);
        foreach (var file in files) {
            var saveName = Path.GetFileNameWithoutExtension(file);
            try {
                var metadata = await LoadMetadata(saveName, seed);
                saveGames.Add(metadata);
            } catch (Exception) {
            }
        }
        return saveGames;
    }

    public async Task SaveMetadata(string saveName, TMetadata metadata, string? seed = null) {
        metadata.UpdateDate = DateTime.Now;
        if (!Directory.Exists(GetSavegameFolder())) Directory.CreateDirectory(GetSavegameFolder());
        var metadataInfo = GetMetadataInfo(saveName);

        // Save metadata
        if (seed == null) {
            await using var stream = File.Create(metadataInfo.FullName);
            await JsonSerializer.SerializeAsync(stream, metadata, JsonSerializerOptions());
        } else {    
            using var encryptor = CreateEncryptor(seed);
            await using var stream = new CryptoStream(File.Create(metadataInfo.FullName), encryptor, CryptoStreamMode.Write);
            await JsonSerializer.SerializeAsync(stream, metadata, JsonSerializerOptions());
            await stream.FlushFinalBlockAsync();
        }
    }

    public async Task<TMetadata> LoadMetadata(string saveName, string? seed = null, bool decompress = true) {
        var metadataInfo = GetMetadataInfo(saveName);
        if (!metadataInfo.Exists) throw new FileNotFoundException($"Savegame metadata not found: {saveName}", metadataInfo.FullName);

        var savegameInfo = GetSavegameInfo(saveName);
        if (!savegameInfo.Exists) throw new FileNotFoundException($"Savegame data not found: {saveName}", savegameInfo.FullName);

        TMetadata metadata = null!;
        if (seed == null) {
            await using var stream = File.OpenRead(metadataInfo.FullName);
            metadata = (await JsonSerializer.DeserializeAsync<TMetadata>(stream, JsonSerializerOptions()))!;
        } else {
            using var decryptor = CreateDecryptor(seed);
            await using var stream = new CryptoStream(File.OpenRead(metadataInfo.FullName), decryptor, CryptoStreamMode.Read);
            metadata = (await JsonSerializer.DeserializeAsync<TMetadata>(stream, JsonSerializerOptions()))!;
        }
        
        metadata.Name = saveName;
        metadata.ReadDate = DateTime.Now;
        return metadata;
    }

    public async Task Save(string saveName, TMetadata metadata, List<SaveObject> saveObjects, Action<float>? progress = null, string? seed = null, bool compress = true) {
        var savegameInfo = GetSavegameInfo(saveName);
        var progressList = CreateProgressList(saveObjects, progress);
        if (seed == null) {
            // json --> hash --> compress --> file
            using var sha256 = SHA256.Create();
            await using var fileStream = File.Create(savegameInfo.FullName);
            await using var compressStream = Compress(fileStream, compress);
            await using var hashStream = new CryptoStream(compressStream, sha256, CryptoStreamMode.Write);
            await JsonSerializer.SerializeAsync(hashStream, progressList, JsonSerializerOptions());

            await hashStream.FlushFinalBlockAsync();
            metadata.Hash = BitConverter.ToString(sha256.Hash).Replace("-", "");
            // Console.WriteLine("Save " + saveName + " " + BitConverter.ToString(sha256.Hash).Replace("-", ""));
        } else {
            // json --> hash --> compress --> encrypt --> file
            using var encryptor = CreateEncryptor(seed);             
            using var sha256 = SHA256.Create();
            await using var fileStream = File.Create(savegameInfo.FullName);
            await using var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write);
            await using var compressStream = Compress(cryptoStream, compress);
            await using var hashStream = new CryptoStream(compressStream, sha256, CryptoStreamMode.Write);
            await JsonSerializer.SerializeAsync(hashStream, progressList, JsonSerializerOptions());
            
            await hashStream.FlushFinalBlockAsync();
            metadata.Hash = BitConverter.ToString(sha256.Hash).Replace("-", "");
            // Console.WriteLine("Save " + saveName + " " + BitConverter.ToString(sha256.Hash).Replace("-", ""));
        }
        await SaveMetadata(saveName, metadata, seed);
    }

    public async Task<SaveGame<TMetadata>> Load(string saveName, Action<float>? progress = null, string? seed = null, bool decompress = true) {
        progress?.Invoke(0f);
        var metadata = await LoadMetadata(saveName, seed);
        var savegameInfo = GetSavegameInfo(saveName);
        float total = savegameInfo.Length;
        void OnRead(long readPos) => progress?.Invoke(readPos / total);
        if (seed == null) {
            // json <-- hash <-- decompress <-- progress <-- file
            using var sha256 = SHA256.Create();
            await using var fileStream = File.OpenRead(savegameInfo.FullName);
            await using var progressFileStream = new ProgressReadStream(fileStream, OnRead);
            await using var decompressStream = Decompress(progressFileStream, decompress);
            await using var hashStream = new CryptoStream(decompressStream, sha256, CryptoStreamMode.Read);
            var gameObjects = (await JsonSerializer.DeserializeAsync<List<SaveObject>>(hashStream, JsonSerializerOptions()))!;

            // Console.WriteLine("Load " + saveName + " " + BitConverter.ToString(sha256.Hash).Replace("-", ""));
            var hash = BitConverter.ToString(sha256.Hash).Replace("-", "");
            if (hash != metadata.Hash) throw new Exception($"Savegame hash mismatch: {saveName} {hash} != {metadata.Hash}");
            return new SaveGame<TMetadata>(metadata, gameObjects);
        } else {
            // json <-- hash <-- decompress <-- decrypt <-- progress <-- file
            using var decryptor = CreateDecryptor(seed);
            using var sha256 = SHA256.Create();
            await using var fileStream = File.OpenRead(savegameInfo.FullName);
            await using var progressFileStream = new ProgressReadStream(fileStream, OnRead);
            await using var decryptStream = new CryptoStream(progressFileStream, decryptor, CryptoStreamMode.Read);
            await using var decompressStream = Decompress(decryptStream, decompress);
            await using var hashStream = new CryptoStream(decompressStream, sha256, CryptoStreamMode.Read);
            var gameObjects = (await JsonSerializer.DeserializeAsync<List<SaveObject>>(hashStream, JsonSerializerOptions()))!;
            
            var hash = BitConverter.ToString(sha256.Hash).Replace("-", "");
            if (hash != metadata.Hash) throw new Exception($"Savegame hash mismatch: {saveName} {hash} != {metadata.Hash}");
            // Console.WriteLine("Load " + saveName + " " + BitConverter.ToString(sha256.Hash).Replace("-", ""));
            return new SaveGame<TMetadata>(metadata, gameObjects);
        }
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