using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Bsp;

public class BspTree {
    private static readonly Random DefaultRandom = new Random();

    public static bool Debug = false;

    public Random Random { get; init; } = DefaultRandom;

    public int Width { get; init; } = 80;
    public int Height { get; init; } = 80;

    public int MinRoomWidth { get; set; } = 3;
    public int MinRoomHeight { get; set; } = 3;
    public float MaxRatio { get; set; } = 16f / 9;

    /// <summary>
    /// Stop function will be called for each node in the BSP tree, if it returns true, the node will be considered a leaf and a room will be created.
    /// Use this function to stop the BSP tree generation at a specific depth or condition.
    /// By default, it will only stop if the node creates a room smaller than MinRoomWidth or MinRoomHeight, or the ratio is bigger than MaxRatio.
    /// An interesting way to use is to stop the generation at a specific depth with a random chance, like: (node, i) => i > 5 && Random.NextDouble() > 0.5
    /// </summary>
    public Func<BspNode, int, bool> Stop { get; init; } = (_, i) => false;
    
    /// <summary>
    /// CreateRoom function will be called for each leaf node in the BSP tree to create a room.
    /// Use this function to create a room with a different size or position. Reducing by a random number and move it a bit to the right, for example.
    /// </summary>
    public Func<int, int, int, int, Rect2I> CreateRoom { get; set; } = (x, y, width, height) => new Rect2I(x, y, width, height);

    /// <summary>
    /// The Splitter function may or may not return a random value for the "splitBy" variable. If using the "splitBy" results in two sections that are too small
    /// (using MinRoomWidth and MinRoomHeight) or have too large a ratio (using MaxRatio), the algorithm will retry to split the node again in case the random
    /// value is different. Therefore, this Retries variable defines the maximum number of times this function will be called.
    /// </summary>
    public int Retries { get; init; } = 5;

    /// <summary>
    /// Splitter function will determine if the node will be split horizontally or vertically, and the position of the split (0.0 to 1.0).
    /// </summary>
    public Func<BspNode, int, (bool horizontal, float splitBy)> Splitter { get; init; } = (node, depth) => (node.Height > node.Width, 0.5f);


    public BspNode Root { get; private set; }

    public List<Rect2I> Rooms { get; private set; } = new();

    public void Generate() {
        if (MaxRatio < 1) MaxRatio = 1f / MaxRatio; // Convert ratios like 9:16 to 16:9
        MinRoomWidth = Math.Max(1, MinRoomWidth);
        MinRoomHeight = Math.Max(1, MinRoomHeight);
        
        Root = new BspNode(0, 0, Width, Height);
        Rooms.Clear();
        SplitNode(Root, 0);
        return;

        void SplitNode(BspNode node, int depth) {
            if (Stop(node, depth)) {
                node.CreateRoom(CreateRoom);
                Rooms.Add(node.Rect2I!.Value);
                if (Debug) Console.WriteLine("Stopping at depth " + depth+" Room created: "+node.Rect2I.Value.Size.X+"/"+node.Rect2I.Value.Size.Y);
                return;
            }
            if (node.Split(Retries, Splitter, depth, MinRoomHeight, MinRoomWidth, MaxRatio)) {
                SplitNode(node.A!, depth + 1);
                SplitNode(node.B!, depth + 1);
            } else {
                node.CreateRoom(CreateRoom);
                Rooms.Add(node.Rect2I!.Value);
                if (Debug) Console.WriteLine("Rejected at " + depth+" Room created: "+node.Rect2I!.Value.Size.X+"/"+node.Rect2I.Value.Size.Y);
            }
        }
    }

    /// <summary>
    /// Returns a list of connections between the deepest rooms in the BSP tree.
    /// It traverses the BSP tree, finds the deepest rooms in each node, and adds a connection between them.
    /// So, for this reason, rooms are connected to more than one room.
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