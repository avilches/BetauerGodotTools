using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Betauer.Application.Persistent.Json;
using Betauer.Core;

namespace Betauer.Application.Persistent;

public class JsonGameObjectRepository : GameObjectRepository {
    public JsonPolymorphismOptions JsonPolymorphismOptions { get; init; }
    public IList<JsonConverter> JsonConverters { get; init; }
    public bool JsonPretty { get; init; }
    private JsonSerializerOptions? _jsonSerializerOptions;
    private JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions ??= BuildJsonSerializerOptions();

    public override async Task SaveObjects(FileStream createStream, IEnumerable<SaveObject> saveObjects) {
        await JsonSerializer.SerializeAsync(createStream, saveObjects, JsonSerializerOptions);
    }

    public override async Task<List<SaveObject>> LoadSaveObjects(FileStream openStream) {
        return (await JsonSerializer.DeserializeAsync<List<SaveObject>>(openStream, JsonSerializerOptions))!;
    }

    protected virtual JsonSerializerOptions BuildJsonSerializerOptions() {
        void Item(JsonTypeInfo typeInfo) {
            if (typeInfo.Type != typeof(SaveObject)) return;
            typeInfo.PolymorphismOptions = JsonPolymorphismOptions;
        }

        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(Item);

        var options = new JsonSerializerOptions {
            WriteIndented = JsonPretty,
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
}