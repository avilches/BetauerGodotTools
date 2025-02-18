using Godot;

namespace Veronenger.Game.Dungeon.World;

public class Location(EntityBase entity, WorldMap worldMap, Vector2I position) {
    private Vector2I _position = position;

    public EntityBase EntityBase { get; } = entity;
    public WorldMap WorldMap { get; } = worldMap;
    public WorldCell Cell => WorldMap != null ? WorldMap[Position] : null;

    public Vector2I Position {
        get => _position;
        set {
            if (_position == value) return;
            var oldPosition = _position;
            _position = value;
            WorldMap.MoveEntity(EntityBase, oldPosition, _position);
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