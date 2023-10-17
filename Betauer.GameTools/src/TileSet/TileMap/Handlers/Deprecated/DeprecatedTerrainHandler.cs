using System;
using System.Collections.Generic;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Godot;

namespace Betauer.TileSet.TileMap.Handlers.Deprecated;

public class DeprecatedDeprecatedTileTerrainHandler<TTile> : IDeprecatedTileHandler<TTile> where TTile : Enum {
    public int SourceId { get; init; }
    public int TerrainId { get; init; }
    
    public List<TerrainRule> Rules { get; init; }

    public Texture2D GetTexture(global::Godot.TileMap godotTileMap) {
        var t = godotTileMap.TileSet.GetSource(SourceId);
        return ((TileSetAtlasSource)t).Texture;
    }
    private readonly int[,] _buffer = new int[3,3];

    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y) {
        // tileMap.TypeGrid.CopyGrid(x - 1, y - 1, 3, 3, _buffer, tileInfo => tileInfo.ToInt(), -1);
        // tileMap.TileInfoGrid[layer].CopyGrid(x - 1, y - 1, 3, 3, _buffer, tileInfo => tileInfo.TerrainId, -1);
        for (var r = 0; r < Rules.Count; r++) {
            var rule = Rules[r];
            // if (!rule.Check(_buffer, TerrainId)) continue;
            tileMap.SetTileId(layer, x, y, rule.TileId);
            var (atlasX, atlasY) = TileSetLayouts.Blob47Godot.GetTilePositionById(rule.TileId);
            tileMap.SetAtlasCoords(layer, SourceId, x, y, new Vector2I(atlasX, atlasY));
            return;
        }
    }
}