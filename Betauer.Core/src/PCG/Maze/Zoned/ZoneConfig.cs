using System;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZoneConfig {
    public ZoneConfig(int nodes, int parts, int maxExitNodes, bool corridor) {
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
    }

    public int Nodes { get; set; }
    
    /// <summary>
    /// The algorithm will try to split the zone in this number of parts, if it's possible.
    /// </summary>
    public int Parts { get; set; }

    /// <summary>
    /// The maximum number of exit nodes for the zone. -1 means no limit.
    /// </summary>
    public int MaxExitNodes { get; set; }
    
    public bool Corridor { get; set; } = false;

}