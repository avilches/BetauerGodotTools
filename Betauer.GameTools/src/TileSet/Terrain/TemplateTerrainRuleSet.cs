using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TemplateTerrainRuleSet : BaseTerrainRuleSet {
    public TemplateTerrainRuleSet(List<(int, TilePattern)> rules) : base(rules) {
    }

    public TerrainRuleSet WithTerrain(int terrainId) {
        return new TerrainRuleSet(Rules, terrainId);
    }
}