namespace Betauer.TileSet.TileMap;

public interface ITileHandler<TTile> : ISource where TTile : struct {
    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y);
}