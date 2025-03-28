using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Examples.ThirdPartyCode.Hauberk;
using Godot;

namespace Betauer.Core.Examples.ThirdPartyCode;

public class MazeDartDemo {
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
            var overlaps = rooms.Any(other => DataMath.Geometry.Geometry.IntersectRectangles(other, room));
            if (!overlaps) rooms.Add(room);
        }
        return rooms;
    }

    private static void PrintRegions(MazeDungeon dungeon) {
        Console.WriteLine(dungeon.Stage.GetString(tile => 
            tile.Region == 0 
                ? " " 
                : tile.Region.ToString("x8").Substring(7, 1)));
        Console.WriteLine("--------------------");
    }

    public static void PrintStage(MazeDungeon dungeon) {
        Console.WriteLine(dungeon.Stage.GetString(tile => {
            var c = tile.Type switch {
                // TileType.Wall => ' ',
                // TileType.Floor => '*',
                // TileType.Path => '#',
                // TileType.OpenDoor => '+',
                // TileType.ClosedDoor => '+',
                TileType.Wall => '\u2588',
                TileType.Floor => ' ',
                TileType.Path => '·',
                TileType.OpenDoor => '+',
                TileType.ClosedDoor => '-',
                _ => ' '
            };
            return c.ToString();
        }));
    }
}