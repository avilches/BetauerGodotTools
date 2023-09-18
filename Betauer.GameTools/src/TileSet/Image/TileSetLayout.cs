using System.Collections.Generic;

namespace Betauer.TileSet.Image;

public class TileSetLayout {
    
    public readonly struct TilePosition {
        public readonly int TileId;
        public readonly int X;
        public readonly int Y;

        public TilePosition(int tileId, int x, int y) {
            TileId = tileId;
            X = x;
            Y = y;
        }
    }

    public int Columns { get; protected set; }
    public int Rows { get; protected set; }
    protected readonly Dictionary<int, TilePosition> Tiles = new();
    protected int[,] Positions;
    
    protected TileSetLayout() {
    }

    public TileSetLayout(int[,] layout) {
        _Load(layout);
    }

    protected void _Load(int[,] layout) {
        Tiles.Clear();
        Positions = layout;
        Rows = layout.GetLength(0);
        Columns = layout.GetLength(1);
        for (var y = 0; y < Rows; y++) {
            for (var x = 0; x < Columns; x++) {
                var tileId = layout[y, x];
                if (tileId >= 0) {
                    Tiles[tileId] = new TilePosition(tileId, x, y);
                }
            }
        }
    }

    public IReadOnlyCollection<TilePosition> GetTiles() {
        return Tiles.Values;
    }

    public IReadOnlyCollection<int> GetTileIds() {
        return Tiles.Keys;
    }

    public bool HasTile(int tileId) {
        return Tiles.ContainsKey(tileId);
    }

    public (int, int) GetTilePositionById(int tileId) {
        var tile = Tiles[tileId];
        return (tile.X, tile.Y);
    }

    public int GetTileIdByPosition(int x, int y) {
        return Positions[y, x];
    }

    public int[,] Export() {
        var positions = new int[Rows, Columns];
        for (var y = 0; y < Rows; y++) {
            for (var x = 0; x < Columns; x++) {
                positions[y, x] = Positions[y, x];
            }
        }
        return positions;
    }

}