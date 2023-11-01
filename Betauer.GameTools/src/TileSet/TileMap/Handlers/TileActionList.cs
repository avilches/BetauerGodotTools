using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Betauer.Core;
using Betauer.TileSet.Terrain;
using Godot;

namespace Betauer.TileSet.TileMap.Handlers;

public class TileActionList {
    private readonly List<ITilePipe> _tilePipe = new();
    private readonly TileMap _tileMap;
    public bool Applied { get; private set; } = false;

    public TileActionList(TileMap tileMap) {
        _tileMap = tileMap;
    }

    public TileActionList Do(Action<TileMap, int, int> tileHandler) {
        Do(new TileHandler(tileHandler));
        return this;
    }

    public TileActionList Do(Action<int, int> tileHandler) {
        Do(new TileHandler(tileHandler));
        return this;
    }

    public TileActionList Do(ITileHandler handler) {
        _tilePipe.Add(handler);
        return this;
    }

    public TileActionList If(Func<TileMap, int, int, bool> filter) {
        If(new TileFilter(filter));
        return this;
    }

    public TileActionList If(Func<int, int, bool> filter) {
        If(new TileFilter(filter));
        return this;
    }

    public TileActionList IfTerrain<T>(T terrain) where T : Enum {
        if (_tileMap is not TileMap<T> tileMapGen) throw new Exception($"TileMap is not of type {typeof(TileMap<T>).GetTypeName()}>");
        return IfTerrain(tileMapGen.EnumToTerrain(terrain));
    }

    public TileActionList IfTerrain(int terrain) {
        If(new TerrainFilter(terrain));
        return this;
    }

    public TileActionList IfTileId(int tileId) {
        If(new TileIdFilter(tileId));
        return this;
    }

    public TileActionList IfPattern(TilePattern tilePattern) {
        If(new TilePatternFilter(tilePattern));
        return this;
    }

    public TileActionList IfPatternRuleSet<T>(TilePatternRuleSet<T> tilePatternRuleSet) where T : Enum {
        If(new TilePatternRuleSetFilter<T>(tilePatternRuleSet));
        return this;
    }

    public TileActionList If(ITileFilter filter) {
        _tilePipe.Add(filter);
        return this;
    }

    public void Apply() {
        if (Applied) return;
        Applied = true;
        _tileMap.Execute(_Apply);
    }

    internal void _Apply(TileMap map, int x, int y) {
        var span = CollectionsMarshal.AsSpan(_tilePipe);
        for (var idx = 0; idx < span.Length; idx++) {
            var pipe = span[idx];
            if (pipe is ITileFilter filter) {
                if (!filter.Filter(_tileMap, x, y)) {
                    return;
                }
            } else if (pipe is ITileHandler handler) {
                handler.Apply(_tileMap, x, y);
            }
        }
    }

    public TileActionList UpdateAtlasCoords(int layer, TileMapSource source, int x, int y) {
        Do((tileMap, x, y) => tileMap.UpdateAtlasCoords(layer, source, x, y));
        return this;
    }

    public TileActionList SetAtlasCoords(int layer, TileMapSource source, Vector2I atlasCoords) {
        Do((tileMap, x, y) => tileMap.SetAtlasCoords(layer, source, x, y, atlasCoords));
        return this;
    }

    public TileActionList SetAtlasCoords(int layer, TileMapSource source, int tileId) {
        Do((tileMap, x, y) => tileMap.SetAtlasCoords(layer, source, x, y, tileId));
        return this;
    }

    public TileActionList SetAtlasCoords(int layer, int sourceId, Vector2I atlasCoords) {
        Do((tileMap, x, y) => tileMap.SetAtlasCoords(layer, sourceId, x, y, atlasCoords));
        return this;
    }

    public TileActionList SetTileId(int tileId) {
        Do((tileMap, x, y) => tileMap.SetTileId(x, y, tileId));
        return this;
    }

    public TileActionList SetTerrain(int layer, int terrain) {
        Do((tileMap, x, y) => tileMap.SetTerrain(x, y, terrain));
        return this;
    }

    public TileActionList SetTerrain<T>(int layer, T terrainEnum) where T : Enum {
        if (_tileMap is not TileMap<T> tileMapGen) throw new Exception($"TileMap is not of type {typeof(TileMap<T>).GetTypeName()}>");
        var terrain = tileMapGen.EnumToTerrain(terrainEnum);
        Do((tileMap, x, y) => tileMap.SetTerrain(x, y, terrain));
        return this;
    }
}