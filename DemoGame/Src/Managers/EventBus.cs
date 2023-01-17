using System;
using Betauer.Bus;
using Betauer.DI;
using Godot;
using Veronenger.Character;
using Veronenger.Character.Enemy;
using Veronenger.Character.Player;

namespace Veronenger.Managers; 

public class PlayerAttack {
    public readonly PlayerNode Player;
    public readonly Area2D Enemy;
    public readonly MeleeWeapon Weapon;

    public PlayerAttack(PlayerNode player, Area2D enemy, MeleeWeapon weapon) {
        Player = player;
        Enemy = enemy;
        Weapon = weapon;
    }
}


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