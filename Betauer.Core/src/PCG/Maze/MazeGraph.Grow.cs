using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;

public interface IMazeZonedConstraints {
    int MaxZones { get; }
    int MaxTotalNodes { get; }
    int GetNodesPerZone(int currentZone);
    int GetDoorsIn(int currentZone);
    int GetMaxDoorsOut(int currentZone);
}

public class MazeZonedConstraints(int maxZones) : IMazeZonedConstraints {
    public int MaxZones { get; set; } = maxZones;
    public int MaxTotalNodes { get; set; } = -1;
    public int MaxDoorsIn { get; set; } = -1;
    public int MaxDoorsOut { get; set; } = -1;
    public int[] NodesPerZone { get; set; } = [5];

    public MazeZonedConstraints(int maxZones, int maxTotalNodes) : this(maxZones) {
        if (maxTotalNodes < maxZones) throw new ArgumentException($"Max total nodes {maxTotalNodes} must be greater than max zones {maxZones}");
        MaxTotalNodes = maxTotalNodes;
        NodesPerZone = [maxTotalNodes / maxZones];
    }

    public int GetNodesPerZone(int zone) {
        return NodesPerZone[Math.Min(zone, NodesPerZone.Length - 1)];
    }

    public int GetDoorsIn(int zone) {
        return MaxDoorsIn;
    }

    public int GetMaxDoorsOut(int zone) {
        return MaxDoorsOut;
    }

    public MazeZonedConstraints SetMaxDoorsIn(int maxDoorsIn) {
        if (maxDoorsIn < 1) throw new ArgumentException($"Max doors in must be greater than 1: {maxDoorsIn}");
        MaxDoorsIn = maxDoorsIn;
        return this;
    }

    public MazeZonedConstraints SetMaxDoorsOut(int maxDoorsOut) {
        MaxDoorsOut = maxDoorsOut;
        return this;
    }

    public MazeZonedConstraints SetNodesPerZones(params int[] nodesPerZone) {
        NodesPerZone = nodesPerZone;
        return this;
    }
}

public class MazePerZoneConstraints(int maxTotalNodes = -1) : IMazeZonedConstraints {
    public List<MazeZone> Zones { get; set; } = [];

    public int MaxTotalNodes { get; set; } = maxTotalNodes;

    public int MaxZones => Zones.Count;

    public MazePerZoneConstraints Zone(int zone, int nodes, int doorsIn = 2, int maxDoorsOut = 2) {
        if (nodes < 1) throw new ArgumentException($"nodes must be greater than 1: {nodes}");
        if (zone > 0 && doorsIn < 1) throw new ArgumentException($"doorsIn {doorsIn }must be greater than 1 for zones greater than 0");
        if (zone == 0 && doorsIn != 0) throw new ArgumentException($"doorsIn must be 0 for zone 0: {doorsIn}");
        if (doorsIn > nodes) throw new ArgumentException($"doorsIn must be less than nodes: {doorsIn}");
        if (zone == Zones.Count) {
            Zones.Add(new MazeZone(nodes, doorsIn, maxDoorsOut));
        } else if (zone < Zones.Count) {
            Zones[zone] = new MazeZone(nodes, doorsIn, maxDoorsOut);
        } else {
            while (Zones.Count <= zone) {
                Zones.Add(new MazeZone(nodes, doorsIn, maxDoorsOut));
            }
        } 
        return this;
    }

    public int GetNodesPerZone(int zone) {
        return Zones[zone].Nodes;
    }

    public int GetDoorsIn(int zone) {
        return Zones[zone].DoorsIn;
    }

    public int GetMaxDoorsOut(int zone) {
        return Zones[zone].MaxDoorsOut;
    }
    
    public MazePerZoneConstraints SetMaxTotalNodes(int maxTotalNodes) {
        MaxTotalNodes = maxTotalNodes;
        return this;
    }
    
}

public class MazeZone(int nodes, int doorsIn, int maxDoorsOut) {
    public int Nodes { get; set; } = nodes;
    public int DoorsIn { get; set; } = doorsIn;
    public int MaxDoorsOut { get; set; } = maxDoorsOut;
}

