using Godot;
using System;

public class StateJump : PlayerState {
    public StateJump(PlayerController playerController) : base(playerController) {
    }

    public override void Execute() {
        var motion = Motion;
        if (XInput != 0) {
            _playerController.SetMotionX(Motion.x + XInput * PlayerConfig.ACCELERATION * Delta);
        } else {
            _playerController.SetMotionX(Motion.x * PlayerConfig.FRICTION);
        }

        _playerController.ApplyGravity();
        _playerController.LimitMotion();
        _playerController.MoveSnapping();

        if (XInput == 0 && Mathf.Abs(Motion.x) < PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN) {
            _playerController.SetMotionX(0);
            _playerController.GoToIdleState();
        }
    }
}