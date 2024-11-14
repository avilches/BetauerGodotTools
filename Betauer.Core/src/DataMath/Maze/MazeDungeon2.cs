using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.DataMath.Maze;

public class MazeDungeon2 {
    private const int CELL_SOLID = -2;
    private const int TILE_MERGED = -1;
    private const int TILE_FLOOR = 0;
    private const int TILE_SOLID = 1;
    private const int TILE_WALL = 2;
    private const int TILE_H_DOOR = 3;
    private const int TILE_V_DOOR = 4;

    private readonly int _width;
    private readonly int _height;
    private readonly int[,] _floors;
    private readonly Random _rng = new Random();
    private readonly List<Room> _rooms = new List<Room>();
    private int _currentRegion;

    public MazeDungeon2(int width, int height) {
        _width = width;
        _height = height;
        _floors = new int[width, height];
        Initialize();
    }

    private void Initialize() {
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                _floors[x, y] = CELL_SOLID;
            }
        }
    }

    public void Generate() {
        AddRooms();
        StartMaze();
    }

    private void AddRooms() {
        int roomTries = 100;
        int maxRoomSize = 5;

        while (roomTries > 0) {
            int width = _rng.Next(2, maxRoomSize) * 2 + 1;
            int height = _rng.Next(2, maxRoomSize) * 2 + 1;

            int x = _rng.Next(0, (_width - width) / 2) * 2 + 1;
            int y = _rng.Next(0, (_height - height) / 2) * 2 + 1;

            var room = new Room(x, y, width, height);

            if (!_rooms.Any(other => room.Intersects(other))) {
                _rooms.Add(room);
                _currentRegion++;
                CarveRoom(room);
            }

            roomTries--;
        }
    }

    private void CarveRoom(Room room) {
        for (int x = room.X; x < room.X + room.Width; x++) {
            for (int y = room.Y; y < room.Y + room.Height; y++) {
                _floors[x, y] = TILE_FLOOR;
            }
        }
    }

    private void StartMaze() {
        for (int x = 1; x < _width - 1; x += 2) {
            for (int y = 1; y < _height - 1; y += 2) {
                if (_floors[x, y] == CELL_SOLID) {
                    CarveMaze(x, y);
                }
            }
        }
    }

    private void CarveMaze(int startX, int startY) {
        var stack = new Stack<(int x, int y)>();
        stack.Push((startX, startY));
        _floors[startX, startY] = TILE_MERGED;

        while (stack.Count > 0) {
            var (x, y) = stack.Peek();
            var directions = new List<(int dx, int dy)> { (2, 0), (-2, 0), (0, 2), (0, -2) };
            directions = directions.OrderBy(d => _rng.Next()).ToList();

            bool carved = false;
            foreach (var (dx, dy) in directions) {
                int nx = x + dx;
                int ny = y + dy;

                if (nx > 0 && nx < _width - 1 && ny > 0 && ny < _height - 1 && _floors[nx, ny] == CELL_SOLID) {
                    _floors[nx - dx / 2, ny - dy / 2] = TILE_MERGED; // Carve the wall between
                    _floors[nx, ny] = TILE_MERGED; // Carve the cell
                    stack.Push((nx, ny));
                    carved = true;
                    break;
                }
            }

            if (!carved) {
                stack.Pop();
            }
        }
    }

    public void Draw() {
        for (int y = 0; y < _height; y++) {
            for (int x = 0; x < _width; x++) {
                switch (_floors[x, y]) {
                    case TILE_FLOOR:
                        Console.Write(".");
                        break;
                    case TILE_MERGED:
                        Console.Write(" ");
                        break;
                    case CELL_SOLID:
                        Console.Write("\u2588");
                        break;
                }
            }
            Console.WriteLine();
        }
    }

    private class Room {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public Room(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Intersects(Room other) {
            return X < other.X + other.Width && X + Width > other.X &&
                   Y < other.Y + other.Height && Y + Height > other.Y;
        }
    }
}

public class ProgramMaze {
    public static void Main() {
        var dungeon = new MazeDungeon2(201, 33);
        dungeon.Generate();
        dungeon.Draw();
    }
}