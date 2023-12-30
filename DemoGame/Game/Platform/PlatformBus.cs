using System;
using Betauer.Bus;
using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game.Platform.Character.Npc;
using Veronenger.Game.Platform.Character.Player;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform;

[Singleton]
public class PlatformBus {

    private readonly Multicast<PlayerAttackEvent> _attackBus = new();
    private readonly Multicast<PlayerInventoryChangeEvent> _inventoryBus = new();
    private readonly Multicast<PlayerHealthEvent> _healthBush = new();
    private readonly Multicast<PlayerDropEvent> _dropBus = new();
    private readonly Multicast<PlatformEvent> _events = new();
    
    public void Publish(PlayerAttackEvent e) => _attackBus.Publish(e);
    public void Publish(PlayerInventoryChangeEvent e) => _inventoryBus.Publish(e);
    public void Publish(PlayerHealthEvent e) => _healthBush.Publish(e);
    public void Publish(PlayerDropEvent e) => _dropBus.Publish(e);
    
    public void SpawnZombie() => _events.Publish(PlatformEvent.SpawnZombie);
    
    public EventConsumer<PlayerAttackEvent> Subscribe(Action<PlayerAttackEvent> action) => _attackBus.Subscribe(action);
    public EventConsumer<PlayerInventoryChangeEvent> Subscribe(Action<PlayerInventoryChangeEvent> action) => _inventoryBus.Subscribe(action);
    public EventConsumer<PlayerHealthEvent> Subscribe(Action<PlayerHealthEvent> action) => _healthBush.Subscribe(action);
    public EventConsumer<PlayerDropEvent> Subscribe(Action<PlayerDropEvent> action) => _dropBus.Subscribe(action);
    public EventConsumer<PlatformEvent> Subscribe(Action<PlatformEvent> action) => _events.Subscribe(action);

    public void Clear() {
        _attackBus.Dispose();
        _inventoryBus.Dispose();
        _healthBush.Dispose();
        _events.Dispose();
    }
}

public enum PlatformEvent { SpawnZombie }
public enum PlayerInventoryEventType { Add, Remove, Equip, Unequip, SlotAmountUpdate, Refresh }
public record struct PlayerDropEvent(PickableGameObject Item, Vector2 GlobalPosition, Vector2 DropVelocity);
public record struct PlayerInventoryChangeEvent(Inventory Inventory, PlayerInventoryEventType Type, PickableGameObject PickableGameObject);
public record struct PlayerHealthEvent(PlayerNode PlayerNode, float FromHealth, float ToHealth, float Max);
public record struct PlayerAttackEvent(PlayerNode Player, NpcNode NpcNode, WeaponGameObject Weapon);