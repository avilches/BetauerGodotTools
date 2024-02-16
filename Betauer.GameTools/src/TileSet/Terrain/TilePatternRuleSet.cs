using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Betauer.Core;

namespace Betauer.TileSet.Terrain;

public class TilePatternRuleSet {
    public int? TemplateTerrain { get; set; }
    public List<(int, TilePattern)> Rules { get; }

    public TilePatternRuleSet(List<(int, TilePattern)> rules, int? templateTerrain = null) {
        Rules = rules;
        TemplateTerrain = templateTerrain;
    }
    
    public void Do(TileMap.TileMap tileMap, int x, int y, Action<int> action) {
        foreach (var rule in Rules) {
            if (rule.Item2.Matches(tileMap.TerrainGrid, x, y, TemplateTerrain)) action(rule.Item1);
        }
    }

    public bool MatchAnyRule(TileMap.TileMap tileMap, int x, int y) {
        foreach (var rule in Rules) {
            if (rule.Item2.Matches(tileMap.TerrainGrid, x, y, TemplateTerrain)) return true;
        }
        return false;
    }

    public int FindRuleId(TileMap.TileMap tileMap, int x, int y, int defaultValue) {
        foreach (var rule in Rules) {
            if (rule.Item2.Matches(tileMap.TerrainGrid, x, y, TemplateTerrain)) return rule.Item1;
        }
        return defaultValue;
    }
}