using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public interface ICityTile;

public class City(int width, int height) {
    public int Width { get; } = width;
    public int Height { get; } = height;
    public Array2D<ICityTile?> Data { get; } = new(width, height);
    public List<Intersection> Intersections { get; } = [];
    public List<Building> Buildings { get; } = [];

    public Action<Vector2I>? OnUpdate;

    public IEnumerable<Path> GetAllPaths() {
        // Only get output paths or input path, never both or you will get duplicates
        return Intersections.SelectMany(intersection => intersection.GetOutputPaths());
    }

    public void Reset() {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[y, x] = null;
            }
        }
        Intersections.Clear();
        Buildings.Clear();
    }

    /// <summary>
    /// Creates a path between two arbitrary points in the city.
    /// </summary>
    /// <param name="startPoint">The starting point of the path</param>
    /// <param name="endPoint">The ending point of the path</param>
    /// <returns>A list of created paths that form the route</returns>
    public List<Path> CreatePath(Vector2I startPoint, Vector2I endPoint) {
        if (!Data.IsInBounds(startPoint) || !Data.IsInBounds(endPoint)) {
            throw new ArgumentException("Start or end point is out of bounds");
        }
        if (startPoint == endPoint) return [];
        if (!startPoint.SameDirection(endPoint)) {
            throw new ArgumentException("End intersection is not in the same line as the path direction");
        }
        var startIntersection = GetOrCreateIntersectionAt(startPoint);
        var endIntersection = GetOrCreateIntersectionAt(endPoint);
        var direction = startIntersection.Position.DirectionTo(endIntersection.Position);

        List<Path> createdPaths = [];

        // Create initial path
        var path = startIntersection.CreatePathTo(direction);
        createdPaths.Add(path);

        // Current position as we trace the path
        var currentPos = startIntersection.Position + direction;

        // Path tracer
        while (currentPos != endIntersection.Position) {
            var tile = Data[currentPos];
            if (tile is Intersection intersection) {
                // Found an intersection on the way - end current path here
                path.SetEnd(intersection);
                // Start a new path from this intersection
                path = intersection.CreatePathTo(direction);
                createdPaths.Add(path);
            } else if (tile is Path crossPath) {
                // Found a crossing path - split it
                var newIntersection = SplitPath(crossPath, currentPos);
                path.SetEnd(newIntersection);
                // Start a new path from this intersection
                path = newIntersection.CreatePathTo(direction);
                createdPaths.Add(path);
            } else {
                // Mark this position as part of the path
                Data[currentPos] = path;
            }
            OnUpdate?.Invoke(currentPos);
            currentPos += direction;
        }
        path.SetEnd(endIntersection);
        OnUpdate?.Invoke(endIntersection.Position);
        return createdPaths;
    }

    /// <summary>
    /// Gets an existing intersection at the specified point or creates a new one.
    /// </summary>
    public Intersection GetOrCreateIntersectionAt(Vector2I position) {
        return Data[position] switch {
            Intersection intersection => intersection,
            Path existingPath => SplitPath(existingPath, position),
            _ => AddIntersection(position)
        };
    }

    private Intersection AddIntersection(Vector2I position, int? index = null) {
        var intersection = new Intersection(Intersections.Count, position);

        if (!index.HasValue) {
            Intersections.Add(intersection);
        } else {
            Intersections.Insert(index.Value, intersection);
        }
        Data[position] = intersection;
        return intersection;
    }

    /// <summary>
    /// Removes a path. After removing the path: if the beginning or the start intersections of the path are empty,
    /// they are removed as well.
    /// </summary>
    /// <param name="path"></param>
    public void RemovePath(Path path) {
        // Store references to the intersections before removing them
        var startIntersection = path.Start;
        var endIntersection = path.End;

        // Remove the path from its start and end intersections correctly
        startIntersection.RemoveOutputPath(path);
        endIntersection?.RemoveInputPath(path);

        // Clear the path positions in the data matrix
        foreach (var tilePosition in path.GetPositions()) {
            if (Data[tilePosition] is Path p && p == path) {
                Data[tilePosition] = null;
                OnUpdate?.Invoke(tilePosition);
            }
        }

        // Check if the start intersection should be removed
        if (startIntersection.GetAllPaths().Count == 0) {
            Intersections.Remove(startIntersection);
            Data[startIntersection.Position] = null;
            OnUpdate?.Invoke(startIntersection.Position);
        } else {
            // Try to flatten if there are two paths in opposite directions
            FlatIntersection(startIntersection);
        }

        if (endIntersection != null) {
            // Check if the end intersection should be removed
            if (endIntersection.GetAllPaths().Count == 0) {
                Intersections.Remove(endIntersection);
                Data[endIntersection.Position] = null;
                OnUpdate?.Invoke(endIntersection.Position);
            } else {
                // Try to flatten if there are two paths in opposite directions
                FlatIntersection(endIntersection);
            }
        }
    }

    /// <summary>
    /// Adds a new intersection in the middle of a path, creating two paths.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Intersection SplitPath(Path path, Vector2I position) {
        if (Data[position] is not Path p || p != path) {
            throw new Exception("Can't split path at position " + position);
        }
        var nodeBegIndex = Intersections.FindIndex(intersection => intersection == path.Start);
        var newIntersection = AddIntersection(position, nodeBegIndex + 1);
        var continuePath = newIntersection.CreatePathTo(path.Direction);
        OnUpdate?.Invoke(newIntersection.Position);

        if (path.End != null) {
            path.End.RemoveInputPath(path);
            continuePath.SetEnd(path.End);
            OnUpdate?.Invoke(newIntersection.Position);
        } else {
            continuePath.SetCursor(path.GetCursor());
        }

        // Refill matrix with new path
        foreach (var tilePosition in continuePath.GetPositions()) {
            if (Data[tilePosition] is Path) {
                Data[tilePosition] = continuePath;
                OnUpdate?.Invoke(tilePosition);
            }
        }
        path.SetEnd(newIntersection);
        return newIntersection;
    }

    /// <summary>
    /// If the intersection has two paths in opposite directions, removes the intersection and creates a single path
    /// This "X----X----X" becomes "X---------X"
    /// </summary>
    /// <param name="intersection"></param>
    /// <returns></returns>
    public bool FlatIntersection(Intersection intersection) {
        var (path1, path2) = intersection.GetOppositeDirectionPaths();

        if (path1 == null || path2 == null) {
            return false;
        }

        // Determine which paths are incoming and outgoing
        var incomingPath = path1;
        var outgoingPath = path2;

        // If both are the same type, we need to determine which is the "first"
        if (intersection.GetInputPaths().Contains(path1) && intersection.GetInputPaths().Contains(path2)) {
            // Both are input paths, choose one as "incoming" for our logic
            incomingPath = path1;
            outgoingPath = path2;
        } else if (intersection.GetOutputPaths().Contains(path1) && intersection.GetOutputPaths().Contains(path2)) {
            // Both are output paths, choose one as "incoming" for our logic
            incomingPath = path1;
            outgoingPath = path2;
        }

        // Determine the extreme intersections (not the one we're flattening)
        var startIntersection = incomingPath.Start == intersection ? incomingPath.End : incomingPath.Start;
        var endIntersection = outgoingPath.End == intersection ? outgoingPath.Start : outgoingPath.End;

        // If either extreme is not a valid intersection, we can't flatten
        if (startIntersection == null || endIntersection == null) {
            return false;
        }

        // Remove the paths correctly from their respective intersections
        if (path1.Start == startIntersection) {
            startIntersection.RemoveOutputPath(path1);
        } else if (path1.End == startIntersection) {
            startIntersection.RemoveInputPath(path1);
        }

        if (path2.Start == endIntersection) {
            endIntersection.RemoveOutputPath(path2);
        } else if (path2.End == endIntersection) {
            endIntersection.RemoveInputPath(path2);
        }

        // Remove the intersection
        Intersections.Remove(intersection);
        Data[intersection.Position] = null;
        OnUpdate?.Invoke(intersection.Position);

        // Create a new path between the extreme intersections
        var direction = startIntersection.Position.DirectionTo(endIntersection.Position);
        var newPath = startIntersection.CreatePathTo(direction);
        newPath.SetEnd(endIntersection);

        // Update the data matrix for the new path
        foreach (var position in newPath.GetPositions()) {
            if (position != startIntersection.Position && position != endIntersection.Position) {
                Data[position] = newPath;
                OnUpdate?.Invoke(position);
            }
        }
        return true;
    }
}