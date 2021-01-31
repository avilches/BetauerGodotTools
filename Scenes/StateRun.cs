using Godot;
using System;

public class StateRun : StateGround {
    public StateRun(PlayerController player) : base(player) {
    }

    public override void Start() {
        Player.AnimateRun();
    }

    public override void Execute() {
        if (!Player.IsOnFloor()) {
            GoToFallState();
            return;
        }

        if (XInput == 0 && Motion.x == 0) {
            GoToIdleState();
            return;
        }

        if (Jump.Pressed) {
            GoToJumpState();
            return;
        }

        // Suelo + no salto + movimiento/inercia. Movemos lateralmente y empujamos hacia abajo

        Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.FRICTION,
            PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);

        Player.Flip(XInput);
        Player.ApplyGravity();
        Player.LimitMotion();
        Player.MoveSnapping();


    }
}