using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeLinearity {
    public class LinearityResult {
        // La medida de no-linearidad: el número total de "re-visitas" necesarias
        // Un valor de 0 significa que el camino es perfectamente linear (ninguna habitación se visita dos veces)
        // Cada valor por encima de 0 indica cuántas veces hay que volver a pasar por habitaciones ya visitadas
        public int NonLinearityScore { get; set; }
        
        // Guarda cuántas veces se pasa por cada habitación en el camino más corto
        public Dictionary<MazeNode, int> RoomTraversals { get; set; }
         
        public int GetTraversals(MazeNode node) {
            return RoomTraversals.GetValueOrDefault(node, 0);
        }
        
        // El camino más corto completo (entrada -> llave1 -> llave2 -> ... -> meta)
        public List<MazeNode> ShortestPath { get; set; }
    }

    public static LinearityResult CalculateLinearity(List<MazeNode> keyLocations) {
        var result = new LinearityResult {
            RoomTraversals = [],
            ShortestPath = []
        };

        // Comenzar desde el primer nodo
        var currentNode = keyLocations.First();
        var firstNode = true;

        // Para cada punto a visitar en orden
        foreach (var keyNode in keyLocations.Skip(1)) {
            // Encontrar el camino más corto a la siguiente llave
            var path = currentNode.FindShortestPath(keyNode);
            if (path.Count <= 1) continue;
            
            // Every path starts in the same node than the previous zone ends, so the first needs to be ignored to avoid duplicates
            var skip = firstNode ? 0 : 1;
            // Registrar cada habitación atravesada
            foreach (var node in path.Skip(skip)) {
                result.RoomTraversals.TryAdd(node, 0);
                result.RoomTraversals[node]++;
            }

            result.ShortestPath.AddRange(path.Skip(skip)); // Skip(1) para no duplicar nodos en las uniones
            currentNode = keyNode;
            firstNode = false;
        }

        // Calcular non-linearity: suma de todas las visitas adicionales
        // (cada habitación que se visita más de una vez suma sus visitas extras)
        result.NonLinearityScore = result.RoomTraversals
            .Sum(kv => Math.Max(0, kv.Value - 1));

        return result;
    }

    // Método de utilidad para imprimir el análisis
    public static void PrintAnalysis(LinearityResult result) {
        Console.WriteLine($"Non-linearity Score: {result.NonLinearityScore}");
        Console.WriteLine("\nRoom Traversals:");
        foreach (var kv in result.RoomTraversals.OrderByDescending(x => x.Value)) {
            Console.WriteLine($"Room {kv.Key}: visited {kv.Value} times" + 
                            (kv.Value > 1 ? $" (+{kv.Value - 1} backtrack)" : ""));
        }
        
        Console.WriteLine($"\nTotal unique rooms: {result.RoomTraversals.Count}");
        Console.WriteLine($"Rooms with backtracking: {result.RoomTraversals.Count(kv => kv.Value > 1)}");
        Console.WriteLine($"Total path length: {result.ShortestPath.Count}");
    }
}