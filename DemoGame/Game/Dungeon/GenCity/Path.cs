using System;
using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Path(Intersection start, Vector2I direction) : ICityTile {
    public Vector2I Direction { get; private set; } = direction;

    public readonly List<Building> Buildings = [];

    public Intersection? End = null;
    public Intersection Start = start;

    private Vector2I _cursor = start.Position;

    public int GetLength() {
        var (a, b) = GetPositions();
        return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
    }

    private (Vector2I beg, Vector2I end) GetPositions() {
        return (Start.Position, End?.Position ?? _cursor);
    }

    public bool IsCompleted() {
        return End != null;
    }

    public Vector2I GetNextCursor() {
        return _cursor + Direction;
    }

    public Vector2I GetCursor() {
        return _cursor;
    }

    public void SetCursor(Vector2I position) {
        _cursor = position;
    }

    public void SetEnd(Intersection end) {
        end.AddInputPath(this);
        End = end;
        _cursor = end.Position;
    }

    public Building AddBuilding(List<Vector2I> vertices) {
        Building building = new Building(this, vertices);
        Buildings.Add(building);
        return building;
    }

    public void Remove() {
        Start.RemoveOutputPath(this);
        End?.RemoveInputPath(this);
    }

    public IEnumerable<Vector2I> Each() {
        var length = GetLength();
        var position = Start.Position;
        for (var i = 0; i <= length; i++) {
            yield return position;
            position += Direction;
        }
    }
}