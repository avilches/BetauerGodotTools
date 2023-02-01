using System;
using Betauer.Bus;
using Betauer.DI;
using Veronenger.Character.Player;

namespace Veronenger.Managers;

[Service]
public class EventBus {

    private readonly Unicast<MainEvent> _mainBus = new();
    public void Publish(MainEvent e) => _mainBus.Publish(e);
    public Unicast<MainEvent>.EventConsumer Subscribe(Action<MainEvent> action) => _mainBus.Subscribe(action);

    private readonly Multicast<PlayerAttackEvent> _attackBus = new();
    public void Publish(PlayerAttackEvent e) => _attackBus.Publish(e);
    public Multicast<PlayerAttackEvent>.EventConsumer Subscribe(Action<PlayerAttackEvent> action) => _attackBus.Subscribe(action);

    private readonly Multicast<IHudEvent> _hudBus = new();
    public void Publish(IHudEvent e) => _hudBus.Publish(e);
    public Multicast<IHudEvent>.EventConsumer Subscribe(Action<PlayerUpdateHealthEvent> action) => _hudBus.Subscribe(action);
}