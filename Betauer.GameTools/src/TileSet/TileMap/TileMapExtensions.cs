using System;
using Betauer.Core;

namespace Betauer.TileSet.TileMap;

public static class TileMapExtensions {
    public static void Smooth(this TileMap tileMap, int seed = 0) {
        var steps = 0;
        var random = new Random(seed);
        while (SmoothStep(tileMap, random) && steps++ < 15) {
        }
    }

    public static bool SmoothStep(this TileMap tileMap, Random random) {
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
                SetData(x, y, random.NextBool() ? left : up);
            } else if (equalLat) {
                SetData(x, y, left);
            } else if (equalVer) {
                SetData(x, y, up);
            }
        });
        return worked;

        int GetSafeData(int x, int y, int def) {
            if (x < 0 || x >= tileMap.Width ||
                y < 0 || y >= tileMap.Height) return def;
            return tileMap.GetTerrain(x, y);
        }

        void SetData(int x, int y, int other) {
            worked = tileMap.SetTerrain(x, y, other) || worked;
        }
    }

}