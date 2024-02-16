using System;
using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TilePatternRuleSetFilter : ITileFilter {
    public TilePatternRuleSet TilePatternRuleSet { get; }

    public TilePatternRuleSetFilter(TilePatternRuleSet tilePatternRuleSet) {
        TilePatternRuleSet = tilePatternRuleSet;
    }

    public bool Filter(TileMap tileMap, int x, int y) {
        return TilePatternRuleSet.MatchAnyRule(tileMap, x, y);
    }
}