using Godot;
using System;

public abstract class PlayerState : State {

    protected readonly PlayerController _playerController;

    public PlayerState(PlayerController playerController) {
        _playerController = playerController;
    }

    protected float XInput => _playerController.lateralMotion.Strength;
    protected ActionState Jump => _playerController.Jump;
    protected ActionState Attack => _playerController.Attack;
    protected float Delta => _playerController.Delta;
    protected Vector2 Motion => _playerController.Motion;
    protected PlayerConfig PlayerConfig => _playerController.playerConfig;
}
