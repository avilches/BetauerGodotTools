using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

/**
 * Set the TileId based on the TerrainRuleSet
 */
public class SetTileIdFromTerrainHandler : ITileHandler {
    public TilePatternRuleSet<int> TilePatternRuleSet { get; }

    public SetTileIdFromTerrainHandler(TilePatternRuleSet<int> tilePatternRuleSet) {
        TilePatternRuleSet = tilePatternRuleSet;
    }

    public void Apply(TileMap tileMap, int x, int y) {
        var tileId = TilePatternRuleSet.FindValue(tileMap, x, y, int.MinValue);
        if (tileId != int.MinValue) tileMap.SetTileId(x, y, tileId);
    }
}