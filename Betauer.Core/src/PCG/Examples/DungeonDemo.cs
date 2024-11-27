using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class DungeonDemo {
    public static void Main() {
        var random = new Random(2);

        const int width = 41, height = 41;

        var grid = new Array2D<bool>(width, height, false);
        const float ratio = 16 / 9f;
        var rooms = CreateRooms(10, 3, 9, ratio, width, height, random);
        rooms.ForEach(room => DataMath.Geometry.Geometry.GetEnumerator(room).ForEach(pos => grid[pos] = true));

        // PrintMaze(grid);

        var mc = MazeCarver.Create(grid);

        for (var y = 1; y < mc.Height; y += 2) {
            for (var x = 1; x < mc.Width; x += 2) {
                var pos = new Vector2I(x, y);
                if (!mc.IsCarved(pos)) {
                    mc.Grow(pos, MazeConstraints.CreateWindy(0.7f, random).With(c => c.MaxPaths = 1));
                }
            }
        }

        PrintMaze(grid);

        var array2DRegionConnections = new RegionConnections(grid);
        PrintRegions(array2DRegionConnections);

        var regionConnectionsMap = array2DRegionConnections.GetConnectingCellsByRegion();
        ReduceConnections(regionConnectionsMap, width, height, random);
        var candidates = regionConnectionsMap.Values.SelectMany(i => i).ToList();
        random.Shuffle(candidates);

        // Fill all the connections, creating a unique big region with all the mazes and rooms
        candidates.ForEach(pos => { array2DRegionConnections.ToggleCell(pos, true); });

        var unnecessary = new List<Vector2I>();
        // Try to remove every connection to see if the regions are still connected, deleting the unnecessary connections
        candidates.RemoveAll(pos => {
            array2DRegionConnections.ToggleCell(pos, false);
            if (array2DRegionConnections.GetRegions() > 1) {
                array2DRegionConnections.ToggleCell(pos, true);
                return false;
            }
            unnecessary.Add(pos);
            return true;
        });

        DeadEndRemover.Create(grid).RemoveAll();
        // RemoveDeadEnds(mc);
        PrintMazeConnections(grid, candidates.ToHashSet());
        // PrintRegions(array2DRegionConnections);
    }

    private static void PrintMazeConnections(Array2D<bool> grid, HashSet<Vector2I> doors) {
        Console.WriteLine(grid.GetString(b => b.Value 
            ? doors.Contains(b.Position) ? "d" : " " 
            : "█"));
    }

    private static void PrintMaze(Array2D<bool> grid) {
        Console.WriteLine(grid.GetString((v) => v ? "·" : "█"));
    }

    private static void PrintRegions(RegionConnections regionConnections) {
        regionConnections.Update();
        Console.WriteLine(regionConnections.Labels.GetString(tile =>
            tile == 0
                ? " "
                : tile.ToString("x8").Substring(7, 1)));
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
        var gridCleaner = new RegionConnections(grid);
        regionsConnectedMap.Values.ForEach(connectors => {
            var candidates = new List<Vector2I>();
            gridCleaner.Grid.Fill(false);
            gridCleaner.Update();
            connectors.ForEach(conn => gridCleaner.ToggleCell(conn, true));
            gridCleaner.GetRegionsIds().ForEach(id => { candidates.Add(random.Next(gridCleaner.GetRegionCells(id))); });
            connectors.Clear();
            connectors.AddRange(candidates);
        });
    }


    private static List<Rect2I> CreateRooms(int roomCount, int min, int max, float ratio, int boundsWidth, int boundsHeight, Random rng) {
        var rooms = new List<Rect2I>();
        var bounds = new Rect2I(0, 0, boundsWidth - 1, boundsHeight - 1);
        var numRoomTries = 0;

        var pairsWithinRatio = GetEvenPairs(min, max, ratio);
        while (numRoomTries++ < 1000 && rooms.Count < roomCount) {
            var pair = rng.Next(pairsWithinRatio);
            var room = new Rect2I(Vector2I.Zero, pair.X, pair.Y);
            var numPositionTries = 0;
            while (numPositionTries++ < 1000) {
                room = DataMath.Geometry.Geometry.PositionRect2IRandomly(room, bounds, rng);
                if (room.Position.X % 2 == 0) room = new Rect2I(new Vector2I(room.Position.X + 1, room.Position.Y), room.Size);
                if (room.Position.Y % 2 == 0) room = new Rect2I(new Vector2I(room.Position.X, room.Position.Y + 1), room.Size);
                if (!rooms.Any(other => DataMath.Geometry.Geometry.IntersectRectangles(other, room))) {
                    rooms.Add(room);
                    break;
                }
            }
        }
        int landscapes = 0, portraits = 0, squareRooms = 0;
        rooms.ForEach(room => {
            landscapes += room.Size.X > room.Size.Y ? 1 : 0;
            portraits += room.Size.X < room.Size.Y ? 1 : 0;
            squareRooms += room.Size.X == room.Size.Y ? 1 : 0;
        });
        Console.WriteLine($"Rooms: {rooms.Count} = Landscapes: {landscapes} ({(float)landscapes * 100 / rooms.Count:0.00}%) Portraits: {portraits} ({(float)portraits * 100 / rooms.Count:0.00}%) Squares: {squareRooms} ({(float)squareRooms * 100 / rooms.Count:0.00}%)");
        return rooms;
    }

    /// <summary>
    /// Returns a list of pairs of integers that are within the ratio and are odd numbers, squares only appear once, and the pairs are rotated (some there is
    /// a portrait and a landscape version of the same ratio).
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="ratio"></param>
    /// <returns></returns>
    private static List<Vector2I> GetEvenPairs(int min, int max, float ratio) {
        var pairsWithinRatio = DataMath.Geometry.Geometry.GetPairsWithinRatio(min, max, 1, ratio, 2).ToList();
        pairsWithinRatio.AddRange(pairsWithinRatio
            .Where(pair => pair.X != pair.Y) // ignore squares
            .Select(pair => new Vector2I(pair.Y, pair.X)).ToList()); // Rotate
        pairsWithinRatio.ForEach(par => {
            var (x, y) = par;
            // Console.WriteLine("Pair: " + x + ", " + y+". "+(x > y ? "Landscape" : (x < y ? "Portrait" : "Square")));
        });
        return pairsWithinRatio;
    }
}