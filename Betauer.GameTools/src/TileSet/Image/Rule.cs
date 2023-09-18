using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Image;

public class Rule : BaseRule {
    private readonly Action<TileSetImage> _action;

    public Rule(int tileId, Action<TileSetImage> action) {
        TileId = tileId;
        Dependencies = Array.Empty<int>();
        _action = action;
    }

    public Rule(int tileId, IEnumerable<int> dependencies, Action<TileSetImage> action) {
        TileId = tileId;
        Dependencies = dependencies;
        _action = action;
    }

    public override void Apply(TileSetImage tileSet) {
        _action(tileSet);
    }
}