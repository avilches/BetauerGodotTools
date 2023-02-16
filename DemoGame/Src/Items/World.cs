using System.Collections.Generic;
using Betauer.DI;
using Godot;
using Veronenger.Character.Enemy;

namespace Veronenger.Items;

[Service]
public class World {
    private int _lastId = 0;
    private int NextId() => ++_lastId;
    private readonly Dictionary<int, Item> _itemRegistry = new();
    private readonly Dictionary<string, Item> _itemAlias = new();

    public void Clear() {
        _lastId = 0;
        _itemRegistry.Clear();
        _itemAlias.Clear();
    }

    public WeaponMeleeItem CreateMeleeWeapon(WeaponConfig.Melee config, string name, string alias = null) =>
        Add(new WeaponMeleeItem(NextId(), name, alias, config));
    
    public WeaponRangeItem CreateRangeWeapon(WeaponConfig.Range model, string name, string alias = null) =>
        Add(new WeaponRangeItem(NextId(), name, alias, model));

    public EnemyItem CreateEnemy(ZombieNode zombieNode) =>
        Add(new EnemyItem(NextId(), zombieNode.Name, null, zombieNode));

    /*
     * Items
     */
    public Item Get(int id) => _itemRegistry[id];

    public T Get<T>(int id) where T : Item => (T)_itemRegistry[id];

    public Item Get(string alias) => _itemAlias[alias];

    public T Get<T>(string alias) where T : Item => (T)_itemAlias[alias];

    public void Remove(Item item) => Remove(item.Id);

    public void Remove(int id) {
        var item = _itemRegistry[id];
        if (item.Alias != null) _itemAlias.Remove(item.Alias);
        _itemRegistry.Remove(id);
    }

    private T Add<T>(T worldItem) where T : Item {
        _itemRegistry.Add(worldItem.Id, worldItem);
        if (worldItem.Alias != null) _itemAlias.Add(worldItem.Alias, worldItem);
        return worldItem;
    }
}

public static class WorldExtension {
    private const string WorldId = "__WorldId";
    public static void SetWorldId(this GodotObject o, Item item) => o.SetMeta(WorldId, item.Id);
    public static bool MatchesWorldId(this GodotObject o, Item item) => o.GetMeta(WorldId).AsInt32() == item.Id;
    public static int GetWorldId(this GodotObject o) => o.GetMeta(WorldId).AsInt32();
}