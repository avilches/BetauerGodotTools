using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeZone(int nodes, int parts, int maxDoorsOut) {
    public int Nodes { get; set; } = nodes;
    public int Parts { get; set; } = parts;
    public int MaxDoorsOut { get; set; } = maxDoorsOut;
}

public class Zone(int id) {
    public int Id { get; internal set; } = id;
    public int Nodes { get; internal set; } = 0;
    public List<NodeGrid> AvailableNodes { get; internal set; } = new();
    public int Parts { get; internal set; } = 0;
    public int DoorsOut { get; internal set; } = 0;
}


public class MazeGraphZoned(int width, int height, Func<Vector2I, bool>? isValid = null, Action<NodeGrid>? onCreateNode = null, Action<NodeGridEdge>? onConnect = null)
    : MazeGraph(width, height, isValid, onCreateNode, onConnect) {
    public List<Zone> GrowZoned(Vector2I start, IMazeZonedConstraints constraints, Random? rng = null) {
        if (!IsValid(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        var maxTotalNodes = constraints.MaxTotalNodes == -1 ? int.MaxValue : constraints.MaxTotalNodes;
        if (maxTotalNodes == 0 || constraints.MaxZones == 0) return [];

        ValidateGrowZonedConstraints(constraints);

        NodeGridRoot = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        LastId = 0;

        rng ??= new Random();

        NodeGridRoot = GetOrCreateNode(start);
        NodeGridRoot.Zone = 0;

        var zones = new List<Zone>();
        var globalZone = new Zone(-1) { Nodes = 1 };
        var currentZone = new Zone(0) { Nodes = 1, Parts = 1, AvailableNodes = [NodeGridRoot] };
        zones.Add(currentZone);

        // Special case: when the first zone has a size of 1, we can start with the next zone
        if (constraints.GetNodesPerZone(0) == 1) {
            if (constraints.MaxZones == 1) {
                // Special case: only one zone with one node
                return zones;
            }
            currentZone.AvailableNodes.Clear();
            globalZone.AvailableNodes.Add(NodeGridRoot);
            currentZone = new Zone(1) { Nodes = 0 };
            zones.Add(currentZone);
        }

        /*
         * The loop has two parts for every zone:
         * - First it creates the parts of the current zone until it reaches the limit of parts. That means creating new nodes connected to any of the global
         * available nodes (newDoorAdded). When a zone reaches the limit of doors out, the nodes of this zone are remove from the global available nodes.
         * - Then it expands the current zone until it reaches the limit of nodes per zone. That means creating new nodes connected to the current zone nodes,
         * using the current zone available nodes.
         */

        while (true) {
            NodeGrid currentNode = null!;
            var newPart = currentZone.Parts < constraints.GetParts(currentZone.Id);
            if (newPart) {
                // The current zone still doesn't have all the parts: we pick a node from the global to create a new door to the current zone
                if (globalZone.AvailableNodes.Count == 0) {
                    throw new NotAvailableNodeException($"No more available nodes in the maze to open new doors to the the zone {currentZone.Id}");
                }
                currentNode = rng.Next(globalZone.AvailableNodes);
                // currentNode = globalZone.AvailableNodes[^1];
            } else {
                // Expanding the current zone: we pick a node from the current zone to make the zone bigger
                if (currentZone.AvailableNodes.Count == 0) {
                    throw new NotAvailableNodeException($"No more available nodes in the zone {currentZone.Id} to expand");
                }
                currentNode = rng.Next(currentZone.AvailableNodes);
                // currentNode = currentZone.AvailableNodes[^1];
            }

            var availableDirections = GetAvailableDirections(currentNode.Position);

            if (availableDirections.Count > 0) {
                if (newPart) {
                    var oldZone = zones[currentNode.Zone];
                    oldZone.DoorsOut++;
                    if (oldZone.DoorsOut >= constraints.GetMaxDoorsOut(oldZone.Id)) {
                        // We reach the limit of doors out for this zone: removing the available nodes from the global pending nodes,
                        // so we will not use the nodes from this zone anymore
                        globalZone.AvailableNodes.RemoveAll(node => node.Zone == oldZone.Id);
                    }
                    currentZone.Parts++;
                }
                var nextDir = rng.Next(availableDirections);
                var nextPos = currentNode.Position + nextDir;
                var newNode = GetOrCreateNode(nextPos);
                newNode.Parent = currentNode;
                newNode.Zone = currentZone.Id;
                ConnectNode(currentNode, nextDir, true);
                globalZone.Nodes++;
                if (globalZone.Nodes == maxTotalNodes) break;
                currentZone.AvailableNodes.Add(newNode);
                currentZone.Nodes++;
                if (currentZone.Nodes >= constraints.GetNodesPerZone(currentZone.Id)) {
                    // The current zone is full, we need to create a new zone
                    if (currentZone.Id == constraints.MaxZones - 1) {
                        // last zone, we can't create more zones
                        break;
                    }
                    if (constraints.GetMaxDoorsOut(currentZone.Id) > 0) {
                        // Only if the new zone has doors out, we add the available nodes from the current zone to the global pending nodes
                        globalZone.AvailableNodes.AddRange(currentZone.AvailableNodes);
                    }
                    currentZone = new Zone(currentZone.Id + 1);
                    zones.Add(currentZone);
                }
            } else {
                // invalid node, removing from the zone and from the global pending nodes
                globalZone.AvailableNodes.Remove(currentNode);
                currentZone.AvailableNodes.Remove(currentNode);
            }
            // foreach (var zone in zones) {
                // Console.WriteLine($"Zone {zone.Id} Nodes: {zone.Nodes} Parts: {zone.Parts} DoorsOut: {zone.DoorsOut}/{constraints.GetMaxDoorsOut(zone.Id)}");
            // }
        }
        return zones;
    }

    private static void ValidateGrowZonedConstraints(IMazeZonedConstraints constraints) {
        int totalParts = 0, totalMaxDoorsOut = 0;
        for (var zone = 0; zone < constraints.MaxZones; zone++) {
            totalParts += constraints.GetParts(zone);
            if (zone == constraints.MaxZones - 1) {
                if (constraints.GetMaxDoorsOut(zone) != 0) {
                    throw new ArgumentException($"The max doors out for the last zone must be 0. Please, set it to 0");
                }
            } else {
                totalMaxDoorsOut += constraints.GetMaxDoorsOut(zone);
            }
        }
        if (totalMaxDoorsOut < totalParts - 1) {
            throw new ArgumentException($"The sum of max doors out must be greater than the sum of the parts. Please, increase the max doors out by " + (totalParts - totalMaxDoorsOut - 1) + " in any zone except the last one, which must be 0");
        }
    }
}

public class NotAvailableNodeException(string message) : Exception(message);