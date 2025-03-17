using System;
using System.Collections.Generic;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class Building(int id, Path path, Rect2I bounds) : ICityTile {
    public int Id { get; } = id;
    public Path Path { get; } = path;
    public Rect2I Bounds { get; set; } = bounds;

    // Nuevas propiedades para el portal
    public Vector2I Entrance { get; set; }
    public Vector2I PathEntrance { get; set; }
    public Vector2I PathDirection { get; set; }

    public Vector2I Position => Bounds.Position;
    public Vector2I Size => Bounds.Size;
    public int Width => Bounds.Size.X;
    public int Height => Bounds.Size.Y;
    
    public byte GetDirectionFlags() {
        return 0;
    }

    public IEnumerable<Vector2I> GetPositions() {
        return Bounds.GetPositions();
    }
}