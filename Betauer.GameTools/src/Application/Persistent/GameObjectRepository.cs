using System;
using System.Collections.Generic;
using Betauer.DI.Attributes;
using Godot;

namespace Betauer.Application.Persistent;

public class GameObjectRepository {
    private int _lastId = 0;
    private int NextId() => ++_lastId;
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
        item.Id = NextId();
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