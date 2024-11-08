using System.Collections.Generic;
using Godot;

namespace Betauer.Core.Math.Collision;

public static class PrimMST {

    /// <summary>
    /// Connects all vectors in the grid using a Minimum Spanning Tree (MST) approach.
    /// </summary>
    public static List<(Vector2I, Vector2I)> GetConnections(IEnumerable<Vector2I> points) {
        List<(Vector2I, Vector2I)> connections = new List<(Vector2I, Vector2I)>();
        HashSet<Vector2I> connectedShapes = new HashSet<Vector2I>();
        List<Vector2I> unconnectedShapes = new List<Vector2I>(points);

        // Start with the first shape
        connectedShapes.Add(unconnectedShapes[0]);
        unconnectedShapes.RemoveAt(0);

        // Continue until all shapes are connected using a Minimum Spanning Tree approach
        while (unconnectedShapes.Count > 0) {
            Vector2I? shapeA = null;
            Vector2I? shapeB = null;
            var shortestDistance = float.MaxValue;

            // Find the closest pair of shapes (one connected and one unconnected)
            foreach (Vector2I connected in connectedShapes) {
                foreach (Vector2I unconnected in unconnectedShapes) {
                    var distance = System.Math.Abs(connected.X - unconnected.X) + System.Math.Abs(connected.Y - unconnected.Y);

                    if (distance < shortestDistance) {
                        shortestDistance = distance;
                        shapeA = connected;
                        shapeB = unconnected;
                    }
                }
            }

            // Add the connection to the list
            if (shapeA != null && shapeB != null) {
                connections.Add((shapeA.Value, shapeB.Value));
                connectedShapes.Add(shapeB.Value);
                unconnectedShapes.Remove(shapeB.Value);
            }
        }
        return connections;
    }
}