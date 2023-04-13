using System;
using Betauer.Nodes;
using Godot;

namespace Betauer.Application.Monitor;
public class Speedometer2D {
    public Vector2 SpeedVector { get; private set; } = Vector2.Zero;
    public float Speed { get; private set; } = 0f;
    public float MaxSpeed { get; private set; } = 0f;
    private Vector2 _prevPosition = Vector2.Zero;
    public Func<Vector2> Provider;
    private IEventHandler? _nodeEvent;

    public static Speedometer2D From(Func<Vector2> provider) {
        return new Speedometer2D(provider);
    }

    public static Speedometer2D Position(Node2D node2D) {
        return new Speedometer2D(() => node2D.Position).UpdateOnPhysicsProcess(node2D);
    }

    public static Speedometer2D GlobalPosition(Node2D node2D) {
        return new Speedometer2D(() => node2D.GlobalPosition).UpdateOnPhysicsProcess(node2D);;
    }

    public Speedometer2D(Func<Vector2> provider) {
        Provider = provider;
    }

    public Speedometer2D UpdateOnPhysicsProcess(Node node) {
        _nodeEvent?.Destroy();
        _nodeEvent = node.OnPhysicsProcess(Update);
        return this;
    }

    public Speedometer2D UpdateOnProcess(Node node) {
        _nodeEvent?.Destroy();
        _nodeEvent = node.OnProcess(Update);
        return this;
    }

    public void Update(double delta) {
        var position = Provider.Invoke();
        SpeedVector = (_prevPosition - position) / (float)delta;
        Speed = SpeedVector.Length();
        _prevPosition = position;
        MaxSpeed = Math.Max(MaxSpeed, Speed);
    }

    public string GetInfo(string format = "000.00") {
        return $"{SpeedVector.ToString(format)} {Speed.ToString(format)}";
    }
}