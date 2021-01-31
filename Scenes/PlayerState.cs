using Godot;
using System;

public abstract class PlayerState : State {

    protected readonly PlayerController Player;

    protected PlayerState(PlayerController player) {
        Player = player;
    }

    public void Debug(bool flag, string message) {
        if (flag) {
            Debug(message);
        }
    }

    public void Debug(string message) {
        GD.Print("#" + Player.GetFrame() + ": " + GetType().FullName + " | " + message);
    }


    protected float XInput => Player.PlayerActions.LateralMotion.Strength;
    protected ActionState Jump => Player.PlayerActions.Jump;
    protected ActionState Attack => Player.PlayerActions.Attack;
    protected float Delta => Player.Delta;
    protected Vector2 Motion => Player.Motion;
    protected PlayerConfig PlayerConfig => Player.PlayerConfig;

    protected void GoToRunState() {
        // Change to run is immediate
        Player.ChangeStateTo(typeof(StateRun));
    }

    protected void GoToIdleState() {
        // Idle is deferred to the next frame
        Player.SetNextState(typeof(StateIdle));
    }

    protected void GoToFallState() {
        // Idle is deferred to the next frame
        Player.SetNextState(typeof(StateFall));
    }

    protected void GoToJumpState() {
        // Idle is deferred to the next frame
        Player.SetNextState(typeof(StateJump));
    }

}
