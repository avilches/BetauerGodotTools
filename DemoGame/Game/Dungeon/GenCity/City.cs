using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public interface ICityTile {
    byte GetDirectionFlags();
}

public class Other(char c) : ICityTile {
    public char C = c;
    public byte GetDirectionFlags() {
        return 0;
    }
}

public class City(int width, int height) {
    public int Width { get; } = width;
    public int Height { get; } = height;
    public Vector2I Size => new(Width, Height);
    public Array2D<ICityTile?> Data { get; } = new(width, height);
    public List<Intersection> Intersections { get; } = [];
    public List<Building> Buildings { get; } = [];
    public Action<Vector2I>? OnUpdate;

    private int _intersectionId = 0;

    public City(Vector2I size) : this(size.X, size.Y) {
    }

    public IEnumerable<Path> GetAllPaths() {
        // A completed path is a path that has a start and an end
        // An incomplete path is a path that has a start but not an end
        return Intersections.SelectMany(intersection => intersection.GetOutputPaths());
    }

    public IEnumerable<Path> GetCompletedPaths() {
        // Return the inputs ensure returns only path completed
        return Intersections.SelectMany(intersection => intersection.GetInputPaths());
    }

    public IEnumerable<Path> GetIncompletePaths() {
        return Intersections.SelectMany(intersection => intersection.GetOutputPaths().Where(p => !p.IsCompleted()));
    }

    public void Clear() {
        Data.Fill(null);
        Intersections.Clear();
        Buildings.Clear();
        _intersectionId = 0;
    }

    public void RemoveAllPaths() {
        foreach (var path in GetAllPaths()) {
            foreach (var position in path.GetPositions()) {
                Data[position] = null;
            }
        }
        Intersections.Clear();
        Buildings.Clear();
        _intersectionId = 0;
    }

    public void RemoveBuildings() {
        Buildings.Clear();
        foreach (var (p, e) in Data.GetIndexedValues()) {
            if (e is Building) {
                Data[p] = null;
            }
        }
    }

    public float GetDensity() {
        var totalCells = Data.Width * Data.Height;
        var occupied = 0;
        foreach (var tile in Data.GetValues()) {
            if (tile is Intersection or Path) {
                occupied++;
            }
        }
        var density = (float)occupied / totalCells;
        return density;
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
        var direction = startPoint.GetOrthogonalDirectionTo(endPoint); // This will fail if not orthogonal

        // First, split all perpendicular paths that are in the way, creating intersections
        var intersections = GetIntersections(startPoint, endPoint, direction);

        List<Path> createdPaths = [];
        for (var n = 1; n < intersections.Count; n++) {
            var start = intersections[n - 1];
            var end = intersections[n];
            if (start.FindPathTo(direction) != null) {
                // There is already a path between these two intersections
                continue;
            }
            CreateSegment(start, end);
        }

        return createdPaths;

        void CreateSegment(Intersection start, Intersection end) {
            var path = start.CreatePathTo(direction);
            path.SetEnd(end);
            var pos = start.Position + direction;
            while (pos != end.Position) {
                Data[pos] = path;
                OnUpdate?.Invoke(pos);
                pos += direction;
            }
            createdPaths.Add(path);
        }

        List<Intersection> GetIntersections(Vector2I start, Vector2I end, Vector2I dir) {
            var pos = start;
            List<Intersection> intersections = [];
            while (true) {
                var tile = Data[pos];
                if (tile == null && (pos == start || pos == end)) {
                    var intersection = AddIntersection(pos);
                    OnUpdate?.Invoke(pos);
                    intersections.Add(intersection);
                } else if (tile is Intersection i) {
                    intersections.Add(i);
                } else if (tile is Path p && p.Direction.IsPerpendicular(dir)) {
                    var intersection = SplitPath(p, pos);
                    intersections.Add(intersection);
                } else {
                    // tile is null but the position is not the start or the end
                    // tile is a path but it's in the same line of the direction
                }
                if (pos == end) break;
                pos += dir;
            }
            return intersections;
        }
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

    public Intersection AddIntersection(Vector2I position, int? index = null) {
        if (Data[position] is Intersection) {
            throw new Exception("Intersection already exists at " + position);
        } else if (Data[position] is Path path) {
            throw new Exception("Can't create intersection at " + position + " There is a path: " + path);
        }
        var intersection = new Intersection(_intersectionId++, position);
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
            }
        }

        // Check if the start intersection should be removed
        if (startIntersection.GetAllPaths().Count == 0) {
            Intersections.Remove(startIntersection);
            Data[startIntersection.Position] = null;
        }

