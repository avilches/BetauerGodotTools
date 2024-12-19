using System;
using System.Collections.Generic;
using System.Linq;
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
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
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
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
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
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.IsEmpty(path, "Path should be null when there is no possible route");
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
            cell => grid[cell], // Cost equals cell value
            _ => true // All cells walkable
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
            _ => 1f,
            _ => true
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
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
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
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
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
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
        );

        var path = graph.FindPath(new Vector2I(-1, 0), new Vector2I(2, 2));

        Assert.IsEmpty(path);
    }

    [Test]
    public void FindPath_InvalidEndPosition_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(3, 3));

        Assert.IsEmpty(path);
    }

    [Test]
    public void FindPath_StartPositionNotWalkable_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[0, 0] = false;
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.IsEmpty(path);
    }

    [Test]
    public void FindPath_EndPositionNotWalkable_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[2, 2] = false;
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f, // Uniform cost
            cell => grid[cell] // isWalkable
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.IsEmpty(path);
    }

    [Test]
    public void GetReachableZone_EmptyGrid_ReturnsAllNodes() {
        // 3x3 grid, all walkable
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        var nodes = graph.GetReachableZone(new Vector2I(1, 1));

        // Should return all 9 nodes
        Assert.That(nodes, Has.Count.EqualTo(9));
        // Verify all positions are included
        for (var y = 0; y < 3; y++) {
            for (var x = 0; x < 3; x++) {
                Assert.That(nodes, Contains.Item(new Vector2I(x, y)));
            }
        }
    }

    [Test]
    public void GetReachableZone_StartNotWalkable_ReturnsEmpty() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[1, 1] = false; // Center not walkable
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        var nodes = graph.GetReachableZone(new Vector2I(1, 1));

        Assert.That(nodes, Is.Empty);
    }

    [Test]
    public void GetReachableZone_WithObstacles_ReturnsReachableNodes() {
        // Create a grid with a wall in the middle
        // [ ][ ][ ]
        // [ ][X][ ]
        // [ ][ ][ ]
        var grid = new Array2D<bool>(3, 3, true);
        grid[1, 1] = false; // Center is blocked
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        var nodes = graph.GetReachableZone(new Vector2I(0, 0));

        Assert.That(nodes, Has.Count.EqualTo(8)); // All except the center
        Assert.That(nodes, Does.Not.Contain(new Vector2I(1, 1))); // Center should not be included
    }

    [Test]
    public void GetReachableZone_WithMaxNodes_LimitsResults() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        var nodes = graph.GetReachableZone(new Vector2I(1, 1), 4);

        Assert.That(nodes, Has.Count.EqualTo(4));
        // Center should be included as it's the start
        Assert.That(nodes, Contains.Item(new Vector2I(1, 1)));
    }

    [Test]
    public void GetReachableZone_IsolatedAreas_ReturnsOnlyReachableNodes() {
        // Create a grid with two isolated areas
        // [ ][ ][X][ ]
        // [ ][X][X][ ]
        // [X][X][X][ ]
        // [ ][ ][ ][ ]
        var grid = new Array2D<bool>(4, 4, true);
        grid[0, 2] = false;
        grid[1, 1] = false;
        grid[1, 2] = false;
        grid[2, 0] = false;
        grid[2, 1] = false;
        grid[2, 2] = false;

        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        // Start from top-left area
        var nodesTopLeft = graph.GetReachableZone(new Vector2I(0, 0));
        // Start from bottom-right area
        var nodesBottomRight = graph.GetReachableZone(new Vector2I(3, 3));

        Assert.That(nodesTopLeft, Has.Count.EqualTo(3)); // [0,0], [1,0], and [0,1] are reachable
        Assert.That(nodesBottomRight, Has.Count.EqualTo(7)); // Toda la columna derecha y la fila inferior

        // Verificar explícitamente las posiciones alcanzables desde la esquina superior izquierda
        Assert.That(nodesTopLeft, Contains.Item(new Vector2I(0, 0))); // Posición inicial
        Assert.That(nodesTopLeft, Contains.Item(new Vector2I(1, 0))); // Derecha
        Assert.That(nodesTopLeft, Contains.Item(new Vector2I(0, 1))); // Abajo

        // Verificar que las posiciones bloqueadas no son alcanzables
        Assert.That(nodesTopLeft, Does.Not.Contain(new Vector2I(2, 0))); // Bloqueado por pared
        Assert.That(nodesTopLeft, Does.Not.Contain(new Vector2I(1, 1))); // Pared

        // Verificar las posiciones alcanzables desde la esquina inferior derecha
        var expectedBottomRight = new[] {
            new Vector2I(3, 3), // Posición inicial
            new Vector2I(2, 3), // Fila inferior
            new Vector2I(1, 3),
            new Vector2I(0, 3),
            new Vector2I(3, 2), // Columna derecha
            new Vector2I(3, 1),
            new Vector2I(3, 0)
        };

        foreach (var pos in expectedBottomRight) {
            Assert.That(nodesBottomRight, Contains.Item(pos),
                $"La posición {pos} debería ser alcanzable desde (3,3)");
        }
    }

    [Test]
    public void GetReachableZoneInRange_VerifyCircularExpansion() {
        var grid = new Array2D<bool>(5, 5, true);
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        // Test with different ranges
        var range1 = graph.GetReachableZoneInRange(new Vector2I(2, 2), 1);
        var range2 = graph.GetReachableZoneInRange(new Vector2I(2, 2), 2);

        // Range 1 should include center + 4 adjacent = 5 nodes
        Assert.That(range1, Has.Count.EqualTo(5));
        // Verify the shape: should include only direct neighbors
        Assert.That(range1, Contains.Item(new Vector2I(2, 2))); // Center
        Assert.That(range1, Contains.Item(new Vector2I(1, 2))); // Left
        Assert.That(range1, Contains.Item(new Vector2I(3, 2))); // Right
        Assert.That(range1, Contains.Item(new Vector2I(2, 1))); // Up
        Assert.That(range1, Contains.Item(new Vector2I(2, 3))); // Down

        // Range 2 should include more nodes in a diamond pattern
        Assert.That(range2, Has.Count.EqualTo(13));
    }

    [Test]
    public void GetReachableZoneInRange_WithObstacles_RespectsWalls() {
        // Create a grid with a L-shaped wall
        // [ ][ ][ ][ ]
        // [ ][X][X][ ]
        // [ ][X][ ][ ]
        // [ ][ ][ ][ ]
        var grid = new Array2D<bool>(4, 4, true);
        grid[1, 1] = false;
        grid[1, 2] = false;
        grid[2, 1] = false;

        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        var nodes = graph.GetReachableZoneInRange(new Vector2I(0, 0), 2);

        // Verify nodes behind the wall are not included
        Assert.That(nodes, Does.Not.Contain(new Vector2I(2, 2)));
        // Verify nodes that are within range but blocked by wall are not included
        Assert.That(nodes, Does.Not.Contain(new Vector2I(1, 1)));
        Assert.That(nodes, Does.Not.Contain(new Vector2I(1, 2)));
        Assert.That(nodes, Does.Not.Contain(new Vector2I(2, 1)));
    }

    [Test]
    public void GetReachableZoneInRange_EdgeCases() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new Array2DGraph<bool>(
            grid,
            _ => 1f,
            cell => grid[cell]
        );

        // Test range 0
        var range0 = graph.GetReachableZoneInRange(new Vector2I(1, 1), 0);
        Assert.That(range0, Has.Count.EqualTo(1)); // Only start position

        // Test negative range (should throw)
        Assert.Throws<ArgumentException>(() => { graph.GetReachableZoneInRange(new Vector2I(1, 1), -1); });

        // Test range larger than grid
        var rangeLarge = graph.GetReachableZoneInRange(new Vector2I(1, 1), 10);
        Assert.That(rangeLarge, Has.Count.EqualTo(9)); // Should include all walkable cells
    }
}