namespace Betauer.TileSet.TileMap.Handlers;

public class TileIdFilter : ITileFilter {
    public int TileId { get; }
    public int Layer { get; }

    public TileIdFilter(int layer, int tileId) {
        Layer = layer;
        TileId = tileId;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return tileMap.GetCellInfoRef(Layer, x, y).TileId == TileId;
    }
}