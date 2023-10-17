using System;
using Godot;

namespace Betauer.TileSet.TileMap.Handlers.Deprecated;

public class DeprecatedRandomDeprecatedTileHandler<TTile> : IDeprecatedTileHandler<TTile> where TTile : Enum {
    public TTile Key { get; init; }
    public int SourceId { get; init; }
    public int Id { get; init; }
    public Rect2I Region;

    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y) {
        var realRegion = new Rect2I(Region.Position + new Vector2I(0, Id), Region.Size);
        var tilePos = TileMapTools.GetRandomAtlasCoordsDifferent(tileMap, realRegion, layer, x, y);
        tileMap.SetAtlasCoords(layer, SourceId, x, y, new Vector2I(tilePos.X, tilePos.Y));
    }
}
