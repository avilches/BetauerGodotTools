using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.TileSet.TileMap.Handlers;
using Godot;

namespace Betauer.TileSet.TileMap;

public static class TileMapExtensions {
    private static readonly Random Random = new Random();

    public static void Apply<TType>(this TileMap<TType> tileMap, Action<int, int> action) where TType : Enum {
        tileMap.Apply((tileMap, layer, x, y) => action(x, y));
    }

    public static void Apply<TType>(this TileMap<TType> tileMap, Action<TileMap<TType>, int, int> action) where TType : Enum {
        tileMap.Apply((tileMap, layer, x, y) => action(tileMap, x, y));
    }

    public static void Apply<TType>(this TileMap<TType> tileMap, Action<int, int, int> action) where TType : Enum {
        tileMap.Apply((tileMap, layer, x, y) => action(layer, x, y));
    }

    public static void Apply<TType>(this TileMap<TType> tileMap, Action<TileMap<TType>, int, int, int> action) where TType : Enum {
        for (var l = 0; l < tileMap.Layers; l++) {
            for (var y = 0; y < tileMap.Height; y++) {
                for (var x = 0; x < tileMap.Width; x++) {
                    action(tileMap, l, x, y);
                }
            }
        }
    }

    public static void Apply<TType>(this TileMap<TType> tileMap, ITileHandler handler) where TType : Enum {
        tileMap.Apply((x, y) => handler.Apply(tileMap, x, y));
    }

    public static void Apply<TType>(this TileMap<TType> tileMap, params ITileHandler[] handlers) where TType : Enum {
        tileMap.Apply((x, y) => {
            handlers.ForEach(handler => {
                handler.Apply(tileMap, x, y);
            });
        });
    }

    public static void Apply<TType>(this TileMap<TType> tileMap, IEnumerable<ITileHandler> handlers) where TType : Enum {
        tileMap.Apply((x, y) => {
            handlers.ForEach(handler => {
                handler.Apply(tileMap, x, y);
            });
        });
    }

    public static void Flush<TType>(this TileMap<TType> tileMap, global::Godot.TileMap godotTileMap) where TType : Enum {
        tileMap.Apply((layer, x, y) => {
            ref var cellInfo = ref tileMap.GetCellInfoRef(layer, x, y);
            if (!cellInfo.AtlasCoords.HasValue) return;
            godotTileMap.SetCell(layer, new Vector2I(x, y), cellInfo.SourceId, cellInfo.AtlasCoords.Value);
        });
    }
    
    public static void Smooth<TType>(this TileMap<TType> tileMap) where TType : Enum {
        var steps = 0;
        while (SmoothStep(tileMap) && steps++ < 15) {
        }
    }

    public static bool SmoothStep<TType>(this TileMap<TType> tileMap) where TType : Enum {
        var worked = false;

        tileMap.Apply((x, y) => {
            var type = tileMap.GetType(x, y);
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

        TType? GetSafeData(int x, int y, TType? def) {
            if (x < 0 || x >= tileMap.Height ||
                y < 0 || y >= tileMap.Width) return def;
            return tileMap.GetType(x, y);
        }

        void SetData(int x, int y, TType? other) {
            worked = tileMap.SetType(x, y, other) || worked;
        }
    }

}