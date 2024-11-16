using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Maze;

public class MazeCarver {
    public Array2D<bool> Stage { get; init; }
    private static readonly Vector2I[] Directions = { Vector2I.Up, Vector2I.Down, Vector2I.Right, Vector2I.Left };

    public MazeCarver(Array2D<bool> stage) {
        Stage = stage;
    }

    public int FillMazes(float windyRatio, int startRegion, Random rng) {
        var mazes = 0;
        for (var y = 1; y < Stage.Height; y += 2) {
            for (var x = 1; x < Stage.Width; x += 2) {
                var pos = new Vector2I(x, y);
                if (IsCarved(Stage[x, y])) continue;
                GrowMaze(pos, windyRatio, startRegion + mazes, rng);
                mazes++;
            }
        }
        return mazes;
    }

    private bool IsCarved(bool b) {
        return b;
    }

    /// <summary>
    /// Generate a maze starting from a given position
    /// </summary>
    /// <param name="start"></param>
    /// <param name="windyRatio">0 means straight lines, 1 means winding paths... 0.7f means 70% winding paths, 30% change straight line</param>
    /// <param name="label">The maze section, just a number to identify the mazo</param>
    /// <param name="rng">The maze section, just a number to identify the mazo</param>
    public void GrowMaze(Vector2I start, float windyRatio, int label, Random rng) {
        var cells = new Stack<Vector2I>();
        Vector2I? lastDir = null;

        Carve(start, TileType.OpenDoor, label);

        cells.Push(start);
        while (cells.Count > 0) {
            var cell = cells.Peek();
            var nextCellsAvailable = Directions.Where(dir => CanCarve(cell, dir)).ToList();
            if (nextCellsAvailable.Count == 0) {
                cells.Pop();
                lastDir = null;
                Carve(start, TileType.ClosedDoor, label);
                continue;
            }
            Vector2I dir = lastDir.HasValue && nextCellsAvailable.Contains(lastDir.Value) && rng.NextSingle() < windyRatio
                ? lastDir.Value // Windy path, keep the same direction
                : rng.Next(nextCellsAvailable); // Random path

            Carve(cell + dir, TileType.Path, label);
            Carve(cell + dir * 2, TileType.Path, label);
            cells.Push(cell + dir * 2);
            lastDir = dir;
        }
    }

    public void Carve(Vector2I pos, TileType type, int region) {
        var dataCells = Stage;
        dataCells[pos] = true;
    }

    private bool CanCarve(Vector2I pos, Vector2I direction) {
        var vector2I = pos + direction * 3;
        return Geometry.Geometry.IsPointInRectangle(vector2I.X, vector2I.Y, 0f, 0f, Stage.Width, Stage.Height) &&
               !IsCarved(Stage[pos + direction * 2]);
    }
}

public class MyMazeDungeonDemo {
    public static void Main() {
        var random = new Random(3);

        const int width = 141, height = 141;

        var grid = new Array2D<bool>(width, height).Fill(false);
        var ratio = 16 / 9f;
        var rooms = CreateRooms(100, 5, 13, ratio, width, height, random);
        rooms.ForEach(room => Geometry.Geometry.GetEnumerator(room).ForEach(pos => grid[pos] = true));

        foreach (var b in grid) {
            Console.Write(b.Value ? " " : "#");
            if (b.Position.X == grid.Width - 1) {
                Console.WriteLine();
            }
        }

        var mc = new MazeCarver(grid);
        mc.FillMazes(0.7f, rooms.Count, random);

        foreach (var b in grid) {
            Console.Write(b.Value ? " " : "#");
            if (b.Position.X == grid.Width - 1) {
                Console.WriteLine();
            }
        }

        var gc = new Array2DRegionConnections(grid);
        PrintRegions(gc.Labels);

        foreach (var cell in gc.FindConnectingCells().ConnectingCells.Keys) {
            grid[cell] = true;
        }

        foreach (var b in grid) {
            Console.Write(b.Value ? " " : "#");
            if (b.Position.X == grid.Width - 1) {
                Console.WriteLine();
            }
        }
    }

    private static List<Rect2I> CreateRooms(int numRoomTries, int min, int max, float ratio, int boundsWidth, int boundsHeight, Random rng) {
        var rooms = new List<Rect2I>();
        var bounds = new Rect2I(0, 0, boundsWidth, boundsHeight);

        /*
        var landscapes = 0;
        var tries = 1000000;
        for (var i = 0; i < tries; i++) {
            var ratio = rng.NextRatio(minRatio, maxRatio);
            var length = rng.Next(min, max + 1);
            var room = Geometry.Geometry.CreateRect2I(ratio, length, Geometry.Geometry.RectanglePart.Ratio);
            landscapes += room.Size.X > room.Size.Y ? 1 : 0;
        }
        Console.WriteLine("Landscapes: "+landscapes+" of "+tries+" = "+((float)landscapes*100/tries)+"%");
        */

        for (var i = 0; i < numRoomTries * 10; i++) {
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
                // Console.WriteLine(room.Position + " " + room.Size + " " + (randomRatio < 1 ? 1f / randomRatio : randomRatio));
            }
        }
        return rooms;
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

    public static void PrintStage(Array2D<Cell> data) {
        foreach (var cell in data) {
            var c = cell.Value.Type switch {
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
            if (cell.Position.X == data.Width - 1) {
                Console.WriteLine();
            }
        }
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