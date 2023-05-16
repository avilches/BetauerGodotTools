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

    public virtual string GetSavegameFullPath(string saveName, string type) => 
        Path.Combine(GetSavegameFolder(), Path.GetFileName($"{saveName}.{type}"));

    protected FileStream OpenMetadata(string saveName) => File.OpenRead(GetSavegameFullPath(saveName, "metadata"));
    protected FileStream WriteMetadata(string saveName) => File.Create(GetSavegameFullPath(saveName, "metadata"));
    protected FileStream OpenData(string saveName) => File.OpenRead(GetSavegameFullPath(saveName, "data"));
    protected FileStream WriteData(string saveName) => File.Create(GetSavegameFullPath(saveName, "data"));


    public async Task Save(TSaveGame savegame, List<SaveObject> saveObjects, string saveName) {
        if (savegame.CreateDate == null) savegame.CreateDate = DateTime.Now;
        savegame.Version = Version;
        savegame.UpdateDate = DateTime.Now;
        
        await using FileStream createStreamMetadata = WriteMetadata(saveName);
        await using FileStream createStreamSaveObjects = WriteData(saveName);
        await JsonSerializer.SerializeAsync(createStreamMetadata, savegame, JsonSerializerOptions());
        await JsonSerializer.SerializeAsync(createStreamSaveObjects, saveObjects, JsonSerializerOptions());
    }

    public async Task<TSaveGame> Load(string saveName) {
        await using FileStream openStreamMetadata = OpenMetadata(saveName);
        await using FileStream openStreamSaveObjects = OpenData(saveName);

        var savegame = (await JsonSerializer.DeserializeAsync<TSaveGame>(openStreamMetadata, JsonSerializerOptions()))!;
        var saveObjects = (await JsonSerializer.DeserializeAsync<List<SaveObject>>(openStreamSaveObjects, JsonSerializerOptions()))!;
        savegame.GameObjects = saveObjects;
        savegame.ReadDate = DateTime.Now;
        return savegame;
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

}