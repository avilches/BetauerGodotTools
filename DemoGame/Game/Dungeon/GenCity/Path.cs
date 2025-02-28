using System;
using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Path(Intersection start, Vector2I direction) : ICityTile {
    public Vector2I Direction { get; } = direction;

    public Intersection? End = null;
    public Intersection Start = start;

    private Vector2I _cursor = start.Position;

    public int GetLength() {
        var (a, b) = (Start.Position, End?.Position ?? _cursor);
        return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
    }

    public bool IsCompleted() {
        return End != null;
    }

    public Vector2I GetCursor() {
        return _cursor;
    }

    public void SetCursor(Vector2I position) {
        if (position == _cursor) {
            return;
        }
        if (!Start.Position.SameDirection(Direction, position)) {
            throw new ArgumentException("Cursor is not in the same line as the path direction");
        }
        _cursor = position;
    }

    public void SetEnd(Intersection end) {
        if (!Start.Position.SameDirection(Direction, end.Position)) {
            throw new ArgumentException("End intersection is not in the same line as the path direction");
        }
        end.AddInputPath(this);
        End = end;
        _cursor = end.Position;
    }

    public IEnumerable<Vector2I> GetPositions() {
        var length = GetLength();
        var position = Start.Position;
        for (var i = 0; i <= length; i++) {
            yield return position;
            position += Direction;
        }
    }

    public override string ToString() {
        var endText = End != null ? $" to {End.Position}" : " (incomplete)";
        return $"Path from {Start.Position}{endText}, Direction: {Direction}, Length: {GetLength()}";
    }
}