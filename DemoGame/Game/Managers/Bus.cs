using System;
using Betauer.Bus;
using Betauer.DI;
using Veronenger.Character.Player;

namespace Veronenger.Managers {

    public enum GlobalEvent {
        
    }
    [Service]
    public class Bus {
        private readonly Unicast<MainTransition> _mainBus = new();
        private readonly Unicast<PlayerTransition> _playerBus = new();

        public Unicast<MainTransition>.EventConsumer Subscribe(Action<MainTransition> action) => _mainBus.Subscribe(action);
        public void Publish(MainTransition transition) => _mainBus.Publish(transition);
        
        public Unicast<PlayerTransition>.EventConsumer Subscribe(Action<PlayerTransition> action) => _playerBus.Subscribe(action);
        public void Publish(PlayerTransition transition) => _playerBus.Publish(transition);

    }
}