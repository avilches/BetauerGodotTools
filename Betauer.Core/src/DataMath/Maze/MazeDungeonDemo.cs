using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Maze;

public class MazeDungeonDemo {
    public static void Main() {
        var random = new Random(0);
        var dungeon = new MazeDungeon(41, 41);
        
        // GenerateRandomRoomsNew(random, dungeon);

        var rooms = GenerateRandomRooms(0, dungeon, random);

        var region = rooms.Count;
        region += dungeon.FillMazes( 0.7f, region-1);
        dungeon.ConnectRegions(region);
        dungeon.RemoveDeadEnds();
        PrintRegions(dungeon);
        PrintStage(dungeon);
    }

    private static void GenerateRandomRoomsNew(Random random, MazeDungeon dungeon) {
        var lastRegion = 0;
        
        for (int i = 0; i < 10; i++) {
            var rect = Geometry.Geometry.CreateRandomRect2I(9f / 16, 16f / 9, 5, 10, Geometry.Geometry.RectanglePart.Longer, random);
            rect = Geometry.Geometry.PositionRect2IRandomly(new Rect2I(0, 0, dungeon.Stage.Width, dungeon.Stage.Height), rect, random);
            lastRegion++;
            dungeon.Carve(rect, TileType.Floor, lastRegion);
            PrintRegions(dungeon);
        }
        // dungeon.LastRegion = lastRegion;
    }

    private static List<Rect2I> GenerateRandomRooms(int startRegion, MazeDungeon dungeon, Random random) {
        var rooms = AddRooms(30, 0, dungeon.Stage.Width, dungeon.Stage.Height, random);
        foreach (var room in rooms) {
            dungeon.Carve(room, TileType.Floor, startRegion);
            startRegion++;
        }
        return rooms;
    }
    
    public static List<Rect2I> AddRooms(int numRoomTries, int roomExtraSize, int boundsWidth, int boundsHeight, Random rng) {
        List<Rect2I> rooms = new List<Rect2I>();
        for (int i = 0; i < numRoomTries; i++) {
            var size = (rng.Next(1, 3 + roomExtraSize) * 2) + 1;
            var rectangularity = rng.Next(0, 1 + size / 2) * 2;
            var width = size;
            var height = size;
            if (rng.Next(2) == 0) {
                width += rectangularity;
            } else {
                height += rectangularity;
            }
            var x = rng.Next((boundsWidth - width) / 2) * 2 + 1;
            var y = rng.Next((boundsHeight - height) / 2) * 2 + 1;
            var room = new Rect2I(x, y, width, height);
            var overlaps = rooms.Any(other => Geometry.Geometry.IntersectRectangles(other, room));
            if (!overlaps) rooms.Add(room);
        }
        return rooms;
    }

    private static void PrintRegions(MazeDungeon dungeon) {
        var stage = dungeon.Stage;
        for (int y = 0; y < stage.Height; y++) {
            for (int x = 0; x < stage.Width; x++) {
                var tile = stage[x, y];
                if (tile.Region == 0) {
                    Console.Write(" ");
                } else {
                    Console.Write(tile.Region.ToString("x8").Substring(7, 1));
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine("--------------------");
    }

    public static void PrintStage(MazeDungeon dungeon) {
        var stage = dungeon.Stage;
        for (int y = 0; y < stage.Height; y++) {
            for (int x = 0; x < stage.Width; x++) {
                var tile = stage.GetValue(new Vector2I(x, y));
                var c = tile.Type switch {
                    // TileType.Wall => ' ',
                    // TileType.Floor => '*',
                    // TileType.Path => '#',
                    // TileType.OpenDoor => '+',
                    // TileType.ClosedDoor => '+',
                    TileType.Wall => '#',
                    TileType.Floor => ' ',
                    TileType.Path => '.',
                    TileType.OpenDoor => '+',
                    TileType.ClosedDoor => '-',
                    _ => ' '
                };
                Console.Write(c);
            }
            Console.WriteLine();
        }
    }
}