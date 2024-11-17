using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Maze;

public class MyMazeDungeonDemo {
    public static void Main() {
        var random = new Random(2);

        const int width = 41, height = 41;

        var grid = new Array2D<bool>(width, height).Fill(false);
        const float ratio = 16 / 9f;
        var rooms = CreateRooms(15, 3, 9, ratio, width, height, random);
        rooms.ForEach(room => Geometry.Geometry.GetEnumerator(room).ForEach(pos => grid[pos] = true));

        PrintMaze(grid);

        var mc = new MazeCarver(grid);
        mc.FillMazes(0.7f, rooms.Count, random);

        PrintMaze(grid);

        var array2DRegionConnections = new Array2DRegionConnections(grid);
        // PrintRegions(array2DRegionConnections.Labels);

        var regionConnectionsMap = array2DRegionConnections.GetConnectingCellsByRegion();
        ReduceConnections(regionConnectionsMap, width, height, random);
        var candidates = regionConnectionsMap.Values.SelectMany(i=>i).ToList();
        random.Shuffle(candidates);
        
        var unnecessary = new List<Vector2I>();
        var doors = new HashSet<Vector2I>(); 
        candidates.ForEach(pos => {
            if (array2DRegionConnections.GetRegions() > 1) {
                array2DRegionConnections.ToggleCell(pos, true);
                doors.Add(pos);
            } else {
                unnecessary.Add(pos);
            }
        });
        mc.RemoveDeadEnds();

        foreach (var b in grid) {
            if (b.Value) {
                if (doors.Contains(b.Position)) {
                    Console.Write("o");
                } else {
                    Console.Write(" ");
                }
            } else {
                Console.Write("#");
            }
            if (b.Position.X == grid.Width - 1) {
                Console.WriteLine();
            }
        }
    }

    private static void PrintMaze(Array2D<bool> grid) {
        foreach (var b in grid) {
            Console.Write(b.Value ? " " : "#");
            if (b.Position.X == grid.Width - 1) {
                Console.WriteLine();
            }
        }
    }

    private static void PrintRegions(Array2D<int> stage) {
        for (int y = 0; y < stage.Height; y++) {
            for (int x = 0; x < stage.Width; x++) {
                var tile = stage[x, y];
                if (tile == 0) {
                    Console.Write(" ");
                } else {
                    Console.Write(tile.ToString("x8").Substring(7, 1));
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine("--------------------");
    }

    /// <summary>
    /// Connect the regions by adding a connector between them.
    /// It receives a dictionary with the regions as keys and the list of connecting cells as values and mutate these lists to reduce the connections.
    /// These lists contain a lot of adjacent cells, so it will select one randomly. To do that, it generates a new Array2DRegionConnections empty, and put all
    /// the connections them. Then, it gets the regions and select one cell randomly from each region.
    /// That will result in a list of cells that will be the connectors, but not adjacent between them.
    /// 
    /// For example, in the following map, the regions 1 and 2 have multiple connection cells marked with "·":
    /// 1111111·2222·111
    ///       1·2222·1 1
    ///    55 1·2222·1 1
    ///  5555 1·2222·1 1
    ///    55 1        1
    ///    55 1 11111111
    /// 
    /// This method will create a temporal grid like this:
    ///        ·    ·
    ///        ·    ·
    ///        ·    ·
    ///        ·    ·
    /// The connection cells will create two regions, this method will select only one cell from each region randomly. In this case, the third of the first
    /// column and the first of the second column. If this connection are enabled, the regions 1 and 2 will be connected, creating a one single region "1"
    ///  111111 11111111
    ///       1 1111 1 1
    ///    55 111111 1 1
    ///  5555 1 1111 1 1
    ///    55 1        1
    ///    55 1 11111111
    /// 
    /// </summary>
    /// <param name="regionsConnectedMap"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="random"></param>
    private static void ReduceConnections(Dictionary<string, List<Vector2I>> regionsConnectedMap, int width, int height, Random random) {
        var grid = new Array2D<bool>(width, height);
        var gridCleaner = new Array2DRegionConnections(grid);
        regionsConnectedMap.Values.ForEach(connectors => {
            var candidates = new List<Vector2I>();
            gridCleaner.Grid.Fill(false);
            gridCleaner.Update();
            connectors.ForEach(conn => gridCleaner.ToggleCell(conn, true));
            gridCleaner.GetRegionsIds().ForEach(id => {
                candidates.Add(random.Next(gridCleaner.GetRegionCells(id)));
            });
            connectors.Clear();
            connectors.AddRange(candidates);
        });
    }


    private static List<Rect2I> CreateRooms(int roomCount, int min, int max, float ratio, int boundsWidth, int boundsHeight, Random rng) {
        var rooms = new List<Rect2I>();
        var bounds = new Rect2I(0, 0, boundsWidth, boundsHeight);

        var landscapes = 0;
        /*
        var tries = 1000000;
        for (var i = 0; i < tries; i++) {
            var ratio = rng.NextRatio(minRatio, maxRatio);
            var length = rng.Next(min, max + 1);
            var room = Geometry.Geometry.CreateRect2I(ratio, length, Geometry.Geometry.RectanglePart.Ratio);
            landscapes += room.Size.X > room.Size.Y ? 1 : 0;
        }
        Console.WriteLine("Landscapes: "+landscapes+" of "+tries+" = "+((float)landscapes*100/tries)+"%");
        */
        var numRoomTries = 0;
        while (rooms.Count < roomCount || numRoomTries++ > 1000) {
            var randomRatio = rng.NextRatio(1f / ratio, ratio);
            var length = rng.Next(min, max + 1);
            var room = Geometry.Geometry.CreateRect2I(randomRatio, length, Geometry.Geometry.RectanglePart.Ratio);
            // Ensure odd size.  
            if (room.Size.X % 2 == 0) {
                room = new Rect2I(room.Position, new Vector2I(room.Size.X + 1, room.Size.Y));
                if (room.Size.X > max) room = new Rect2I(room.Position, new Vector2I(room.Size.X - 2, room.Size.Y));
            }
            if (room.Size.Y % 2 == 0) {
                room = new Rect2I(room.Position, new Vector2I(room.Size.X, room.Size.Y + 1));
                if (room.Size.Y > max) room = new Rect2I(room.Position, new Vector2I(room.Size.X, room.Size.Y - 2));
            }

            room = Geometry.Geometry.PositionRect2IRandomly(room, bounds, rng);
            if (room.Position.X % 2 == 0) {
                room = new Rect2I(new Vector2I(room.Position.X + 1, room.Position.Y), room.Size);
                if (room.End.X > bounds.Size.X) room = new Rect2I(new Vector2I(room.Position.X - 2, room.Position.Y), room.Size);
            }
            if (room.Position.Y % 2 == 0) {
                room = new Rect2I(new Vector2I(room.Position.X, room.Position.Y + 1), room.Size);
                if (room.End.Y > bounds.Size.Y) room = new Rect2I(new Vector2I(room.Position.X, room.Position.Y - 2), room.Size);
            }
            var overlaps = rooms.Any(other => Geometry.Geometry.IntersectRectangles(other, room));
            if (!overlaps) {
                rooms.Add(room);
                landscapes += room.Size.X > room.Size.Y ? 1 : 0;
                // Console.WriteLine(room.Position + " " + room.Size + " " + (randomRatio < 1 ? 1f / randomRatio : randomRatio));
            }
        }
        Console.WriteLine("Landscapes: "+landscapes+" of "+rooms.Count+" = "+((float)landscapes*100/rooms.Count)+"%");
        return rooms;
    }
}