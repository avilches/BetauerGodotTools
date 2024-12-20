using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Collision.Spatial2D;
using Betauer.Core.PCG.GridTools;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class NavigationGridTests {
    private void PrintGrid(NavigationGrid<int> navigationGrid, IReadOnlyList<Vector2I> path = null, HashSet<Vector2I> targets = null) {
        var grid = navigationGrid.Graph.Array2D;
        var sb = new StringBuilder();
        sb.AppendLine();
        Console.WriteLine("Targets: " + string.Join(",", targets ?? []));
        Console.WriteLine("Path: " + path?.Count);
        for (int y = 0; y < grid.Height; y++) {
            for (int x = 0; x < grid.Width; x++) {
                var pos = new Vector2I(x, y);
                if (path != null && path.Contains(pos)) {
                    sb.Append("("); // Ruta
                } else {
                    sb.Append(" ");
                }
                if (targets != null && targets.Contains(pos)) {
                    sb.Append("G"); // Target
                } else if (navigationGrid.Graph.IsBlocked(pos)) {
                    sb.Append("#"); // Obstáculo
                } else {
                    sb.Append("·"); // Espacio libre
                }
                if (path != null && path.Contains(pos)) {
                    sb.Append(")"); // Ruta
                } else {
                    sb.Append(" ");
                }
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    [Test]
    public void TestDifferentHeuristics() {
        var grid = new Array2D<int>(10, 10);

        // Test con diferentes heurísticas
        var start = new Vector2I(1, 1);
        var end = new Vector2I(8, 8);

        // 1. Manhattan (actual)
        var aiGrid1 = new NavigationGrid<int>(grid);
        var pathManhattan = aiGrid1.Graph.FindPath(start, end);

        // 2. Euclidean (línea recta)
        var aiGrid2 = new NavigationGrid<int>(grid);
        var pathEuclidean = aiGrid2.Graph.FindPath(start, end, Heuristics.Euclidean);

        // 3. Diagonal (Chebyshev)
        var aiGrid3 = new NavigationGrid<int>(grid);
        var pathDiagonal = aiGrid3.Graph.FindPath(start, end, Heuristics.Chebyshev);

        // Imprimir los tres caminos
        Console.WriteLine("Manhattan distance:");
        PrintGrid(aiGrid1, pathManhattan, new HashSet<Vector2I> { end });

        Console.WriteLine("\nEuclidean distance:");
        PrintGrid(aiGrid2, pathEuclidean, new HashSet<Vector2I> { end });

        Console.WriteLine("\nDiagonal (Chebyshev) distance:");
        PrintGrid(aiGrid3, pathDiagonal, new HashSet<Vector2I> { end });
    }

    [Test]
    public void TestBasicPathFinding() {
        var grid = new Array2D<int>(10, 10);
        var aiGrid = new NavigationGrid<int>(grid);

        var start = new Vector2I(1, 1);
        var end = new Vector2I(8, 8);
        var path = aiGrid.Graph.FindPath(start, end);

        // Imprimir el grid con la ruta
        PrintGrid(aiGrid, path, new HashSet<Vector2I> { end });

        // Verificaciones
        Assert.That(path, Is.Not.Null);
        Assert.That(path[0], Is.EqualTo(start));
        Assert.That(path[^1], Is.EqualTo(end));
    }

    [Test]
    public void TestPathFindingWithObstacles() {
        var grid = new Array2D<int>(10, 10);
        var aiGrid = new NavigationGrid<int>(grid);

        var obstacles = new List<Shape>();
        // Crear una línea de obstáculos
        for (int y = 0; y < 8; y++) {
            obstacles.Add(aiGrid.AddObstacle(new Vector2I(5, y)));
        }
        for (int x = 1; x < 7; x++) {
            obstacles.Add(aiGrid.AddObstacle(new Vector2I(x, 8)));
        }

        // Encontrar camino alrededor de los obstáculos
        var start = new Vector2I(1, 5);
        var end = new Vector2I(8, 5);
        var path = aiGrid.Graph.FindPath(start, end);

        // Imprimir el grid con obstáculos y ruta
        PrintGrid(aiGrid, path, new HashSet<Vector2I> { end });

        // Verificaciones
        Assert.That(path, Is.Not.Null);
        Assert.That(path[0], Is.EqualTo(start));
        Assert.That(path[^1], Is.EqualTo(end));

        // Limpiar
        foreach (var obstacle in obstacles) {
            aiGrid.RemoveObstacle(obstacle);
        }
    }

    [Test]
    public void TestPathToClosestTarget() {
        var grid = new Array2D<int>(10, 10);
        var aiGrid = new NavigationGrid<int>(grid);

        var start = new Vector2I(1, 1);
        var targets = new List<(Vector2I pos, float weight)> {
            (new Vector2I(8, 8), 1.0f),
            (new Vector2I(3, 3), 2.0f)
        };

        var path = aiGrid.Graph.FindNearestPath(start, targets);
        PrintGrid(aiGrid, path, targets.Select(t => t.pos).ToHashSet());

        // Verificaciones
        Assert.That(path, Is.Not.Null);
        Assert.That(path[0], Is.EqualTo(start));
        Assert.That(targets.Select(t => t.pos).Contains(path[^1]));
    }

    [Test]
    public void TestCustomWeights() {
        var grid = new Array2D<int>(10, 10);

        // Crear una función de peso que hace que ciertas áreas sean más "costosas"
        Func<Vector2I, float> getWeight = pos => {
            // Hacer que el centro del mapa sea más costoso
            if (pos.X >= 4 && pos.X <= 6 && pos.Y >= 4 && pos.Y <= 6) {
                return 5.0f; // Área de alto costo
            }
            return 1.0f;
        };

        var aiGrid = new NavigationGrid<int>(grid, getWeight);

        // Encontrar camino
        var start = new Vector2I(1, 1);
        var end = new Vector2I(8, 8);
        var path = aiGrid.Graph.FindPath(start, end);

        // Imprimir el grid con la ruta
        PrintGrid(aiGrid, path);

        // Verificaciones
        Assert.That(path, Is.Not.Null);
        Assert.That(path[0], Is.EqualTo(start));
        Assert.That(path[^1], Is.EqualTo(end));

        // Contar cuántos puntos del camino pasan por el área de alto costo
        int pointsInHighCostArea = path.Count(p =>
            p.X >= 4 && p.X <= 6 && p.Y >= 4 && p.Y <= 6);

        // El camino debería preferir evitar el área de alto costo
        Console.WriteLine($"Points in high cost area: {pointsInHighCostArea}");
        Assert.That(pointsInHighCostArea, Is.LessThan(path.Count / 2),
            "Path should prefer to avoid high cost areas");
    }

    [Test]
    public void TestDifferentBlockZones() {
        var grid = new Array2D<int>(15, 15);
        var aiGrid = new NavigationGrid<int>(grid);

        var blockZones = new List<Shape>();
        
        // Añadir diferentes tipos de BlockZones
        blockZones.Add(aiGrid.AddObstacle(new Vector2I(5, 5)));
        blockZones.Add(aiGrid.AddObstacle(new Vector2I(10, 10), 2.0f));
        
        var start = new Vector2I(1, 1);
        var end = new Vector2I(13, 13);
        var path = aiGrid.Graph.FindPath(start, end);

        // Imprimir el grid
        PrintGrid(aiGrid, path, new HashSet<Vector2I> { end });

        // Verificaciones
        Assert.That(path, Is.Not.Null);
        Assert.That(path[0], Is.EqualTo(start));
        Assert.That(path[^1], Is.EqualTo(end));

        // Limpiar
        foreach (var zone in blockZones) {
            aiGrid.RemoveObstacle(zone);
        }
    }

    [Test]
    public void TestShortestPathToTarget() {
        var grid = new Array2D<int>(15, 15);
        var aiGrid = new NavigationGrid<int>(grid);

        // Create a scenario where the euclidean-closest target requires a longer path
        var start = new Vector2I(7, 1);

        // Add targets: one close but blocked, one further but more accessible
        var targets = new List<(Vector2I pos, float weight)> {
            (new Vector2I(7, 5), 1.0f), // Close but requires long path around obstacles
            (new Vector2I(3, 3), 1.0f)  // Further but with clear path
        };

        // Add obstacles that force a long path to the closest target
        var obstacles = new List<Shape>();
        for (int x = 5; x <= 9; x++) {
            obstacles.Add(aiGrid.AddObstacle(new Vector2I(x, 3)));
        }
        for (int y = 3; y <= 7; y++) {
            obstacles.Add(aiGrid.AddObstacle(new Vector2I(5, y)));
            obstacles.Add(aiGrid.AddObstacle(new Vector2I(9, y)));
        }

        // Get paths using both methods
        var closestPath = aiGrid.Graph.FindNearestPath(start, targets);
        var shortestPath = aiGrid.Graph.FindShortestPath(start, targets);

        Console.WriteLine("\nPath to closest target (euclidean distance):");
        PrintGrid(aiGrid, closestPath, targets.Select(t => t.pos).ToHashSet());

        Console.WriteLine("\nShortest actual path to a target:");
        PrintGrid(aiGrid, shortestPath, targets.Select(t => t.pos).ToHashSet());

        // Limpiar
        foreach (var obstacle in obstacles) {
            aiGrid.RemoveObstacle(obstacle);
        }

        // Calculate and print the differences
        var euclideanToClose = Heuristics.Euclidean(start, targets[0].pos);
        var euclideanToFurther = Heuristics.Euclidean(start, targets[1].pos);
        Console.WriteLine($"Euclidean distances - Close target: {euclideanToClose:F2}, Further target: {euclideanToFurther:F2}");
        Console.WriteLine($"Actual path lengths - Closest: {closestPath.Count}, Shortest: {shortestPath.Count}");
    }
}