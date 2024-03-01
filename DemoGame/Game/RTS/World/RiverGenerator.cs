using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Data;
using Betauer.Core.Image;
using Godot;

namespace Veronenger.Game.RTS.World;

/// <summary>
/// Generate rivers moving pixels from the highest points to the lowest points until reach the sea
/// </summary>
public class RiverGenerator {
    private static float initialWater = 1f;
    private static float increaseWater = 0.01f;

    public static void GenerateRivers(BiomeCell[,] biomeCells, int numberOfPoints, float minDistance, Random random) {
        FindRiverStartPoints(biomeCells, numberOfPoints, minDistance).ForEach((startPoint) => {
            var cell = biomeCells[startPoint.X, startPoint.Y];
            SimulateRiverFlow(biomeCells, cell, random);
        });
    }

    public static List<Vector2I> FindRiverStartPoints(BiomeCell[,] biomeCells, int numberOfPoints, float minDistance) {
        var width = biomeCells.GetLength(0);
        var height = biomeCells.GetLength(1);
        var highestPoints = new List<Vector2I>();
        var pointsWithHeight = new List<KeyValuePair<Vector2I, float>>();

        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                if (biomeCells[x, y].Height > 0.6f) {
                    pointsWithHeight.Add(new KeyValuePair<Vector2I, float>(new Vector2I(x, y), biomeCells[x, y].Height));
                }
            }
        }

        // Sort points by height
        pointsWithHeight.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

        // Choose the points with a minimum distance between them
        foreach (var point in pointsWithHeight) {
            if (highestPoints.Count >= numberOfPoints) break;
            if (highestPoints.All(startPoint => ((Vector2)startPoint).DistanceTo(point.Key) > minDistance)) {
                highestPoints.Add(point.Key);
            }
        }

        return highestPoints;
    }

    private static BiomeCell? FindLowestNeighbour(BiomeCell[,] biomeCells, BiomeCell startCell, IReadOnlySet<Vector2I> river, Random random) {
        var width = biomeCells.GetLength(0);
        var height = biomeCells.GetLength(1);

        var lowestCells = new List<BiomeCell>();
        var searchRadius = 1;

        while (lowestCells.Count == 0 && searchRadius < Math.Max(width, height)) {
            for (var dx = -searchRadius; dx <= searchRadius; dx++) {
                for (var dy = -searchRadius; dy <= searchRadius; dy++) {
                    var x = startCell.Position.X + dx;
                    var y = startCell.Position.Y + dy;

                    // Ignora las celdas fuera de la cuadrícula, la celda de inicio o en el conjunto river
                    if (x < 0 || x >= width || y < 0 || y >= height || river.Contains(new Vector2I(x, y)) || (dx == 0 && dy == 0)) {
                        continue;
                    }

                    var cell = biomeCells[x, y];
                    if (lowestCells.Count < 2) {
                        lowestCells.Add(cell);
                    } else {
                        var highestCell = lowestCells.OrderByDescending(c => c.Height).First();
                        if (cell.Height < highestCell.Height) {
                            lowestCells.Remove(highestCell);
                            lowestCells.Add(cell);
                        }
                    }
                }
            }
            searchRadius++;
        }

        return lowestCells.Count > 0 ? lowestCells[random.Next(lowestCells.Count)] : null;
    }

    public static void SimulateRiverFlow(BiomeCell[,] biomeCells, BiomeCell startPoint, Random random) {
        var waterAmount = initialWater;
        var currentCell = startPoint;
        var river = new HashSet<Vector2I>();
        var riverList = new List<Vector2I>();
        while (true) {
            currentCell.Water += waterAmount;
            river.Add(currentCell.Position);
            riverList.Add(currentCell.Position);
            var lowestNeighbour = FindLowestNeighbour(biomeCells, currentCell, river, random);
            if (lowestNeighbour == null || lowestNeighbour.Sea) {
                break;
            }
            currentCell = lowestNeighbour;
            waterAmount += increaseWater;
        }

        var additionalRiverPoints = new HashSet<Vector2I>();
        foreach (var point in riverList) {
            var cell = biomeCells[point.X, point.Y];
            // var searchRadius = Math.Min((int)Math.Floor(cell.Water), max); // Ajusta esto según cómo quieras que el agua afecte al grosor del río
            var searchRadius = Mathf.RoundToInt(random.NextFloat() * Math.Min(cell.Water, 1f));
            
            if (searchRadius == 0) continue;

            for (var dx = -searchRadius; dx <= searchRadius; dx++) {
                for (var dy = -searchRadius; dy <= searchRadius; dy++) {
                    var x = cell.Position.X + dx;
                    var y = cell.Position.Y + dy;

                    // Ignora las celdas fuera de la cuadrícula o ya en el río
                    if (x < 0 || x >= biomeCells.GetLength(0) || y < 0 || y >= biomeCells.GetLength(1) || river.Contains(new Vector2I(x, y)) || additionalRiverPoints.Contains(new Vector2I(x, y))) {
                        continue;
                    }

                    additionalRiverPoints.Add(new Vector2I(x, y));
                    var newCell = biomeCells[x, y];
                    newCell.Water = cell.Water / 2;
                }
            }
        }

        // Añade los puntos adicionales al río
        foreach (var point in additionalRiverPoints) {
            river.Add(point);
        }
    }
}

