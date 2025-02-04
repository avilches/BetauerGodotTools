using System;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Renders a maze graph using a template selector function
/// </summary>
public static class MazeGraphRender {

    public static Array2D<char> Render(this MazeGraph graph, Func<MazeNode, Array2D<char>> templateSelector) {
        return Render(graph, templateSelector, (_,c) => c);
    }

    public static Array2D<T> Render<T>(this MazeGraph graph, Func<MazeNode, Array2D<char>> templateSelector, Func<Vector2I, char, T> converter) {
        // We need to find the cell size by getting the first template
        var firstTemplate = templateSelector(graph.GetNodes().First());
        var width = firstTemplate.Width;
        var height = firstTemplate.Height;
        var size = graph.GetSize();
        var array2D = new Array2D<T>((size.X + 1) * width, (size.Y + 1) * height, default);

        RenderTo(graph, array2D, templateSelector, converter);
        return array2D;
    }

    public static void RenderTo(this MazeGraph graph, Array2D<char> array2D, Func<MazeNode, Array2D<char>> templateSelector) {
        RenderTo(graph, array2D, templateSelector, (_,c) => c);
    }

    public static void RenderTo<T>(this MazeGraph graph, Array2D<T> array2D, Func<MazeNode, Array2D<char>> templateSelector, Func<Vector2I, char, T> converter) {
        var firstTemplate = templateSelector(graph.GetNodes().First());
        var width = firstTemplate.Width;
        var height = firstTemplate.Height;

        var graphOffset = graph.GetOffset();
        foreach (var node in graph.GetNodes()) {
            var template = templateSelector(node);
            if (template.Width != width || template.Height != height) {
                throw new ArgumentException($"All templates must have the same size. First template size: {width}x{height}. Current template size: {template.Width}x{template.Height}");
            }

            var offset = (node.Position - graphOffset) * width;
            for (var y = 0; y < template.Height; y++) {
                for (var x = 0; x < template.Width; x++) {
                    var pos = new Vector2I(offset.X + x, offset.Y + y);
                    array2D[pos] = converter(pos, template[y, x]);
                }
            }
        }
    }
}