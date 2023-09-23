namespace Betauer.TileSet.Image;

public static class TileSetLayouts {
    // Godot 
    private static TileSetLayout? _blob47;
    public static TileSetLayout Blob47 => _blob47 ??= new TileSetLayout(new[,] {
        { 16, 20, 84, 80, 213,  92, 116,  87, 28, 125, 124, 112 },
        { 17, 21, 85, 81,  29, 127, 253, 113, 31, 119,  -1, 245 },
        {  1,  5, 69, 65,  23, 223, 247, 209, 95, 255, 221, 241 },
        {  0,  4, 68, 64, 117,  71, 197,  93,  7, 199, 215, 193 }
    });

    private static TileSetLayout? _wangSubset13;
    public static TileSetLayout WangSubset13 => _wangSubset13 ??= new TileSetLayout(new [,] {
        { 28, 124, 112, 247, 223 },
        { 31, 255, 241, 253, 127 },
        {  7, 199, 193,  -1,  -1 }
    });
}