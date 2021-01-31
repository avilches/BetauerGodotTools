using Godot;
using System;

public class StateFall : StateAir {
    public StateFall(PlayerController player) : base(player) {
    }

    public override void Start() {
        if (Player.IsOnFloor()) {
            throw new Exception("Invalid change!");
        }
    }

    public override void Execute() {
        if (Motion.y > PlayerConfig.START_FALLING_SPEED) {
            Player.AnimateFall();
        }

        Player.AddLateralMovement(XInput, PlayerConfig.ACCELERATION, PlayerConfig.AIR_RESISTANCE,
            PlayerConfig.STOP_IF_SPEED_IS_LESS_THAN, 0);
        Player.Flip(XInput);

        bool wasOnFloor = Player.IsOnFloor();
        Player.ApplyGravity();
        Player.LimitMotion();
        Player.Slide();

        CheckLanding();
    }
}