/// <summary>
/// Generate rivers drawing lines between the candidate points from the highest points to the lowest points until reach the sea
/// </summary>
public class RiverGeneratorPoint {
    private static float initialWater = 1f;
    private static float increaseWater = 0.01f;

    public static void GenerateRivers(List<BiomeCell> points, BiomeCell[,] biomeCells, int numberOfPoints, float minDistance, Random random) {
        FindRiverStartPoints(points, biomeCells, numberOfPoints, minDistance).ForEach((cell) => {
            SimulateRiverFlow(points, biomeCells, cell, random);
        });
    }

    public static List<BiomeCell> FindRiverStartPoints(List<BiomeCell> points, BiomeCell[,] biomeCells, int numberOfPoints, float minDistance) {
        var highestPoints = new List<BiomeCell>();

        // Sort points by height
        points.Sort((pair1, pair2) => pair2.Height.CompareTo(pair1.Height));

        // Choose the points with a minimum distance between them
        foreach (var point in points) {
            if (highestPoints.Count >= numberOfPoints) break;
            if (highestPoints.All(startPoint => ((Vector2)startPoint.Position).DistanceTo(point.Position) > minDistance)) {
                highestPoints.Add(point);
            }
        }
        return highestPoints;
    }

    public static void SimulateRiverFlow(List<BiomeCell> points, BiomeCell[,] biomeCells, BiomeCell startPoint, Random random) {
        var waterAmount = initialWater;
        var currentCell = startPoint;
        var river = new HashSet<BiomeCell>();
        var riverList = new List<BiomeCell>();
        while (true) {
            currentCell.Water += waterAmount;
            river.Add(currentCell);
            riverList.Add(currentCell);
            var lowestNeighbour = FindLowestNeighbourPoint(points, currentCell, river, random);
            if (lowestNeighbour == null || lowestNeighbour.Sea) {
                break;
            }
            currentCell = lowestNeighbour;
            waterAmount += increaseWater;
        }
        DrawRiver(riverList, biomeCells);

    }

    public static void DrawRiver(List<BiomeCell> points, BiomeCell[,] biomeCells) {
        for (var i = 0; i < points.Count - 1; i++) {
            var start = points[i];
            var end = points[i + 1];
            Draw.Line(start.Position.X, start.Position.Y, end.Position.X, end.Position.Y, 1, (x, y) => {
                biomeCells[x, y].Water = 1f;
            });
        }
    }

