using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;

namespace Betauer.Core.PCG.Maze;

public static class MazeGraphRender {
    public static Array2D<char> Render(this MazeGraph graph, TemplateSet templateSet, Random? random = null, string[]? requiredFlags = null, string[]? optionalFlags = null) {
        var offset = graph.GetOffset();
        var size = graph.GetSize();
        var cellSize = templateSet.CellSize;
        var array2D = new Array2D<char>((size.X + 1) * cellSize, (size.Y + 1) * cellSize, ' ');
        random ??= new Random();

        foreach (var node in graph.GetNodes()) {
            var nodeRequiredFlags = node.GetAttribute("requiredFlags");
            var nodeOptionalFlags = node.GetAttribute("optionalFlags");
            
            var templates = templateSet.FindTemplates(FromNode(node), Merge(requiredFlags, nodeRequiredFlags), Merge(optionalFlags, nodeOptionalFlags));
            var template = random.Next(templates);
            
            var pos = (node.Position - offset) * cellSize;
            // Console.WriteLine($"Copying template size " +
            //                   $"({template.Width}x{template.Height}) " +
            //                   $"at position ({position.X},{position.Y})");
            for (var y = 0; y < template.Height; y++) {
                for (var x = 0; x < template.Width; x++) {
                    array2D[pos.Y + y, pos.X + x] = template[y, x];
                }
            }
        }
        return array2D;
        
        int FromNode(MazeNode node) {
            var directions = 0;
            if (node.Up != null) directions |= (int)DirectionFlags.Up;
            if (node.Right != null) directions |= (int)DirectionFlags.Right;
            if (node.Down != null) directions |= (int)DirectionFlags.Down;
            if (node.Left != null) directions |= (int)DirectionFlags.Left;
            return directions;
        }
        
        string[] Merge(object? flags1, object? flags2) {
            // Helper method to convert an object to string[]
            string[] ConvertToStringArray(object? obj) {
                if (obj == null) return [];
                return obj switch {
                    string[] array => array,
                    IList<string> list => list.ToArray(),
                    IEnumerable<string> enumerable => enumerable.ToArray(),
                    string str when !string.IsNullOrEmpty(str) => str.Split(',').Select(s => s.Trim()).ToArray(),
                    _ => []
                };
            }

            // Convert both objects to string arrays
            var array1 = ConvertToStringArray(flags1);
            var array2 = ConvertToStringArray(flags2);

            // If both are empty, return empty array
            if (array1.Length == 0 && array2.Length == 0) return [];
    
            // If one is empty, return the other
            if (array1.Length == 0) return array2;
            if (array2.Length == 0) return array1;
    
            // Merge both arrays and remove duplicates
            return array1.Concat(array2).Distinct().ToArray();
        }
    }
}