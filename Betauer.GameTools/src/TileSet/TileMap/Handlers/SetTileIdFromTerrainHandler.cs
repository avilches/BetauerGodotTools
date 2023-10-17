using System;
using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

/**
 * Set the TileId based on the TerrainRuleSet
 */
public class SetTileIdFromTerrainHandler : ITileHandler {
    public TerrainRuleSet TerrainRuleSet { get; }
    public int Layer { get; } = 0;

    public SetTileIdFromTerrainHandler(int layer, TerrainRuleSet terrainRuleSet) {
        TerrainRuleSet = terrainRuleSet;
        Layer = layer;
    }

    public void Apply<TType>(TileMap<TType> tileMap, int x, int y) where TType : Enum {
        var tileId = TerrainRuleSet.FindTileId(tileMap, x, y);
        if (tileId != -1) tileMap.SetTileId(Layer, x, y, tileId);
    }
}