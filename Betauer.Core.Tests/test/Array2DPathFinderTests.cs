using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Graph;
using Betauer.Core.PCG.GridTools;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class Array2DPathFinderTests {
    [Test]
    public void FindPath_EmptyGrid_ReturnsCorrectPath() {
        // Create a 3x3 grid where all cells are walkable
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell, // isWalkable
            _ => 1f // Uniform cost
        );

        // Find path from top-left to bottom-right
        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        // Assert path exists and has correct length
        Assert.NotNull(path);
        Assert.That(path, Has.Count.EqualTo(5)); // Start + 3 moves + end = 5 positions

        // Verify start and end positions
        Assert.That(path[0], Is.EqualTo(new Vector2I(0, 0)));
        Assert.That(path[^1], Is.EqualTo(new Vector2I(2, 2)));

        // Verify each move is adjacent to the previous one
        for (var i = 1; i < path.Count; i++) {
            var diff = path[i] - path[i - 1];
            Assert.That(Math.Abs(diff.X) + Math.Abs(diff.Y), Is.EqualTo(1),
                "Each move should be to an adjacent cell");
        }
    }

    [Test]
    public void FindPath_WithObstacles_FindsAlternatePath() {
        // Create a 3x3 grid with an obstacle in the middle
        var grid = new Array2D<bool>(3, 3, true);
        grid[1, 1] = false; // Center cell is blocked
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.NotNull(path);
        Assert.That(path, Has.Count.EqualTo(5)); // Should go around the obstacle
        Assert.That(path, Has.None.EqualTo(new Vector2I(1, 1))); // Verify the path doesn't include the obstacle
    }

    [Test]
    public void FindPath_NoPathExists_ReturnsNull() {
        // Create a 3x3 grid with a wall that completely separates start from end
        var grid = new Array2D<bool>(3, 3, true);
        grid[0, 1] = false;
        grid[1, 1] = false;
        grid[2, 1] = false;
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.Null(path, "Path should be null when there is no possible route");
    }

    [Test]
    public void FindPath_PreferLowerWeightPath() {
        // Create a 3x3 grid with different weights
        // 1  10 1
        // 1  1  1
        // 1  1  1
        var grid = new Array2D<int>(3, 3, 1); // All cells cost 1
        grid[0, 1] = 10; // grid[y, x] para la posición (1,0)
    
        var graph = new Array2DGraph<int>(
            grid,
            _ => true, // All cells walkable
            cell => cell // Cost equals cell value
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 0), Heuristics.Manhattan);

        Assert.NotNull(path);
        Console.WriteLine("Path found:");
        foreach (var pos in path) {
            Console.WriteLine($"Position: {pos}, Weight: {grid[pos.Y, pos.X]}");
        }
    
        Assert.That(path, Has.Count.EqualTo(5)); // Longer but cheaper path
        Assert.That(path, Has.None.EqualTo(new Vector2I(1, 0))); // Should avoid the expensive cell
    
        // Verify the exact path
        CollectionAssert.AreEqual(new[] {
            new Vector2I(0, 0),
            new Vector2I(0, 1),
            new Vector2I(1, 1),
            new Vector2I(2, 1),
            new Vector2I(2, 0)
        }, path);
    }
    
    [Test]
    public void FindPath_VerifyHeuristicEfficiency() {
        // Create a larger grid to make the difference more obvious
        var grid = new Array2D<bool>(5, 5, true);
        var visitedNodes = new List<Vector2I>();
    
        // Una función que registra los nodos visitados en orden
        void TrackVisitedNode(Vector2I pos) {
            visitedNodes.Add(pos);
        }

        var graph = new Array2DGraph<bool>(
            grid,
            _ => true,
            _ => 1f
        );

        // Test con diferentes heurísticas
        void TestHeuristic(string name, Func<Vector2I, Vector2I, float> heuristic) {
            visitedNodes.Clear();

            var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(4, 4), heuristic, TrackVisitedNode);
        
            Console.WriteLine($"\nHeuristic: {name}");
            Console.WriteLine($"Nodes explored: {visitedNodes.Count}");
            Console.WriteLine("Exploration order:");
            foreach (var pos in visitedNodes) {
                Console.WriteLine($"  {pos}");
            }
        }

        // Dijkstra (sin heurística)
        TestHeuristic("Dijkstra", (_, _) => 0f);
        var dijkstraCount = visitedNodes.Count;

        // Manhattan
        TestHeuristic("Manhattan", Heuristics.Manhattan);
        var manhattanCount = visitedNodes.Count;

        // La heurística Manhattan debería explorar menos nodos que Dijkstra
        Assert.That(manhattanCount, Is.LessThan(dijkstraCount), 
            "A* with Manhattan heuristic should explore fewer nodes than Dijkstra");
    }    

    [Test]
    public void FindPath_WithDifferentHeuristics() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[1, 1] = false; // Obstacle in the middle

        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        // Find paths with different heuristics
        var pathManhattan = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2), Heuristics.Manhattan);
        var pathEuclidean = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2), Heuristics.Euclidean);
        var pathChebyshev = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2), Heuristics.Chebyshev);

        Assert.NotNull(pathManhattan);
        Assert.NotNull(pathEuclidean);
        Assert.NotNull(pathChebyshev);

        // All paths should be valid and avoid the obstacle
        Assert.That(pathManhattan, Has.None.EqualTo(new Vector2I(1, 1)));
        Assert.That(pathEuclidean, Has.None.EqualTo(new Vector2I(1, 1)));
        Assert.That(pathChebyshev, Has.None.EqualTo(new Vector2I(1, 1)));
    }

    [Test]
    public void FindPath_WithCustomHeuristic() {
        var grid = new Array2D<bool>(3, 3, true);
        
        // Custom heuristic that always returns 0 (will make A* behave like Dijkstra)
        float ZeroHeuristic(Vector2I a, Vector2I b) => 0f;
        
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2), ZeroHeuristic);

        Assert.NotNull(path);
        Assert.That(path, Has.Count.EqualTo(5));
    }

    [Test]
    public void FindPath_InvalidStartPosition_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        var path = graph.FindPath(new Vector2I(-1, 0), new Vector2I(2, 2));

        Assert.Null(path);
    }

    [Test]
    public void FindPath_InvalidEndPosition_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(3, 3));

        Assert.Null(path);
    }

    [Test]
    public void FindPath_StartPositionNotWalkable_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[0, 0] = false;
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.Null(path);
    }

    [Test]
    public void FindPath_EndPositionNotWalkable_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[2, 2] = false;
        var graph = new Array2DGraph<bool>(
            grid, 
            cell => cell,
            _ => 1f
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.Null(path);
    }
}