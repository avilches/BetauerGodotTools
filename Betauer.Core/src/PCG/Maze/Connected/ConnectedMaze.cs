using System;
using System.Linq;
using Godot;
using Betauer.Core.DataMath;
namespace Betauer.Core.PCG.Maze.Connected;

public class MazeNode {
    public readonly Vector2I Position;
    public readonly MazeNode[] Edges;
    
    public MazeNode(Vector2I position) {
        Position = position;
        // 0: arriba, 1: derecha, 2: abajo, 3: izquierda
        Edges = new MazeNode[4];
    }
    
    public void ConnectTo(MazeNode other, int direction) {
        Edges[direction] = other;
        // Conectar en la dirección opuesta
        other.Edges[(direction + 2) % 4] = this;
    }
}

public class MazeGenerator {
    private readonly Array2D<bool> OccupiedCells;
    private readonly Array2D<MazeNode> Maze;

    private readonly Random _random;
    
    public MazeGenerator(Array2D<bool> occupiedCells, Random? rand) {
        OccupiedCells = occupiedCells;
        _random = rand;
        Maze = new Array2D<MazeNode>(occupiedCells.Width, occupiedCells.Height);
    }
    
    public Array2D<MazeNode> Generate() {
        // Encontrar primera posición no ocupada
        foreach(var cell in OccupiedCells) {
            if(!cell.Value && Maze[cell.Position] == null) {
                GenerateFromPoint(cell.Position);
                break;
            }
        }
        return Maze;
    }
    
    private void GenerateFromPoint(Vector2I pos) {
        if(!OccupiedCells.IsValidPosition(pos) || OccupiedCells[pos] || Maze[pos] != null) 
            return;
            
        Maze[pos] = new MazeNode(pos);
        
        // Obtener direcciones en orden aleatorio
        var directions = Enumerable.Range(0, 4).ToList();
        
        
        while(directions.Count > 0) {
            int dirIndex = _random.Next(directions.Count);
            int dir = directions[dirIndex];
            directions.RemoveAt(dirIndex);
            
            Vector2I newPos = pos + Array2D.Directions[dir];
            
            if(OccupiedCells.IsValidPosition(newPos) && !OccupiedCells[newPos] && Maze[newPos] == null) {
                GenerateFromPoint(newPos);
                Maze[pos].ConnectTo(Maze[newPos], dir);
            }
        }
    }
    
    // Método helper para imprimir el laberinto (útil para debug)
    public void PrintMaze() {
        for(int y = 0; y < Maze.Height; y++) {
            // Imprimir conexiones horizontales
            for(int x = 0; x < Maze.Width; x++) {
                var pos = new Vector2I(x, y);
                Console.Write(Maze[pos] == null ? "   " : " O ");
                if(x < Maze.Width - 1 && Maze[pos]?.Edges[1] != null)
                    Console.Write("-");
                else
                    Console.Write(" ");
            }
            Console.WriteLine();
            
            // Imprimir conexiones verticales
            if(y < Maze.Height - 1) {
                for(int x = 0; x < Maze.Width; x++) {
                    var pos = new Vector2I(x, y);
                    if(Maze[pos]?.Edges[2] != null)
                        Console.Write(" | ");
                    else
                        Console.Write("   ");
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
    }
}