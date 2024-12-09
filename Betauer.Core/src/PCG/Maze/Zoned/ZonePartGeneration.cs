using System.Collections.Generic;

namespace Betauer.Core.PCG.Maze.Zoned;

internal class ZonePartGeneration(int partId, MazeNode startNode) {
    internal int PartId { get; } = partId;
    internal MazeNode StartNode { get; } = startNode;
    internal List<MazeNode> Nodes { get; } = [startNode];
}