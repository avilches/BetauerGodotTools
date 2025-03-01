using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public interface ICityTile;

public class City(int width, int height) {
    private Random _random;
    private CityGenerationOptions _options = new();

    public int Width { get; } = width;
    public int Height { get; } = height;
    public Array2D<ICityTile?> Data { get; } = new(width, height);
    public List<Intersection> Intersections { get; } = [];
    public List<Building> Buildings { get; } = [];
    public int Seed => _options.Seed;

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

    public void Configure(CityGenerationOptions? options = null) {
        Reset();
        _options = options ?? _options;
        _random = new Random(_options.Seed);
    }

    public void Start() {
        // Generate start position
        var startPosition = _options.StartPosition ?? new Vector2I(Width / 2, Height / 2);
        var intersection = AddIntersection(startPosition);
        foreach (var direction in _options.StartDirections) {
            intersection.CreatePathTo(direction);
            _options.OnUpdate?.Invoke(intersection.Position);
        }
    }

    public void Grow() {
        var isCompleted = false;
        while (!isCompleted) {
            var paths = GetAllPaths().ToArray();
            foreach (var path in paths) {
                if (!path.IsCompleted()) {
                    // A path could be completed by other path when joining
                    GrowPath(path);
                }
            }
            isCompleted = GetAllPaths().All(path => path.IsCompleted());
        }

        const float HeadingBorderPathLimit = 3;
        const float HeadingBorderPathProbability = 0.3f;

        return;

        void GrowPath(Path path) {
            var newPos = path.GetCursor() + path.Direction;
            if (!Data.IsInBounds(newPos)) {
                if (path.GetLength() == 0) {
                    RemovePath(path);
                } else {
                    ClosePath(path);
                }
                return;
            }

            var tile = Data[newPos];
            if (tile != null) {
                if (tile is Intersection intersection) {
                    // The path collides with an intersection, join to it
                    path.SetEnd(intersection);
                } else if (tile is Path crossPath) {
                    // The path collides with another path
                    SplitPath(crossPath, newPos);
                }
                return;
            }

            path.SetCursor(newPos);

            if (_random.NextBool(_options.ProbabilityStreetEnd) || StopPathHeadingToBorder(path)) {
                // Console.WriteLine("Closing path randomly");
                ClosePath(path);
                return;
            }

            Data[newPos] = path;
            _options.OnUpdate?.Invoke(newPos);

            var streetLength = _options.StreetMinLength;
            if (path.GetLength() > streetLength &&
                GetNextTile(path.GetCursor(), path.Direction, streetLength) == null &&
                !_random.NextBool(_options.ProbabilityExtend)) {
                _random.Pick(new (Action<Path>, float)[] {
                    (CreateCrossPath, _options.ProbabilityCross),
                    (CreateForkPath, _options.ProbabilityFork),
                    (CreateTurnPath, _options.ProbabilityTurn)
                })(path);
            }
        }

        Intersection ClosePath(Path path) {
            var cursor = path.GetCursor();
            Intersection intersection = AddIntersection(cursor);
            path.SetEnd(intersection);
            _options.OnUpdate?.Invoke(intersection.Position);
            return intersection;
        }

        void CreateCrossPath(Path path) {
            var directions = ForkDirection(path.Direction).ToList();
            var intersection = ClosePath(path);
            foreach (var direction in directions) {
                intersection.CreatePathTo(direction);
                _options.OnUpdate?.Invoke(intersection.Position);
            }
        }

        void CreateForkPath(Path path) {
            List<Vector2I> directions = ForkDirection(path.Direction)
                .OrderBy(x => _random.Next())
                .ToList();
            var intersection = ClosePath(path);
            foreach (var direction in directions.Take(directions.Count - 1)) {
                intersection.CreatePathTo(direction);
                _options.OnUpdate?.Invoke(intersection.Position);
            }
        }

        void CreateTurnPath(Path path) {
            var direction = _random.Next(TurnDirection(path.Direction));
            var intersection = ClosePath(path);
            intersection.CreatePathTo(direction);
            _options.OnUpdate?.Invoke(intersection.Position);
        }

        bool StopPathHeadingToBorder(Path path) {
            var projectedX = path.GetCursor().X + path.Direction.X * HeadingBorderPathLimit;
            var projectedY = path.GetCursor().Y + path.Direction.Y * HeadingBorderPathLimit;
            var headingToBorder = projectedX >= Width || projectedX < 0 || projectedY >= Height || projectedY < 0;
            return headingToBorder && _random.NextBool(HeadingBorderPathProbability);
        }

        Vector2I? GetNextTile(Vector2I start, Vector2I direction, int length) {
            for (var i = 1; i <= length; i++) {
                var position = start + direction * i;
                if (Data.IsInBounds(position) && Data[position] != null) {
                    return position;
                }
            }
            return null;
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

    private Intersection AddIntersection(Vector2I position, int? index = null) {
        if (Data[position] is Intersection) {
            throw new Exception("Intersection already exists at " + position);
        }
        var intersection = new Intersection(Intersections.Count, position);

        if (!index.HasValue) {
            Intersections.Add(intersection);
        } else {
            Intersections.Insert(index.Value, intersection);
        }
        Data[position] = intersection;
        return intersection;
    }

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
                _options.OnUpdate?.Invoke(tilePosition);
            }
        }

