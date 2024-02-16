using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Betauer.Core;
using Betauer.TileSet.Terrain;
using Godot;

namespace Betauer.TileSet.TileMap.Handlers;

public class TileActionBuilder {
    private readonly List<ITilePipe> _tilePipe = new();
    private readonly TileMap _tileMap;
    public bool Applied { get; private set; } = false;

    public TileActionBuilder(TileMap tileMap) {
        _tileMap = tileMap;
    }

    public TileActionBuilder Do(Action<TileMap, int, int> tileHandler) {
        Do(new TileHandler(tileHandler));
        return this;
    }

    public TileActionBuilder Do(Action<int, int> tileHandler) {
        Do(new TileHandler(tileHandler));
        return this;
    }

    public TileActionBuilder Do(ITileHandler handler) {
        _tilePipe.Add(handler);
        return this;
    }

    public TileActionBuilder If(Func<TileMap, int, int, bool> filter) {
        If(new TileFilter(filter));
        return this;
    }

    public TileActionBuilder If(Func<int, int, bool> filter) {
        If(new TileFilter(filter));
        return this;
    }

    public TileActionBuilder IfTerrain<T>(T terrain) where T : Enum {
        if (_tileMap is not TileMap<T> tileMapGen) throw new Exception($"TileMap is not of type {typeof(TileMap<T>).GetTypeName()}>");
        return IfTerrain(terrain.ToInt());
    }

    public TileActionBuilder IfTerrain(int terrain) {
        If(new TerrainFilter(terrain));
        return this;
    }

    public TileActionBuilder IfTileId(int tileId) {
        If(new TileIdFilter(tileId));
        return this;
    }

    public TileActionBuilder IfPattern(TilePattern tilePattern) {
        If(new TilePatternFilter(tilePattern));
        return this;
    }

    public TileActionBuilder IfPattern(TilePattern tilePattern, Func<int, int, int> loader) {
        If(new TilePatternLoaderFilter(tilePattern, loader));
        return this;
    }

    public TileActionBuilder IfPatternRuleSet(TilePatternRuleSet tilePatternRuleSet) {
        If(new TilePatternRuleSetFilter(tilePatternRuleSet));
        return this;
    }

    public TileActionBuilder If(ITileFilter filter) {
        _tilePipe.Add(filter);
        return this;
    }

    public void Apply() {
        if (Applied) return;
        Applied = true;
        _tileMap.Loop(_Apply);
    }

    internal void _Apply(int x, int y) {
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

    public TileActionBuilder UpdateAtlasCoords(int layer, TileMapSource source, int x, int y) {
        Do((tileMap, x, y) => tileMap.UpdateAtlasCoords(layer, source, x, y));
        return this;
    }

    public TileActionBuilder SetAtlasCoords(int layer, TileMapSource source, Vector2I atlasCoords) {
        Do((tileMap, x, y) => tileMap.SetAtlasCoords(layer, source, x, y, atlasCoords));
        return this;
    }

    public TileActionBuilder SetAtlasCoords(int layer, TileMapSource source, int tileId) {
        Do((tileMap, x, y) => tileMap.SetAtlasCoords(layer, source, x, y, tileId));
        return this;
    }

    public TileActionBuilder SetAtlasCoords(int layer, int sourceId, Vector2I atlasCoords) {
        Do((tileMap, x, y) => tileMap.SetAtlasCoords(layer, sourceId, x, y, atlasCoords));
        return this;
    }

    public TileActionBuilder SetTileId(int tileId) {
        Do((tileMap, x, y) => tileMap.SetTileId(x, y, tileId));
        return this;
    }

    public TileActionBuilder SetTerrain(int layer, int terrain) {
        Do((tileMap, x, y) => tileMap.SetTerrain(x, y, terrain));
        return this;
    }

    public TileActionBuilder SetTerrain<T>(int layer, T terrainEnum) where T : Enum {
        if (_tileMap is not TileMap<T> tileMapGen) throw new Exception($"TileMap is not of type {typeof(TileMap<T>).GetTypeName()}>");
        var terrain = terrainEnum.ToInt();
        Do((tileMap, x, y) => tileMap.SetTerrain(x, y, terrain));
        return this;
    }
}