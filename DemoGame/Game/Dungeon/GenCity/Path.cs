using System;
using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Path(string id, Intersection start, Vector2I direction) : ICityTile {

    public string Id { get; } = id;
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
        if (!Start.Position.IsSameDirection(Direction, position)) {
            throw new ArgumentException($"Cursor is not in the same line starting from {Start.Position} to the {Direction.ToDirectionString()}");
        }
        _cursor = position;
    }

    public void SetEnd(Intersection end) {
        if (!Start.Position.IsSameDirection(Direction, end.Position)) {
            throw new ArgumentException($"End intersection is not in the same line starting from {Start.Position} to the {Direction.ToDirectionString()}");
        }
        end.AddInputPath(this);
        End = end;
        _cursor = end.Position;
    }

    public void Remove() {
        Start.RemoveOutputPath(this);
        End?.RemoveInputPath(this);
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
        var endText = End != null ? $"To Id:'{End.Id}' {End.Position}" : $" To {_cursor} (incomplete)";
        return $"Path Id:'{Id}' - Start Id:'{Start.Id}' {Start.Position} - {Direction.ToDirectionString()} - {endText}, Length: {GetLength()}";
    }

    public bool IsPerpendicular(Path other) {
        return Direction.IsPerpendicular(other.Direction);
    }

    public bool IsParallel(Path other) {
        return Direction.IsParallel(other.Direction);
    }

    public bool IsSameLine(Path other) {
        return Start.Position.IsSameLine(Direction, other.Start.Position, other.Direction);
    }
}