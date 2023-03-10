using System.Collections.Generic;
using Betauer.DI;
using Betauer.DI.Factory;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Config;

namespace Veronenger.Persistent;

[Service]
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

    public WeaponMeleeItem AddMeleeWeapon(WeaponConfig.Melee config, string name, float damageBase, string alias = null) =>
        Add(new WeaponMeleeItem(NextId(), name, alias, config, damageBase));
    
    public WeaponRangeItem AddRangeWeapon(WeaponConfig.Range config, string name, float damageBase, string alias = null) =>
        Add(new WeaponRangeItem(NextId(), name, alias, config, damageBase));

    public NpcItem AddEnemy(NpcConfig config, string name, INpcItemNode npcItemNode, string alias = null) {
        var item = new NpcItem(NextId(), name, null, npcItemNode, config);
        npcItemNode.OnAddToWorld(this, item);
        return Add(item);
    }

    /*
     * Items
     */
    public Item Get(int id) => _itemRegistry[id];

    public Item? GetOrNull(int id) => _itemRegistry.TryGetValue(id, out var r) ? r : null; 

    public T GetFromMeta<T>(GodotObject godotObject) where T : Item => (T)_itemRegistry[godotObject.GetItemIdFromMeta()];
    
    public T Get<T>(int id) where T : Item => (T)_itemRegistry[id];

    public T? GetOrNull<T>(int id) where T : Item => _itemRegistry.TryGetValue(id, out var r) ? r as T : null; 

    public Item Get(string alias) => _itemAlias[alias];

    public Item? GetOrNull(string alias) => _itemAlias.TryGetValue(alias, out var r) ? r : null; 

    public T Get<T>(string alias) where T : Item => (T)_itemAlias[alias];

    public T? GetOrNull<T>(string alias) where T : Item => _itemAlias.TryGetValue(alias, out var r) ? r as T : null; 

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
    private static readonly StringName WorldId = "__WorldId";
    
    public static int GetItemIdFromMeta(this GodotObject o) => 
        o.GetMeta(WorldId).AsInt32();

    public static void LinkMetaToItemId(this GodotObject o, Item item) => 
        o.SetMeta(WorldId, item.Id);

    public static bool HasMetaItemId(this GodotObject o) => 
        o.HasMeta(WorldId);
}