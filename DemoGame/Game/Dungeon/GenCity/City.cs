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
    private CityGenerationOptions _options = new CityGenerationOptions();

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

    public void Generate(CityGenerationOptions? options = null) {
        Reset();
        _options = options ?? new CityGenerationOptions();
        _random = new Random(_options.Seed);
        GenerateStartIntersection();
        GeneratePaths();
        GenerateBuildings();
    }

    private void GenerateStartIntersection() {
        var startPosition = _options.StartPosition ?? new Vector2I(Width / 2, Height / 2);
        var intersection = AddIntersection(startPosition);
        foreach (var direction in _options.StartDirections) {
            intersection.AddOutputPath(direction);
            _options.OnUpdate?.Invoke(intersection.Position);
        }
    }

    private void GeneratePaths() {
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

    }

    private void GrowPath(Path path) {
        var newPos = path.GetCursor() + path.Direction;
        if (!Data.IsInBounds(newPos)) {
            if (path.GetLength() == 0) {
                path.Remove();
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

        if (VariabilityChance(_options.ProbabilityStreetEnd)) {
            // Console.WriteLine("Closing path randomly");
            ClosePath(path);
            return;
        }

        // Continuation path
        Data[newPos] = path;
        _options.OnUpdate?.Invoke(newPos);

        var streetLength = _options.StreetMinLength;
        if (path.GetLength() > streetLength &&
            GetNextTile(path.GetCursor(), path.Direction, streetLength) == null) {
            if (!VariabilityChance(_options.ProbabilityExtend)) {
                _random.Pick(new (Action<Path>, float)[] {
                    (CreateCrossPath, _options.ProbabilityCross),
                    (CreateForkPath, _options.ProbabilityFork),
                    (CreateTurnPath, _options.ProbabilityTurn)
                })(path);
            }
        }
    }

    private void GenerateBuildings() {
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
    }

    private void ProcessingBuilding(Path path, Vector2I position, int[] size, List<Vector2I> directions) {
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

    private Intersection ClosePath(Path path) {
        var cursor = path.GetCursor();
        Intersection intersection = AddIntersection(cursor);
        path.SetEnd(intersection);
        _options.OnUpdate?.Invoke(intersection.Position);
        return intersection;
    }

    private void CreateCrossPath(Path path) {
        var directions = ForkDirection(path.Direction).ToList();
        var intersection = ClosePath(path);
        foreach (var direction in directions) {
            intersection.AddOutputPath(direction);
            _options.OnUpdate?.Invoke(intersection.Position);
        }
    }

    private void CreateForkPath(Path path) {
        List<Vector2I> directions = ForkDirection(path.Direction)
            .OrderBy(x => _random.Next())
            .ToList();
        var intersection = ClosePath(path);
        foreach (var direction in directions.Take(directions.Count - 1)) {
            intersection.AddOutputPath(direction);
            _options.OnUpdate?.Invoke(intersection.Position);
        }
    }

    private void CreateTurnPath(Path path) {
        var direction = _random.Next(TurnDirection(path.Direction));
        var intersection = ClosePath(path);
        intersection.AddOutputPath(direction);
        _options.OnUpdate?.Invoke(intersection.Position);
    }

    private void SplitPath(Path path, Vector2I position) {
        var nodeBegIndex = Intersections.FindIndex(intersection => intersection == path.Start);
        var newIntersection = AddIntersection(position, nodeBegIndex + 1);
        var continuePath = newIntersection.AddOutputPath(path.Direction);
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
    }

    private Vector2I? GetNextTile(Vector2I start, Vector2I direction, int length) {
        for (var i = 1; i <= length; i++) {
            var position = start + direction * i;
            if (Data.IsInBounds(position) && Data[position] != null) {
                return position;
            }
        }
        return null;
    }

    public bool VariabilityChance(float value) {
        return _random.NextDouble() > 1.0 - value;
    }

    public static IList<Vector2I> TurnDirection(Vector2I direction) {
        return [direction.Rotate90Left(), direction.Rotate90Right()];
    }

    public static IList<Vector2I> ForkDirection(Vector2I direction) {
        return [direction, direction.Rotate90Left(), direction.Rotate90Right()];
    }
}