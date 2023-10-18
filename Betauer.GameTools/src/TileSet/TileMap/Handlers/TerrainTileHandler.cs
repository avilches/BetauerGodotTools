using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TerrainTileHandler : ITileHandler {
    private readonly SetTileIdFromTerrainHandler _setTileIdFromTerrainHandler;
    private readonly SetAtlasCoordsFromTileSetLayoutHandler _setAtlasCoordsFromTileSetLayoutHandler;
    private readonly int _terrainId;

    public TerrainTileHandler(int layer, int terrainId, TemplateTerrainRuleSet terrainRuleSet, int sourceId, ITileSetLayout tileSetLayout) : 
        this(layer, terrainId, terrainRuleSet.WithTerrain(terrainId), sourceId, tileSetLayout) {
    }

    public TerrainTileHandler(int layer, int terrainId, TerrainRuleSet terrainRuleSet, int sourceId, ITileSetLayout tileSetLayout) {
        _terrainId = terrainId;
        _setTileIdFromTerrainHandler = new SetTileIdFromTerrainHandler(layer, terrainRuleSet);
        _setAtlasCoordsFromTileSetLayoutHandler = new SetAtlasCoordsFromTileSetLayoutHandler(layer, sourceId, tileSetLayout);
    }

    public void Apply(TileMap tileMap, int x, int y) {
        if (tileMap.GetTerrain(x, y) != _terrainId) {
            return;
        }
        _setTileIdFromTerrainHandler.Apply(tileMap, x, y);
        _setAtlasCoordsFromTileSetLayoutHandler.Apply(tileMap, x, y);
    }
}