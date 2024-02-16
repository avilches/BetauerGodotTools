using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TemplateTilePatternRuleSet {
    public List<(int, TilePattern)> Rules { get; }
    public TemplateTilePatternRuleSet(List<(int, TilePattern)> rules) {
        Rules = rules;
    }

    public TilePatternRuleSet WithTerrain(int terrain) {
        return new TilePatternRuleSet(Rules, terrain);
    }
}