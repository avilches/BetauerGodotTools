using Betauer.DI.Attributes;
using Godot;
using Veronenger.Game;
using Veronenger.Game.Config;
using static Godot.Mathf;

namespace Veronenger.Game.Worlds.Animation; 

public partial class RotatingPlatformNode : CharacterBody2D {

    [Export] public Vector2 Radius = new Vector2(50, 50);
    [Export] public float RotationDuration = 4.0f;
    [Inject] public PlatformConfig PlatformConfig { get; set;}
    private float _angle = 0;
    private float _speed;

    public override void _Ready() {
        Configure();
    }

    public override void _PhysicsProcess(double delta) {
        UpdateAngle(delta);
        UpdatePosition();
    }

    private void Configure() {
        _speed = Tau / RotationDuration;
    }

    private void UpdateAngle(double delta) {
        _angle = Wrap(_angle + _speed * (float)delta, 0, Tau); // # Infinite rotation(in radians)
    }

    private void UpdatePosition() {
        var x = Sin(_angle) * Radius.X;
        var y = Cos(_angle) * Radius.Y;
        Position = new Vector2(x, y);
    }

}