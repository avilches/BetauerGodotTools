using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.PCG.Maze.Zoned;
using Godot;

namespace Betauer.Core.PCG.Maze;

public partial class MazeGraph {
    /*
     Factor 3 means this distribution:
     [0] ############# (27.9%)
     [1] ########## (20.5%)
     [2] ####### (15.2%)
     [3] ##### (11.3%)
     [4] #### (8.3%)
     [5] ### (6.2%)
     [6] ## (4.6%)
     [7] # (3.4%)
     [8] # (2.5%)
     [9]  (0.0%)

     So, most probable to get nodes from the beginning of the list (closer to the start of the maze)
     */
    public float EntryNodeFactor { get; set; } = 3.0f;

    public MazeZones GrowZoned(Vector2I start, IMazeZonedConstraints constraints, Random? rng = null) {
        if (!IsValidPosition(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        var maxTotalNodes = constraints.MaxTotalNodes == -1 ? int.MaxValue : constraints.MaxTotalNodes;
        if (maxTotalNodes == 0 || constraints.MaxZones == 0) return new MazeZones(this, constraints, []);

        rng ??= new Random();
        var zones = new List<ZoneGeneration>();

        Root = CreateNode(start);
        Root.ZoneId = 0;
        Root.PartId = 0;
        var globalZone = new ZoneGeneration(this, constraints, -1) { NodesCreated = 1 };
        var currentZone = new ZoneGeneration(this, constraints, 0) { NodesCreated = 1, AvailableNodes = [Root] };
        currentZone.CreateNewPart(Root);
        zones.Add(currentZone);

        // Special case: when the first zone has a size of 1, we don't need to expand it,
        // so we can just start with second zone 
        if (constraints.GetNodesPerZone(0) == 1) {
            if (constraints.MaxZones == 1) {
                // Special case: only one zone with one node
                return CreateMazeZones(constraints, zones);
            }
            currentZone.AvailableNodes.Clear();
            globalZone.AvailableNodes.Add(Root);
            currentZone = new ZoneGeneration(this, constraints, 1) { NodesCreated = 0 };
            zones.Add(currentZone);
        }

        while (true) {
            /*
             * The loop has two parts for every zone:
             * 1) First it creates the parts of the current zone until all parts are created. A zone part is a set of one or more nodes that are
             * not connected to other parts of the same zone. Each part will have only one connection to the previous zone. To create a new part,
             * it first finds a random node from the global available nodes and connect it to a new node of the current part.
             * After add it, the current zone will have one more part, and the previous zone will have one more exit node. If the previous zone reaches
             * the limit of exit nodes, the nodes of this zone are remove from the global available nodes.
             *
             * The first time we create a new zone, the previous zone nodes will be added to the global available zones if, and only if, the zone had
             * more than 0 exit nodes. This ensures the zone will not have any connection (exit node) to a new zone.
             *
             * 2) When all parts of the current zone are created, it expands the parts randomly it until the zone reaches the limit of nodes per zone.
             * To expand the zone, it finds a random node from the current zone available nodes and connect it to a new node.
             */
            var (parentNode, newNode) = CreateNextNode(constraints, globalZone, currentZone, rng);

            if (newNode.ZoneId != parentNode.ZoneId) {
                // New part created! newNode is the start of the new part. parentNode is an exit node from the previous zone
                var previousZone = zones[parentNode.ZoneId];
                previousZone.ExitNodesCreated++;
                if (previousZone.ExitNodesCreated >= previousZone.MaxExitNodes) {
                    // We reach the limit of exit nodes for this zone: removing the available nodes from the global pending nodes,
                    // so we will not use the nodes from this zone anymore
                    globalZone.AvailableNodes.RemoveAll(node => node.ZoneId == previousZone.ZoneId);
                }
                currentZone.CreateNewPart(newNode);
            } else {
                // The new node belongs to the same zone and part as the parent node
                currentZone.AddNode(parentNode.PartId, newNode);
            }

            if (globalZone.NodesCreated == maxTotalNodes) {
                break;
            }
            currentZone.AvailableNodes.Add(newNode);
            currentZone.NodesCreated++;
            if (currentZone.NodesCreated >= constraints.GetNodesPerZone(currentZone.ZoneId)) {
                // The current zone is full, we need to create a new zone
                if (currentZone.ZoneId == constraints.MaxZones - 1) {
                    // last zone, we can't create more zones
                    break;
                }
                if (currentZone.MaxExitNodes > 0) { // -1 means no limit
                    // Only if the new zone has still exit nodes without using it, we add the available nodes from the current zone
                    // to the global pending nodes, so they can be used to create more parts in other zones
                    globalZone.AvailableNodes.AddRange(currentZone.AvailableNodes);
                }
                // Recover temporarily discarded nodes. They were removed because the part was FlexibleParts disabled and the nodes, even if they
                // were valid, they will have adjacent nodes without free positions to grow. We recover them because they could be used in future zones
                // currentZone.RecoverTemporarilyDiscardedNodes();
                globalZone.RecoverTemporarilyDiscardedNodes();
                currentZone.AvailableNodes.Clear(); // We don't need the nodes anymore, clean them
                currentZone = new ZoneGeneration(this, constraints, currentZone.ZoneId + 1);
                zones.Add(currentZone);
            }
            // foreach (var zone in zones) {
            //     var maxExitNodes = constraints.GetMaxExitNodes(zone.ZoneId);
            //     Console.WriteLine($"Zone {zone.ZoneId} Nodes: {zone.NodesCreated} Parts: {zone.Parts.Count}/{constraints.GetParts(zone.ZoneId)} Exits: {zone.ExitNodesCreated}/{(maxExitNodes == -1 ? "∞" : maxExitNodes)}");
            // }
        }
        return CreateMazeZones(constraints, zones);
    }

    private MazeZones CreateMazeZones(IMazeZonedConstraints constraints, List<ZoneGeneration> zones) {
        var list = zones.Select(zone => zone.ToZone()).ToList();
        var mazeZones = new MazeZones(this, constraints, list);
        foreach (var zone in list) {
            zone.MazeZones = mazeZones;
        }
        return mazeZones;
    }

    // The algorithm try to create as many parts as free exit nodes are in still available in the previous zones.
    // Every new part create is just a new node connected to a random node from the previous zone. This random node is the 
    // exit node of the other zone part.
    private (MazeNode parentNode, MazeNode newNode) CreateNextNode(IMazeZonedConstraints constraints, ZoneGeneration globalZone, ZoneGeneration currentZone, Random rng) {
        var parentNode = PickParentNode(constraints, globalZone, currentZone, rng);
        var availablePositions = GetAvailableAdjacentPositions(parentNode.Position).ToList();

        while (true) {
            // 1) Discard the invalid nodes with 0 adjacent free positions
            // 2) Discard bad candidates too: nodes with free adjacent positions, but these adjacent positions
            // are surrounded by other nodes, so they can't grow anymore:
            //  a-a-a-a
            //  |     |
            //  a-1 · a
            //  |     |
            //  a · a-a
            //  |   | |
            //  a-a-a-a
            // The node 1 has two free adjacent positions (right and down), but these positions are surrounded
            // by other nodes, so the node 1 can't grow anymore. This is a bad candidate.
            // This is only for zones with FlexibleParts false. If FlexiblePart is true, then this is not a problem because they can't grow
            // anymore, a new zone could be created.
            if (availablePositions.Count > 0) {
                if (constraints.IsFlexibleParts(currentZone.ZoneId)) {
                    // If the part is flexible, we can use all the available positions even if some of them are bad candidates :)
                    break;
                }
                // Removing the available positions without free adjacent positions 
                availablePositions.RemoveAll(pos => !GetAvailableAdjacentPositions(pos).Any());
                if (availablePositions.Count > 0) {
                    // If after the removal the list is not empty, we can use it
                    break;
                }
                // The parent node is a bad candidate, remove it from the available nodes temporarily
                if (globalZone.AvailableNodes.Contains(parentNode)) globalZone.TemporarilyDiscardedNodes.Add(parentNode);
            }

            // invalid node, removing from the zone and from the global pending nodes
            globalZone.AvailableNodes.Remove(parentNode);
            currentZone.AvailableNodes.Remove(parentNode);
            parentNode = PickParentNode(constraints, globalZone, currentZone, rng);
            availablePositions = GetAvailableAdjacentPositions(parentNode.Position).ToList();
        }
        var nextPos = rng.Next(availablePositions);
        var newNode = CreateNode(nextPos, parentNode);
        newNode.ZoneId = currentZone.ZoneId;
        parentNode.ConnectTo(newNode);
        newNode.ConnectTo(parentNode);
        // OnNodeCreated(newNode);
        globalZone.NodesCreated++;
        return (parentNode, newNode);
    }

    private MazeNode PickParentNode(IMazeZonedConstraints constraints, ZoneGeneration globalZone, ZoneGeneration currentZone, Random rng) {
        // newPart = we have to create new part in the current zone
        var newEntry = currentZone.ZoneId > 0 && currentZone.Parts.Count < constraints.GetParts(currentZone.ZoneId);

        if (newEntry) {
            // The current zone still doesn't have all the parts:
            // Then pick a random node from the global to create a new entry node the maze to the current zone
            if (globalZone.AvailableNodes.Count > 0) return PickNewEntryNode(currentZone, globalZone, rng);
            // No more available nodes in the global zone!
            // IsFlexibleParts is true: expand the current zone picking up a node in the currentZone. That means the zone will not have
            // all the parts, but at least it will have all the nodes needed for the zone.
            if (constraints.IsFlexibleParts(currentZone.ZoneId)) {
                if (currentZone.AvailableNodes.Count == 0)
                    throw new NoMoreNodesException(
                        $"No more available nodes to create new parts in zone {currentZone.ZoneId} (IsFlexibleParts is true but the current zone can't be expanded neither). " +
                        "Consider increasing nodes and MaxExitNodes in previous zones.");
                return PickNodeToExpand(currentZone, rng);
            }
            // IsFlexibleParts is false, throw an exception
            throw new NoMoreNodesException(
                $"No more available nodes to create new parts in zone {currentZone.ZoneId}. " +
                "Consider increasing nodes and/or MaxExitNodes in previous zones, or enable IsFlexibleParts.");
        }

        // The current zone has all the parts needed: expanding the current zone: picking up a random node in the currentZone
        if (currentZone.AvailableNodes.Count > 0) return PickNodeToExpand(currentZone, rng);

        // Can't expand the current zone? Check FlexibleParts flag is enabled, so we can create new parts
        if (constraints.IsFlexibleParts(currentZone.ZoneId)) {
            if (globalZone.AvailableNodes.Count == 0)
                throw new NoMoreNodesException(
                    $"No more available nodes to expand zone {currentZone.ZoneId} (IsFlexibleParts is true but there are not available nodes to create new parts) " +
                    "Consider increasing nodes and MaxExitNodes in previous zones.");
            return PickNewEntryNode(currentZone, globalZone, rng);
        }

        // create a new part (if it's possible)
        throw new NoMoreNodesException(
            $"No more available nodes to expand zone {currentZone.ZoneId}. " +
            "Consider enabling FlexibleParts to create new parts when expansion is not possible.");
    }

    private MazeNode PickNodeToExpand(ZoneGeneration currentZone, Random rng) {
        if (currentZone.IsCorridor) {
            // Corridors always try to expand the last node of one of the parts
            var candidates = currentZone.AvailableNodes
                .GroupBy(node => node.PartId)
                .Select(group => group.Last())
                .ToList();
            return rng.Next(candidates);
        }
        return rng.Next(currentZone.AvailableNodes); // No corridors pick a random node to expand
    }

    private MazeNode PickNewEntryNode(ZoneGeneration currentZone, ZoneGeneration globalZone, Random rng) {
        // The zone 0 doesn't have any previous zone.
        // The zone 1 has the zone 0 has previous zone, so there is no need to filter by "previous zone only" because all globalZone.AvailableNodes are from 
        // the zone 0. We only filter by "previos zone only" in the zone 2 and above.
        if (currentZone.ZoneId >= 2 && currentZone.Parts.Count == 0) {
            // First part of the zone, pick a random node only from the previous zone. Example: ensure the zone 2 has an entry from the zone 1.
            var candidates = globalZone.AvailableNodes
                .Where(node => node.ZoneId == currentZone.ZoneId - 1)
                .OrderBy(node => node.Depth)
                .ToList();
            if (candidates.Count > 0) return rng.NextExponential(candidates, EntryNodeFactor);
        }
        // Not the first part of the zone, pick a random node from the global zone (it could be any lower node than the current zone)
        // Example: the zone 3 could have entries from the zone 0, 1 or 2
        return rng.NextExponential(globalZone.AvailableNodes.OrderBy(node => node.Depth).ToList(), EntryNodeFactor);
    }
}