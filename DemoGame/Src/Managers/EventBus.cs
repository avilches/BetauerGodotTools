using System;
using Betauer.Bus;
using Betauer.DI;
using Betauer.DI.Attributes;
using Veronenger.Character.Player;

namespace Veronenger.Managers;

[Singleton]
public class EventBus {

    private readonly Unicast<MainEvent> _mainBus = new();
    public void Publish(MainEvent e) => _mainBus.Publish(e);
    public Unicast<MainEvent>.EventConsumer Subscribe(Action<MainEvent> action) => _mainBus.Subscribe(action);

    private readonly Multicast<PlayerAttackEvent> _attackBus = new();
    public void Publish(PlayerAttackEvent e) => _attackBus.Publish(e);
    public Multicast<PlayerAttackEvent>.EventConsumer Subscribe(Action<PlayerAttackEvent> action) => _attackBus.Subscribe(action);
}