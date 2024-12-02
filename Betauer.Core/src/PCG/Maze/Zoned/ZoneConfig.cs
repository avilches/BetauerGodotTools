using System;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZoneConfig {
    public ZoneConfig(int nodes, int parts, int maxDoorsOut, bool autoSplitOnExpand, bool corridor) {
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
        MaxDoorsOut = maxDoorsOut;
        AutoSplitOnExpand = autoSplitOnExpand;
        Corridor = corridor;
    }

    public int Nodes { get; set; }
    public int Parts { get; set; }
    public int MaxDoorsOut { get; set; }
    
    /// <summary>
    /// Allows the zone to create more parts if there are no more available nodes to expand, creating more parts than initial defined in the Parts field
    /// Imagine there only 3 nodes to left to fill the space, and these nodes are not adjacent to any part of the current zone. If AutoSplitOnExpand is true,
    /// a new part of the zone will be created to fill the space. 
    /// </summary>
    public bool AutoSplitOnExpand { get; set; }

    public bool Corridor { get; set; } = false;

}