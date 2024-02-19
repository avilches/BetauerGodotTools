using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

/**
 * Set the TileId based on the TerrainRuleSet
 */
public class SetTileIdFromTerrainHandler : ITileHandler {
    public TilePatternRuleSet TilePatternRuleSet { get; }

    public SetTileIdFromTerrainHandler(TilePatternRuleSet tilePatternRuleSet) {
        TilePatternRuleSet = tilePatternRuleSet;
    }

    public void Apply(TileMap tileMap, int x, int y) {
        var tileId = TilePatternRuleSet.FindRuleId(tileMap.TerrainGrid, x, y);
        if (tileId.HasValue) tileMap.SetTileId(x, y, tileId.Value);
    }
}