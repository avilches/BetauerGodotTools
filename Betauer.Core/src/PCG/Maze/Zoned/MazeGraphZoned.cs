using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeGraphZoned<T>(int width, int height) : BaseMazeGraph<T>(width, height) {
    public List<ZoneCreated<T>> GrowZoned(Vector2I start, IMazeZonedConstraints constraints, Random? rng = null) {
        if (!IsValidPosition(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        var maxTotalNodes = constraints.MaxTotalNodes == -1 ? int.MaxValue : constraints.MaxTotalNodes;
        if (maxTotalNodes == 0 || constraints.MaxZones == 0) return [];

        rng ??= new Random();

        Root = CreateNode(start);
        Root.Zone = 0;

        var zones = new List<ZoneCreated<T>>();
        var globalZone = new ZoneCreated<T>(constraints, -1) { Nodes = 1 };
        var currentZone = new ZoneCreated<T>(constraints, 0) { Nodes = 1, AvailableNodes = [Root] };
        currentZone.CreateNewPart(Root);
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
            currentZone = new ZoneCreated<T>(constraints, 1) { Nodes = 0 };
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
            var (currentNode, newDoorOut) = PickNextNode(constraints, globalZone, currentZone, rng);

            var availablePositions = GetValidFreeAdjacentPositions(currentNode.Position).ToList();

            if (availablePositions.Count == 0) {
                // invalid node, removing from the zone and from the global pending nodes
                globalZone.AvailableNodes.Remove(currentNode);
                currentZone.AvailableNodes.Remove(currentNode);
                continue;
            }
            var nextPos = rng.Next(availablePositions);
            var newNode = CreateNode(nextPos, currentNode);
            newNode.Zone = currentZone.ZoneId;
            currentNode.ConnectTo(newNode);
            newNode.ConnectTo(currentNode);
            globalZone.Nodes++;

            if (newDoorOut) {
                var previousZone = zones[currentNode.Zone];
                previousZone.DoorsOut++;
                if (previousZone.DoorsOut >= previousZone.MaxDoorsOut) {
                    // We reach the limit of doors out for this zone: removing the available nodes from the global pending nodes,
                    // so we will not use the nodes from this zone anymore
                    globalZone.AvailableNodes.RemoveAll(node => node.Zone == previousZone.ZoneId);
                }
                currentZone.CreateNewPart(newNode);
            } else {
                currentZone.FindPart(currentNode).AddNode(newNode);
            }

            if (globalZone.Nodes == maxTotalNodes) break;
            currentZone.AvailableNodes.Add(newNode);
            currentZone.Nodes++;
            if (currentZone.Nodes >= constraints.GetNodesPerZone(currentZone.ZoneId)) {
                // The current zone is full, we need to create a new zone
                if (currentZone.ZoneId == constraints.MaxZones - 1) {
                    // last zone, we can't create more zones
                    break;
                }
                if (currentZone.MaxDoorsOut > 0) { // -1 means no limit
                    // Only if the new zone has doors out, we add the available nodes from the current zone to the global pending nodes
                    globalZone.AvailableNodes.AddRange(currentZone.AvailableNodes);
                }
                currentZone = new ZoneCreated<T>(constraints, currentZone.ZoneId + 1);
                zones.Add(currentZone);
            }
            // foreach (var zone in zones) {
            // Console.WriteLine($"Zone {zone.Id} Nodes: {zone.Nodes} Parts: {zone.Parts} DoorsOut: {zone.DoorsOut}/{constraints.GetMaxDoorsOut(zone.Id)}");
            // }
        }
        // Remove nodes without free adjacent positions
        foreach (var zone in zones) {
            zone.AvailableNodes.RemoveAll(node => !GetValidFreeAdjacentPositions(node.Position).Any());
        }
        return zones;
    }

    private static (MazeNode<T> currentNode, bool newDoorOut) PickNextNode(IMazeZonedConstraints constraints, ZoneCreated<T> globalZone, ZoneCreated<T> currentZone, Random rng) {
        // The algorithm will try to
        // create as many parts as free doors out are in still available in the previous zones.
        // But if there are no more doors out, or there are, but there are no more free nodes to
        // connect them, then the zone will have fewer parts than expected.
        //
        // On the other hand, if all the parts were created and the zone is expanding, but there are
        // no more free nodes available to connect, the algorithm could create more parts (if there are
        // doors out available and there are free nodes to connect them). In this case, the zone will
        // have more parts than expected.

        var newDoorOut = currentZone.ZoneId > 0 && currentZone.Parts.Count < constraints.GetParts(currentZone.ZoneId);

        if (newDoorOut) {
            // The current zone still doesn't have all the parts: pick a random node from the global to create a new door in the maze to the current zone
            if (globalZone.AvailableNodes.Count > 0) return PickDoorOutNode(globalZone, rng);
            // No more available nodes in the global zone! WORKAROUND: expand the current zone (it means the zone will not have all the parts)
            if (currentZone.AvailableNodes.Count > 0) return PickNodeToExpand(currentZone, rng);
            throw new NoMoreNodesException(
                $"No more available nodes to create new parts in zone {currentZone.ZoneId} (and the current zone can't be expanded neither). " +
                "Consider increasing nodes and maxDoorOut in previous zones.");
        }

        // Expanding the current zone
        if (currentZone.AvailableNodes.Count > 0) return PickNodeToExpand(currentZone, rng);
        // Can't expand the current zone. WORKAROUND: create a new part
        if (globalZone.AvailableNodes.Count > 0) return PickDoorOutNode(globalZone, rng);
        throw new NoMoreNodesException(
            $"No more available nodes to expand zone {currentZone.ZoneId} (and there are not available nodes to create new parts) " +
            "Consider increasing nodes and maxDoorOut in previous zones.");
    }

    private static (MazeNode<T> currentNode, bool newDoorOut) PickNodeToExpand(ZoneCreated<T> currentZone, Random rng) {
        if (currentZone.Corridor) {
            // Corridors always try to expand a last node of every part
            var candidates = currentZone.AvailableNodes.Where(node => node.OutDegree == 1).ToList();
            var expansionNode = rng.Next(candidates.Count > 0 ? candidates : currentZone.AvailableNodes);
            return (expansionNode, false);
        }
        return (rng.Next(currentZone.AvailableNodes), false); // No corridors pick a random node to expand
    }

    private static (MazeNode<T> currentNode, bool newDoorOut) PickDoorOutNode(ZoneCreated<T> globalZone, Random rng) {
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
        var index = rng.NextIndexExponential(globalZone.AvailableNodes.Count, 3.0f);
        return (globalZone.AvailableNodes[index], true);
    }
}