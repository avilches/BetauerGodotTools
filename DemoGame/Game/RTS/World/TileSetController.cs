using System;
using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.RTS.World;

public class TileSetController<TTile> where TTile : struct, Enum {

    internal readonly TTile?[,,] Data;
    internal readonly int Layers;
    internal readonly int Width;
    internal readonly int Height;

    private readonly Dictionary<TTile, ITilePattern<TTile>> _tilePatterns = new();

    public Random Random { get; set; }

    public TileSetController(int layers, int width, int height, Random random) {
        Width = width;
        Height = height;
        Random = random;
        Layers = layers;
        Data = new TTile?[layers, width, height];
    }

    public void Add(TTile key, ITilePattern<TTile> tilePattern) {
        _tilePatterns[key] = tilePattern;
    }

    public T Get<T>(TTile key) where T : class, ITilePattern<TTile>{
        return _tilePatterns[key] as T;
    }

    public void Set(int layer, Vector2I pos, TTile? tile) => Set(layer, pos.X, pos.Y, tile);

    public void Set(int layer, int x, int y, TTile? tile) => Data[layer, x, y] = tile;

    public void Remove(int layer, Vector2I pos) => Remove(layer, pos.X, pos.Y);

    public void Remove(int layer, int x, int y) => Data[layer, x, y] = null;

    public void Smooth(int layer) {
        var steps = 0;
        while (SmoothStep(layer) && steps ++ < 15) {
        }
    }

    public bool SmoothStep(int layer) {
        var worked = false;

        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var data = Data[layer, x, y];
                var left = GetData(x - 1, y, data);
                var right = GetData(x + 1, y, data);
                var up = GetData(x, y - 1, data);
                var down = GetData(x, y + 1, data);
                var equalLat = Compare(left, right);
                var equalVer = Compare(up, down);
                if (equalLat && equalVer) {
                    SetData(x, y, Random.NextBool() ? left : up);
                } else if (equalLat) {
                    SetData(x, y, left);
                } else if (equalVer) {
                    SetData(x, y, up);
                }
            }
        }
        return worked;

        TTile? GetData(int x, int y, TTile? def) {
            if (x < 0 || x >= Height ||
                y < 0 || y >= Width) return def;
            return Data[layer, x, y];
        }

        bool Compare(TTile? left, TTile? right) {
            return (left == null && right == null) || (left?.Equals(right) ?? false);
        }

        void SetData(int x, int y, TTile? left) {
            if (Compare(Data[layer, x, y], left)) return;
            Data[layer, x, y] = left;
            worked = true;
        }
    }

    /// <summary>
    /// Write in the TileMap the tiles defined in controller
    /// </summary>
    public void Flush(TileMap tileMap) {
        for (var layer = 0; layer < Layers; layer += 1) {
            for (var y = 0; y < Height; y++) {
                for (var x = 0; x < Width; x++) {
                    var tile = Data[layer, x, y];
                    if (tile.HasValue) {
                        var tilePattern = _tilePatterns[tile.Value];
                        if (tilePattern.SourceId >= 0) {
                            var pos = new Vector2I(x, y);
                            tilePattern.Apply(tileMap, layer, pos);
                        }
                    }
                }
            }
        }
    }
}