using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Bsp;

public class BspTree {
    private static readonly Random DefaultRandom = new Random();

    public Random Random { get; init; } = DefaultRandom;

    public int Width { get; init; } = 80;
    public int Height { get; init; } = 80;

    public int MinRoomWidth { get; set; } = 3;
    public int MinRoomHeight { get; set; } = 3;
    public float MaxRatio { get; set; } = 16f / 9;

    public Func<BspNode, int, bool> Stop { get; init; } = (_, i) => false;
    public Func<int, int, int, int, Rect2I> CreateRoom { get; set; } = (x, y, width, height) => new Rect2I(x, y, width, height);

    /// <summary>
    /// Retries define the number of times the algorithm will try to split a node before giving up.
    /// Split a node will call to the Splitter function to determine if the split will be horizontal or vertical, and the position of the split (0.0 to 1.0).
    /// </summary>
    public int Retries { get; init; } = 5;

    /// <summary>
    /// Splitter function will determine if the node will be split horizontally or vertically, and the position of the split (0.0 to 1.0).
    /// </summary>
    public Func<BspNode, int, (bool horizontal, float splitBy)> Splitter { get; init; } = (node, depth) => (node.Height > node.Width, 0.5f);


    public BspNode Root { get; private set; }

    public void Generate() {
        MaxRatio = Math.Max(1f / MaxRatio, MaxRatio); // Convert ratios like 9:16 to 16:9
        MinRoomWidth = Math.Max(1, MinRoomWidth);
        MinRoomHeight = Math.Max(1, MinRoomHeight);
        Root = new BspNode(0, 0, Width, Height);
        SplitNode(Root, 0);
        return;

        void SplitNode(BspNode node, int depth) {
            if (Stop(node, depth)) {
                node.CreateRoom(CreateRoom);
                Console.WriteLine("Stopping at depth " + depth+" Room created: "+node.Rect2I.Value.Size.X+"/"+node.Rect2I.Value.Size.Y);
                return;
            }
            if (node.Split(Retries, Splitter, depth, MinRoomHeight, MinRoomWidth, MaxRatio)) {
                SplitNode(node.A!, depth + 1);
                SplitNode(node.B!, depth + 1);
            } else {
                node.CreateRoom(CreateRoom);
                Console.WriteLine("Rejected at " + depth+" Room created: "+node.Rect2I!.Value.Size.X+"/"+node.Rect2I.Value.Size.Y);
            }
        }
    }

    /// <summary>
    /// Retrieves a list of all rooms in the BSP tree.
    /// </summary>
    /// <returns>A list of Rect2I representing the rooms.</returns>
    public List<Rect2I> GetRooms() {
        var rooms = new List<Rect2I>();
        FillRooms(Root, rooms);
        return rooms;

        void FillRooms(BspNode node, List<Rect2I> rooms) {
            if (node.Rect2I != null) {
                rooms.Add(node.Rect2I.Value);
            } else {
                FillRooms(node.A!, rooms);
                FillRooms(node.B!, rooms);
            }
        }
    }

    /// <summary>
    /// Returns a list of connections between the deepest rooms in the BSP tree.
    /// It traverses the BSP tree, finds the deepest rooms in each node, and adds a connection between them.
    /// </summary>
    /// <returns>A list of Rect2I tuples representing all the connections between the rooms.</returns>
    public List<(Rect2I, Rect2I)> GetConnections() {
        var connections = new List<(Rect2I, Rect2I)>();
        FillConnections(Root, connections);
        return connections;

        Rect2I GetDeepestRoom(BspNode node) {
            return node.Rect2I ?? GetDeepestRoom(node.A!);
        }

        void FillConnections(BspNode node, List<(Rect2I, Rect2I)> connections) {
            if (node.Rect2I != null) return;
            Rect2I roomL = GetDeepestRoom(node.A!);
            Rect2I roomR = GetDeepestRoom(node.B!);
            connections.Add((roomR, roomL));
            FillConnections(node.A!, connections);
            FillConnections(node.B!, connections);
        }
    }
}