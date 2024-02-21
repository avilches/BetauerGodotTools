using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TilePatternSet<T, TT> {
    public List<(T, TilePattern<TT>)> Patterns { get; }
    
    public TilePatternSet(List<(T, TilePattern<TT>)> patterns) {
        Patterns = patterns;
    }
    
    public void Do(TT[,] data, Action<T> action) {
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) action(rule.Item1);
        }
    }

    public bool MatchAny(TT[,] data) {
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) return true;
        }
        return false;
    }

    public T? FindTilePatternId(TT[,] data, T? defaultValue = default) { 
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) return rule.Item1;
        }
        return defaultValue;
    }
}