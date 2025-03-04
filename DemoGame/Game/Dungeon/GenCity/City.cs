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


    private int _intersectionId = 0;

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
        var direction = startPoint.GetOrthogonalDirectionTo(endPoint); // This will fail if not orthogonal
        var startIntersection = GetOrCreateIntersectionAt(startPoint);
        var endIntersection = GetOrCreateIntersectionAt(endPoint);

        List<Intersection> intersections = [startIntersection, endIntersection];

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
                intersections.Add(intersection);
                path?.SetEnd(intersection);
                // Found an intersection on the way
                if (intersection.FindPathTo(direction) != null) {
                    // The intersection has a path in the same direction - we don't create a new path, but continue
                    path = null;
                } else {
                    // The intersection doesn't have a path in the same direction (its the start of the end of a perpendicular path)
                    // Start a new path from this intersection
                    path = intersection.CreatePathTo(direction);
                    createdPaths.Add(path);
                }
            } else if (tile is Path crossPath) {
                if (path == null) {
                    // When path is null, it means we are overlapping with another path, so we ignore it
                } else {
                    // Found a crossing path - split it
                    var newIntersection = SplitPath(crossPath, currentPos);
                    path.SetEnd(newIntersection);
                    // Start a new path from this intersection
                    path = newIntersection.CreatePathTo(direction);
                    createdPaths.Add(path);
                }
            } else {
                // Mark this position as part of the path
                Data[currentPos] = path;
            }
            OnUpdate?.Invoke(currentPos);
            currentPos += direction;
        }
        path?.SetEnd(endIntersection);
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
                OnUpdate?.Invoke(tilePosition);
            }
        }

        // Check if the start intersection should be removed
        if (startIntersection.GetAllPaths().Count == 0) {
            Intersections.Remove(startIntersection);
            Data[startIntersection.Position] = null;
            OnUpdate?.Invoke(startIntersection.Position);
        } else {
            FlatIntersection(startIntersection);
        }

        if (endIntersection != null) {
            // Check if the end intersection should be removed
            if (endIntersection.GetAllPaths().Count == 0) {
                Intersections.Remove(endIntersection);
                Data[endIntersection.Position] = null;
                OnUpdate?.Invoke(endIntersection.Position);
            } else {
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
        FlatIntersection(path.Start);
        return newIntersection;
    }

    /*
    public int FlatAllIntersections() {
        var count = 0;
        while (true) {
            var i = Intersections.FirstOrDefault(i => i.CanBeFlatten(out var one, out var other) && one.IsCompleted() && other.IsCompleted());
            if (i == null) break;
            if (!FlatIntersection(i)) throw new Exception("Can't flatten intersection. Check if the condition to filter intersections is correct");
            count++;
        }
        return count;
    }
    */


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

        // Both paths must be completed (have start and end)
        // This is only possible for output paths, as input paths are always completed by definition
        if (!path1.IsCompleted() || !path2.IsCompleted()) {
            // Console.WriteLine("Can't flatten intersection - paths are not completed: " + intersection);
            return false;
        }

        // Get the other two intersections at the ends of these paths
        (Intersection start, Intersection end) = (path1, path2) switch {
            var (p1, p2) when p1.Start == intersection &&
                              p2.Start == intersection => (p1.End!, p2.End!), // Both are output paths

            var (p1, p2) when p1.End == intersection &&
                              p2.End == intersection => (p1.Start, p2.Start),  // Both are input paths

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

    public void ValidateIntersections(bool checkFlattenIntersections = false) {
        var errors = new List<string>();
        foreach (var intersection in Intersections) {
            if (checkFlattenIntersections && intersection.CanBeFlatten(out var path1, out var path2)) {
                errors.Add($"Intersection {intersection.Id} at {intersection.Position} can be flattened: {path1.Id} and {path2.Id}");
            }
            if (Data[intersection.Position] != intersection) {
                errors.Add($"Wrong intersection position: {intersection.Position}");
            }
        }
        foreach (var (pos, intersection) in Data.GetIndexedValues<Intersection>()) {
            if (!Intersections.Contains(intersection)) {
                errors.Add($"Intersection in {pos} doesn't belong to the city");
            }
            if (intersection.Position != pos) {
                errors.Add($"Wrong intersection position in {pos}");
            }
        }
        if (errors.Count > 0) {
            throw new Exception("City intersections are invalid:\n" + string.Join("\n", errors));
        }
    }

    public void ValidateIntersectionPaths() {
        var output = Intersections.SelectMany(intersection => intersection.GetOutputPaths()).ToHashSet();
        var input = Intersections.SelectMany(intersection => intersection.GetInputPaths()).ToHashSet();
        if (!output.SetEquals(input)) {
            var outputOnly = output.Except(input).ToList();
            var inputOnly = input.Except(output).ToList();
            throw new Exception($"Output paths and input paths do not contain the same elements. " +
                       $"Only in output: {outputOnly.Count}, Only in input: {inputOnly.Count}");
        }
    }

    public void ValidateRoads() {
        var errors = new List<string>();
        var output = Intersections.SelectMany(intersection => intersection.GetOutputPaths()).ToHashSet();
        foreach (var path in output) {
            var start = path.Start.Position;
            var end = path.End?.Position ?? path.GetCursor();

            foreach (var pos in path.GetPositions()) {
                var tile = Data[pos];
                if (pos == start) {
                    if (tile != path.Start) {
                        errors.Add($"Wrong start position {start} for path {path}. Expected {path.Start.Id} but got {tile}");
                    }
                } else if (pos == end) {
                    if (path.End == null) {
                        if (tile != path) {
                            errors.Add($"Wrong end position (cursor) for path {path} at {pos}. Expected {path} but got {tile}");
                        }
                    } else {
                        if (tile != path.End) {
                            errors.Add($"Wrong end position for path {path} at {pos}. Expected {path.End} but got {tile}");
                        }
                    }
                } else {
                    if (tile != path) {
                        errors.Add($"Expected at {pos}. Expected {path} but got {tile}");
                    }
                }
            }
        }
        foreach (var (pos, tile) in Data.GetIndexedValues()) {
            if (tile is not Path path) continue;
            if (!output.Contains(path)) {
                errors.Add($"Path in {pos} doesn't belong to the city");
            }
        }
        if (errors.Count > 0) {
            throw new Exception("City roads are invalid:\n" + string.Join("\n", errors));
        }
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