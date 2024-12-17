using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.GridTemplate;

public static class MazeGraphToArray2D {
    public static Array2D<char> Convert(this MazeGraph graph, TemplateSet templateSet, params string[] flags) {
        var offset = graph.GetOffset();
        var size = graph.GetSize();
        var cellSize = templateSet.CellSize;
        var array = new Array2D<char>((size.X + 1) * cellSize, (size.Y + 1) * cellSize, ' ');

        foreach (var node in graph.GetNodes()) {
            var template = templateSet.FindTemplates(node, flags)[0];
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
    }
}