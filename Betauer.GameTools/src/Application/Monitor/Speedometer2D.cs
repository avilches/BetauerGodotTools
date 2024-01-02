using System;
using Betauer.Nodes;
using Godot;

namespace Betauer.Application.Monitor;

public abstract class Speedometer2D {
    public abstract Vector2 SpeedVector { get; }
    public abstract float Speed { get; }

    public static SpeedometerCharacter2D Velocity(CharacterBody2D characterBody2D) => new(characterBody2D);

    /// <summary>
    /// The speedometer will stop when the watcher node is freed.
    /// </summary>
    /// <param name="watcher"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static SpeedometerVector2D From(Node watcher, Func<Vector2> provider) => new(watcher, provider);

    public static SpeedometerVector2D Position(Node2D node2D) => From(node2D, () => node2D.Position);

    public static SpeedometerVector2D GlobalPosition(Node2D node2D) => From(node2D, () => node2D.GlobalPosition);
}

public class SpeedometerCharacter2D : Speedometer2D {
    private readonly CharacterBody2D _characterBody2D;

    public SpeedometerCharacter2D(CharacterBody2D characterBody2D) {
        _characterBody2D = characterBody2D;
    }

    public override Vector2 SpeedVector => _characterBody2D.GetRealVelocity();
    public override float Speed => SpeedVector.Length();
}

public class SpeedometerVector2D : Speedometer2D {
    private Vector2 _speedVector = Vector2.Zero;
    private float _speed = 0f;
    private Vector2 _prevPosition = Vector2.Zero;
    
    public override Vector2 SpeedVector => _speedVector;
    public override float Speed => _speed;
    public float MaxSpeed { get; private set; } = 0f;
    public Func<Vector2> Provider { get; }

    public SpeedometerVector2D(Node node, Func<Vector2> provider) {
        Provider = provider;
        node.OnPhysicsProcess(Update);
    }
    
    public void Update(double delta) {
        var position = Provider.Invoke();
        _speedVector = (_prevPosition - position) / (float)delta;
        _speed = SpeedVector.Length();
        _prevPosition = position;
        MaxSpeed = Math.Max(MaxSpeed, Speed);
    }
}