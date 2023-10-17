using System;

namespace Betauer.TileSet.TileMap.Handlers.Deprecated;

public interface IDeprecatedTileHandler<TTile> : IDeprecatedSource where TTile : Enum {
    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y);
}