namespace Betauer.TileSet.TileMap.Handlers;

public class TerrainFilter : ITileFilter {
    public int Terrain { get; }

    public TerrainFilter(int terrain) {
        Terrain = terrain;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return tileMap.GetTerrain(x, y) == Terrain;
    }
}