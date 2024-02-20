using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TilePatternRuleSet<T> {
    public int? TemplateTerrain { get; set; }
    public List<(T, TilePattern)> Rules { get; }

    public TilePatternRuleSet(List<(T, TilePattern)> rules, int? templateTerrain = null) {
        Rules = rules;
        TemplateTerrain = templateTerrain;
    }
    
    public void Do(int[,] data, Action<T> action) {
        foreach (var rule in Rules) {
            if (rule.Item2.Matches(data, TemplateTerrain)) action(rule.Item1);
        }
    }

    public bool MatchAnyRule(int[,] data) {
        foreach (var rule in Rules) {
            if (rule.Item2.Matches(data, TemplateTerrain)) return true;
        }
        return false;
    }

    public T? FindRuleId(int[,] data, T? defaultValue = default) { 
        foreach (var rule in Rules) {
            if (rule.Item2.Matches(data, TemplateTerrain)) return rule.Item1;
        }
        return defaultValue;
    }
}