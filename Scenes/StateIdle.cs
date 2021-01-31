using Godot;
using System;

public class StateIdle : StateGround {
    public StateIdle(PlayerController player) : base(player) {
    }

    public override void Start() {
        Player.AnimateIdle();
    }

    public override void Execute() {
        if (!Player.IsOnFloor()) {
            GoToFallState();
            return;
        }

        if (XInput != 0) {
            GoToRunState();
            return;
        }

        if (Jump.Pressed) {
            GoToJumpState();
            return;
        }

        // Suelo + no salto + sin movimiento. Empujamos hacia abajo

        Player.ApplyGravity();
        Player.LimitMotion();
        Player.MoveSnapping();

    }
}