using System;

namespace Betauer.TileSet.TileMap.Handlers;

public class TileHandler : ITileHandler {
    private readonly Action<TileMap, int, int> _tileHandler;

    public TileHandler(Action<TileMap, int, int> tileHandler) {
        _tileHandler = tileHandler;
    }
    
    public TileHandler(Action<int, int> tileHandler) {
        _tileHandler = (t, x, y) => tileHandler(x, y);
    }

    public void Apply(TileMap tileMap, int x, int y) {
        _tileHandler(tileMap, x, y);
    }
}