namespace Betauer.TileSet.TileMap.Handlers;

public class TileIdFilter : ITileFilter {
    public int TileId { get; }

    public TileIdFilter(int tileId) {
        TileId = tileId;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return tileMap.GetTileId(x, y) == TileId;
    }
}