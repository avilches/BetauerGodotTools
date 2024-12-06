using System;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZoneConfig {
    public ZoneConfig(int nodes, int maxParts, int maxDoorsOut, bool corridor) {
        if (nodes < 1) {
            throw new ArgumentException($"Value {nodes} for nodes is wrong, it must be at least 1", nameof(nodes));
        }
        if (maxParts < 1) {
            throw new ArgumentException($"Value {maxParts} for parts is wrong, it must be at least 1", nameof(maxParts));
        }
        if (maxParts > nodes) {
            throw new ArgumentException($"Parts must be equals or greater than nodes. Parts: {maxParts}, Nodes: {nodes}", nameof(maxParts));
        }
        
        Nodes = nodes;
        MaxParts = maxParts;
        MaxDoorsOut = maxDoorsOut;
        Corridor = corridor;
    }

    public int Nodes { get; set; }
    
    /// <summary>
    /// The algorithm will try to split the zone in this number of parts, if it's possible.
    /// </summary>
    public int MaxParts { get; set; }
    public int MaxDoorsOut { get; set; }
    
    public bool Corridor { get; set; } = false;

}