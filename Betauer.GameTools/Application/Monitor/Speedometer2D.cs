using System;
using Betauer.Nodes;
using Godot;
using Object = Godot.Object;

namespace Betauer.Application.Monitor {
    public class Speedometer2D {
        public Vector2 SpeedVector { get; private set; } = Vector2.Zero;
        public float Speed { get; private set; } = 0f;
        public float MaxSpeed { get; private set; } = 0f;
        private Vector2 _prevPosition = Vector2.Zero;
        public Func<Vector2> Provider;

        public static Speedometer2D From(Func<Vector2> provider) {
            return new Speedometer2D(provider);
        }

        public static Speedometer2D Position(Node2D node2D) {
            return new Speedometer2D(() => node2D.Position);
        }

        public static Speedometer2D GlobalPosition(Node2D node2D) {
            return new Speedometer2D(() => node2D.GlobalPosition);
        }

        private Speedometer2D(Func<Vector2> provider) {
            Provider = provider;
        }

        public INodeEvent UpdateOnPhysicsProcess(Node node) {
            return node.OnPhysicsProcess(Update);
        }

        public INodeEvent UpdateOnProcess(Node node) {
            return node.OnPhysicsProcess(Update);
        }

        public void Update(float delta) {
            var position = Provider.Invoke();
            SpeedVector = (_prevPosition - position) / delta;
            Speed = SpeedVector.Length();
            _prevPosition = position;
            MaxSpeed = Math.Max(MaxSpeed, Speed);
        }


    }
}