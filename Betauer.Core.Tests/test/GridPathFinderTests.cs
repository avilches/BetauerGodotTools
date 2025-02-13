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
public class GridPathFinderTests {
    [TestCase(false, 5)]
    [TestCase(true, 3)]
    public void FindPath_EmptyGrid_ReturnsCorrectPath(bool diagonal, int expectedLength) {
        // Create a 3x3 grid where all cells are walkable
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
        );

        graph.EnableDiagonalMovement(diagonal);

        // Find path from top-left to bottom-right
        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        // Assert path exists and has correct length
        Assert.NotNull(path);
        Assert.That(path, Has.Count.EqualTo(expectedLength)); // Start + 3 moves + end = 5 positions

        // Verify start and end positions
        Assert.That(path[0], Is.EqualTo(new Vector2I(0, 0)));
        Assert.That(path[^1], Is.EqualTo(new Vector2I(2, 2)));

        if (!diagonal) {
            // Verify each move is adjacent to the previous one
            for (var i = 1; i < path.Count; i++) {
                var diff = path[i] - path[i - 1];
                Assert.That(Math.Abs(diff.X) + Math.Abs(diff.Y), Is.EqualTo(1),
                    "Each move should be to an adjacent cell");
            }
        }
    }

    [Test]
    public void FindPath_WithObstacles_FindsAlternatePath() {
        // Create a 3x3 grid with an obstacle in the middle
        var grid = new Array2D<bool>(3, 3, true);
        grid[1, 1] = false; // Center cell is blocked
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
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
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
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

        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            _ => false, // All cells walkable
            cell => grid[cell] // Cost equals cell value
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

        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            _ => false, // All cells walkable
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

        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
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

        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2), ZeroHeuristic);

        Assert.NotNull(path);
        Assert.That(path, Has.Count.EqualTo(5));
    }

    [Test]
    public void FindPath_InvalidStartPosition_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
        );

        var path = graph.FindPath(new Vector2I(-1, 0), new Vector2I(2, 2));

        Assert.IsEmpty(path);
    }

    [Test]
    public void FindPath_InvalidEndPosition_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(3, 3));

        Assert.IsEmpty(path);
    }

    [Test]
    public void FindPath_StartPositionNotWalkable_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[0, 0] = false;
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.IsEmpty(path);
    }

    [Test]
    public void FindPath_EndPositionNotWalkable_ReturnsNull() {
        var grid = new Array2D<bool>(3, 3, true);
        grid[2, 2] = false;
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f // Uniform cost
        );

        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(2, 2));

        Assert.IsEmpty(path);
    }

    [Test]
    public void GetReachableZone_EmptyGrid_ReturnsAllNodes() {
        // 3x3 grid, all walkable
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
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
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
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
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
        );

        var nodes = graph.GetReachableZone(new Vector2I(0, 0));

        Assert.That(nodes, Has.Count.EqualTo(8)); // All except the center
        Assert.That(nodes, Does.Not.Contain(new Vector2I(1, 1))); // Center should not be included
    }

    [Test]
    public void GetReachableZone_WithMaxNodes_LimitsResults() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
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

        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
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
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
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

        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
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
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell], // isBlocked
            _ => 1f
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

    [Test]
    public void TestDiagonalMovementModes() {
        var grid = new Array2D<bool>(5, 5, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            _ => false, // All cells walkable
            _ => 1f // Uniform cost
        );

        var start = new Vector2I(0, 0);
        var end = new Vector2I(4, 4);

        // Test with diagonals disabled (default)
        Console.WriteLine($"DiagonalWeight: {graph.DiagonalWeight}");
        Console.WriteLine($"IsDiagonalMovementEnabled: {graph.IsDiagonalMovementEnabled()}");

        // Test with diagonals disabled (default)
        var pathNodiagonal = graph.FindPath(start, end);
        Console.WriteLine("Path with no diagonals:");
        foreach (var pos in pathNodiagonal) {
            Console.WriteLine($"Position: {pos}");
        }

        // Debug: Imprimir las diferencias entre posiciones consecutivas
        Console.WriteLine("\nMoves:");
        for (var i = 1; i < pathNodiagonal.Count; i++) {
            var diff = pathNodiagonal[i] - pathNodiagonal[i - 1];
            Console.WriteLine($"Move from {pathNodiagonal[i - 1]} to {pathNodiagonal[i]}: diff=({diff.X},{diff.Y})");
            // Si abs(X) + abs(Y) = 2, es un movimiento diagonal
            if (Math.Abs(diff.X) + Math.Abs(diff.Y) > 1) {
                Console.WriteLine("WARNING: This is a diagonal move!");
            }
        }

        // Debug: Imprimir edges adyacentes
        Console.WriteLine("\nAdjacent edges from start position:");
        foreach (var edge in graph.Adjacent(start)) {
            Console.WriteLine($"Edge to {edge.To} with weight {edge.Weight}");
        }

        // Verificar que el camino está conectado y solo usa movimientos ortogonales
        Assert.That(pathNodiagonal[0], Is.EqualTo(start), "Path should start at start position");
        Assert.That(pathNodiagonal[^1], Is.EqualTo(end), "Path should end at end position");

        for (var i = 1; i < pathNodiagonal.Count; i++) {
            var diff = pathNodiagonal[i] - pathNodiagonal[i - 1];
            Assert.That(Math.Abs(diff.X) + Math.Abs(diff.Y), Is.EqualTo(1),
                $"Each move should be orthogonal, but move from {pathNodiagonal[i - 1]} to {pathNodiagonal[i]} isn't");
        }

        // Debug: Imprimir las diferencias entre posiciones consecutivas
        Console.WriteLine("\nMoves:");
        for (var i = 1; i < pathNodiagonal.Count; i++) {
            var diff = pathNodiagonal[i] - pathNodiagonal[i - 1];
            Console.WriteLine($"Move from {pathNodiagonal[i - 1]} to {pathNodiagonal[i]}: diff=({diff.X},{diff.Y})");
            // Si abs(X) + abs(Y) = 2, es un movimiento diagonal
            if (Math.Abs(diff.X) + Math.Abs(diff.Y) > 1) {
                Console.WriteLine("WARNING: This is a diagonal move!");
            }
        }

        // Test adjacent edges for the first position
        Console.WriteLine("\nAdjacent edges from start position:");
        foreach (var edge in graph.Adjacent(start)) {
            Console.WriteLine($"Edge to {edge.To} with weight {edge.Weight}");
        }

        // Verificar que no hay movimientos diagonales
        for (var i = 1; i < pathNodiagonal.Count; i++) {
            var diff = pathNodiagonal[i] - pathNodiagonal[i - 1];
            Assert.That(Math.Abs(diff.X) + Math.Abs(diff.Y), Is.EqualTo(1),
                "Each move should be to an adjacent cell");
        }

        Assert.That(pathNodiagonal.Count, Is.EqualTo(9),
            $"Expected 9 positions but got {pathNodiagonal.Count}. Path: {string.Join(" -> ", pathNodiagonal)}");

        // Test with equal cost diagonals
        graph.EnableDiagonalMovement();
        var pathDiagonal = graph.FindPath(start, end);
        Console.WriteLine($"\nDiagonal path count: {pathDiagonal.Count}");
        Console.WriteLine("Diagonal path positions:");
        foreach (var pos in pathDiagonal) {
            Console.WriteLine($"Position: {pos}");
        }

        Assert.That(pathDiagonal.Count, Is.EqualTo(5),
            $"Expected 5 positions but got {pathDiagonal.Count}. Path: {string.Join(" -> ", pathDiagonal)}");

        // Verify the exact diagonal path
        CollectionAssert.AreEqual(new[] {
            new Vector2I(0, 0),
            new Vector2I(1, 1),
            new Vector2I(2, 2),
            new Vector2I(3, 3),
            new Vector2I(4, 4)
        }, pathDiagonal);

        // Test with physical (√2) cost diagonals
        graph.EnablePhysicalDiagonalMovement();
        var pathPhysical = graph.FindPath(start, end);
        Assert.That(pathPhysical, Has.Count.EqualTo(5)); // Still diagonal, as √2 < 2
        CollectionAssert.AreEqual(pathDiagonal, pathPhysical); // Should be the same path

        // Test with expensive diagonals
        graph.SetDiagonalMovementWeight(15f);
        var pathExpensive = graph.FindPath(start, end);
        Assert.That(pathExpensive, Has.Count.EqualTo(9)); // Should prefer orthogonal movement

        // Test disabling diagonals
        graph.DisableDiagonalMovement();
        var pathDisabled = graph.FindPath(start, end);
        Assert.That(pathDisabled, Has.Count.EqualTo(9)); // Back to orthogonal only
    }

    [Test]
    public void TestDiagonalMovementWithObstacles() {
        // Create a grid with obstacles forming a diagonal wall
        // [S][ ][ ][ ]
        // [ ][#][ ][ ]
        // [ ][ ][#][ ]
        // [ ][ ][ ][E]
        var grid = new Array2D<bool>(4, 4, true);
        grid[1, 1] = false; // Obstacles
        grid[2, 2] = false;

        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell],
            _ => 1f
        );

        var start = new Vector2I(0, 0);
        var end = new Vector2I(3, 3);

        // Test with diagonals enabled
        graph.EnableDiagonalMovement();
        var pathWithDiagonals = graph.FindPath(start, end);
        Assert.That(pathWithDiagonals, Is.Not.Empty);
        Console.WriteLine("Path with diagonals:");
        Console.WriteLine(string.Join(" -> ", pathWithDiagonals));

        // Should find path around obstacles using both diagonal and orthogonal moves
        Assert.That(pathWithDiagonals, Has.None.EqualTo(new Vector2I(1, 1)));
        Assert.That(pathWithDiagonals, Has.None.EqualTo(new Vector2I(2, 2)));

        // Test with diagonals disabled
        graph.DisableDiagonalMovement();
        var pathWithoutDiagonals = graph.FindPath(start, end);
        Assert.That(pathWithoutDiagonals, Is.Not.Empty);
        Console.WriteLine("Path without diagonals:");
        Console.WriteLine(string.Join(" -> ", pathWithoutDiagonals));

        // Path should be different and only use orthogonal moves
        Assert.That(pathWithoutDiagonals.Count, Is.GreaterThan(pathWithDiagonals.Count));
    }

    [Test]
    public void TestInvalidDiagonalWeight() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new GridGraph(grid.Width, grid.Height, _ => false);

        // Should throw for weights less than 1.0
        Assert.Throws<ArgumentException>(() => graph.SetDiagonalMovementWeight(0.5f));
        Assert.Throws<ArgumentException>(() => graph.SetDiagonalMovementWeight(0f));
        Assert.Throws<ArgumentException>(() => graph.SetDiagonalMovementWeight(-1f));

        // Should not throw for weights >= 1.0
        Assert.DoesNotThrow(() => graph.SetDiagonalMovementWeight(1.0f));
        Assert.DoesNotThrow(() => graph.SetDiagonalMovementWeight(1.5f));
    }

    [Test]
    public void TestDiagonalWeightOptimization() {
        // Create a 5x5 grid where all cells are walkable
        var grid = new Array2D<bool>(5, 5, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            _ => false, // No blocked cells
            _ => 1f // Uniform cost
        );

        var start = new Vector2I(0, 0);
        var end = new Vector2I(4, 4);

        // Test with equal cost diagonals
        graph.EnableDiagonalMovement();
        var pathDiagonal = graph.FindPath(start, end);
        Console.WriteLine($"\nDiagonal path count: {pathDiagonal.Count}");
        Console.WriteLine("Diagonal path positions:");
        foreach (var pos in pathDiagonal) {
            Console.WriteLine($"Position: {pos}");
        }

        Assert.That(pathDiagonal.Count, Is.EqualTo(5),
            $"Expected 5 positions but got {pathDiagonal.Count}. Path: {string.Join(" -> ", pathDiagonal)}");

        // Verify the exact diagonal path
        CollectionAssert.AreEqual(new[] {
            new Vector2I(0, 0),
            new Vector2I(1, 1),
            new Vector2I(2, 2),
            new Vector2I(3, 3),
            new Vector2I(4, 4)
        }, pathDiagonal);

        // Test with physical (√2) cost diagonals
        graph.EnablePhysicalDiagonalMovement();
        var pathPhysical = graph.FindPath(start, end);
        Assert.That(pathPhysical, Has.Count.EqualTo(5)); // Still diagonal, as √2 < 2
        CollectionAssert.AreEqual(pathDiagonal, pathPhysical); // Should be the same path

        // Test with expensive diagonals
        graph.SetDiagonalMovementWeight(15f);
        var pathExpensive = graph.FindPath(start, end);
        Assert.That(pathExpensive, Has.Count.EqualTo(9)); // Should prefer orthogonal movement

        // Test disabling diagonals
        graph.DisableDiagonalMovement();
        var pathDisabled = graph.FindPath(start, end);
        Assert.That(pathDisabled, Has.Count.EqualTo(9)); // Back to orthogonal only
    }

    [Test]
    public void TestDiagonalCornerCutting() {
        // Create a 3x3 grid with obstacles in corners
        // [S][ ][·]
        // [ ][#][·]
        // [·][·][E]
        var grid = new Array2D<bool>(3, 3, true);
        grid[1, 1] = false; // Center obstacle
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            cell => !grid[cell],
            _ => 1f
        );

        graph.EnableDiagonalMovement();
        var start = new Vector2I(0, 0);
        var end = new Vector2I(2, 2);

        var path = graph.FindPath(start, end);

        Console.WriteLine($"Path found: {string.Join(" -> ", path)}");

        // Verificar que el camino existe y no pasa por el obstáculo
        Assert.That(path, Is.Not.Empty, "Should find a path");
        Assert.That(path[0], Is.EqualTo(start), "Path should start at start position");
        Assert.That(path[^1], Is.EqualTo(end), "Path should end at end position");
        Assert.That(path, Has.None.EqualTo(new Vector2I(1, 1)), "Path should not go through the obstacle");

        // Verificar que todos los movimientos son válidos (no corta esquinas)
        for (var i = 1; i < path.Count; i++) {
            var current = path[i];
            var previous = path[i - 1];
            var diff = current - previous;

            // Si es un movimiento diagonal, verificar que las casillas adyacentes no son obstáculos
            if (Math.Abs(diff.X) + Math.Abs(diff.Y) == 2) {
                var adjacentPos1 = new Vector2I(previous.X + diff.X, previous.Y);
                var adjacentPos2 = new Vector2I(previous.X, previous.Y + diff.Y);

                Assert.That(graph.IsAccesible(adjacentPos1) || graph.IsAccesible(adjacentPos2),
                    $"Diagonal move from {previous} to {current} cuts through obstacles");
            }
        }
    }

    [Test]
    public void TestGetReachableZoneWithDiagonals() {
        var grid = new Array2D<bool>(5, 5, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            _ => false,
            _ => 1f
        );

        var center = new Vector2I(2, 2);

        // Test without diagonals
        graph.DisableDiagonalMovement();
        var zoneNodiagonal = graph.GetReachableZoneInRange(center, 1);
        Assert.That(zoneNodiagonal.Count, Is.EqualTo(5)); // Center + 4 orthogonal neighbors

        // Verify orthogonal neighbors
        Assert.That(zoneNodiagonal, Contains.Item(center));
        Assert.That(zoneNodiagonal, Contains.Item(new Vector2I(1, 2))); // Left
        Assert.That(zoneNodiagonal, Contains.Item(new Vector2I(3, 2))); // Right
        Assert.That(zoneNodiagonal, Contains.Item(new Vector2I(2, 1))); // Up
        Assert.That(zoneNodiagonal, Contains.Item(new Vector2I(2, 3))); // Down

        // Test with diagonals
        graph.EnableDiagonalMovement();
        var zoneWithDiagonal = graph.GetReachableZoneInRange(center, 1);
        Assert.That(zoneWithDiagonal.Count, Is.EqualTo(9)); // Center + 4 orthogonal + 4 diagonal

        // Verify diagonal neighbors
        Assert.That(zoneWithDiagonal, Contains.Item(new Vector2I(1, 1))); // Up-Left
        Assert.That(zoneWithDiagonal, Contains.Item(new Vector2I(3, 1))); // Up-Right
        Assert.That(zoneWithDiagonal, Contains.Item(new Vector2I(1, 3))); // Down-Left
        Assert.That(zoneWithDiagonal, Contains.Item(new Vector2I(3, 3))); // Down-Right
    }

    [Test]
    public void TestDiagonalEdgeCases() {
        var grid = new Array2D<bool>(4, 4, true);
        var graph = new GridGraph(
            grid.Width,
            grid.Height,
            _ => false,
            _ => 1f
        );

        graph.EnableDiagonalMovement();

        // Test diagonal movement from corners
        var cornerStart = new Vector2I(0, 0);
        var cornerEnd = new Vector2I(3, 3);
        var cornerPath = graph.FindPath(cornerStart, cornerEnd);
        Assert.That(cornerPath.Count, Is.EqualTo(4)); // Should be a straight diagonal line

        // Test diagonal movement along edges
        var edgeStart = new Vector2I(0, 0);
        var edgeEnd = new Vector2I(3, 0);
        var edgePath = graph.FindPath(edgeStart, edgeEnd);
        Assert.That(edgePath.Count, Is.EqualTo(4)); // Should be a straight horizontal line

        // Test with equal cost diagonals (weight = 1)
        var pathWithWeight1 = graph.FindPath(cornerStart, cornerEnd);
        Console.WriteLine($"Path with weight 1: {string.Join(" -> ", pathWithWeight1)}");
        Assert.That(pathWithWeight1, Has.Count.EqualTo(4)); // Diagonal path
        // Verify it's a pure diagonal path
        for (var i = 0; i < pathWithWeight1.Count; i++) {
            Assert.That(pathWithWeight1[i], Is.EqualTo(new Vector2I(i, i)));
        }

        // With high diagonal weight, should prefer orthogonal moves
        graph.SetDiagonalMovementWeight(15f); // Using a much higher weight to force orthogonal path
        var pathWithHighWeight = graph.FindPath(cornerStart, cornerEnd);
        Console.WriteLine($"Path with high weight: {string.Join(" -> ", pathWithHighWeight)}");
        Assert.That(pathWithHighWeight, Has.Count.EqualTo(7)); // Should be longer, using orthogonal moves
        // Verify only orthogonal moves
        for (var i = 1; i < pathWithHighWeight.Count; i++) {
            var diff = pathWithHighWeight[i] - pathWithHighWeight[i - 1];
            Assert.That(Math.Abs(diff.X) + Math.Abs(diff.Y), Is.EqualTo(1),
                "Path should only use orthogonal moves when diagonal weight is high");
        }

        // Test with diagonal movement disabled
        graph.DisableDiagonalMovement();
        var nodiagonalPath = graph.FindPath(cornerStart, cornerEnd);
        Assert.That(nodiagonalPath.Count, Is.EqualTo(7)); // Should use only orthogonal moves
        // Verify no diagonal moves
        for (var i = 1; i < nodiagonalPath.Count; i++) {
            var diff = nodiagonalPath[i] - nodiagonalPath[i - 1];
            Assert.That(Math.Abs(diff.X) + Math.Abs(diff.Y), Is.EqualTo(1),
                "Path should only use orthogonal moves when diagonals are disabled");
        }
    }

    [Test]
    public void BasicDiagonalMovement() {
        // Create a grid with diagonals enabled from the start
        var grid = new Array2D<bool>(5, 5, true);
        var graph = new GridGraph(grid.Width, grid.Height, _ => false);
        graph.EnableDiagonalMovement();

        // Test path from corner to corner
        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(4, 4));

        Assert.That(path, Has.Count.EqualTo(5)); // Should be a direct diagonal path

        // Verify it's a pure diagonal path
        for (var i = 0; i < path.Count; i++) {
            Assert.That(path[i], Is.EqualTo(new Vector2I(i, i)),
                "Path should follow diagonal line");
        }
    }

    [Test]
    public void GetReachableZoneWithDiagonalsEnabled() {
        var grid = new Array2D<bool>(3, 3, true);
        var graph = new GridGraph(grid.Width, grid.Height, _ => false);
        graph.EnableDiagonalMovement();

        var center = new Vector2I(1, 1);
        var zone = graph.GetReachableZoneInRange(center, 1);

        // Should include center + 4 orthogonal + 4 diagonal = 9 cells
        Assert.That(zone, Has.Count.EqualTo(9));

        // Check orthogonal neighbors
        Assert.That(zone, Contains.Item(center)); // Center
        Assert.That(zone, Contains.Item(new Vector2I(0, 1))); // Left
        Assert.That(zone, Contains.Item(new Vector2I(2, 1))); // Right
        Assert.That(zone, Contains.Item(new Vector2I(1, 0))); // Up
        Assert.That(zone, Contains.Item(new Vector2I(1, 2))); // Down

        // Check diagonal neighbors
        Assert.That(zone, Contains.Item(new Vector2I(0, 0))); // Up-Left
        Assert.That(zone, Contains.Item(new Vector2I(2, 0))); // Up-Right
        Assert.That(zone, Contains.Item(new Vector2I(0, 2))); // Down-Left
        Assert.That(zone, Contains.Item(new Vector2I(2, 2))); // Down-Right
    }

    [Test]
    public void DiagonalPathWithObstacles() {
        // Create a 4x4 grid with an L-shaped obstacle
        var grid = new Array2D<bool>(4, 4, true);
        grid[1, 1] = false; // Part of L
        grid[1, 2] = false; // Part of L
        grid[2, 1] = false; // Part of L

        var graph = new GridGraph(grid.Width, grid.Height, cell => !grid[cell]);
        graph.EnableDiagonalMovement();

        // Try to find a path that would normally go through the L obstacle
        var path = graph.FindPath(new Vector2I(0, 0), new Vector2I(3, 3));

        // Path should exist but should go around the obstacle
        Assert.That(path, Is.Not.Empty);
        Assert.That(path, Has.None.EqualTo(new Vector2I(1, 1))); // Should not pass through obstacles
        Assert.That(path, Has.None.EqualTo(new Vector2I(1, 2)));
        Assert.That(path, Has.None.EqualTo(new Vector2I(2, 1)));

        // Verify start and end points
        Assert.That(path[0], Is.EqualTo(new Vector2I(0, 0)));
        Assert.That(path[^1], Is.EqualTo(new Vector2I(3, 3)));

        // Print path for debugging
        Console.WriteLine("Path found:");
        foreach (var pos in path) {
            Console.WriteLine($"Position: {pos}");
        }
    }
}