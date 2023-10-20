namespace Betauer.TileSet.TileMap.Handlers;

public interface ITileFilter  : ITilePipe {
    public bool Filter(TileMap tileMap, int x, int y);
}