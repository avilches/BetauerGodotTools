using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TilePatternSet<T, TT> {
    public List<(T, TilePattern<TT>)> Patterns { get; private set; }
    public Dictionary<string, Func<TT, bool>> DefaultRules { get; init; }

    public TilePatternSet<T, TT> Add(T id, TilePattern<TT> pattern) {
        if (Patterns == null) Patterns = new List<(T, TilePattern<TT>)>();
        Patterns.Add((id, pattern));
        return this;
    }
    
    public TilePatternSet<T, TT> Add(T id, string pattern) {
        return Add(id, TilePattern.Parse(pattern, DefaultRules));
    }

    /// <summary>
    /// Data must be a grid of the same size as the pattern and indexed by [y,x] (you can use Array2D<TT>.Data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public T? FindTilePatternId(TT[,] data, T? defaultValue = default) { 
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) return rule.Item1;
        }
        return defaultValue;
    }
}