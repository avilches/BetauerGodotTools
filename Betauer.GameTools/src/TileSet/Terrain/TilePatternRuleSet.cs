using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Betauer.TileSet.Terrain;

public class TilePatternRuleSet<T> : BaseTilePatternRuleSet<T> {
    public int? TemplateTerrain { get; set; }

    public TilePatternRuleSet(List<(T, TilePattern)> rules) : base(rules) {
        if (rules.Exists(tuple => tuple.Item2.IsTemplate)) {
            throw new Exception("Cannot create a TerrainRuleSet with template rules");
        }
    }

    public TilePatternRuleSet(List<(T, TilePattern)> rules, int templateTerrain) : base(rules) {
        TemplateTerrain = templateTerrain;
    }
    
    public void Do(TileMap.TileMap tileMap, int x, int y, Action<T> action) {
        var valueTuples = CollectionsMarshal.AsSpan(Rules);
        for (var r = 0; r < valueTuples.Length; r++) {
            var rule = valueTuples[r];
            if (TemplateTerrain.HasValue) {
                if (rule.Item2.MatchesTemplate(tileMap, TemplateTerrain.Value, x, y)) action(rule.Item1);
            } else {
                if (rule.Item2.Matches(tileMap, x, y)) action(rule.Item1);
            }
        }
    }

    public bool Matches(TileMap.TileMap tileMap, int x, int y) {
        var valueTuples = CollectionsMarshal.AsSpan(Rules);
        for (var r = 0; r < valueTuples.Length; r++) {
            var rule = valueTuples[r];
            if (TemplateTerrain.HasValue) {
                if (rule.Item2.MatchesTemplate(tileMap, TemplateTerrain.Value, x, y)) return true;
            } else {
                if (rule.Item2.Matches(tileMap, x, y)) return true;
            }
        }
        return false;
    }

    public T FindValue(TileMap.TileMap tileMap, int x, int y, T defaultValue) {
        var valueTuples = CollectionsMarshal.AsSpan(Rules);
        for (var r = 0; r < valueTuples.Length; r++) {
            var rule = valueTuples[r];
            if (TemplateTerrain.HasValue) {
                if (rule.Item2.MatchesTemplate(tileMap, TemplateTerrain.Value, x, y)) return rule.Item1;
            } else {
                if (rule.Item2.Matches(tileMap, x, y)) return rule.Item1;
            }
        }
        return defaultValue;
    }
}