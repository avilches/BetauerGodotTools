using System;
using Betauer.TileSet.Image;

namespace Betauer.TileSet.TileMap.Handlers;

/**
 * Read the TileIf from every cell and set the AtlasCoords from the TileSetLayout
 */
public class SetAtlasCoordsFromTileSetLayoutHandler : ITileHandler {
    public TileSetLayout TileSetLayout { get; }
    public int Layer { get; } = 0;
    public int SourceId { get; } = 0;

    public SetAtlasCoordsFromTileSetLayoutHandler(int layer, int sourceId, TileSetLayout tileSetLayout) {
        Layer = layer;
        TileSetLayout = tileSetLayout;
        SourceId = sourceId;
    }

    public void Apply<TType>(TileMap<TType> tileMap, int x, int y) where TType : Enum {
        var tileId = tileMap.GetCellInfoRef(Layer, x, y).TileId;
        if (tileId == -1) return;
        var atlasCoords = TileSetLayout.GetTilePositionById(tileId);
        tileMap.SetAtlasCoords(Layer, SourceId, x, y, atlasCoords);
    }
}