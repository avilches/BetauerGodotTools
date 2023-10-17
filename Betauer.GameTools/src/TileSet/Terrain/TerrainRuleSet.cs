using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.TileSet.Terrain;

public class TerrainRuleSet : IReadOnlyList<TerrainRule> {
    private List<TerrainRule> Rules { get; }

    public int Count => Rules.Count;

    public TerrainRuleSet(List<TerrainRule> rules) {
        Rules = rules;
    }

    public TerrainRuleSet ApplyTerrain(int terrainId) {
        return new TerrainRuleSet(Rules.Select(r => r.ApplyTerrain(terrainId)).ToList());
    }

    public void Do(TileMap.TileMap tileMap, int x, int y, Action<int> action) {
        for (var r = 0; r < Rules.Count; r++) {
            var rule = this[r];
            if (rule.Check(tileMap, x, y)) action(rule.TileId);
        }
    }

    public int FindTileId(TileMap.TileMap tileMap, int x, int y) {
        for (var r = 0; r < Rules.Count; r++) {
            var rule = this[r];
            if (rule.Check(tileMap, x, y)) return rule.TileId;
        }
        return - 1;
    }


    public TerrainRule this[int i] => Rules[i];

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public IEnumerator<TerrainRule> GetEnumerator() {
        return Rules.GetEnumerator();
    }
}