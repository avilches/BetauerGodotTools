using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Building : ICityTile {
    public List<Vector2I> Vertices { get; }
    public Vector2I Position { get; }
    public int Width { get; }
    public int Height { get;  }
    public Path Path { get; private set; }

    public byte GetDirectionFlags() {
        return 0;
    }

    public Building(Path path, List<Vector2I> vertices) {
        if (vertices.Count != 4) {
            throw new ArgumentException("Invalid building vertices");
        }

        Path = path;
        Vertices = vertices;

        var minX = vertices.Min(pos => pos.X);
        var minY = vertices.Min(pos => pos.Y);
        var maxX = vertices.Max(pos => pos.X);
        var maxY = vertices.Max(pos => pos.Y);

        Position = new Vector2I(minX, minY);
        Width = maxX - minX + 1;
        Height = maxY - minY + 1;
    }

    public IEnumerable<Vector2I> GetPositions() {
        for (var x = 0; x < Width; x++) {
            for (var y = 0; y < Height; y++) {
                yield return Position + new Vector2I(x, y);
            }
        }
    }
}