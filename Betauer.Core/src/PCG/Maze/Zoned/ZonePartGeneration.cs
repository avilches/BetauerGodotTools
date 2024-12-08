using System.Collections.Generic;

namespace Betauer.Core.PCG.Maze.Zoned;

internal class ZonePartGeneration<T>(int partId, MazeNode<T> startNode) {
    internal int PartId { get; } = partId;
    internal MazeNode<T> StartNode { get; } = startNode;
    internal List<MazeNode<T>> Nodes { get; } = [startNode];
}