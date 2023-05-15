using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Betauer.Application.Persistent.Json;
using Betauer.Core;

namespace Betauer.Application.Persistent;

public class GameObjectRepository {

    public JsonPolymorphismOptions JsonPolymorphismOptions { get; init; }
    public IList<JsonConverter> JsonConverters { get; init; }
    public bool JsonPretty { get; init; }
    private JsonSerializerOptions? _jsonSerializerOptions;
    private JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions ??= BuildJsonSerializerOptions();

    private int _lastId = 0;
    private readonly Dictionary<int, GameObject> _registry = new();
    private readonly Dictionary<string, GameObject> _alias = new();

    public void Clear() {
        _lastId = 0;
        _registry.Clear();
        _alias.Clear();
    }

    public TItem Create<TItem>(string name, string? alias = null) where TItem : GameObject {
        TItem item = Activator.CreateInstance<TItem>();
        item.Name = name;
        item.Alias = alias;
        item.Id = ++_lastId;
        return Add(item);
    }

    private GameObject Create(SaveObject saveObject) {
        GameObject item = (GameObject)Activator.CreateInstance(saveObject.GameObjectType)!;
        item.Name = saveObject.Name;
        item.Alias = saveObject.Alias;
        item.Id = saveObject.Id;
        _lastId = Math.Max(_lastId, item.Id);
        return Add(item);
    }

    public GameObject Get(int id) => _registry[id];

    public GameObject? GetOrNull(int id) => _registry.TryGetValue(id, out var r) ? r : null;

    public TItem Get<TItem>(int id) where TItem : GameObject => (TItem)_registry[id];

    public TItem? GetOrNull<TItem>(int id) where TItem : GameObject => _registry.TryGetValue(id, out var r) ? r as TItem : null;

    public GameObject Get(string alias) => _alias[alias];

    public GameObject? GetOrNull(string alias) => _alias.TryGetValue(alias, out var r) ? r : null;

    public TItem Get<TItem>(string alias) where TItem : GameObject => (TItem)_alias[alias];

    public TItem? GetOrNull<TItem>(string alias) where TItem : GameObject => _alias.TryGetValue(alias, out var r) ? r as TItem : null;

    public void Remove(GameObject gameObject) {
        if (gameObject.Alias != null) _alias.Remove(gameObject.Alias);
        _registry.Remove(gameObject.Id);
        gameObject.OnRemove();
    }

    public void Remove(int id) {
        Remove(_registry[id]);
    }

    private TItem Add<TItem>(TItem item) where TItem : GameObject {
        _registry.Add(item.Id, item);
        if (item.Alias != null) _alias.Add(item.Alias, item);
        return item;
    }

    public async void Save(string saveName = "savegame.json") {
        await Persist(saveName, _registry.Values.Select(g => g.CreateSaveObject()));
    }


    public async Task Persist(string saveName, IEnumerable<SaveObject> saveObjects) {
        var fileName = AppTools.GetUserFile(AppTools.GetUserFile(saveName));
        await using FileStream createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, saveObjects, JsonSerializerOptions);
    }

    public async Task<Dictionary<int, SaveObject>> Load(string saveName = "savegame.json") {
        var objects = await LoadSaveObjects(saveName);
        Clear();

        var save = new Dictionary<int, SaveObject>();
        foreach (var saveObject in objects) {
            save[saveObject.Id] = saveObject;
            Create(saveObject);
        }
        return save;
    }

    public async Task<List<SaveObject>> LoadSaveObjects(string saveName) {
        await using FileStream createStream = File.OpenRead(AppTools.GetUserFile(saveName));
        return (await JsonSerializer.DeserializeAsync<List<SaveObject>>(createStream, JsonSerializerOptions))!;
    }
    
    private JsonSerializerOptions BuildJsonSerializerOptions() {
        var resolver = new DefaultJsonTypeInfoResolver();
        void Item(JsonTypeInfo typeInfo) {
            if (typeInfo.Type != typeof(SaveObject)) return;
            typeInfo.PolymorphismOptions = JsonPolymorphismOptions;
        }
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
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        }.ForEach(converter => options.Converters.Add(converter));
        
        JsonConverters?.ForEach(converter => options.Converters.Add(converter));
        return options;
    }
}

public static class GameObjectExtensions {
    private static readonly StringName GameObjectId = "__GameObjectId";
    private static readonly StringName NodeId = "__NodeId";

    public static int GetGameObjectIdFromMeta(this GodotObject o) =>
        o.GetMeta(GameObjectId).AsInt32();

    public static void SetNodeIdToMeta(this GodotObject o, GameObject gameObject) =>
        o.SetMeta(GameObjectId, gameObject.Id);

    public static bool HasMetaGameObjectId(this GodotObject o) =>
        o.HasMeta(GameObjectId);

    public static ulong GetNodeIdFromMeta(this GodotObject o) =>
        o.GetMeta(NodeId).AsUInt64();

    public static T GetNodeFromMeta<T>(this GodotObject o) where T : GodotObject =>
        (T)GodotObject.InstanceFromId(GetNodeIdFromMeta(o));

    public static void SetNodeIdToMeta(this GodotObject o, GodotObject item) =>
        o.SetMeta(NodeId, item.GetInstanceId());

    public static bool HasMetaNodeId(this GodotObject o) =>
        o.HasMeta(NodeId);
}