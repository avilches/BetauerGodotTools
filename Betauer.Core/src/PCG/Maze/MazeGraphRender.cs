using System;
using System.Linq;
using Betauer.Core.DataMath;

namespace Betauer.Core.PCG.Maze;

public static class MazeGraphRender {
    /// <summary>
    /// Renders a maze graph using a template selector function
    /// </summary>
    public static Array2D<char> Render(this MazeGraph graph, Func<MazeNode, Array2D<char>> templateSelector) {
        var offset = graph.GetOffset();
        var size = graph.GetSize();
        
        // We need to find the cell size by getting the first template
        var firstTemplate = templateSelector(graph.GetNodes().First());
        var cellSize = firstTemplate.Width; // Assuming all templates are square

        var array2D = new Array2D<char>((size.X + 1) * cellSize, (size.Y + 1) * cellSize, ' ');

        foreach (var node in graph.GetNodes()) {
            var template = templateSelector(node);
            var pos = (node.Position - offset) * cellSize;
            for (var y = 0; y < template.Height; y++) {
                for (var x = 0; x < template.Width; x++) {
                    array2D[pos.Y + y, pos.X + x] = template[y, x];
                }
            }
        }
        return array2D;
    }
}