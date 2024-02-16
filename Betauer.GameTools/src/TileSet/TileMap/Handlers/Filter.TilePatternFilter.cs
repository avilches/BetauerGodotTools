using System;
using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TilePatternFilter : ITileFilter {
    public TilePattern TilePattern { get; }

    public TilePatternFilter(TilePattern tilePattern) {
        TilePattern = tilePattern;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return TilePattern.Matches(tileMap.TerrainGrid, x, y);
    }
}

public class TilePatternLoaderFilter : ITileFilter {
    public TilePattern TilePattern { get; }
    public Func<int, int, int> Loader { get; }

    public TilePatternLoaderFilter(TilePattern tilePattern, Func<int, int, int> loader) {
        TilePattern = tilePattern;
        Loader = loader;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return TilePattern.Matches(Loader, x, y);
    }
}