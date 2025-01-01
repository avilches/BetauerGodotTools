using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public static class MazeZonedExtensions {
    public static MazeZones GrowZoned(this MazeGraph maze, Vector2I start, IMazeZonedConstraints constraints, Random? rng = null, float? entryNodeFactor = null) {
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
        var generator = new MazeZonedGenerator(entryNodeFactor ?? 3.0f);
        return generator.Generate(maze, start, constraints, rng);
    }
}

/// <summary>
/// Generates a zoned maze structure where each zone can have multiple disconnected parts.
/// Each zone and part has specific constraints for growth and connections.
/// </summary>
public class MazeZonedGenerator(float entryNodeFactor) {
    private MazeGraph _maze = null!;
    private IMazeZonedConstraints _constraints = null!;
    private Random _rng = null!;

    private readonly List<ZoneGeneration> _zones = [];
    private ZoneGeneration _globalZone = null!;
    private ZoneGeneration _currentZone = null!;
    private MazeNode _startNode = null!;
    private int _maxTotalNodes;

    public MazeZones Generate(MazeGraph maze, Vector2I start, IMazeZonedConstraints constraints, Random? rng = null) {
        if (!maze.IsValidPosition(start)) {
            throw new ArgumentException("Invalid start position", nameof(start));
        }
        
        InitializeGeneration(maze, start, constraints, rng ?? new Random());
        
        if (_maxTotalNodes == 0 || constraints.MaxZones == 0) {
            return new MazeZones(_maze, null, constraints, []);
        }

        GenerateFirstZone(start);
        
        // Special case: when the first zone has a size of 1, we don't need to expand it,
        // so we can just start with second zone 
        if (_constraints.GetNodesPerZone(0) == 1) {
            if (_constraints.MaxZones == 1) {
                // Special case: only one zone with one node
                return CreateMazeZones();
            }
            StartSecondZone();
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
            var (parentNode, newNode) = CreateNextNode();

            if (newNode.ZoneId != parentNode.ZoneId) {
                // New part created! newNode is the start of the new part. parentNode is an exit node from the previous zone
                HandleNewPartCreation(parentNode, newNode);
            } else {
                // The new node belongs to the same zone and part as the parent node
                _currentZone.AddNode(parentNode.PartId, newNode);
            }

            if (_globalZone.NodesCreated == _maxTotalNodes) {
                break;
            }
            
            _currentZone.AvailableNodes.Add(newNode);
            _currentZone.NodesCreated++;
            
            if (ShouldCreateNewZone()) {
                if (_currentZone.ZoneId == _constraints.MaxZones - 1) {
                    // last zone, we can't create more zones
                    break;
                }
                CreateNewZone();
            }
        }
        
        return CreateMazeZones();
    }

    private MazeZones CreateMazeZones() {
        var list = _zones.Select(zone => zone.ToZone()).ToList();
        var mazeZones = new MazeZones(_maze, _startNode, _constraints, list);
        foreach (var zone in list) {
            zone.MazeZones = mazeZones;
        }
        return mazeZones;
    }

    private void InitializeGeneration(MazeGraph maze, Vector2I start, IMazeZonedConstraints constraints, Random rng) {
        _maze = maze;
        _constraints = constraints;
        _rng = rng;
        _maxTotalNodes = constraints.MaxTotalNodes == -1 ? int.MaxValue : constraints.MaxTotalNodes;
        _zones.Clear();
    }

    private void GenerateFirstZone(Vector2I start) {
        _startNode = _maze.CreateNode(start);
        _startNode.ZoneId = 0;
        _startNode.PartId = 0;
        _globalZone = new ZoneGeneration(_maze, _constraints, -1) { NodesCreated = 1 };
        _currentZone = new ZoneGeneration(_maze, _constraints, 0) { NodesCreated = 1, AvailableNodes = [_startNode] };
        _currentZone.CreateNewPart(_startNode);
        _zones.Add(_currentZone);
    }

    private void StartSecondZone() {
        _currentZone.AvailableNodes.Clear();
        _globalZone.AvailableNodes.Add(_startNode);
        _currentZone = new ZoneGeneration(_maze, _constraints, 1) { NodesCreated = 0 };
        _zones.Add(_currentZone);
    }

    private void HandleNewPartCreation(MazeNode parentNode, MazeNode newNode) {
        var previousZone = _zones[parentNode.ZoneId];
        previousZone.ExitNodesCreated++;
        if (previousZone.ExitNodesCreated >= previousZone.MaxExitNodes) {
            // We reach the limit of exit nodes for this zone: removing the available nodes from the global pending nodes,
            // so we will not use the nodes from this zone anymore
            _globalZone.AvailableNodes.RemoveAll(node => node.ZoneId == previousZone.ZoneId);
        }
        _currentZone.CreateNewPart(newNode);
    }

    private bool ShouldCreateNewZone() {
        return _currentZone.NodesCreated >= _constraints.GetNodesPerZone(_currentZone.ZoneId);
    }

    private void CreateNewZone() {
        if (_currentZone.MaxExitNodes > 0) { // -1 means no limit
            // Only if the new zone has still exit nodes without using it, we add the available nodes from the current zone
            // to the global pending nodes, so they can be used to create more parts in other zones
            _globalZone.AvailableNodes.AddRange(_currentZone.AvailableNodes);
        }
        // Recover temporarily discarded nodes. They were removed because the part was FlexibleParts disabled and the nodes, even if they
        // were valid, they will have adjacent nodes without free positions to grow. We recover them because they could be used in future zones
        _globalZone.RecoverTemporarilyDiscardedNodes();
        _currentZone.AvailableNodes.Clear(); // We don't need the nodes anymore, clean them
        _currentZone = new ZoneGeneration(_maze, _constraints, _currentZone.ZoneId + 1);
        _zones.Add(_currentZone);
    }

