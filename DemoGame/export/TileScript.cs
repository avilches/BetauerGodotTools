using System;
using System.IO;
using Betauer.Core;
using Betauer.TileSet.Godot;
using Betauer.TileSet.Image;
using Godot;

namespace Veronenger.export; 

public partial class TileScript : SceneTree {

    const string Destination = "Game/RTS/World/Terrain";

    public override void _Initialize() {
        var godotTileSet = new TileSetResourceBuilder(new Vector2I(16, 16));

        Directory.GetFiles(Destination).ForEach(File.Delete);
        
        godotTileSet
            .UseTerrain(SproutBushes, $"{Destination}/sprout-bush.png")
            .SourceId(0)
            .CopyFrom("RTS/Assets/Sprout Lands/tilesets/Ground tiles/new tiles/bush tiles.png")
            .Add(Colors.Blue, "Bushes");

        godotTileSet
            .CreateTerrain(TileSetLayouts.Blob47Godot, $"{Destination}/sprout-dark-grass-hill.png")
            .SourceId(1)
            .From(SproutGround, "RTS/Assets/Sprout Lands/tilesets/Ground tiles/new tiles/Darker Grass hill tiles v.2.png")
            .Add(Colors.Green, "Dark Grass Hill");

        godotTileSet.Save($"{Destination}/Tileset.tres");
        Console.WriteLine("Done!");

        Quit(0);
    }

    public static ITileSetLayout SproutBushes = new TileSetLayout(new [,] {
        { 28, 124, 112, 16, 20,  116,  92,  80,  84, 221,  -1 },
        { 31, 255, 241, 17, 23,  247, 223, 209, 215, 119,  -1 },
        {  7, 199, 193,  1, 29,  253, 127, 113, 125,  93, 117 },
        {  4,  68, 64,   0,  5,  197,  71,  65,  69,  87, 213 },
        { -1,  -1, -1,  -1, 21,  245,  95,  81,  85,  -1,  -1 }
    }.Resize(11, 12, -1));

    public static ITileSetLayout SproutGround = new TileSetLayout(new [,] {
        { 28, 124, 112, 16, 20,  116,  92,  80,  84, 221,  -1 },
        { 31, 255, 241, 17, 23,  247, 223, 209, 215, 119,  -1 },
        {  7, 199, 193,  1, 29,  253, 127, 113, 125,  93, 117 },
        {  4,  68, 64,   0,  5,  197,  71,  65,  69,  87, 213 },
        { -1,  -1, -1,  -1, 21,  245,  95,  81,  85,  -1,  -1 }
    }.Resize(11, 7, -1));
}