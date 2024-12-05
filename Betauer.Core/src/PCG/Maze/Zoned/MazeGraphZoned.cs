using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeGraphZoned(int width, int height) : BaseMazeGraph(width, height) {
    public List<ZoneCreated> GrowZoned(Vector2I start, IMazeZonedConstraints constraints, Random? rng = null) {
        if (!IsValidPosition(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        var maxTotalNodes = constraints.MaxTotalNodes == -1 ? int.MaxValue : constraints.MaxTotalNodes;
        if (maxTotalNodes == 0 || constraints.MaxZones == 0) return [];

        Root = null;
        NodeGrid.Fill(null);
        Nodes.Clear();
        LastId = 0;

        rng ??= new Random();

        Root = CreateNode(start);
        Root.Zone = 0;

        var zones = new List<ZoneCreated>();
        var globalZone = new ZoneCreated(constraints, -1) { Nodes = 1 };
        var currentZone = new ZoneCreated(constraints, 0) { Nodes = 1, Parts = 1, AvailableNodes = [Root] };
        zones.Add(currentZone);

        // Special case: when the first zone has a size of 1, we don't need to expand it,
        // so we can just start with second zone 
        if (constraints.GetNodesPerZone(0) == 1) {
            if (constraints.MaxZones == 1) {
                // Special case: only one zone with one node
                return zones;
            }
            currentZone.AvailableNodes.Clear();
            globalZone.AvailableNodes.Add(Root);
            currentZone = new ZoneCreated(constraints, 1) { Nodes = 0 };
            zones.Add(currentZone);
        }

        while (true) {
            /*
             * The loop has two parts for every zone:
             * 1) First it creates the parts of the current zone until all parts are created. A zone part is a set of one or more nodes that are
             * not connected to other parts of the same zone. Each part will have only one connection to the previous zone. To create a new part,
             * it first finds a random node from the global available nodes and connect it to a new node of the current part.
             * After add it, the current zone will have one more part, and the previous zone will have one more door out. If the previous zone reaches
             * the limit of doors out, the nodes of this zone are remove from the global available nodes.
             *
             * The first time we create a new zone, the previous zone nodes will be added to the global available zones if, and only if, the zone had
             * more than 0 doors out. This ensures the zone will not have any connection (door out) to a new zone.
             *
             * 2) When all parts of the current zone are created, it expands the parts randomly it until the zone reaches the limit of nodes per zone.
             * To expand the zone, it finds a random node from the current zone available nodes and connect it to a new node.
             */
            var (currentNode, newPart) = FindNextNode(constraints, globalZone, currentZone, rng);

            var availablePositions = GetValidFreeAdjacentPositions(currentNode.Position).ToList();

            if (availablePositions.Count == 0) {
                // invalid node, removing from the zone and from the global pending nodes
                globalZone.AvailableNodes.Remove(currentNode);
                currentZone.AvailableNodes.Remove(currentNode);
                continue;
            }
            if (newPart) {
                var previousZone = zones[currentNode.Zone];
                previousZone.DoorsOut++;
                if (previousZone.DoorsOut >= previousZone.MaxDoorsOut) {
                    // We reach the limit of doors out for this zone: removing the available nodes from the global pending nodes,
                    // so we will not use the nodes from this zone anymore
                    globalZone.AvailableNodes.RemoveAll(node => node.Zone == previousZone.Id);
                }
                currentZone.Parts++;
            }
            var nextPos = rng.Next(availablePositions);
            var newNode = CreateNode(nextPos, currentNode);
            newNode.Parent = currentNode;
            newNode.Zone = currentZone.Id;
            ConnectNodes(currentNode, newNode);
            ConnectNodes(newNode, currentNode);
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
                if (currentZone.MaxDoorsOut > 0) { // -1 means no limit
                    // Only if the new zone has doors out, we add the available nodes from the current zone to the global pending nodes
                    globalZone.AvailableNodes.AddRange(currentZone.AvailableNodes);
                }
                currentZone = new ZoneCreated(constraints, currentZone.Id + 1);
                zones.Add(currentZone);
            }
            // foreach (var zone in zones) {
            // Console.WriteLine($"Zone {zone.Id} Nodes: {zone.Nodes} Parts: {zone.Parts} DoorsOut: {zone.DoorsOut}/{constraints.GetMaxDoorsOut(zone.Id)}");
            // }
        }
        return zones;
    }

    private static (MazeNode currentNode, bool newPart) FindNextNode(IMazeZonedConstraints constraints, ZoneCreated globalZone, ZoneCreated currentZone, Random rng) {
        var newPart = currentZone.Id != 0 && currentZone.Parts < constraints.GetParts(currentZone.Id);
        if (newPart) {
            // The current zone still doesn't have all the parts: we pick a random node from the global to create a new door to the current zone
            if (globalZone.AvailableNodes.Count == 0) {
                throw new NoMoreNodesException(
                    $"No more available nodes in the maze to open new doors to zone {currentZone.Id}. " +
                    "Consider increasing nodes and maxDoorOut in previous zones.");
            }
            return (rng.Next(globalZone.AvailableNodes), true);
        }

        // Try to expand the current zone
        if (currentZone.AvailableNodes.Count > 0) {
            // Expanding the current zone is possible: we pick a node from the current zone to make the zone bigger
            return (currentZone.PickNextNode(rng), false);
        }
        // Can't expand the current zone. If AutoSplitOnExpand is true, we are allowed create a new part
        if (constraints.IsAutoSplitOnExpand(currentZone.Id)) {
            // AutoSplitOnExpand is enabled!
            if (globalZone.AvailableNodes.Count == 0) {
                // We are allowed, but we can't because there are not available nodes in the global zone...
                throw new NoMoreNodesException(
                    $"No more available nodes to create new parts in zone {currentZone.Id} with AutoSplitOnExpand enabled. " +
                    "Consider increasing nodes and maxDoorOut in previous zones.");
            }
            // AutoSplitOnExpand is enabled and we have available nodes in the global zone: pick a random one to create a new part
            return (rng.Next(globalZone.AvailableNodes), true);
        }
        if (globalZone.AvailableNodes.Count > 0) {
            throw new NoMoreNodesException(
                $"No more available nodes in zone {currentZone.Id} to expand. " +
                "Consider enabling AutoSplitOnExpand for this zone.");
        }

        throw new NoMoreNodesException($"No more available nodes in zone {currentZone.Id} to expand");
    }
}