namespace Betauer.TileSet.Image;

public static class TileSetLayouts {
    public static readonly TileSetLayout Blob47Godot = new TileSetLayout(new[,] {
        { 16, 20, 84, 80, 213, 92, 116, 87, 28, 125, 124, 112 },
        { 17, 21, 85, 81, 29, 127, 253, 113, 31, 119, -1, 245 },
        { 1, 5, 69, 65, 23, 223, 247, 209, 95, 255, 221, 241 },
        { 0, 4, 68, 64, 117, 71, 197, 93, 7, 199, 215, 193 }
    });

    public static readonly TileSetLayout Blob47TileSetter = new TileSetLayout(new[,] {
        { 28, 124, 112, 16, 20, 116, 92, 80, 84, 221, -1 },
        { 31, 255, 241, 17, 23, 247, 223, 209, 215, 119, -1 },
        { 7, 199, 193, 1, 29, 253, 127, 113, 125, 93, 117 },
        { 4, 68, 64, 0, 5, 197, 71, 65, 69, 87, 213 },
        { -1, -1, -1, -1, 21, 245, 95, 81, 85, -1, -1 }
    });

    public static readonly TileSetLayout WangSubset13 = new TileSetLayout(new[,] {
        { 28, 124, 112, 247, 223 },
        { 31, 255, 241, 253, 127 },
        { 7, 199, 193, -1, -1 }
    });
}