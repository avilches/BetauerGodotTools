using System;
using Godot;

namespace Veronenger.Game.Dungeon.World;

public class Location(TurnWorld world, Entity entity, Vector2I position) {
    private Vector2I _position = position;

    public TurnWorld World { get; } = world;
    public Entity Entity { get; } = entity;

    public Func<Vector2I, bool> CanMove { get; set; } = (_) => true;
    public event Action<Vector2I, Vector2I>? OnMoved;

    public Location(TurnWorld world, Entity entity, int x, int y) : this(world, entity, new Vector2I(x, y)) {}

    public bool TryMove(Vector2I offset) {
        if (!CanMove.Invoke(_position + offset)) return false;
        Move(offset);
        return true;
    }

    public void Move(Vector2I offset) {
        Position += offset;
    }

    public Vector2I Position {
        get => _position;
        set {
            if (_position == value) return;
            var oldPosition = _position;
            _position = value;
            OnMoved?.Invoke(oldPosition, _position);
            World.UpdatedEntityPosition(Entity, oldPosition);
        }
    }

    public int X {
        get => _position.X;
        set => Position = new Vector2I(value, _position.Y);
    }

    public int Y {
        get => _position.Y;
        set => Position = new Vector2I(_position.X, value);
    }
}