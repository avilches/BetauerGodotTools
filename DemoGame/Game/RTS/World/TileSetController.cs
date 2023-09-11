using System;
using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.RTS.World;

public class TileSetController<TTile> where TTile : Enum {
    internal class TileData {
        internal readonly TTile Tile;

        public TileData(TTile tile) {
            Tile = tile;
        }
    }

    internal readonly TileData[,,] Data;
    internal readonly int Layers;
    internal readonly int Size;
    internal readonly TileMap TileMap;

    private readonly Dictionary<TTile, ITilePattern<TTile>> _tilePatterns = new();

    public Random Random { get; set; }

    public TileSetController(TileMap tileMap, int layers, int size, Random random) {
        TileMap = tileMap;
        Size = size;
        Random = random;
        Layers = layers;
        Data = new TileData[layers, size, size];
    }

    public void Add(TTile key, ITilePattern<TTile> tilePattern) {
        _tilePatterns[key] = tilePattern;
    }
	
    public void Set(int layer, Vector2I pos, TTile tile) {
        Data[layer, pos.X, pos.Y] = new TileData(tile);
    }

    public void Smooth(int layer) {
        var steps = 0;
        while (SmoothStep(layer) && steps ++ < 15) {
        }
    }

    public bool SmoothStep(int layer) {
        var worked = false;
        void SetData(int x, int y, TileData data) {
            if (Data[layer, x, y].Tile.ToInt() == data.Tile.ToInt()) return;
            Data[layer, x, y] = new TileData(data.Tile);
            worked = true;
        }

        TileData GetData(int x, int y, TileData def) {
            if (x < 0 || x >= Size ||
                y < 0 || y >= Size) return def;
            return Data[layer, x, y];
        }
		
        for (var y = 0; y < Size; y++) {
            for (var x = 0; x < Size; x++) {
                var data = Data[layer, x, y];
                var left = GetData(x - 1, y, data);
                var right = GetData(x + 1, y, data);
                var up = GetData(x, y - 1, data);
                var down = GetData(x, y + 1, data);
                bool equalLat = left.Tile.ToInt() == right.Tile.ToInt();
                bool equalVer = up.Tile.ToInt() == down.Tile.ToInt();
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
    }

    /// <summary>
    /// Write in the TileMap the tiles defined in controller
    /// </summary>
    public void Flush() {
        for (var layer = 0; layer < Layers; layer += 1) {
            for (var y = 0; y < Size; y++) {
                for (var x = 0; x < Size; x++) {
                    var data = Data[layer, x, y];
                    if (data != null) {
                        var tilePattern = _tilePatterns[data.Tile];
                        if (tilePattern.SourceId >= 0) {
                            var pos = new Vector2I(x, y);
                            tilePattern.Apply(TileMap, layer, pos);
                        }
                    }
                }
            }
        }
    }

    public void Clear() {
        TileMap.Clear();
    }
}