using System.Collections.Generic;
using Godot;

namespace Betauer.TileSet.Image;

public class TileSetLayout : ITileSetLayout {
    
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    protected readonly Dictionary<int, Vector2I> Tiles = new();
    protected int[,] Positions;
    
    protected TileSetLayout() {
    }

    public TileSetLayout(int[,] layout) {
        _Load(layout);
    }

    protected void _Load(int[,] layout) {
        Tiles.Clear();
        Positions = layout;
        Height = layout.GetLength(0);
        Width = layout.GetLength(1);
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var tileId = layout[y, x];
                if (tileId >= 0) {
                    Tiles[tileId] = new Vector2I(x, y);
                }
            }
        }
    }

    public ICollection<int> GetTileIds() {
        return Tiles.Keys;
    }

    public bool HasTile(int tileId) {
        return Tiles.ContainsKey(tileId);
    }

    public Vector2I GetAtlasCoordsByTileId(int tileId) {
        return Tiles.TryGetValue(tileId, out var position) ? position : new Vector2I(-1, -1);
    }

    public int GetTileIdByPosition(int x, int y) {
        return Positions[y, x];
    }

    public int[,] Export() {
        var positions = new int[Height, Width];
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                positions[y, x] = Positions[y, x];
            }
        }
        return positions;
    }
}