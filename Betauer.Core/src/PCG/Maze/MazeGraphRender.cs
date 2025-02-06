using System;
using System.Linq;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.Maze;

/// <summary>
/// Provides static methods to render a maze graph into a 2D array using customizable node and cell rendering functions.
/// Each node in the maze is rendered using a template, and each cell in the template can be further transformed.
/// </summary>
public static class MazeGraphRender {

    /// <summary>
    /// Creates an Array2D by directly mapping each node to a single cell.
    /// This is the simplest form of rendering where each node corresponds to exactly one cell.
    /// </summary>
    /// <typeparam name="TCell">The type of cells in the output array</typeparam>
    /// <param name="graph">The maze graph to render</param>
    /// <param name="nodeRenderer">A function that converts a node position and node data into a single cell value.
    /// The position parameter represents the normalized position of the node in the output array</param>
    /// <returns>A 2D array where each node is represented by a single cell</returns>
    public static Array2D<TCell> ToArray2D<TCell>(this MazeGraph graph, Func<Vector2I, MazeNode, TCell> nodeRenderer) {
        var graphOffset = graph.GetOffset();
        var array2D = new Array2D<TCell>(graph.GetSize());
        foreach (var node in graph.GetNodes()) {
            var pos = node.Position - graphOffset;
            array2D[pos] = nodeRenderer(pos, node);;
        }
        return array2D;
    }


    /// <summary>
    /// Renders a maze graph into a 2D array by applying a node renderer function to each node in the graph.
    /// </summary>
    /// <typeparam name="TCell">The type of cells in the output array</typeparam>
    /// <param name="graph">The maze graph to render</param>
    /// <param name="nodeRenderer">A function that converts a maze node into a square template array.
    /// All templates must have the same dimensions</param>
    /// <returns>A 2D array containing the rendered maze where each node is represented by its template</returns>
    /// <exception cref="ArgumentException">Thrown when templates returned by nodeRenderer have inconsistent dimensions</exception>
    public static Array2D<TCell> Render<TCell>(this MazeGraph graph, Func<Vector2I, MazeNode, Array2D<TCell>> nodeRenderer) {
        return Render(graph, nodeRenderer, (_, c) => c);
    }

    /// <summary>
    /// Renders a maze graph into a 2D array by first converting nodes to templates and then transforming each cell.
    /// </summary>
    /// <typeparam name="TNode">The intermediate type used for node templates</typeparam>
    /// <typeparam name="TCell">The final type of cells in the output array</typeparam>
    /// <param name="graph">The maze graph to render</param>
    /// <param name="nodeRenderer">A function that converts a maze node into a square template array.
    /// All templates must have the same dimensions</param>
    /// <param name="cellRenderer">A function that transforms each template cell into the final cell type.
    /// Receives the cell position and the template cell value</param>
    /// <returns>A 2D array containing the rendered maze where each node is represented by its transformed template</returns>
    /// <exception cref="ArgumentException">Thrown when templates returned by nodeRenderer have inconsistent dimensions</exception>
    public static Array2D<TCell> Render<TNode, TCell>(this MazeGraph graph, Func<Vector2I, MazeNode, Array2D<TNode>> nodeRenderer, Func<Vector2I, TNode, TCell> cellRenderer) {
        // We need to find the template size, we call to the renderer with the first node, only to get the size and create the array2D
        var graphOffset = graph.GetOffset();
        var firstNode = graph.GetNodes().First();
        var firstNodePosition = firstNode.Position - graphOffset;
        var firstTemplate = nodeRenderer(firstNodePosition, firstNode);

        if (firstTemplate.Width != firstTemplate.Height) {
            throw new ArgumentException($"All templates must have the same size and must be squared (width == height). First template size is wrong: {firstTemplate.Width}x{firstTemplate.Height}");
        }

        var array2D = new Array2D<TCell>(graph.GetSize() * firstTemplate.Width);
        RenderTo(graph, array2D, nodeRenderer, cellRenderer);
        return array2D;
    }

    /// <summary>
    /// Renders a maze graph directly into an existing 2D array using a node renderer function.
    /// </summary>
    /// <typeparam name="TCell">The type of cells in the output array</typeparam>
    /// <param name="graph">The maze graph to render</param>
    /// <param name="array2D">The target array where the maze will be rendered</param>
    /// <param name="nodeRenderer">A function that converts a maze node into a square template array.
    /// All templates must have the same dimensions</param>
    /// <exception cref="ArgumentException">Thrown when templates returned by nodeRenderer have inconsistent dimensions</exception>
    public static void RenderTo<TCell>(this MazeGraph graph, Array2D<TCell> array2D, Func<Vector2I, MazeNode, Array2D<TCell>> nodeRenderer) {
        RenderTo(graph, array2D, nodeRenderer, (_, c) => c);
    }

    /// <summary>
    /// Renders a maze graph directly into an existing 2D array using both node and cell renderer functions.
    /// </summary>
    /// <typeparam name="TNode">The intermediate type used for node templates</typeparam>
    /// <typeparam name="TCell">The final type of cells in the output array</typeparam>
    /// <param name="graph">The maze graph to render</param>
    /// <param name="array2D">The target array where the maze will be rendered</param>
    /// <param name="nodeRenderer">A function that converts a maze node into a square template array.
    /// All templates must have the same dimensions</param>
    /// <param name="cellRenderer">A function that transforms each template cell into the final cell type.
    /// Receives the cell position and the template cell value</param>
    /// <exception cref="ArgumentException">Thrown when templates returned by nodeRenderer have inconsistent dimensions</exception>
    public static void RenderTo<TNode, TCell>(this MazeGraph graph, Array2D<TCell> array2D, Func<Vector2I, MazeNode, Array2D<TNode>> nodeRenderer, Func<Vector2I, TNode, TCell> cellRenderer) {
        // We need to find the template size and use it to validate the rest of the templates
        var graphOffset = graph.GetOffset();
        var firstNode = graph.GetNodes().First();
        var firstNodePosition = firstNode.Position - graphOffset;
        var firstTemplate = nodeRenderer(firstNodePosition, firstNode);

        if (firstTemplate.Width != firstTemplate.Height) {
            throw new ArgumentException($"All templates must have the same size and must be squared (width == height). First template size is wrong: {firstTemplate.Width}x{firstTemplate.Height}");
        }

        var size = firstTemplate.Width;
        foreach (var node in graph.GetNodes()) {
            var nodePosition = node.Position - graphOffset;
            var template = nodeRenderer(nodePosition, node);
            if (template.Width != size || template.Height != size) {
                throw new ArgumentException($"All templates must have the same size. First template size: {size}x{size}. Current template size: {template.Width}x{template.Height}");
            }

            var offset = nodePosition * size;
            for (var y = 0; y < template.Height; y++) {
                for (var x = 0; x < template.Width; x++) {
                    var pos = new Vector2I(offset.X + x, offset.Y + y);
                    array2D[pos] = cellRenderer(pos, template[y, x]);
                }
            }
        }
    }
}