using System;

namespace Betauer.TileSet.TileMap;

public class TileMapData<TType, TTileData> : TileMap<TType> where TType : Enum {
    public TTileData?[,] Data { get; private set; }
    private bool _hasData = false;

    public TileMapData(int width, int height) : base(width, height) {
    }

    public TileMapData(int layers, int width, int height) : base(layers, width, height) {
    }

    public TTileData? GetCellData(int x, int y) {
        return _hasData ? Data[x, y] : default;
    }

    public void RemoveCellData(int x, int y) {
        if (!_hasData) return;
        Data[x, y] = default;
    }

    public void SetCellData(int x, int y, TTileData? data) {
        if (!_hasData) {
            Data = new TTileData[Width, Height];
            _hasData = true;
        }
        Data[x, y] = data;
    }
}