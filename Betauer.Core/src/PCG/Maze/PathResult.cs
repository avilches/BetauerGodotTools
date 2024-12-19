using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze;

public readonly struct PathResult(IReadOnlyList<MazeNode> path, float totalCost) {
    public IReadOnlyList<MazeNode> Path { get; } = path;
    public float TotalCost { get; } = totalCost;

    // Helper method to calculate only the edges cost
    public float GetEdgesCost() {
        float cost = 0;
        for (var i = 0; i < Path.Count - 1; i++) {
            var edge = Path[i].GetEdgeTo(Path[i + 1]);
            if (edge != null) cost += edge.Weight;
        }
        return cost;
    }

    // Helper method to calculate only the nodes cost
    public float GetNodesCost() {
        return Path.Sum(node => node.Weight);
    }
}