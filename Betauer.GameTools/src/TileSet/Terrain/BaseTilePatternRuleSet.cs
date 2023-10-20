using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public abstract class BaseTilePatternRuleSet<T> {
    public List<(T, TilePattern)> Rules { get; }

    protected BaseTilePatternRuleSet(List<(T, TilePattern)> rules) {
        Rules = rules;
    }
}