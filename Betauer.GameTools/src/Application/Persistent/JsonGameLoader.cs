using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Betauer.Application.Persistent.Json;
using Betauer.Core;

namespace Betauer.Application.Persistent;

public class JsonGameLoader : IGameObjectLoader {
    public IList<JsonDerivedType> JsonDerivedTypes { get; init; } = new List<JsonDerivedType>();
    public IList<JsonConverter> JsonConverters { get; init; } = new List<JsonConverter>();
    public bool JsonPretty { get; init; }
    
    private JsonSerializerOptions? _jsonSerializerOptions;
    public JsonSerializerOptions JsonSerializerOptions() => _jsonSerializerOptions ??= BuildJsonSerializerOptions();

    protected virtual JsonSerializerOptions BuildJsonSerializerOptions() {
        if (JsonDerivedTypes == null || JsonDerivedTypes.Count == 0) {
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList()
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(SaveObject).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<SaveObject>()
                .ForEach(t => JsonDerivedTypes.Add(new JsonDerivedType(t.GetType(), t.Discriminator())));
        }
        
        
        void Item(JsonTypeInfo jsonTypeInfo) {
            if (jsonTypeInfo.Type == typeof(SaveObject)) {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
                    // TypeDiscriminatorPropertyName = "_case",
                    // UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                    // DerivedTypes = JsonDerivedTypes
                };
                JsonDerivedTypes.ForEach(dt => jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(dt));
            }
        }

        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(Item);

        var options = new JsonSerializerOptions {
            WriteIndented = JsonPretty,
            AllowTrailingCommas = true,
            TypeInfoResolver = resolver,
        };
        new JsonConverter[] {
            new Rect2Converter(),
            new Vector2Converter(),
            new Vector3Converter(),
            new Rect2IConverter(),
            new Vector2IConverter(),
            new Vector3IConverter(),
            new ColorConverter(),
        }.ForEach(converter => options.Converters.Add(converter));

        JsonConverters?.ForEach(converter => options.Converters.Add(converter));
        return options;
    }

    public async Task Save(List<SaveObject> saveObjects, string saveName) {
        var fileName = AppTools.GetUserFile(saveName);
        await using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, saveObjects, JsonSerializerOptions());
    }

    public async Task<List<SaveObject>> Load(string saveName) {
        var fileName = AppTools.GetUserFile(saveName);
        await using FileStream openStream = File.OpenRead(fileName);
        return (await JsonSerializer.DeserializeAsync<List<SaveObject>>(openStream, JsonSerializerOptions()))!;
    }
}