public partial class MazeGraph {
    /// <summary>
    /// Grows a maze from a starting position using the specified constraints.
    /// </summary>
    /// <param name="start">Starting position for the maze generation.</param>
    /// <param name="constraints">Constraints for the maze generation.</param>
    /// <param name="backtracker">A function to locate the next node to backtrack. By default, it takes the last one (LIFO)</param>
    /// <returns>The number of paths created.</returns>
    public void Grow(Vector2I start, BacktrackConstraints constraints, Func<List<NodeGrid>, NodeGrid>? backtracker = null) {
        ArgumentNullException.ThrowIfNull(constraints);
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        _lastId = 0;

        var maxTotalCells = constraints.MaxTotalCells == -1 ? int.MaxValue : constraints.MaxTotalCells;
        var maxCellsPerPath = constraints.MaxCellsPerPath == -1 ? int.MaxValue : constraints.MaxCellsPerPath;
        var maxTotalPaths = constraints.MaxPaths == -1 ? int.MaxValue : constraints.MaxPaths;
        if (maxTotalCells == 0 || maxCellsPerPath == 0 || maxTotalPaths == 0) return;

        var pendingNodes = new List<NodeGrid>();
        Vector2I? lastDirection = null;

        var pathsCreated = 0;
        var totalNodesCreated = 1;
        var nodesCreatedInCurrentPath = 1;

        var currentNode = NodeGridRoot = GetOrCreateNode(start);
        pendingNodes.Add(NodeGridRoot);
        while (pendingNodes.Count > 0) {
            var availableDirections = GetAvailableDirections(currentNode.Position);

            if (availableDirections.Count == 0 || nodesCreatedInCurrentPath >= maxCellsPerPath || totalNodesCreated == maxTotalCells) {
                // path stopped, backtracking
                pendingNodes.Remove(currentNode);
                if (pendingNodes.Count > 0) {
                    currentNode = backtracker != null ? backtracker.Invoke(pendingNodes) : pendingNodes[pendingNodes.Count - 1];
                    if (GetAvailableDirections(currentNode.Position).Count == 0) {
                        continue;
                    }
                }
                // No more nodes to backtrack (end of the path and the maze) or next node has no available directions (end of the path)
                pathsCreated++;
                // Console.WriteLine($"Path #{pathsCreated} finished: Cells: {nodesCreatedInCurrentPath}");
                if (pathsCreated == maxTotalPaths || totalNodesCreated == maxTotalCells) break;
                nodesCreatedInCurrentPath = 1;
                lastDirection = null;
                continue;
            }

            var validCurrentDir = lastDirection.HasValue && availableDirections.Contains(lastDirection.Value)
                ? lastDirection
                : null;

            var nextDir = constraints.DirectionSelector(validCurrentDir, availableDirections);
            lastDirection = nextDir;

            var nextPos = currentNode.Position + nextDir;
            var nextNode = GetOrCreateNode(nextPos);
            nextNode.Parent = currentNode;
            ConnectNode(currentNode, nextDir, true);
            pendingNodes.Add(nextNode);
            totalNodesCreated++;
            nodesCreatedInCurrentPath++;

            currentNode = nextNode;
        }
        // Console.WriteLine("Cells created: " + totalNodesCreated + " Paths created: " + pathsCreated);
    }

