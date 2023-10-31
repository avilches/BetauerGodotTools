using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TerrainTileHandler : ITileHandler {
    private readonly SetTileIdFromTerrainHandler _setTileIdFromTerrainHandler;
    private readonly SetAtlasCoordsFromTileSetLayoutHandler _setAtlasCoordsFromTileSetLayoutHandler;

    public TerrainTileHandler(int layer, TilePatternRuleSet<int> tilePatternRuleSet, TileMapSource tileMapSource) {
        _setTileIdFromTerrainHandler = new SetTileIdFromTerrainHandler(tilePatternRuleSet);
        _setAtlasCoordsFromTileSetLayoutHandler = new SetAtlasCoordsFromTileSetLayoutHandler(layer, tileMapSource);
    }

    public void Apply(TileMap tileMap, int x, int y) {
        _setTileIdFromTerrainHandler.Apply(tileMap, x, y);
        _setAtlasCoordsFromTileSetLayoutHandler.Apply(tileMap, x, y);
    }
}