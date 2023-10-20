using System;
using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TilePatternRuleSetFilter<T> : ITileFilter where T : Enum {
    public TilePatternRuleSet<T> TilePatternRuleSet { get; }

    public TilePatternRuleSetFilter(TilePatternRuleSet<T> tilePatternRuleSet) {
        TilePatternRuleSet = tilePatternRuleSet;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return TilePatternRuleSet.Matches(tileMap, x, y);
    }
}