    public void GrowRandom(Vector2I start, int maxTotalNodes = -1, Random? rng = null) {
        if (maxTotalNodes == 0) return;
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));
        maxTotalNodes = maxTotalNodes == -1 ? int.MaxValue : maxTotalNodes;

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        _lastId = 0;
        rng ??= new Random();
        var pendingNodes = new List<NodeGrid>();
        var totalNodesCreated = 1;
        NodeGridRoot = GetOrCreateNode(start);
        pendingNodes.Add(NodeGridRoot);
        while (pendingNodes.Count > 0 && totalNodesCreated < maxTotalNodes) {
            var currentNode = rng.Next(pendingNodes);
            var availableDirections = GetAvailableDirections(currentNode.Position);

            if (availableDirections.Count == 0) {
                // invalid cell, removing
                pendingNodes.Remove(currentNode);
            } else {
                var nextDir = rng.Next(availableDirections);
                var nextPos = currentNode.Position + nextDir;
                var nextNode = GetOrCreateNode(nextPos);
                nextNode.Parent = currentNode;
                ConnectNode(currentNode, nextDir, true);
                pendingNodes.Add(nextNode);
                totalNodesCreated++;
            }
        }
    }


    private class Zone(int id) {
        internal int Id = id;
        internal int Nodes = 0;
        internal List<NodeGrid> PendingNodes = new();
        internal int DoorsIn = 0;
        internal int DoorsOut = 0;

    }
    public void GrowZoned(Vector2I start, IMazeZonedConstraints constraints, Random? rng = null) {
        if (!IsValid(start)) throw new ArgumentException("Invalid start position", nameof(start));
        var maxTotalNodes = constraints.MaxTotalNodes == -1 ? int.MaxValue : constraints.MaxTotalNodes;
        if (maxTotalNodes == 0) return;

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        _lastId = 0;

        rng ??= new Random();

        NodeGridRoot = GetOrCreateNode(start);
        NodeGridRoot.Zone = 0;

        var zones = new List<Zone>();
        var globalZone = new Zone(-1);
        globalZone.PendingNodes.Add(NodeGridRoot);
        globalZone.Nodes = 1;
        // Special case: when the first zone has a size of 1, we can start with the next zone
        var currentZone = constraints.GetNodesPerZone(0) == 1 
            ? new Zone(1) { Nodes = 0 }
            : new Zone(0) { Nodes = 1, PendingNodes = [NodeGridRoot] };
        
        zones.Add(currentZone);
                
        while (globalZone.Nodes < maxTotalNodes && currentZone.Id < constraints.MaxZones) {
            // Pick a random node from current zone. If the zone has no pending nodes, pick from all pending nodes
            NodeGrid currentNode = null!;
            var expanding = currentZone.DoorsIn == constraints.GetDoorsIn(currentZone.Id);
            if (expanding) {
                if (currentZone.PendingNodes.Count == 0) throw new Exception($"No more free nodes in the zone {currentZone.Id} to expand");
                currentNode = rng.Next(currentZone.PendingNodes);
            } else {
                if (globalZone.PendingNodes.Count == 0) throw new Exception($"No more free nodes in the maze to open new doors to the the zone {currentZone.Id}");
                currentNode = rng.Next(globalZone.PendingNodes);
            }
            
            var availableDirections = GetAvailableDirections(currentNode.Position);
            
            if (availableDirections.Count > 0) {
                var oldZone = currentNode.Zone;
                if (oldZone != currentZone.Id) {
                    // New zone connection
                    zones[oldZone].DoorsOut++;
                    currentZone.DoorsIn++;
                    if (expanding) Console.WriteLine("Mal");
                } else {
                    // expanding: same zone connection
                    if (!expanding) Console.WriteLine("Mal");
                }
                var nextDir = rng.Next(availableDirections);
                var nextPos = currentNode.Position + nextDir;
                var newNode = GetOrCreateNode(nextPos);
                newNode.Parent = currentNode;
                newNode.Zone = currentZone.Id;
                ConnectNode(currentNode, nextDir, true);
                globalZone.Nodes++;
                currentZone.PendingNodes.Add(newNode);
                currentZone.Nodes++;
                if (currentZone.Nodes >= constraints.GetNodesPerZone(currentZone.Id)) {
                    globalZone.PendingNodes.AddRange(currentZone.PendingNodes);
                    currentZone.PendingNodes.Clear();
                    currentZone = new Zone(currentZone.Id + 1);
                    zones.Add(currentZone);
                }
            } else {
                // invalid node, removing from the zone and from the global pending nodes
                globalZone.PendingNodes.Remove(currentNode);
                currentZone.PendingNodes.Remove(currentNode);
            }
        }
    }
}