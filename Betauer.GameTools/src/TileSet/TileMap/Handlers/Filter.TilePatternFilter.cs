using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TilePatternFilter : ITileFilter {
    public TilePattern TilePattern { get; }

    public TilePatternFilter(TilePattern tilePattern) {
        TilePattern = tilePattern;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return TilePattern.Matches(tileMap, x, y);
    }
}