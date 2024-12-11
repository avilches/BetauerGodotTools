using System;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZoneConfig {
    /// <summary>
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="parts"></param>
    /// <param name="maxExitNodes"></param>
    /// <param name="corridor"></param>
    /// <param name="flexibleParts">
    /// If true, the algorithm could create more parts (if there are no more free adjacent nodes in the zone to expand) or
    /// less parts, if there are no more exit nodes in the previous zones.
    /// </param>
    /// <exception cref="ArgumentException"></exception>
    public ZoneConfig(int nodes, int parts, int maxExitNodes, bool corridor, bool flexibleParts = true) {
        if (nodes < 1) {
            throw new ArgumentException($"Value {nodes} for nodes is wrong, it must be at least 1", nameof(nodes));
        }
        if (parts < 1) {
            throw new ArgumentException($"Value {parts} for parts is wrong, it must be at least 1", nameof(parts));
        }
        if (parts > nodes) {
            throw new ArgumentException($"Parts must be equals or greater than nodes. Parts: {parts}, Nodes: {nodes}", nameof(parts));
        }

        Nodes = nodes;
        Parts = parts;
        MaxExitNodes = maxExitNodes;
        Corridor = corridor;
        FlexibleParts = flexibleParts;
    }

    public int Nodes { get; set; }

    /// <summary>
    /// The algorithm will try to split the zone in this number of parts. If FlexibleParts is true, the number of parts could be less or more. If false, the
    /// number of parts created will be exactly this number (and the algorithm will fail if it can't create the number of parts).
    /// </summary>
    public int Parts { get; set; }

    /// <summary>
    /// The maximum number of exit nodes for the zone. -1 means no limit.
    /// </summary>
    public int MaxExitNodes { get; set; }

    public bool Corridor { get; set; }

    /// If true, the algorithm could create more parts (if there are no more free adjacent nodes in the zone to expand) or
    /// less parts, if there are no more exit nodes in the previous zones.
    public bool FlexibleParts { get; set; }
}