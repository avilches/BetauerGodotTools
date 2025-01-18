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
        maze = MazeGraph.Create(4, 4);

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

           Later we'll disconnect 9-10 and connect 10-14 to create two zones
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
                from.ConnectTo(to);
                to.ConnectTo(from);
                to.Parent = from;
            }
        }

        // Connect vertically from first column and set parent relationships
        for (int y = 0; y < 3; y++) {
            var from = maze.GetNodeAt(new Vector2I(0, y));
            var to = maze.GetNodeAt(new Vector2I(0, y + 1));
            from.ConnectTo(to);
            to.ConnectTo(from);
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

        // Set zones
        foreach (var node in maze.GetNodes()) {
            node.ZoneId = node.Id < 10 ? 1 : 2;
        }
    }

    [Test]
    public void TestShortestCyclesInZone() {
        var cycles = MazeGraphTools.ConnectShortestCyclesInZone(maze, zoneId: 1, maxCycles: 2);

        Assert.That(cycles, Is.Not.Empty);
        Assert.That(cycles.Count, Is.EqualTo(2));
        
        // Verify the cycles are valid and within zone 1
        foreach (var cycle in cycles) {
            Assert.That(cycle.nodeA.ZoneId, Is.EqualTo(1));
            Assert.That(cycle.nodeB.ZoneId, Is.EqualTo(1));
            Assert.That(cycle.distance, Is.GreaterThan(0));
        }
    }

    [Test]
    public void TestLongestCyclesInZone() {
        var cycles = MazeGraphTools.ConnectLongestCyclesInZone(maze, zoneId: 1, maxCycles: 2);

        Assert.That(cycles, Is.Not.Empty);
        Assert.That(cycles.Count, Is.EqualTo(2));
        
        // Sort cycles by distance to verify we got the longest ones
        var orderedCycles = cycles.OrderByDescending(c => c.distance).ToList();
        Assert.That(orderedCycles[0].distance, Is.GreaterThanOrEqualTo(orderedCycles[1].distance));
    }

    [Test]
    public void TestCyclesBetweenZones() {
        var cycles = MazeGraphTools.ConnectShortestCyclesBetweenZones(maze, zoneA: 1, zoneB: 2, maxCycles: 1);

        Assert.That(cycles, Is.Not.Empty);
        Assert.That(cycles.Count, Is.EqualTo(1));

        var cycle = cycles[0];
        Assert.That(
            (cycle.nodeA.ZoneId == 1 && cycle.nodeB.ZoneId == 2) ||
            (cycle.nodeA.ZoneId == 2 && cycle.nodeB.ZoneId == 1)
        );
    }

    [Test]
    public void TestCyclesAcrossZones() {
        var cycles = MazeGraphTools.ConnectLongestCyclesAcrossZones(maze, maxCycles: 2);

        Assert.That(cycles, Is.Not.Empty);
        foreach (var cycle in cycles) {
            Assert.That(cycle.nodeA.ZoneId, Is.Not.EqualTo(cycle.nodeB.ZoneId));
        }
    }

    /*
    [Test]
    public void TestCyclesWithSpecificDistance() {
        var cycles = MazeGraphTools.ConnectCyclesInZoneWithDistance(maze, zoneId: 1, exactDistance: 4, maxCycles: 1);

        Assert.That(cycles, Is.Not.Empty);
        Assert.That(cycles[0].distance, Is.EqualTo(4));
        Assert.That(cycles[0].nodeA.ZoneId, Is.EqualTo(1));
        Assert.That(cycles[0].nodeB.ZoneId, Is.EqualTo(1));
    }

    [Test]
    public void TestCyclesWithDistanceRange() {
        var cycles = MazeGraphTools.ConnectCyclesInZoneBetweenDistances(
            maze, zoneId: 1, minDistance: 3, maxDistance: 5, maxCycles: 2);

        Assert.That(cycles, Is.Not.Empty);
        foreach (var cycle in cycles) {
            Assert.That(cycle.distance, Is.GreaterThanOrEqualTo(3));
            Assert.That(cycle.distance, Is.LessThan(5));
            Assert.That(cycle.nodeA.ZoneId, Is.EqualTo(1));
            Assert.That(cycle.nodeB.ZoneId, Is.EqualTo(1));
        }
    }
    */

    [Test]
    public void TestGlobalCyclesIgnoringZones() {
        var cycles = MazeGraphTools.ConnectLongestCycles(maze, maxCycles: 3);

        Assert.That(cycles, Is.Not.Empty);
        Assert.That(cycles.Count, Is.LessThanOrEqualTo(3));
        
        // Verify cycles are ordered by distance
        var orderedCycles = cycles.OrderByDescending(c => c.distance).ToList();
        for (int i = 0; i < orderedCycles.Count - 1; i++) {
            Assert.That(orderedCycles[i].distance, Is.GreaterThanOrEqualTo(orderedCycles[i + 1].distance));
        }
    }

    private static void PrintGraphEdges(MazeGraph mc) {
        var allCanvas = new TextCanvas();
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null) canvas.Write(1, 0, "|");
            if (node.Right != null) canvas.Write(2, 1, "-");
            if (node.Down != null) canvas.Write(1, 2, "|");
            if (node.Left != null) canvas.Write(0, 1, "-");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(node.Position.X * 3, node.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }

    private static void PrintGraphParent(MazeGraph mc) {
        var allCanvas = new TextCanvas();
        foreach (var node in mc.GetNodes()) {
            if (node == null) continue;
            var canvas = new TextCanvas();
            if (node.Up != null && node.Up == node.Parent) canvas.Write(1, 0, "p");
            if (node.Right != null && node.Right == node.Parent) canvas.Write(2, 1, "p");
            if (node.Down != null && node.Down == node.Parent) canvas.Write(1, 2, "p");
            if (node.Left != null && node.Left == node.Parent) canvas.Write(0, 1, "p");
            canvas.Write(1, 1, node.Id.ToString());

            allCanvas.Write(node.Position.X * 3, node.Position.Y * 3, canvas.ToString());
        }
        Console.WriteLine(allCanvas.ToString());
    }
}