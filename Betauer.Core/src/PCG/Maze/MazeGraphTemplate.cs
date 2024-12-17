using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;

namespace Betauer.Core.PCG.Maze;

public static class MazeGraphTemplate {
    public static Array2D<char> Convert(this MazeGraph graph, TemplateSet templateSet, params string[] flags) {
        var offset = graph.GetOffset();
        var size = graph.GetSize();
        var cellSize = templateSet.CellSize;
        var array = new Array2D<char>((size.X + 1) * cellSize, (size.Y + 1) * cellSize, ' ');

        foreach (var node in graph.GetNodes()) {
            var template = templateSet.FindTemplates(FromNode(node), flags)[0];
            var pos = (node.Position - offset) * cellSize;
            // Console.WriteLine($"Copying template size " +
            //                   $"({template.Width}x{template.Height}) " +
            //                   $"at position ({position.X},{position.Y})");
            for (var y = 0; y < template.Height; y++) {
                for (var x = 0; x < template.Width; x++) {
                    array[pos.Y + y, pos.X + x] = template[y, x];
                }
            }
        }
        return array;
        
        int FromNode(MazeNode node) {
            var directions = 0;
            if (node.Up != null) directions |= (int)DirectionFlags.Up;
            if (node.Right != null) directions |= (int)DirectionFlags.Right;
            if (node.Down != null) directions |= (int)DirectionFlags.Down;
            if (node.Left != null) directions |= (int)DirectionFlags.Left;
            return directions;
        }
    }
}