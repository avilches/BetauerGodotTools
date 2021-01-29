using Godot;
using System;

public class StateIdle : PlayerState {
    public StateIdle(PlayerController playerController) : base(playerController) {
    }

    public override void Execute() {
        if (XInput != 0 && _playerController.IsOnFloor()) {
            _playerController.GoToRunState();
            return;
        }
        var mine = (Jump.JustPressed+" "+Jump.JustReleased+" "+Jump.Pressed);
        var godot = (Input.IsActionJustPressed("ui_select")+" "+Input.IsActionJustReleased("ui_select")+" "+Input.IsActionPressed("ui_select"));
        if (!mine.Equals(godot)) {
            GD.Print("Mine:"+mine);
            GD.Print("Godo:"+godot);
        }

        // Fall
        _playerController.ApplyGravity();
        _playerController.LimitMotion();
        _playerController.Slide();

    }
        
}
