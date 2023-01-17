using System;
using Betauer.Bus;
using Betauer.DI;
using Veronenger.Character;
using Veronenger.Character.Enemy;
using Veronenger.Character.Player;

namespace Veronenger.Managers;

[Service]
public class EventBus {
    private readonly Unicast<MainEvent> _mainBus = new();
    private readonly Multicast<PlayerAttack> _attackBus = new();

    public void Publish(PlayerAttack e) => _attackBus.Publish(e);
    public void Publish(MainEvent e) => _mainBus.Publish(e);

    public Multicast<PlayerAttack>.EventConsumer Subscribe(Action<PlayerAttack> action) => _attackBus.Subscribe(action);
    public Unicast<MainEvent>.EventConsumer Subscribe(Action<MainEvent> action) => _mainBus.Subscribe(action);

        
    // private readonly Unicast<PlayerEvent> _playerBus = new();
    // public Unicast<PlayerEvent>.EventConsumer Subscribe(Action<PlayerEvent> action) => _playerBus.Subscribe(action);
    // public void Publish(PlayerEvent e) => _playerBus.Publish(e);

}