using System.Collections;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public abstract class BaseTerrainRuleSet : IReadOnlyList<(int, TilePattern)> {
    protected List<(int, TilePattern)> Rules { get; }
    public int Count => Rules.Count;

    protected BaseTerrainRuleSet(List<(int, TilePattern)> rules) {
        Rules = rules;
    }
    
    public (int, TilePattern) this[int i] => Rules[i];

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public IEnumerator<(int, TilePattern)> GetEnumerator() {
        return Rules.GetEnumerator();
    }
}