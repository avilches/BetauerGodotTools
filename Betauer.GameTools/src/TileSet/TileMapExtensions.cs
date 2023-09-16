using System;
using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet;

public static class TileMapExtensions {
    private static readonly Random Random = new Random();

    public static void Smooth<TTileType>(this TileMap<TTileType> tileMap, int layer) where TTileType : struct {
        var steps = 0;
        while (SmoothStep(tileMap, layer) && steps++ < 15) {
        }
    }

    public static bool SmoothStep<TTileType>(this TileMap<TTileType> tileMap, int layer) where TTileType : struct {
        var worked = false;

        tileMap.LoopCells((x, y) => {
            var type = tileMap.GetCellInfo(layer, x, y).Type;
            var left = GetSafeData(x - 1, y, type);
            var right = GetSafeData(x + 1, y, type);
            var up = GetSafeData(x, y - 1, type);
            var down = GetSafeData(x, y + 1, type);
            var equalLat = left.Equals(right);
            var equalVer = up.Equals(down);
            if (equalLat && equalVer) {
                SetData(x, y, Random.NextBool() ? left : up);
            } else if (equalLat) {
                SetData(x, y, left);
            } else if (equalVer) {
                SetData(x, y, up);
            }
        });
        return worked;

        TTileType? GetSafeData(int x, int y, TTileType? def) {
            if (x < 0 || x >= tileMap.Height ||
                y < 0 || y >= tileMap.Width) return def;
            return tileMap.GetCellInfo(layer, x, y).Type;
        }

        void SetData(int x, int y, TTileType? other) {
            worked = tileMap.SetType(layer, x, y, other) || worked;
        }
    }

    public static void Apply<TTile>(this TileMap<TTile> tileMap, Dictionary<TTile, ITileHandler<TTile>> handlers) where TTile : struct {
        for (var layer = 0; layer < tileMap.Layers; layer++) {
            Apply(tileMap, handlers, layer);
        }
    }

    public static void Apply<TTile>(this TileMap<TTile> tileMap, Dictionary<TTile, ITileHandler<TTile>> handlers, int layer) where TTile : struct {
        for (var y = 0; y < tileMap.Height; y++) {
            for (var x = 0; x < tileMap.Width; x++) {
                var tile = tileMap.GetCellInfo(layer, x, y).Type;
                if (tile.HasValue) {
                    var tileHandler = handlers[tile.Value];
                    tileHandler.Apply(tileMap, layer, x, y);
                }
            }
        }
    }

    public static void Flush<TTile>(this TileMap<TTile> tileMap, Dictionary<TTile, ISource> handlers, TileMap godotTileMap) where TTile : struct {
        tileMap.LoopLayerCells( (layer, x, y) => {
            var cellInfo = tileMap.GetCellInfo(layer, x, y);
            if (cellInfo.Type.HasValue && cellInfo.AtlasCoords.HasValue) {
                var tileRectAtlasConfiguration = handlers[cellInfo.Type.Value];
                godotTileMap.SetCellsTerrainConnect();
                godotTileMap.SetCell(layer, new Vector2I(x, y), tileRectAtlasConfiguration.SourceId, cellInfo.AtlasCoords.Value);
            }
        });
        
    }

    public static void LoopLayerCells<TTile>(this TileMap<TTile> tileMap, Action<int, int, int> action) where TTile : struct {
        for (var layer = 0; layer < tileMap.Layers; layer += 1) {
            for (var y = 0; y < tileMap.Height; y++) {
                for (var x = 0; x < tileMap.Width; x++) {
                    action(layer, x, y);
                }
            }
        }
    }

    public static void LoopCells<TTile>(this TileMap<TTile> tileMap, Action<int, int> action) where TTile : struct {
        for (var y = 0; y < tileMap.Height; y++) {
            for (var x = 0; x < tileMap.Width; x++) {
                action(x, y);
            }
        }
    }
}