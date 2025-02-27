using System;
using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Path(Intersection intersection, int direction) : ICityTile {
    public int Direction { get; private set; } = direction;

    private readonly List<Building> _buildings = [];
    private Intersection? _nodeEnd = null;
    private Vector2I _cursor = intersection.Position;

    public List<Building> GetBuildings() {
        return _buildings;
    }

    private (Vector2I beg, Vector2I end) GetPositions() {
        return (intersection.Position, _nodeEnd?.Position ?? _cursor);
    }

    public bool IsCompleted() {
        return _nodeEnd != null;
    }

    public Vector2I GetNextCursor() {
        Vector2I shift = Utils.GetShift(Direction);
        return new Vector2I(_cursor.X + shift.X, _cursor.Y + shift.Y);
    }

    public Vector2I GetCursor() {
        return _cursor;
    }

    public void SetCursor(Vector2I position) {
        _cursor = position;
    }

    public Intersection GetStart() {
        return intersection;
    }

    public Intersection? GetNodeEnd() {
        return _nodeEnd;
    }

    public void SetNodeEnd(Intersection intersection) {
        intersection.AddInputPath(this);
        _nodeEnd = intersection;
        _cursor = intersection.Position;
    }

    public Building AddBuilding(List<Vector2I> vertices) {
        Building building = new Building(this, vertices);
        _buildings.Add(building);
        return building;
    }

    public float GetLength() {
        var positions = GetPositions();
        return (float)Math.Sqrt(
            Math.Pow(positions.beg.X - positions.end.X, 2) +
            Math.Pow(positions.beg.Y - positions.end.Y, 2)
        );
    }

    public void Remove() {
        intersection.RemoveOutputPath(this);
        if (_nodeEnd != null) {
            _nodeEnd.RemoveInputPath(this);
        }
    }

    public IEnumerable<Vector2I> Each() {
        var (x, y) = Utils.GetShift(Direction);
        var length = GetLength();
        var position = new Vector2I(intersection.Position.X, intersection.Position.Y);

        for (var i = 0; i <= length; i++) {
            yield return position;
            position.X += x;
            position.Y += y;
        }
    }
}