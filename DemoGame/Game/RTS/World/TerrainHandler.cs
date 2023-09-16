using System;
using Betauer.TileSet;
using Godot;

namespace Veronenger.Game.RTS.World;

public class TerrainHandler<TTile> : ITileHandler<TTile> where TTile : struct, Enum {
    public TTile Key { get; init; }
    public int SourceId { get; init; }

    public int Id { get; init; }

    public Rect2I Region = new Rect2I(7, 4, 7, 4);

    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y) {
        var realRegion = new Rect2I(Region.Position + new Vector2I(0, Id), Region.Size);
        var tilePos = TileTools.GetRandomAtlasCoordsDifferent(tileMap, realRegion, layer, x, y);
        tileMap.SetAtlasCoords(layer, x, y, new Vector2I(tilePos.X, tilePos.Y));
    }
}