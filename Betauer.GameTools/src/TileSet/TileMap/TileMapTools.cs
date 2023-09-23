using System;
using Godot;

namespace Betauer.TileSet.TileMap;

public static class TileMapTools {
    private static readonly Random Random = new Random();

    /// <summary>
    /// Returns a the tile inside the region, allow seamless tiling
    /// </summary>
    /// <param name="region"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector2I GetAtlasCoords(Rect2I region, int x, int y) {
        var atlasX = region.Position.X + x % region.Size.Y;
        var atlasY = region.Position.Y + y % region.Size.Y;
        return new Vector2I(atlasX, atlasY);
    }

    /// <summary>
    /// Returns a random tile inside the region
    /// </summary>
    /// <param name="region"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector2I GetRandomAtlasCoords(Rect2I region) {
        var atlasX = region.Position.X + Random.Next(0, region.Size.X);
        var atlasY = region.Position.Y + Random.Next(0, region.Size.Y);

        return new Vector2I(atlasX, atlasY);
    }

    /// <summary>
    /// Returns a random tile inside the region that is different from the tiles around pos (up, left and up-left only)
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="tileMap"></param>
    /// <param name="layer"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector2I GetRandomAtlasCoordsDifferent<TTile>(TileMap<TTile> tileMap, Rect2I region, int layer, int x, int y) where TTile : struct {
        var regionPos = RandomRegionPosition(region.Size);

        Vector2I atlasCoords;
        TileMap<TTile>.TileInfo tile;
        do {
            regionPos = NextPosition(regionPos, region.Size);
            atlasCoords = regionPos + region.Position;
            tile = new TileMap<TTile>.TileInfo(tileMap.GetCellInfo(layer, x, y).Type, atlasCoords);
        } while (
            (y > 0 && tile.Equals(Up(tileMap, layer, x, y))) ||
            (x > 0 && tile.Equals(Left(tileMap, layer, x, y))));
        return atlasCoords;
    }

    public static TileMap<TTile>.TileInfo Left<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x - 1, y);
    }

    public static TileMap<TTile>.TileInfo Right<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x + 1, y);
    }

    public static TileMap<TTile>.TileInfo Up<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x, y - 1);
    }

    public static TileMap<TTile>.TileInfo Down<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x, y + 1);
    }

    public static TileMap<TTile>.TileInfo UpLeft<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x - 1, y - 1);
    }

    public static TileMap<TTile>.TileInfo UpRight<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x + 1, y - 1);
    }

    public static TileMap<TTile>.TileInfo DownLeft<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x - 1, y + 1);
    }

    public static TileMap<TTile>.TileInfo DownRight<TTile>(TileMap<TTile> tileMap, int layer, int x, int y) where TTile : struct {
        return tileMap.GetCellInfo(layer, x + 1, y + 1);
    }

    private static Vector2I NextPosition(Vector2I pos, Vector2I bounds) {
        var x = pos.X + 1;
        var y = pos.Y;
        if (x >= bounds.X) {
            x = 0;
            y += 1;
        }
        if (y >= bounds.Y) {
            y = 0;
        }
        return new Vector2I(x, y);
    }

    private static Vector2I RandomRegionPosition(Vector2I bounds) {
        return new Vector2I(Random.Next(0, bounds.X), Random.Next(0, bounds.Y));
    }
}