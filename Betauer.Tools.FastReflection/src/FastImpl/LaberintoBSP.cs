namespace Betauer.Tools.FastReflection.FastImpl;
using System;
using System.Collections.Generic;

public class BSPNode
{
    public int X, Y, Width, Height;
    public BSPNode Left, Right;
    public (int, int) Center;

    public BSPNode(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool IsLeaf => Left == null && Right == null;
}

public class BSPDungeon
{
    private int[,] grid;
    private int rows, cols;
    private Random random = new Random(0);
    private List<BSPNode> leaves;
    private List<(int, int, int, int, int)> edges; // (distance, x1, y1, x2, y2)

    public BSPDungeon(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        grid = new int[rows, cols];
        leaves = new List<BSPNode>();
        edges = new List<(int, int, int, int, int)>();
        FillSolids();
    }

    private void FillSolids()
    {
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                grid[i, j] = 1;
    }

    public void GenerateBSP(int minRoomSize)
    {
        BSPNode root = new BSPNode(0, 0, cols, rows);
        SplitNode(root, minRoomSize);
        foreach (var leaf in leaves)
        {
            CreateRoom(leaf);
        }
        GenerateConnections();
        ConnectAllRooms();
    }

    private void SplitNode(BSPNode node, int minRoomSize)
    {
        if (node.Width <= minRoomSize * 2 || node.Height <= minRoomSize * 2)
        {
            leaves.Add(node);
            return;
        }

        bool splitVertically = random.Next(2) == 0;
        if (node.Width > node.Height && node.Width / node.Height >= 1.25)
            splitVertically = true;
        else if (node.Height > node.Width && node.Height / node.Width >= 1.25)
            splitVertically = false;

        int max = (splitVertically ? node.Width : node.Height) - minRoomSize;
        if (max <= minRoomSize)
        {
            leaves.Add(node);
            return;
        }

        int split = random.Next(minRoomSize, max);
        if (splitVertically)
        {
            node.Left = new BSPNode(node.X, node.Y, split, node.Height);
            node.Right = new BSPNode(node.X + split, node.Y, node.Width - split, node.Height);
        }
        else
        {
            node.Left = new BSPNode(node.X, node.Y, node.Width, split);
            node.Right = new BSPNode(node.X, node.Y + split, node.Width, node.Height - split);
        }

        SplitNode(node.Left, minRoomSize);
        SplitNode(node.Right, minRoomSize);
    }

    private void CreateRoom(BSPNode node)
    {
        int roomWidth = random.Next(3, node.Width - 1);
        int roomHeight = random.Next(3, node.Height - 1);
        int roomX = node.X + random.Next(1, node.Width - roomWidth - 1);
        int roomY = node.Y + random.Next(1, node.Height - roomHeight - 1);

        for (int i = roomY; i < roomY + roomHeight; i++)
            for (int j = roomX; j < roomX + roomWidth; j++)
                grid[i, j] = 0;

        node.Center = (roomY + roomHeight / 2, roomX + roomWidth / 2);
    }

    private void GenerateConnections()
    {
        for (int i = 0; i < leaves.Count; i++)
        {
            for (int j = i + 1; j < leaves.Count; j++)
            {
                int x1 = leaves[i].Center.Item2;
                int y1 = leaves[i].Center.Item1;
                int x2 = leaves[j].Center.Item2;
                int y2 = leaves[j].Center.Item1;
                int distance = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
                edges.Add((distance, x1, y1, x2, y2));
            }
        }
        edges.Sort((a, b) => a.Item1.CompareTo(b.Item1)); // Sort connections by distance
    }

    private void ConnectAllRooms()
    {
        HashSet<(int, int)> connected = new HashSet<(int, int)>() { (leaves[0].Center.Item2, leaves[0].Center.Item1) };
        Queue<(int, int, int, int, int)> queue = new Queue<(int, int, int, int, int)>(edges);
        while (connected.Count < leaves.Count && queue.Count > 0)
        {
            var (distance, x1, y1, x2, y2) = queue.Dequeue();
            if (connected.Contains((x1, y1)) ^ connected.Contains((x2, y2)))
            {
                CreateCorridor(x1, y1, x2, y2);
                connected.Add((x1, y1));
                connected.Add((x2, y2));
            }
        }

        // Ensure all rooms are connected
        foreach (var leaf in leaves)
        {
            if (!connected.Contains((leaf.Center.Item2, leaf.Center.Item1)))
            {
                // Connect unconnected rooms to the nearest connected point
                var nearest = FindNearestConnected(leaf.Center, connected);
                CreateCorridor(leaf.Center.Item2, leaf.Center.Item1, nearest.Item2, nearest.Item1);
                connected.Add((leaf.Center.Item2, leaf.Center.Item1));
            }
        }
    }

    private (int, int) FindNearestConnected((int, int) point, HashSet<(int, int)> connected)
    {
        (int, int) nearest = (-1, -1);
        int minDistance = int.MaxValue;

        foreach (var conn in connected)
        {
            int distance = Math.Abs(point.Item2 - conn.Item2) + Math.Abs(point.Item1 - conn.Item1);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = conn;
            }
        }

        return nearest;
    }

    private bool IsConnected(int x, int y, HashSet<(int, int)> connected)
    {
        return connected.Contains((x, y));
    }

    private void CreateCorridor(int x1, int y1, int x2, int y2)
    {
        while (x1 != x2)
        {
            grid[y1, x1] = 0;
            x1 += x1 < x2 ? 1 : -1;
        }
        while (y1 != y2)
        {
            grid[y1, x1] = 0;
            y1 += y1 < y2 ? 1 : -1;
        }
    }

    public void Print()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
                Console.Write(grid[i, j] == 0 ? " " : "#");
            Console.WriteLine();
        }
    }
}

public class ProgramBSPDungeon
{
    public static void Main()
    {
        int rows = 120;
        int cols = 90;
        BSPDungeon dungeon = new BSPDungeon(rows, cols);
        
        dungeon.GenerateBSP(4); // Generate rooms with a minimum size
        dungeon.Print();
    }
}
