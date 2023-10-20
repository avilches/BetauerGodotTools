using System.Collections.Generic;
using Godot;

namespace Betauer.TileSet.Image;

public interface ITileSetLayout {
    public IReadOnlyCollection<int> GetTileIds();
    public bool HasTile(int tileId);
    public Vector2I GetAtlasCoordsByTileId(int tileId);
    public int GetTileIdByPosition(int x, int y);
    public int Width { get; }
    public int Height { get; }
    public int[,] Export();
}