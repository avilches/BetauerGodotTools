using System;

namespace Betauer.Core.Examples;

/// <summary>
/// Creates a 2D dungeon-like map by generating a grid of rooms that can expand randomly.
/// The map is represented using a char matrix where:
/// '#' = walls
/// '.' = room interior
/// ' ' = empty space
/// 
/// Each room starts with the same size and is placed in a grid with fixed spacing.
/// Then, rooms will expand randomly in four possible directions (up, right, down, left)
/// during multiple iterations. A room can only expand if there's enough space and
/// won't collide with other rooms or walls.
/// 
/// The final result is an irregular dungeon where each room has a different size and shape
/// while maintaining the original grid structure and spacing between rooms. All rooms
/// remain rectangular and are still separated by empty space, making it suitable for
/// generating game levels with distinct, interconnected areas.
/// 
/// Note: Uses a fixed seed (1) for deterministic generation.
/// </summary>
public class RoomGenerator {
    private readonly Random random = new Random(1);
    private char[,] map;
    private Room[,] rooms;
    private readonly int roomsWidth;
    private readonly int roomsHeight;
    private int initialRoomWidth;
    private int initialRoomHeight;
    private int roomSpacing;

    public class Room {
        public int X, Y, Width, Height;
        public bool CanExpand;

        public Room(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            CanExpand = true;
        }
    }

    public RoomGenerator(int roomsWidth, int roomsHeight, int initialRoomWidth, int initialRoomHeight, int roomSpacing) {
        this.roomsWidth = roomsWidth;
        this.roomsHeight = roomsHeight;
        this.initialRoomWidth = initialRoomWidth;
        this.initialRoomHeight = initialRoomHeight;
        this.roomSpacing = roomSpacing;

        // Calculate total map size based on rooms and spacing
        int mapWidth = (roomsWidth * (initialRoomWidth + roomSpacing)) + roomSpacing;
        int mapHeight = (roomsHeight * (initialRoomHeight + roomSpacing)) + roomSpacing;

        map = new char[mapHeight, mapWidth];
        rooms = new Room[roomsHeight, roomsWidth];

        InitializeMap();
        PlaceInitialRooms();
    }

    private void InitializeMap() {
        // Fill entire map with empty space
        for (int y = 0; y < map.GetLength(0); y++) {
            for (int x = 0; x < map.GetLength(1); x++) {
                map[y, x] = ' ';
            }
        }
    }

    private void PlaceInitialRooms() {
        for (int ry = 0; ry < roomsHeight; ry++) {
            for (int rx = 0; rx < roomsWidth; rx++) {
                int x = rx * (initialRoomWidth + roomSpacing) + roomSpacing;
                int y = ry * (initialRoomHeight + roomSpacing) + roomSpacing;

                rooms[ry, rx] = new Room(x, y, initialRoomWidth, initialRoomHeight);
                DrawRoom(rooms[ry, rx]);
            }
        }
    }

    private void DrawRoom(Room room) {
        // Fill interior with dots
        for (int y = room.Y; y < room.Y + room.Height; y++) {
            for (int x = room.X; x < room.X + room.Width; x++) {
                map[y, x] = '.';
            }
        }

        // Draw walls
        for (int x = room.X - 1; x <= room.X + room.Width; x++) {
            map[room.Y - 1, x] = '#'; // Top wall
            map[room.Y + room.Height, x] = '#'; // Bottom wall
        }
        for (int y = room.Y - 1; y <= room.Y + room.Height; y++) {
            map[y, room.X - 1] = '#'; // Left wall
            map[y, room.X + room.Width] = '#'; // Right wall
        }
    }

    public void ExpandRooms(int iterations) {
        for (int i = 0; i < iterations; i++) {
            bool anyExpansion = false;

            for (int ry = 0; ry < roomsHeight; ry++) {
                for (int rx = 0; rx < roomsWidth; rx++) {
                    Room room = rooms[ry, rx];
                    if (!room.CanExpand) continue;

                    // Try to expand in a random direction
                    int direction = random.Next(4);
                    bool expanded = false;

                    switch (direction) {
                        case 0: // Up
                            if (CanExpandUp(room)) {
                                room.Y--;
                                room.Height++;
                                expanded = true;
                            }
                            break;
                        case 1: // Right
                            if (CanExpandRight(room)) {
                                room.Width++;
                                expanded = true;
                            }
                            break;
                        case 2: // Down
                            if (CanExpandDown(room)) {
                                room.Height++;
                                expanded = true;
                            }
                            break;
                        case 3: // Left
                            if (CanExpandLeft(room)) {
                                room.X--;
                                room.Width++;
                                expanded = true;
                            }
                            break;
                    }

                    if (expanded) {
                        ClearMap();
                        RedrawAllRooms();
                        anyExpansion = true;
                    }
                }
            }

            if (!anyExpansion) break;
        }
    }

    private bool CanExpandUp(Room room) {
        if (room.Y <= 1) return false;

        // Check if expansion would touch another room
        for (int x = room.X - 1; x <= room.X + room.Width; x++) {
            if (map[room.Y - 2, x] != ' ') return false;
        }
        return true;
    }

    private bool CanExpandRight(Room room) {
        if (room.X + room.Width >= map.GetLength(1) - 2) return false;

        for (int y = room.Y - 1; y <= room.Y + room.Height; y++) {
            if (map[y, room.X + room.Width + 1] != ' ') return false;
        }
        return true;
    }

    private bool CanExpandDown(Room room) {
        if (room.Y + room.Height >= map.GetLength(0) - 2) return false;

        for (int x = room.X - 1; x <= room.X + room.Width; x++) {
            if (map[room.Y + room.Height + 1, x] != ' ') return false;
        }
        return true;
    }

    private bool CanExpandLeft(Room room) {
        if (room.X <= 1) return false;

        for (int y = room.Y - 1; y <= room.Y + room.Height; y++) {
            if (map[y, room.X - 2] != ' ') return false;
        }
        return true;
    }

    private void ClearMap() {
        for (int y = 0; y < map.GetLength(0); y++) {
            for (int x = 0; x < map.GetLength(1); x++) {
                map[y, x] = ' ';
            }
        }
    }

    private void RedrawAllRooms() {
        for (int ry = 0; ry < roomsHeight; ry++) {
            for (int rx = 0; rx < roomsWidth; rx++) {
                DrawRoom(rooms[ry, rx]);
            }
        }
    }

    public void PrintMap() {
        for (int y = 0; y < map.GetLength(0); y++) {
            for (int x = 0; x < map.GetLength(1); x++) {
                Console.Write(map[y, x]);
            }
            Console.WriteLine();
        }
    }

    public char[,] GetMap() {
        return map;
    }
}

public class Example {
    public static void Main() {
        var generator = new RoomGenerator(
            roomsWidth: 8,
            roomsHeight: 8,
            initialRoomWidth: 3,
            initialRoomHeight: 3,
            roomSpacing: 10
        );

        generator.ExpandRooms(11);
        generator.PrintMap();
    }
}