using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TilePatternSet<T> {
    public List<(T, TilePattern)> Patterns { get; }
    
    public TilePatternSet(List<(T, TilePattern)> patterns) {
        Patterns = patterns;
    }
    
    public void Do(int[,] data, Action<T> action) {
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) action(rule.Item1);
        }
    }

    public bool MatchAny(int[,] data) {
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) return true;
        }
        return false;
    }

    public T? FindTilePatternId(int[,] data, T? defaultValue = default) { 
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) return rule.Item1;
        }
        return defaultValue;
    }
}