using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.DataMath.Maze;

public class MazeDungeonDemo {
    public static IEnumerable<Vector2I> GetEnumerator(Rect2I rect) {
        for (var x = rect.Position.X; x < rect.End.X; x++) {
            for (var y = rect.Position.Y; y < rect.End.Y; y++) {
                yield return new Vector2I(x, y);
            }
        }
    }
    
    public static void Main() {
        var random = new Random(0);
        var dungeon = new MazeDungeon(201, 33);
        
        // GenerateRandomRoomsNew(random, dungeon);

        GenerateRandomRooms(dungeon);

        dungeon.FillMazes(0.7f, 1);
        dungeon.ConnectRegions();
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

    private static void GenerateRandomRooms(MazeDungeon dungeon) {
        var rooms = MazeDungeon.AddRooms(30, 0, dungeon.Stage.Width, dungeon.Stage.Height);
        var lastRegion = 0;

        foreach (var room in rooms) {
            lastRegion++;
            dungeon.Carve(room, TileType.Floor, lastRegion);
        }
        // dungeon.LastRegion = lastRegion;
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
                char c = tile.Type switch {
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