using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TemplateTilePatternRuleSet<T> : BaseTilePatternRuleSet<T> {
    public TemplateTilePatternRuleSet(List<(T, TilePattern)> rules) : base(rules) {
    }

    public TilePatternRuleSet<T> WithTerrain(int terrain) {
        return new TilePatternRuleSet<T>(Rules, terrain);
    }
}