    private static BiomeCell? FindLowestNeighbourPoint(List<BiomeCell> points, BiomeCell startCell, IReadOnlySet<BiomeCell> river, Random random) {
        var closestPoints = points.Where(p => !river.Contains(p))
            .OrderBy(p => ((Vector2)p.Position).DistanceTo(startCell.Position))
            .Take(5)
            .ToList();

        // Si no hay puntos cercanos, devolver null
        if (!closestPoints.Any()) {
            return null;
        }

        // De los puntos más cercanos, devolver el que tiene el menor height
        return closestPoints.OrderBy(p => p.Height).First();
    }
}

/// <summary>
/// Generate rivers drawing lines between the candidate points from the highest points to the lowest points until reach the sea
/// The candidate points comes from a graph created with the voronoi edges
/// </summary>
public class RiverGeneratorGraph {
    private static float initialWater = 1f;
    private static float increaseWater = 0.01f;

    public static void GenerateRivers(Graph<BiomeCell> points, DataGrid<BiomeCell> biomeCells, int numberOfPoints, float minDistance, Random random) {
        FindRiverStartPoints(points, biomeCells, numberOfPoints, minDistance).ForEach((cell) => {
            SimulateRiverFlow(points, biomeCells, cell, random);
        });
    }

    public static List<BiomeCell> FindRiverStartPoints(Graph<BiomeCell> graph, DataGrid<BiomeCell> biomeCells, int numberOfPoints, float minDistance) {
        var highestPoints = new List<BiomeCell>();
        var points = graph.Data.Keys.ToList(); 

        // Sort points by height
        points.Sort((pair1, pair2) => pair2.Height.CompareTo(pair1.Height));

        // Choose the points with a minimum distance between them
        foreach (var point in points) {
            if (highestPoints.Count >= numberOfPoints) break;
            if (highestPoints.All(startPoint => ((Vector2)startPoint.Position).DistanceTo(point.Position) > minDistance)) {
                highestPoints.Add(point);
            }
        }
        return highestPoints;
    }

    public static void SimulateRiverFlow(Graph<BiomeCell> points, DataGrid<BiomeCell> biomeCells, BiomeCell startPoint, Random random) {
        var waterAmount = initialWater;
        var currentCell = startPoint;
        var river = new HashSet<BiomeCell>();
        var riverList = new List<BiomeCell>();
        while (true) {
            currentCell.Water += waterAmount;
            river.Add(currentCell);
            riverList.Add(currentCell);
            if (currentCell.Sea) {
                break;
            }
            var lowestNeighbour = FindLowestNeighbourPoint(points, currentCell, river, random);
            if (lowestNeighbour == null) {
                break;
            }
            currentCell = lowestNeighbour;
            waterAmount += increaseWater;
        }
        DrawRiver(riverList, biomeCells);

    }

    public static void DrawRiver(List<BiomeCell> points, DataGrid<BiomeCell> biomeCells) {
        for (var i = 0; i < points.Count - 1; i++) {
            var start = points[i];
            var end = points[i + 1];
            Draw.Line(start.Position.X, start.Position.Y, end.Position.X, end.Position.Y, 2, (x, y) => {
                biomeCells[x, y].Water = 1f;
            });
        }
    }

    private static BiomeCell? FindLowestNeighbourPoint(Graph<BiomeCell> points, BiomeCell startCell, IReadOnlySet<BiomeCell> river, Random random) {
        var closestPoints = points.GetConnections(startCell).Where(p => !river.Contains(p))
            .OrderBy(p => ((Vector2)p.Position).DistanceTo(startCell.Position))
            .Take(5)
            .ToList();

        // Si no hay puntos cercanos, devolver null
        if (!closestPoints.Any()) {
            return null;
        }

        // De los puntos más cercanos, devolver el que tiene el menor height
        return closestPoints.OrderBy(p => p.Height).First();
    }
}