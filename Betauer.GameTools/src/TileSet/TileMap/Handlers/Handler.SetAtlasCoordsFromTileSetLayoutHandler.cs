namespace Betauer.TileSet.TileMap.Handlers;

/**
 * Read the TileIf from every cell and set the AtlasCoords from the ITileSetLayout
 */
public class SetAtlasCoordsFromTileSetLayoutHandler : ITileHandler {
    public int Layer { get; } = 0;
    public TileMapSource TileMapSource { get; }

    public SetAtlasCoordsFromTileSetLayoutHandler(int layer, TileMapSource tileMapSource) {
        Layer = layer;
        TileMapSource = tileMapSource;
    }

    public void Apply(TileMap tileMap, int x, int y) {
        var tileId = tileMap.GetCellInfoRef(Layer, x, y).TileId;
        if (tileId == -1) return;
        var atlasCoords = TileMapSource.TileSetLayout.GetAtlasCoordsByTileId(tileId);
        tileMap.SetAtlasCoords(Layer, TileMapSource.SourceId, x, y, atlasCoords);
    }
}