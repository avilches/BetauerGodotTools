using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TerrainRuleSet : BaseTerrainRuleSet {
    public int? TemplateTerrain { get; set; }

    public TerrainRuleSet(List<(int, TilePattern)> rules) : base(rules) {
        if (rules.Exists(tuple => tuple.Item2.IsTemplate)) {
            throw new Exception("Cannot create a TerrainRuleSet with template rules");
        }
    }

    public TerrainRuleSet(List<(int, TilePattern)> rules, int templateTerrain) : base(rules) {
        TemplateTerrain = templateTerrain;
    }
    
    public void Do(TileMap.TileMap tileMap, int x, int y, Action<int> action) {
        for (var r = 0; r < Rules.Count; r++) {
            var rule = this[r];
            if (TemplateTerrain.HasValue) {
                if (rule.Item2.MatchesTemplate(tileMap, TemplateTerrain.Value, x, y)) action(rule.Item1);
            } else {
                if (rule.Item2.Matches(tileMap, x, y)) action(rule.Item1);
            }
        }
    }

    public int FindTileId(TileMap.TileMap tileMap, int x, int y) {
        for (var r = 0; r < Rules.Count; r++) {
            var rule = this[r];
            if (TemplateTerrain.HasValue) {
                if (rule.Item2.MatchesTemplate(tileMap, TemplateTerrain.Value, x, y)) return rule.Item1;
            } else {
                if (rule.Item2.Matches(tileMap, x, y)) return rule.Item1;
            }
        }
        return - 1;
    }
}