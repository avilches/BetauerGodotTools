using System;
using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class MazeNodeCyclesTests {
    private MazeGraph maze;

    [SetUp]
    public void Setup() {
        // Create a 4x4 maze for testing
        maze = new MazeGraph(4, 4);

        /* Create a maze structure like this, parent and edges are the same
           0--1--2--3
           |
           |
           4--5--6--7
           |
           |
           8--9--10-11
           |
           |
           12-13-14-15
         */

        // Create all nodes first
        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 4; x++) {
                maze.CreateNode(new Vector2I(x, y));
            }
        }

        // Connect horizontally and set parent relationships
        for (int y = 0; y < 4; y++) {
            for (int x = 0; x < 3; x++) {
                var from = maze.GetNodeAt(new Vector2I(x, y));
                var to = maze.GetNodeAt(new Vector2I(x + 1, y));
                maze.ConnectNodes(from, to);
                maze.ConnectNodes(to, from);
                to.Parent = from;
            }
        }

        // Connect vertically from first column and set parent relationships
        for (int y = 0; y < 3; y++) {
            var from = maze.GetNodeAt(new Vector2I(0, y));
            var to = maze.GetNodeAt(new Vector2I(0, y + 1));
            maze.ConnectNodes(from, to);
            maze.ConnectNodes(to, from);
            to.Parent = from;
        }

        /* Create a maze structure like this, parent and edges are the same
           0--1--2--3
           |
           |
           4--5--6--7
           |
           |
           8--9  10-11
           |     |
           |     |
           12-13-14-15
         */
        maze.DisconnectNodes(9, 10);
        maze.DisconnectNodes(10, 9);
        maze.ConnectNodes(10, 14);
        maze.ConnectNodes(14, 10);
        maze.GetNode(10).Parent = maze.GetNode(14);

    }

    [Test]
    public void FindPotentialCycles_WithMinDistance3_ReturnsCorrectPairs() {
        var cycles = maze.GetPotentialCycles(useParentDistance: true).GetCyclesGreaterThan(3);

        Assert.That(cycles, Is.Not.Empty);

        // Check first cycle found (should be one of the longest)
        var firstCycle = cycles.First();
        Assert.That(firstCycle.distance, Is.GreaterThanOrEqualTo(3));

        // Verify nodes are adjacent but not connected
        var nodeAPos = firstCycle.nodeA.Position;
        var nodeBPos = firstCycle.nodeB.Position;
        Assert.That((nodeAPos - nodeBPos).Length(), Is.EqualTo(1)); // Adjacent nodes
        Assert.That(!firstCycle.nodeA.HasEdgeTo(firstCycle.nodeB)); // Not connected
    }

    [Test]
    public void AddCyclesByParentDistance_WithMaxCycles_AddsCorrectNumberOfCycles() {
        var maxCycles = 10;
        var initialEdgeCount = CountEdges();
        
        Assert.That(initialEdgeCount, Is.EqualTo(30));

        maze.GetPotentialCycles(useParentDistance: true).GetCyclesGreaterThan(4).ForEach(cycle => {
            Console.WriteLine($"Cycle: {cycle.nodeA.Position} - {cycle.nodeB.Position} ({cycle.distance})");
            maze.ConnectNodes(cycle.nodeA, cycle.nodeB);
            maze.ConnectNodes(cycle.nodeB, cycle.nodeA);
        });
        PrintGraphEdges(maze);

        var newEdgeCount = CountEdges();
        Assert.That(newEdgeCount, Is.EqualTo(40));
    }

    [Test]
    public void VerifyDistanceCalculation_BeforeAndAfterNewConnection() {
        // Get two nodes that we'll connect
        var nodeA = maze.GetNodeAt(new Vector2I(2, 0));
        var nodeB = maze.GetNodeAt(new Vector2I(2, 1));

        // Calculate distances before connection
        var parentDistanceBefore = nodeA.GetDistanceToNode(nodeB);
        var edgeDistanceBefore = nodeA.GetDistanceToNodeByEdges(nodeB);

        Assert.That(parentDistanceBefore, Is.EqualTo(5));
        Assert.That(edgeDistanceBefore, Is.EqualTo(5));

        // Add the new connection
        maze.ConnectNodes(nodeA, nodeB);
        maze.ConnectNodes(nodeB, nodeA);

        // Calculate distances after connection
        var parentDistanceAfter = nodeA.GetDistanceToNode(nodeB);
        var edgeDistanceAfter = nodeA.GetDistanceToNodeByEdges(nodeB);

        Assert.That(parentDistanceAfter, Is.EqualTo(5));
        Assert.That(edgeDistanceAfter, Is.EqualTo(1));
    }


    [Test]
    public void FindPotentialCycles_WithHighMinDistance_ReturnsNoResults() {
        var cycles = maze.GetPotentialCycles(useParentDistance: true).GetCyclesGreaterThan(20).ToList();
        Assert.That(cycles, Is.Empty);
    }

    private int CountEdges() {
        return maze.Nodes.Values.Sum(node => node.GetEdges().Count());
    }

    private static void PrintGraphEdges(MazeGraph mc) {
        var allCanvas = new TextCanvas();
        foreach (var dataCell in mc.NodeGrid) {
            var node = dataCell.Value;
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "-");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(dataCell.Position.X * 3, dataCell.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }

    private static void PrintGraphParent(MazeGraph mc) {
        var allCanvas = new TextCanvas();
        foreach (var dataCell in mc.NodeGrid) {
            var node = dataCell.Value;
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null && node.Up == node.Parent) canvas.Write(1, 0, "p");
            if (node.Right != null && node.Right == node.Parent) canvas.Write(2, 1, "p");
            if (node.Down != null && node.Down == node.Parent) canvas.Write(1, 2, "p");
            if (node.Left != null && node.Left == node.Parent) canvas.Write(0, 1, "p");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(dataCell.Position.X * 3, dataCell.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}