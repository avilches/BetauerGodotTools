using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TemplateTilePatternRuleSet<T> {
    public List<(T, TilePattern)> Rules { get; }
    public TemplateTilePatternRuleSet(List<(T, TilePattern)> rules) {
        Rules = rules;
    }

    public TilePatternRuleSet<T> WithTerrain(int terrain) {
        return new TilePatternRuleSet<T>(Rules, terrain);
    }
}