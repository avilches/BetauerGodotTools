using System;

namespace Betauer.TileSet.TileMap.Handlers;

public class TileFilter : ITileFilter {
    private readonly Func<TileMap, int, int, bool> _tileFilter;

    public TileFilter(Func<TileMap, int, int, bool> tileFilter) {
        _tileFilter = tileFilter;
    }
    
    public TileFilter(Func<int, int, bool> tileHandler) {
        _tileFilter = (t, x, y) => tileHandler(x, y);
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return _tileFilter(tileMap, x, y);
    }
}