        if (endIntersection != null) {
            // Check if the end intersection should be removed
            if (endIntersection.GetAllPaths().Count == 0) {
                Intersections.Remove(endIntersection);
                Data[endIntersection.Position] = null;
            }
        }
    }

    /// <summary>
    /// Returns all the paths in a section. It can return paths that start in this section but end in another
    /// </summary>
    /// <param name="sectionRect"></param>
    /// <returns></returns>
    public IEnumerable<Path> FindPathsInSection(Rect2I sectionRect) {
        var consumed = new HashSet<Path>();
        for (var y = sectionRect.Position.Y; y < sectionRect.Position.Y + sectionRect.Size.Y; y++) {
            for (var x = sectionRect.Position.X; x < sectionRect.Position.X + sectionRect.Size.X; x++) {
                if (Data[y, x] is not Intersection intersection) continue;
                foreach (var path in intersection.GetAllPaths().Where(path => consumed.Add(path))) {
                    yield return path;
                }
            }
        }
    }

    public IEnumerable<Intersection> FindIntersectionsInSection(Rect2I sectionRect) {
        for (var y = sectionRect.Position.Y; y < sectionRect.Position.Y + sectionRect.Size.Y; y++) {
            for (var x = sectionRect.Position.X; x < sectionRect.Position.X + sectionRect.Size.X; x++) {
                if (Data[y, x] is Intersection intersection) {
                    yield return intersection;
                }
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
            throw new Exception($"Can't split path at position {position} because is not part of the path. Data: {Data[position]}");
        }
        Data[position] = null; // needed to allow creating a new intersection there
        var nodeBegIndex = Intersections.FindIndex(intersection => intersection == path.Start);
        var newIntersection = AddIntersection(position, nodeBegIndex + 1);
        var continuePath = newIntersection.CreatePathTo(path.Direction);

        if (path.End != null) {
            path.End.RemoveInputPath(path);
            continuePath.SetEnd(path.End);
        } else {
            continuePath.SetCursor(path.GetCursor());
        }

        // Refill matrix with new path
        foreach (var tilePosition in continuePath.GetPositions()) {
            if (Data[tilePosition] is Path) {
                Data[tilePosition] = continuePath;
            }
        }
        path.SetEnd(newIntersection);
        return newIntersection;
    }

    public int FlatAllIntersections(Func<Intersection, bool>? predicate = null) {
        var count = 0;
        while (true) {
            var first = Intersections.FirstOrDefault(intersection =>
                intersection.CanBeFlatten(out var one, out var other) &&
                one.IsCompleted() &&
                other.IsCompleted() && (predicate == null || predicate(intersection)));
            if (first == null) {
                break;
            }
            if (!FlatIntersection(first)) {
                throw new Exception($"Bug: can't flatten intersection. Check if {nameof(first.CanBeFlatten)}() and {nameof(FlatIntersection)}");
            }
            count++;
        }
        return count;
    }

    /// <summary>
    /// If the intersection has two paths in opposite directions, removes the intersection and creates a single path
    /// This "X----X----X" becomes "X---------X"
    /// </summary>
    /// <param name="intersection"></param>
    /// <returns></returns>
    public bool FlatIntersection(Intersection intersection) {
        if (!intersection.CanBeFlatten(out var path1, out var path2)) {
            // Console.WriteLine("Can't flatten intersection - doesn't contain straight paths: " + intersection);
            return false;
        }

        // Get the other two intersections at the ends of these paths
        (Intersection start, Intersection end) = (path1, path2) switch {
            var (p1, p2) when p1.Start == intersection &&
                              p2.Start == intersection => (p1.End!, p2.End!), // Both are output paths

            var (p1, p2) when p1.End == intersection &&
                              p2.End == intersection => (p1.Start, p2.Start), // Both are input paths

            var (p1, p2) when p1.Start == intersection &&
                              p2.End == intersection => (p1.End!, p2.Start), // path1 is output, path2 is input

            var (p1, p2) when p1.End == intersection &&
                              p2.Start == intersection => (p1.Start, p2.End!), // path1 is input, path2 is output

            _ => throw new Exception("Invalid paths at intersection: " + intersection + " there is a bug in ContainsStraightPath")
        };

        // Remove the intersection from the city
        Intersections.Remove(intersection);
        Data[intersection.Position] = null;

        path1.Start.RemoveOutputPath(path1);
        path1.End!.RemoveInputPath(path1);
        path2.Start.RemoveOutputPath(path2);
        path2.End!.RemoveInputPath(path2);

        var direction = start.Position.GetOrthogonalDirectionTo(end.Position);
        var newPath = start.CreatePathTo(direction);
        newPath.SetEnd(end);

        // Update the grid with the new path
        foreach (var pos in newPath.GetPositions()) {
            // Skip the start and end positions which should be intersections
            if (pos != start.Position && pos != end.Position) {
                Data[pos] = newPath;
            }
        }
        return true;
    }

    private void DebugPathPositions(Path path) {
        Console.WriteLine($"Debugging path: {path}");
        Console.WriteLine($"Start: {path.Start.Position}, End: {path.End?.Position ?? path.GetCursor()}");
        Console.WriteLine("Path positions and corresponding Data entries:");

        foreach (var pos in path.GetPositions()) {
            var dataEntry = Data[pos];
            Console.WriteLine($"  Position: {pos}, Data entry: {dataEntry}");
        }
    }
}