using System.Collections.Generic;

namespace Betauer.TileSet.Image;

public abstract class BaseRule {
    public int TileId;
    public IEnumerable<int>? Dependencies;
    public abstract void Apply(TileSetImage tileSet);
}