using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;
/*
public class Cell {
    public Vector2I Position { get; }
    public int Size { get; }
    
    // Posición de los muros relativos a la celda base
    public int LeftWall { get; set; }   // 0 = posición original
    public int RightWall { get; set; }  // Size = posición original
    public int TopWall { get; set; }    // 0 = posición original
    public int BottomWall { get; set; } // Size = posición original
    
    public Cell(Vector2I position, int size) {
        Position = position;
        Size = size;
        // Inicialmente, los muros están en sus posiciones originales
        LeftWall = 0;
        RightWall = size;
        TopWall = 0;
        BottomWall = size;
    }
    
    // Obtener el área real disponible para la habitación
    public Rect2I GetAvailableArea() {
        return new Rect2I(
            Position.X * Size + LeftWall,
            Position.Y * Size + TopWall,
            RightWall - LeftWall,
            BottomWall - TopWall
        );
    }
}

public class CellGrid {
    private readonly Dictionary<Vector2I, Cell> _cells = new();
    private readonly int _cellSize;
    private readonly Random _random;
    
    public CellGrid(int cellSize, Random random) {
        _cellSize = cellSize;
        _random = random;
    }
    
    public void AddCell(Vector2I position) {
        _cells[position] = new Cell(position, _cellSize);
    }
    
    public void AdjustWalls(float adjustmentProbability = 0.5f) {
        foreach (var cell in _cells.Values) {
            foreach (var direction in Array2D.Directions) {
                if (_random.NextDouble() < adjustmentProbability) {
                    AdjustWallBetweenCells(cell, direction);
                }
            }
        }
    }
    
    private void AdjustWallBetweenCells(Cell cell, Vector2I direction) {
        var neighborPos = cell.Position + direction;
        if (!_cells.TryGetValue(neighborPos, out var neighbor)) return;

        const int maxAdjustment = 2; // Máximo movimiento del muro
        
        if (direction == Vector2I.Right) {
            var adjustment = _random.Next(-maxAdjustment, maxAdjustment + 1);
            cell.RightWall = _cellSize + adjustment;
            neighbor.LeftWall = adjustment;
        }
        else if (direction == Vector2I.Left) {
            var adjustment = _random.Next(-maxAdjustment, maxAdjustment + 1);
            cell.LeftWall = adjustment;
            neighbor.RightWall = _cellSize + adjustment;
        }
        else if (direction == Vector2I.Down) {
            var adjustment = _random.Next(-maxAdjustment, maxAdjustment + 1);
            cell.BottomWall = _cellSize + adjustment;
            neighbor.TopWall = adjustment;
        }
        else if (direction == Vector2I.Up) {
            var adjustment = _random.Next(-maxAdjustment, maxAdjustment + 1);
            cell.TopWall = adjustment;
            neighbor.BottomWall = _cellSize + adjustment;
        }
    }
    
    public Cell GetCell(Vector2I position) => _cells[position];
}

public static class MazeGraphToArray2D {
    private const char Wall = '#';
    private const char Floor = '·';
    
    public static Array2D<char> Convert(MazeGraph graph, int cellSize = 12, float expandProbability = 0.3f, int seed = 12345) {
        var random = new Random(seed);
        var offset = graph.GetOffset();
        
        // 1. Crear la cuadrícula de celdas
        var grid = new CellGrid(cellSize, random);
        foreach (var node in graph.GetNodes()) {
            grid.AddCell(node.Position - offset);
        }
        
        // 2. Ajustar los muros entre celdas
        grid.AdjustWalls(expandProbability);
        
        // 3. Calcular dimensiones finales
        var size = graph.GetSize();
        var array = new Array2D<char>((size.X + 1) * cellSize, (size.Y + 1) * cellSize, Wall);
        
        // 4. Dibujar habitaciones y corredores
        foreach (var node in graph.GetNodes()) {
            var cell = grid.GetCell(node.Position - offset);
            DrawRoom(array, cell);
            
            // Dibujar corredores a vecinos
            foreach (var direction in Array2D.Directions) {
                var neighbor = node.GetEdgeTowards(direction)?.To;
                if (neighbor != null) {
                    var neighborCell = grid.GetCell(neighbor.Position - offset);
                    DrawCorridor(array, cell, neighborCell, direction);
                }
            }
        }
        
        return array;
    }
    
    private static void DrawRoom(Array2D<char> array, Cell cell) {
        var area = cell.GetAvailableArea();
        // Reducir un poco el tamaño de la habitación aleatoriamente
        const int margin = 1;
        for (var y = area.Position.Y + margin; y < area.End.Y - margin; y++) {
            for (var x = area.Position.X + margin; x < area.End.X - margin; x++) {
                array[y, x] = Floor;
            }
        }
    }
    
    private static void DrawCorridor(Array2D<char> array, Cell cell1, Cell cell2, Vector2I direction) {
        var area1 = cell1.GetAvailableArea();
        var area2 = cell2.GetAvailableArea();
        
        if (direction == Vector2I.Right || direction == Vector2I.Left) {
            var corridorY = area1.Position.Y + area1.Size.Y / 2;
            int startX, endX;
            
            if (direction == Vector2I.Right) {
                startX = area1.End.X;
                endX = area2.Position.X;
            } else {
                startX = area2.End.X;
                endX = area1.Position.X;
            }
            
            // Dibujar corredor horizontal
            for (var x = startX; x <= endX; x++) {
                array[corridorY - 1, x] = Floor;
                array[corridorY, x] = Floor;
            }
        } else {
            var corridorX = area1.Position.X + area1.Size.X / 2;
            int startY, endY;
            
            if (direction == Vector2I.Down) {
                startY = area1.End.Y;
                endY = area2.Position.Y;
            } else {
                startY = area2.End.Y;
                endY = area1.Position.Y;
            }
            
            // Dibujar corredor vertical
            for (var y = startY; y <= endY; y++) {
                array[y, corridorX - 1] = Floor;
                array[y, corridorX] = Floor;
            }
        }
    }
}
*/