        // Check if the start intersection should be removed
        if (startIntersection.GetAllPaths().Count == 0) {
            Intersections.Remove(startIntersection);
            Data[startIntersection.Position] = null;
            _options.OnUpdate?.Invoke(startIntersection.Position);
        } else {
            // Try to flatten if there are two paths in opposite directions
            FlatIntersection(startIntersection);
        }

        if (endIntersection != null) {
            // Check if the end intersection should be removed
            if (endIntersection.GetAllPaths().Count == 0) {
                Intersections.Remove(endIntersection);
                Data[endIntersection.Position] = null;
                _options.OnUpdate?.Invoke(endIntersection.Position);
            } else {
                // Try to flatten if there are two paths in opposite directions
                FlatIntersection(endIntersection);
            }
        }
    }

    public Intersection SplitPath(Path path, Vector2I position) {
        if (Data[position] is not Path p || p != path) {
            throw new Exception("Can't split path at position " + position);
        }
        var nodeBegIndex = Intersections.FindIndex(intersection => intersection == path.Start);
        var newIntersection = AddIntersection(position, nodeBegIndex + 1);
        var continuePath = newIntersection.CreatePathTo(path.Direction);
        _options.OnUpdate?.Invoke(newIntersection.Position);

        if (path.End != null) {
            path.End.RemoveInputPath(path);
            continuePath.SetEnd(path.End);
            _options.OnUpdate?.Invoke(newIntersection.Position);
        } else {
            continuePath.SetCursor(path.GetCursor());
        }

        // Refill matrix with new path
        foreach (var tilePosition in continuePath.GetPositions()) {
            if (Data[tilePosition] is Path) {
                Data[tilePosition] = continuePath;
                _options.OnUpdate?.Invoke(tilePosition);
            }
        }
        path.SetEnd(newIntersection);
        return newIntersection;
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
            _options.OnUpdate?.Invoke(currentPos);
            currentPos += direction;
        }
        path.SetEnd(endIntersection);
        _options.OnUpdate?.Invoke(endIntersection.Position);
        return createdPaths;
    }

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
        _options.OnUpdate?.Invoke(intersection.Position);

        // Create a new path between the extreme intersections
        var direction = startIntersection.Position.DirectionTo(endIntersection.Position);
        var newPath = startIntersection.CreatePathTo(direction);
        newPath.SetEnd(endIntersection);

        // Update the data matrix for the new path
        foreach (var position in newPath.GetPositions()) {
            if (position != startIntersection.Position && position != endIntersection.Position) {
                Data[position] = newPath;
                _options.OnUpdate?.Invoke(position);
            }
        }
        return true;
    }

    public static IList<Vector2I> TurnDirection(Vector2I direction) {
        return [direction.Rotate90Left(), direction.Rotate90Right()];
    }

    public static IList<Vector2I> ForkDirection(Vector2I direction) {
        return [direction, direction.Rotate90Left(), direction.Rotate90Right()];
    }

    public void GenerateBuildings() {
        foreach (var path in GetAllPaths()) {
            var nextPos = path.Start.Position + path.Direction;

            foreach (var direction in TurnDirection(path.Direction)) {
                var stepOffset = _options.BuildingOffset;

                while (path.GetLength() > stepOffset) {
                    var stepShift = path.Direction * stepOffset;
                    var shiftFromPath = direction * (_options.BuildingOffset + 1);
                    var startPosition = nextPos + stepShift + shiftFromPath;

                    int[] size = [
                        _random.Next(_options.BuildingMinSize, _options.BuildingMaxSize + 1),
                        _random.Next(_options.BuildingMinSize, _options.BuildingMaxSize + 1)
                    ];

                    if (stepOffset + size[0] + _options.BuildingOffset > path.GetLength()) {
                        break;
                    }
                    ProcessingBuilding(path, startPosition, size, [path.Direction, direction]);

                    var spaceBetweenBuildings = _random.Next(_options.BuildingMinSpace, _options.BuildingMaxSpace + 1);
                    stepOffset += size[0] + spaceBetweenBuildings;
                }
            }
        }
        return;

        void ProcessingBuilding(Path path, Vector2I position, int[] size, List<Vector2I> directions) {
            List<Vector2I> tiles = [];
            List<Vector2I> vertices = [];

            for (var i = 0; i < size[0]; i++) {
                var shiftParallel = directions[0] * i;
                var startFromPathPosition = position + shiftParallel;

                for (var j = 0; j < size[1]; j++) {
                    var shiftPerpendicular = directions[1] * j;
                    var tilePosition = startFromPathPosition + shiftPerpendicular;

                    if (Data.IsInBounds(tilePosition) && Data[tilePosition] == null) {
                        tiles.Add(tilePosition);
                        if ((i == 0 || i == size[0] - 1) && (j == 0 || j == size[1] - 1)) {
                            vertices.Add(tilePosition);
                        }
                    } else {
                        return;
                    }
                }
            }

            var building = new Building(path, vertices);
            Buildings.Add(building);

            foreach (Vector2I tilePosition in tiles) {
                Data[tilePosition] = building;
                _options.OnUpdate?.Invoke(tilePosition);
            }
        }
    }
}