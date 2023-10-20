using Betauer.TileSet.Image;

namespace Betauer.TileSet.TileMap;

public class TileMapSource {
    public TileMap TileMap { get; }
    public int SourceId { get; }
    public ITileSetLayout TileSetLayout { get; }

    public TileMapSource(TileMap tileMap, int sourceId, ITileSetLayout tileSetLayout) {
        TileMap = tileMap;
        SourceId = sourceId;
        TileSetLayout = tileSetLayout;
    }

    public void SetAtlasCoords(int layer, int x, int y, int tileId) {
        TileMap.SetAtlasCoords(layer, this, x, y, tileId);
    }
}