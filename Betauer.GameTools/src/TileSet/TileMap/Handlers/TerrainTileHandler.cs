using System;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TerrainTileHandler : ITileHandler {
    private readonly SetTileIdFromTerrainHandler _setTileIdFromTerrainHandler;
    private readonly SetAtlasCoordsFromTileSetLayoutHandler _setAtlasCoordsFromTileSetLayoutHandler;
    public TerrainTileHandler(int layer, TerrainRuleSet terrainRuleSet, int sourceId, TileSetLayout tileSetLayout) {
        _setTileIdFromTerrainHandler = new SetTileIdFromTerrainHandler(layer, terrainRuleSet);
        _setAtlasCoordsFromTileSetLayoutHandler = new SetAtlasCoordsFromTileSetLayoutHandler(layer, sourceId, tileSetLayout);
    }

    public void Apply<TType>(TileMap<TType> tileMap, int x, int y) where TType : Enum {
        _setTileIdFromTerrainHandler.Apply(tileMap, x, y);
        _setAtlasCoordsFromTileSetLayoutHandler.Apply(tileMap, x, y);
    }
}