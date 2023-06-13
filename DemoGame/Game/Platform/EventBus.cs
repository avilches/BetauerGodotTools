using System;
using Betauer.Bus;
using Betauer.DI.Attributes;
using Veronenger.Game.Character.Player;

namespace Veronenger.Game.Platform;

[Singleton]
public class EventBus {
    private readonly Multicast<PlayerAttackEvent> _attackBus = new();
    public void Publish(PlayerAttackEvent e) => _attackBus.Publish(e);
    public Multicast<PlayerAttackEvent>.EventConsumer Subscribe(Action<PlayerAttackEvent> action) => _attackBus.Subscribe(action);
}