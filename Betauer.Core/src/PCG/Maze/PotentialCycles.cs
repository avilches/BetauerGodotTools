using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class PotentialCycles {
    private readonly bool _useParentDistance;
    private readonly MazeGraph _graph;
    private readonly List<(MazeNode nodeA, MazeNode nodeB, (Vector2I, Vector2I) connection)> _potentialConnections;

    public PotentialCycles(MazeGraph graph, bool useParentDistance) {
        _graph = graph;
        _useParentDistance = useParentDistance;
        // Get all potential cycles once
        _potentialConnections = graph.GetNodes()
            .SelectMany(nodeA =>
                graph.GetAdjacentNodes(nodeA.Position)
                    .Where(nodeB => !nodeA.HasEdgeTo(nodeB))
                    .Select(nodeB => {
                        var posA = nodeA.Position;
                        var posB = nodeB.Position;
                        var connection = posA.X < posB.X || (posA.X == posB.X && posA.Y < posB.Y)
                            ? (posA, posB)
                            : (posB, posA);
                        return (nodeA, nodeB, connection);
                    }))
            .DistinctBy(x => x.connection)
            .ToList();
    }

    public PotentialCyclesQuery Query() => new(this);

    public void RemoveCycle(MazeNode nodeA, MazeNode nodeB) {
        _potentialConnections.RemoveAll(x =>
            (x.nodeA == nodeA && x.nodeB == nodeB) ||
            (x.nodeA == nodeB && x.nodeB == nodeA));
    }

    public class PotentialCyclesQuery {
        private readonly PotentialCycles _cycles;
        private readonly List<Func<MazeNode, MazeNode, int, bool>> _filters = [];
        private bool _orderByDescending = true;

        internal PotentialCyclesQuery(PotentialCycles cycles) {
            _cycles = cycles;
        }

        public PotentialCyclesQuery Where(Func<MazeNode, MazeNode, int, bool> filter) {
            _filters.Add(filter);
            return this;
        }

        public PotentialCyclesQuery WhereNodesInSameZone() {
            _filters.Add((nodeA, nodeB, _) => nodeA.ZoneId == nodeB.ZoneId);
            return this;
        }

        public PotentialCyclesQuery WhereNodesInZone(int zoneId) {
            _filters.Add((nodeA, nodeB, _) => nodeA.ZoneId == zoneId && nodeB.ZoneId == zoneId);
            return this;
        }

        public PotentialCyclesQuery WhereNodesInZones(int zoneIdA, int zoneIdB) {
            _filters.Add((nodeA, nodeB, _) =>
                (nodeA.ZoneId == zoneIdA && nodeB.ZoneId == zoneIdB) ||
                (nodeA.ZoneId == zoneIdB && nodeB.ZoneId == zoneIdA));
            return this;
        }

        public PotentialCyclesQuery WhereNodesInDifferentZones() {
            _filters.Add((nodeA, nodeB, _) => nodeA.ZoneId != nodeB.ZoneId);
            return this;
        }

        public PotentialCyclesQuery WhereCyclesEquals(int distance) {
            _filters.Add((_, _, dist) => dist == distance);
            return this;
        }

        public PotentialCyclesQuery WhereCyclesLongerThan(int min) {
            _filters.Add((_, _, dist) => dist >= min);
            return this;
        }

        public PotentialCyclesQuery WhereCyclesShorterThan(int max) {
            _filters.Add((_, _, dist) => dist <= max);
            return this;
        }

        public PotentialCyclesQuery WhereCyclesBetween(int min, int max) {
            _filters.Add((_, _, dist) => dist >= min && dist < max);
            return this;
        }

        public PotentialCyclesQuery OrderByLongestDistance() {
            _orderByDescending = true;
            return this;
        }

        public PotentialCyclesQuery OrderByShortestDistance() {
            _orderByDescending = false;
            return this;
        }

        public IEnumerable<(MazeNode nodeA, MazeNode nodeB, int distance)> Execute() {
            var query = _cycles._potentialConnections
                .Where(x => _cycles._graph.CanConnect(x.nodeA, x.nodeB))
                .Select(x => {
                    var distance = _cycles._useParentDistance
                        ? x.nodeA.GetTreeDistanceToNode(x.nodeB)
                        : x.nodeA.GetGraphDistanceToNode(x.nodeB);
                    return (x.nodeA, x.nodeB, distance);
                });

            // Apply all filters
            foreach (var filter in _filters) {
                query = query.Where(x => filter(x.nodeA, x.nodeB, x.distance));
            }

            // Apply ordering
            return _orderByDescending
                ? query.OrderByDescending(x => x.distance)
                : query.OrderBy(x => x.distance);
        }

        /// <summary>
        /// Execute the query and connect the nodes that match the criteria, up to maxCycles connections.
        /// The query is re-executed after each connection since distances between nodes might change.
        /// </summary>
        public List<(MazeNode nodeA, MazeNode nodeB, int distance)> Connect(int maxCycles) {
            List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];

            while (cyclesAdded.Count < maxCycles) {
                // Re-execute query each time to get fresh distances
                var nextCycle = Execute().FirstOrDefault();

                // If no more potential cycles found, break
                if (nextCycle == default) break;

                nextCycle.nodeA.ConnectTo(nextCycle.nodeB);
                nextCycle.nodeB.ConnectTo(nextCycle.nodeA);
                cyclesAdded.Add(nextCycle);
            }
            return cyclesAdded;
        }
        /// <summary>
        /// Execute the query and connect the nodes that match the criteria, up to maxCycles connections.
        /// The query is re-executed after each connection since distances between nodes might change.
        /// </summary>
        public List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectAll() {
            List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];
            foreach (var cycle in Execute()) {
                cycle.nodeA.ConnectTo(cycle.nodeB);
                cycle.nodeB.ConnectTo(cycle.nodeA);
                cyclesAdded.Add(cycle);
            }
            return cyclesAdded;
        }
    }
}