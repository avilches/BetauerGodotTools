using System;
using Betauer.Bus;
using Betauer.DI.Attributes;
using Veronenger.Game.Platform.Character.Player;

namespace Veronenger.Game.Platform;

[Singleton]
public class PlatformBus {
    private readonly Multicast<PlayerAttackEvent> _attackBus = new();
    private readonly Multicast<PlayerInventoryChangeEvent> _inventoryBus = new();
    private readonly Multicast<PlayerHealthEvent> _healthBush = new();
    
    public void Publish(PlayerAttackEvent e) => _attackBus.Publish(e);
    public void Publish(PlayerInventoryChangeEvent e) => _inventoryBus.Publish(e);
    public void Publish(PlayerHealthEvent e) => _healthBush.Publish(e);
    
    public Multicast<PlayerAttackEvent>.EventConsumer Subscribe(Action<PlayerAttackEvent> action) => _attackBus.Subscribe(action);
    public Multicast<PlayerInventoryChangeEvent>.EventConsumer Subscribe(Action<PlayerInventoryChangeEvent> action) => _inventoryBus.Subscribe(action);
    public Multicast<PlayerHealthEvent>.EventConsumer Subscribe(Action<PlayerHealthEvent> action) => _healthBush.Subscribe(action);

    public void Clear() {
        _attackBus.Dispose();
        _inventoryBus.Dispose();
        _healthBush.Dispose();
    }
}