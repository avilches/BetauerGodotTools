using System;
using Betauer.Core;

namespace Betauer.TileSet.TileMap;

public static class TileMapExtensions {
    private static readonly Random Random = new Random();


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
            if (x < 0 || x >= tileMap.Width ||
                y < 0 || y >= tileMap.Height) return def;
            return tileMap.GetTerrain(x, y);
        }

        void SetData(int x, int y, int other) {
            worked = tileMap.SetTerrain(x, y, other) || worked;
        }
    }

}