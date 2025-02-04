using System;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.Maze;

public static class MazeGraphRender {
    /// <summary>
    /// Renders a maze graph using a template selector function
    /// </summary>
    public static void Render<T>(this MazeGraph graph, Array2D<T> array2D, Func<MazeNode, Array2D<T>> templateSelector) {

    }

    /// <summary>
    /// Renders a maze graph using a template selector function
    /// </summary>
    public static Array2D<T> Render<T>(this MazeGraph graph, Func<MazeNode, Array2D<T>> templateSelector) {
        var offset = graph.GetOffset();
        var size = graph.GetSize();

        // We need to find the cell size by getting the first template
        var firstTemplate = templateSelector(graph.GetNodes().First());
        var width = firstTemplate.Width;
        var height = firstTemplate.Height;

        var array2D = new Array2D<T>((size.X + 1) * width, (size.Y + 1) * height, default);

        Render(graph, templateSelector, width, height, firstTemplate, offset, array2D);
        return array2D;
    }

    private static void Render<T>(MazeGraph graph, Func<MazeNode, Array2D<T>> templateSelector, int width, int height, Array2D<T> firstTemplate, Vector2I offset, Array2D<T> array2D) {
        foreach (var node in graph.GetNodes()) {
            var template = templateSelector(node);
            if (template.Width != width || template.Height != height) {
                throw new ArgumentException($"All templates must have the same size. First template size: {firstTemplate.Width}x{firstTemplate.Height}. Current template size: {template.Width}x{template.Height}");
            }

            var pos = (node.Position - offset) * width;
            for (var y = 0; y < template.Height; y++) {
                for (var x = 0; x < template.Width; x++) {
                    array2D[pos.Y + y, pos.X + x] = template[y, x];
                }
            }
        }
    }
}