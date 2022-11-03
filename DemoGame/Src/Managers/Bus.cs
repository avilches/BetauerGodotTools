using System;
using Betauer.Bus;
using Betauer.DI;
using Veronenger.Character.Player;

namespace Veronenger.Managers {

    public enum GlobalEvent {
        
    }
    [Service]
    public class Bus {
        private readonly Unicast<MainEvent> _mainBus = new();
        private readonly Unicast<PlayerEvent> _playerBus = new();

        public Unicast<MainEvent>.EventConsumer Subscribe(Action<MainEvent> action) => _mainBus.Subscribe(action);
        public void Publish(MainEvent @event) => _mainBus.Publish(@event);
        
        public Unicast<PlayerEvent>.EventConsumer Subscribe(Action<PlayerEvent> action) => _playerBus.Subscribe(action);
        public void Publish(PlayerEvent @event) => _playerBus.Publish(@event);

    }
}