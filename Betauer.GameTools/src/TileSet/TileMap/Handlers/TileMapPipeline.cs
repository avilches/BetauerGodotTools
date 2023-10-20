using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Betauer.Core;
using Betauer.TileSet.Terrain;

namespace Betauer.TileSet.TileMap.Handlers;

public class TileMapPipeline {
    private readonly List<ITilePipe> _tilePipe = new();
    private readonly TileMap _tileMap;

    public TileMapPipeline(TileMap tileMap) {
        _tileMap = tileMap;
    }

    public TileMapPipeline Do(Action<TileMap, int, int> tileHandler) {
        Do(new TileHandler(tileHandler));
        return this;
    }

    public TileMapPipeline Do(Action<int, int> tileHandler) {
        Do(new TileHandler(tileHandler));
        return this;
    }

    public TileMapPipeline Do(ITileHandler handler) {
        _tilePipe.Add(handler);
        return this;
    }

    public TileMapPipeline Filter(Func<TileMap, int, int, bool> filter) {
        Filter(new TileFilter(filter));
        return this;
    }

    public TileMapPipeline Filter(Func<int, int, bool> filter) {
        Filter(new TileFilter(filter));
        return this;
    }

    public TileMapPipeline FilterTerrain<T>(T terrain) where T : Enum {
        if (_tileMap is not TileMap<T> tileMapGen) throw new Exception($"TileMap is not of type {typeof(TileMap<T>).GetTypeName()}>");
        return FilterTerrain(tileMapGen.EnumToTerrain(terrain));
    }

    public TileMapPipeline FilterTerrain(int terrain) {
        Filter(new TerrainFilter(terrain));
        return this;
    }

    public TileMapPipeline FilterTileId(int layer, int terrain) {
        Filter(new TileIdFilter(layer, terrain));
        return this;
    }

    public TileMapPipeline Filter(TilePattern tilePattern) {
        Filter(new TilePatternFilter(tilePattern));
        return this;
    }

    public TileMapPipeline Filter<T>(TilePatternRuleSet<T> tilePatternRuleSet) where T : Enum {
        Filter(new TilePatternRuleSetFilter<T>(tilePatternRuleSet));
        return this;
    }

    public TileMapPipeline Filter(ITileFilter filter) {
        _tilePipe.Add(filter);
        return this;
    }

    public void Apply() {
        Console.WriteLine("Pipeline Apply");
        int calls = 0;
        _tileMap.Apply((map, x, y) => {
            calls++;
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
        });
        Console.WriteLine("Pipeline Apply calls: "+calls+ " / "+_tileMap.Width*_tileMap.Height);
    }
}