using Godot;

namespace Veronenger.Game.Dungeon.World;

public class Location(Entity entity, TurnWorld world, Vector2I position) {
    private Vector2I _position = position;

    public Entity Entity { get; } = entity;
    public TurnWorld World { get; } = world;
    public WorldCell Cell => World != null ? World[Position] : null;

    public Vector2I Position {
        get => _position;
        set {
            if (_position == value) return;
            var oldPosition = _position;
            _position = value;
            World.MoveEntity(Entity, oldPosition, _position);
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

    public void Move(Vector2I offset) {
        Position += offset;
    }
}