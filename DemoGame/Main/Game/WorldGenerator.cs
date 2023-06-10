using System;
using Godot;

namespace Veronenger.Main.Game; 

public class WorldGenerator {
    private TileSetController _tile;

    public void Load(TileMap tileMap) {
    }

    public void Generate(TileMap tileMap) {
        if (_tile == null) _tile = new TileSetController(tileMap.TileSet);
        // tileMap.Clear();

        for (var x = 0; x < 200; x += 5) {
            if (x % 30 == 0) Console.WriteLine("Setting " + x);
            for (var y = 0; y < 20; y += 5) {
                tileMap.SetCell(0, new Vector2I(x, y), _tile.Platform(x));
            }
        }
        Console.WriteLine("Done");
    }
}
    
public class TileSetController {
    private readonly TileSet _tileSet;

    public TileSetController(TileSet tileSet) {
        _tileSet = tileSet;
        Generate();
    }

    public int Platform(int j) {
        return Platforms[j % Platforms.Length];
    }

    public int[] Platforms = Array.Empty<int>();

    public void Generate() {
        // Platforms = new[] {
        // _tileSet.FindTileByName("platform-1"),
        // _tileSet.FindTileByName("platform-2"),
        // _tileSet.FindTileByName("platform-3"),
        // _tileSet.FindTileByName("platform-4")
        // };
    }
}