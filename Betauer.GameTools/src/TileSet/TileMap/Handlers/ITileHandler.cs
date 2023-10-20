namespace Betauer.TileSet.TileMap.Handlers;

public interface ITileHandler : ITilePipe {
    public void Apply(TileMap tileMap, int x, int y);
}