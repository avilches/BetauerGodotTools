using System.Collections.Generic;
using Betauer.DI;

namespace Veronenger.Character.Items;

[Service]
public class World {
    private int _lastId = 0;

    private readonly Dictionary<int, WorldItem> _itemRegistry = new();
    private readonly Dictionary<string, WorldItem> _itemAlias = new();

    public WeaponItem CreateWeapon(WeaponType type, string name, string alias = null) {
        var w = new WeaponItem(NextId(), name, alias, type);
        Add(w);
        return w;
    }

    private void Add(WorldItem worldItem) {
        _itemRegistry.Add(worldItem.Id, worldItem);
        if (worldItem.Alias != null) _itemAlias.Add(worldItem.Alias, worldItem);
    }

    private int NextId() {
        _lastId++;
        return _lastId;
    }

    public WorldItem Get(int id) => _itemRegistry[id];

    public T? Get<T>(int id) where T : WorldItem => _itemRegistry[id] as T;

    public WorldItem Get(string alias) => _itemAlias[alias];

    public T? Get<T>(string alias) where T : WorldItem => _itemAlias[alias] as T;

    public void Reset() {
        _lastId = 0;
        _itemRegistry.Clear();
        _itemAlias.Clear();
    }
}