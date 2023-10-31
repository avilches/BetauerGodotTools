using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap.Handlers;
using Godot;

namespace Betauer.TileSet.TileMap;

public static class TileMapExtensions {
    private static readonly Random Random = new Random();

    public static void Execute(this TileMap tileMap, Action<TileMap, int, int> action) {
        for (var y = 0; y < tileMap.Height; y++) {
            for (var x = 0; x < tileMap.Width; x++) {
                action(tileMap, x, y);
            }
        }
    }

    public static void Execute(this TileMap tileMap, Action<TileMap, int, int, int> action) {
        for (var layer = 0; layer < tileMap.Layers; layer++) {
            for (var y = 0; y < tileMap.Height; y++) {
                for (var x = 0; x < tileMap.Width; x++) {
                    action(tileMap, layer, x, y);
                }
            }
        }
    }

    public static void Execute(this TileMap tileMap, ITileHandler handler) {
        tileMap.Execute(handler.Apply);
    }

    public static void Execute(this TileMap tileMap, params ITileHandler[] handlers) {
        handlers.ForEach(handler => tileMap.Execute(handler.Apply));
    }

    public static void Execute(this TileMap tileMap, IEnumerable<ITileHandler> handlers) {
        handlers.ForEach(handler => tileMap.Execute(handler.Apply));
    }

    public static void DumpAtlasCoordsTo(this TileMap tileMap, global::Godot.TileMap godotTileMap) {
        tileMap.Execute((t, layer, x, y) => {
            ref var cellInfo = ref tileMap.GetCellInfoRef(layer, x, y);
            if (!cellInfo.AtlasCoords.HasValue) return;
            godotTileMap.SetCell(layer, new Vector2I(x, y), cellInfo.SourceId, cellInfo.AtlasCoords.Value);
        });
    }

    public static TileActionList If(this TileMap tileMap, Func<TileMap, int, int, bool> filter) {
        return tileMap.CreateTileActionList().If(filter);
    }

    public static TileActionList If(this TileMap tileMap, Func<int, int, bool> filter) {
        return tileMap.CreateTileActionList().If(filter);
    }

    public static TileActionList If(this TileMap tileMap, ITileFilter filter) {
        return tileMap.CreateTileActionList().If(filter);
    }

    public static TileActionList IfTerrain<T>(this TileMap<T> tileMap, T terrain) where T : Enum {
        return tileMap.CreateTileActionList().IfTerrain(terrain);
    }

    public static TileActionList IfTerrain(this TileMap tileMap, int terrain) {
        return tileMap.CreateTileActionList().IfTerrain(terrain);
    }

    public static TileActionList IfTileId(this TileMap tileMap, int tileId) {
        return tileMap.CreateTileActionList().IfTileId(tileId);
    }

    public static TileActionList IfPattern(this TileMap tileMap, TilePattern tilePattern) {
        return tileMap.CreateTileActionList().IfPattern(tilePattern);
    }

    public static TileActionList IfPatternRuleSet<T>(this TileMap<T> tileMap, TilePatternRuleSet<T> tilePatternRuleSet) where T : Enum {
        return tileMap.CreateTileActionList().IfPatternRuleSet(tilePatternRuleSet);
    }

    public static void Smooth(this TileMap tileMap) {
        var steps = 0;
        while (SmoothStep(tileMap) && steps++ < 15) {
        }
    }

    public static bool SmoothStep(this TileMap tileMap) {
        var worked = false;

        tileMap.Execute((t, x, y) => {
            var type = tileMap.GetTerrain(x, y);
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

        int GetSafeData(int x, int y, int def) {
            if (x < 0 || x >= tileMap.Height ||
                y < 0 || y >= tileMap.Width) return def;
            return tileMap.GetTerrain(x, y);
        }

        void SetData(int x, int y, int other) {
            worked = tileMap.SetTerrain(x, y, other) || worked;
        }
    }

}