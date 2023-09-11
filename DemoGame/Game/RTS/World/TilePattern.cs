using System;
using Godot;

namespace Veronenger.Game.RTS.World;

public interface ITilePattern<TTile> where TTile : Enum {
    public TTile Key { get; init; }
    public int SourceId { get; init; }
    public void Apply(TileMap tileMap, int layer, Vector2I pos);
}

public class TilePattern<TTile> : ITilePattern<TTile> 
    where TTile : Enum {
    public TTile Key { get; init; }
    public int SourceId { get; init; }
    public Rect2I AtlasCoords { get; init; }

    public void Apply(TileMap tileMap, int layer, Vector2I pos) {
        var x = AtlasCoords.Position.X + pos.X % AtlasCoords.Size.X;
        var y = AtlasCoords.Position.Y + pos.Y % AtlasCoords.Size.Y;
        tileMap.SetCell(layer, pos, SourceId, new Vector2I(x, y));
    }
}