using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

/**
 * Set the TileId based on the TerrainRuleSet
 */
public class SetTileIdFromTerrainHandler : ITileHandler {
    public TilePatternRuleSet<int> TilePatternRuleSet { get; }
    public int Layer { get; } = 0;

    public SetTileIdFromTerrainHandler(int layer, TilePatternRuleSet<int> tilePatternRuleSet) {
        TilePatternRuleSet = tilePatternRuleSet;
        Layer = layer;
    }

    public void Apply(TileMap tileMap, int x, int y) {
        var tileId = TilePatternRuleSet.FindValue(tileMap, x, y, int.MinValue);
        if (tileId != int.MinValue) tileMap.SetTileId(Layer, x, y, tileId);
    }
}