using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Config;

namespace Veronenger.Persistent;

[Singleton]
public class ItemRepository {
    private int _lastId = 0;
    private int NextId() => ++_lastId;
    private readonly Dictionary<int, Item> _itemRegistry = new();
    private readonly Dictionary<string, Item> _itemAlias = new();
    
    [Inject] public PlayerConfig PlayerConfig { get; set; }
    [Inject] private IFactory<PlayerStatus> _playerStatusFactory { get; set; }
    
    public PlayerStatus PlayerStatus { get; private set; }
    public PlayerNode PlayerNode { get; private set; }

    public void Clear() {
        _lastId = 0;
        _itemRegistry.Clear();
        _itemAlias.Clear();
        PlayerStatus = null;
        PlayerNode = null;
    }
    
    public bool IsPlayer(CharacterBody2D player) {
        return PlayerNode.CharacterBody2D == player;
    }
    
    public void SetPlayer(PlayerNode playerNode) {
        PlayerNode = playerNode;
        PlayerStatus = _playerStatusFactory.Get();
        PlayerStatus.Configure(PlayerConfig);
    }

    public TItem Create<TItem>(string name, string? alias = null) where TItem : Item {
        TItem item = Activator.CreateInstance<TItem>();
        item.Name = name;
        item.Alias = alias;
        item.Id = NextId();
        return Add(item);
    }

    /*
     * Items
     */
    public Item Get(int id) => _itemRegistry[id];

    public Item? GetOrNull(int id) => _itemRegistry.TryGetValue(id, out var r) ? r : null; 

    public TItem GetFromMeta<TItem>(GodotObject godotObject) where TItem : Item => (TItem)_itemRegistry[godotObject.GetItemIdFromMeta()];
    
    public TItem Get<TItem>(int id) where TItem : Item => (TItem)_itemRegistry[id];

    public TItem? GetOrNull<TItem>(int id) where TItem : Item => _itemRegistry.TryGetValue(id, out var r) ? r as TItem : null; 

    public Item Get(string alias) => _itemAlias[alias];

    public Item? GetOrNull(string alias) => _itemAlias.TryGetValue(alias, out var r) ? r : null; 

    public TItem Get<TItem>(string alias) where TItem : Item => (TItem)_itemAlias[alias];

    public TItem? GetOrNull<TItem>(string alias) where TItem : Item => _itemAlias.TryGetValue(alias, out var r) ? r as TItem : null; 

    public void Remove(Item item) {
        item.UnlinkNode();
        if (item.Alias != null) _itemAlias.Remove(item.Alias);
        _itemRegistry.Remove(item.Id);
    }

    public void Remove(int id) {
        Remove(_itemRegistry[id]);
    }

    private TItem Add<TItem>(TItem item) where TItem : Item {
        _itemRegistry.Add(item.Id, item);
        if (item.Alias != null) _itemAlias.Add(item.Alias, item);
        return item;
    }
}

public static class WorldExtension {
    private static readonly StringName WorldId = "__WorldId";
    
    public static int GetItemIdFromMeta(this GodotObject o) => 
        o.GetMeta(WorldId).AsInt32();

    public static void LinkMetaToItemId(this GodotObject o, Item item) => 
        o.SetMeta(WorldId, item.Id);

    public static bool HasMetaItemId(this GodotObject o) => 
        o.HasMeta(WorldId);
}