    private (MazeNode parentNode, MazeNode newNode) CreateNextNode() {
        var parentNode = PickParentNode();
        var availablePositions = _maze.GetAvailableAdjacentPositions(parentNode.Position).ToList();

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
                if (_constraints.IsFlexibleParts(_currentZone.ZoneId)) {
                    // If the part is flexible, we can use all the available positions even if some of them are bad candidates :)
                    break;
                }
                // Removing the available positions without free adjacent positions 
                availablePositions.RemoveAll(pos => !_maze.GetAvailableAdjacentPositions(pos).Any());
                if (availablePositions.Count > 0) {
                    // If after the removal the list is not empty, we can use it
                    break;
                }
                // The parent node is a bad candidate, remove it from the available nodes temporarily
                if (_globalZone.AvailableNodes.Contains(parentNode)) _globalZone.TemporarilyDiscardedNodes.Add(parentNode);
            }

            // invalid node, removing from the zone and from the global pending nodes
            _globalZone.AvailableNodes.Remove(parentNode);
            _currentZone.AvailableNodes.Remove(parentNode);
            parentNode = PickParentNode();
            availablePositions = _maze.GetAvailableAdjacentPositions(parentNode.Position).ToList();
        }
        
        var nextPos = _rng.Next(availablePositions);
        var newNode = _maze.CreateNode(nextPos, parentNode);
        newNode.ZoneId = _currentZone.ZoneId;
        parentNode.ConnectTo(newNode);
        newNode.ConnectTo(parentNode);
        _globalZone.NodesCreated++;
        return (parentNode, newNode);
    }

    private MazeNode PickParentNode() {
        // newPart = we have to create new part in the current zone
        var newEntry = _currentZone.ZoneId > 0 && _currentZone.Parts.Count < _constraints.GetParts(_currentZone.ZoneId);

        if (newEntry) {
            // The current zone still doesn't have all the parts:
            // Then pick a random node from the global to create a new entry node the maze to the current zone
            if (_globalZone.AvailableNodes.Count > 0) return PickNewEntryNode();
            // No more available nodes in the global zone!
            // IsFlexibleParts is true: expand the current zone picking up a node in the currentZone. That means the zone will not have
            // all the parts, but at least it will have all the nodes needed for the zone.
            if (_constraints.IsFlexibleParts(_currentZone.ZoneId)) {
                if (_currentZone.AvailableNodes.Count == 0)
                    throw new NoMoreNodesException(
                        $"No more available nodes to create new parts in zone {_currentZone.ZoneId} (IsFlexibleParts is true but the current zone can't be expanded neither). " +
                        "Consider increasing nodes and MaxExitNodes in previous zones.");
                return PickNodeToExpand();
            }
            // IsFlexibleParts is false, throw an exception
            throw new NoMoreNodesException(
                $"No more available nodes to create new parts in zone {_currentZone.ZoneId}. " +
                "Consider increasing nodes and/or MaxExitNodes in previous zones, or enable IsFlexibleParts.");
        }

        // The current zone has all the parts needed: expanding the current zone: picking up a random node in the currentZone
        if (_currentZone.AvailableNodes.Count > 0) return PickNodeToExpand();

        // Can't expand the current zone? Check FlexibleParts flag is enabled, so we can create new parts
        if (_constraints.IsFlexibleParts(_currentZone.ZoneId)) {
            if (_globalZone.AvailableNodes.Count == 0)
                throw new NoMoreNodesException(
                    $"No more available nodes to expand zone {_currentZone.ZoneId} (IsFlexibleParts is true but there are not available nodes to create new parts) " +
                    "Consider increasing nodes and MaxExitNodes in previous zones.");
            return PickNewEntryNode();
        }

        // create a new part (if it's possible)
        throw new NoMoreNodesException(
            $"No more available nodes to expand zone {_currentZone.ZoneId}. " +
            "Consider enabling FlexibleParts to create new parts when expansion is not possible.");
    }

    private MazeNode PickNodeToExpand() {
        if (_currentZone.IsCorridor) {
            // Corridors always try to expand the last node of one of the parts
            var candidates = _currentZone.AvailableNodes
                .GroupBy(node => node.PartId)
                .Select(group => group.Last())
                .ToList();
            return _rng.Next(candidates);
        }
        return _rng.Next(_currentZone.AvailableNodes); // No corridors pick a random node to expand
    }

    private MazeNode PickNewEntryNode() {
        // The zone 0 doesn't have any previous zone.
        // The zone 1 has the zone 0 has previous zone, so there is no need to filter by "previous zone only" because all _globalZone.AvailableNodes are from 
        // the zone 0. We only filter by "previos zone only" in the zone 2 and above.
        if (_currentZone.ZoneId >= 2 && _currentZone.Parts.Count == 0) {
            // First part of the zone, pick a random node only from the previous zone. Example: ensure the zone 2 has an entry from the zone 1.
            var candidates = _globalZone.AvailableNodes
                .Where(node => node.ZoneId == _currentZone.ZoneId - 1)
                .OrderBy(node => node.Depth)
                .ToList();
            if (candidates.Count > 0) return _rng.NextExponential(candidates, entryNodeFactor);
        }
        // Not the first part of the zone, pick a random node from the global zone (it could be any lower node than the current zone)
        // Example: the zone 3 could have entries from the zone 0, 1 or 2
        return _rng.NextExponential(_globalZone.AvailableNodes.OrderBy(node => node.Depth).ToList(), entryNodeFactor);
    }
}