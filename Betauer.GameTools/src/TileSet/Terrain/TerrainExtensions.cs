using System;

namespace Betauer.TileSet.Terrain;

public static class TerrainExtensions {
    public static void ExpandBlob47(this SingleTerrain singleTerrain) {
        singleTerrain.Transform(mask => Blob47Tools.Blob256To47[mask]);
    }

    public static void PrintTileIdsArray(this SingleTerrain singleTerrain) {
        var tiles = singleTerrain.Grid;
        Console.WriteLine("new[,] {");
        for (var y = 0; y < tiles.GetLength(0); y++) {
            Console.Write("  {");
            for (var x = 0; x < tiles.GetLength(1); x++) {
                var tileId = tiles[y, x];
                Console.Write(tileId.ToString().PadLeft(3) + ",");
            }
            Console.WriteLine("},");
        }
        Console.WriteLine("};");
    }

    public static void PrintBlocks(this SingleTerrain singleTerrain) {
        var tiles = singleTerrain.Grid;
        for (var y = 0; y < tiles.GetLength(0); y++) {
            for (var x = 0; x < tiles.GetLength(1); x++) {
                var tileId = tiles[y, x];
                Console.Write(tileId >= 0 ? "#" : " ");
            }
            Console.WriteLine();
        }
    }

}