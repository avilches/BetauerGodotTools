using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public interface ICityTile;

public class City {
    public int Width { get; }
    public int Height { get; }
    public int Seed => _params.Seed;

    private Random _random;

    public readonly Array2D<ICityTile?> Data;
    public readonly List<Intersection> Intersections = [];

    private CityGenerationParameters _params = new CityGenerationParameters();

    public City(int width, int height) {
        Width = width;
        Height = height;
        Data = new Array2D<ICityTile?>(Width, Height);
    }

    public IEnumerable<Building> GetAllBuildings() {
        return GetAllPaths().SelectMany(path => path.Buildings);
    }

    public List<Intersection> GetAllIntersections() {
        return Intersections;
    }

    public IEnumerable<Path> GetAllPaths() {
        // Only get output paths or input path, never both or you will get duplicates
        return Intersections.SelectMany(intersection => intersection.GetOutputPaths());
    }

    public void Generate(CityGenerationParameters customParams = null, Action action = null) {
        Reset();

        _params = customParams ?? new CityGenerationParameters();
        _random = new Random(_params.Seed);

        if (_params.StartPosition == Vector2I.Zero) {
            _params.StartPosition = new Vector2I(Width / 2, Height / 2);
        }

        if (_params.StartDirections.Count == 0) {
            _params.StartDirections = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];
        }

        var intersection = AddIntersection(_params.StartPosition);

        foreach (var direction in _params.StartDirections) {
            intersection.AddOutputPath(direction);
        }

        GeneratePaths(action);
        GenerateBuildings();
    }

    private void Reset() {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Data[y, x] = null;
            }
        }
        Intersections.Clear();
    }

    private void GeneratePaths(Action? action = null) {
        var isCompleted = false;
        while (!isCompleted) {
            var paths = GetAllPaths();
            foreach (var p in paths.Where(path => !path.IsCompleted()).ToArray()) {
                ProcessingPath(p);
                // action?.Invoke();
            }
            isCompleted = GetAllPaths().All(path => path.IsCompleted());
        }
    }

    private void ProcessingPath(Path path) {
        var newPos = path.GetNextCursor();

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
                // Console.WriteLine("Path closed by start intersection");
                path.SetEnd(intersection);
            } else if (tile is Path crossPath) {
                SplitPath(crossPath, newPos);
            }
            return;
        }

        path.SetCursor(newPos);

        if (VariabilityChance(_params.ProbabilityStreetEnd)) {
            // Console.WriteLine("Closing path randomly");
            ClosePath(path);
            return;
        }

        // Continuation path
        Data[newPos] = path;

        var streetLength = _params.StreetMinLength;
        if (path.GetLength() > streetLength && GetNextTile(path.GetCursor(), path.Direction, streetLength) == null) {
            if (VariabilityChance(_params.ProbabilityIntersection)) {
                ForkPath(path);
            } else if (VariabilityChance(_params.ProbabilityTurn)) {
                TurnPath(path);
            }
        }
    }

    private void GenerateBuildings() {
        foreach (var path in GetAllPaths()) {
            var nextPos = path.Start.Position + path.Direction;

            foreach (var direction in Utils.TurnDirection(path.Direction)) {
                var stepOffset = _params.BuildingOffset;

                while (path.GetLength() > stepOffset) {
                    var stepShift = path.Direction * stepOffset;
                    var shiftFromPath = direction * (_params.BuildingOffset + 1);
                    var startPosition = nextPos + stepShift + shiftFromPath;

                    int[] size = [
                        _random.Next(_params.BuildingMinSize, _params.BuildingMaxSize + 1),
                        _random.Next(_params.BuildingMinSize, _params.BuildingMaxSize + 1)
                    ];

                    if (stepOffset + size[0] + _params.BuildingOffset > path.GetLength()) {
                        break;
                    }
                    ProcessingBuilding(path, startPosition, size, [path.Direction, direction]);

                    var spaceBetweenBuildings = _random.Next(_params.BuildingMinSpace, _params.BuildingMaxSpace + 1);
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

        var building = path.AddBuilding(vertices);

        foreach (Vector2I tilePosition in tiles) {
            Data[tilePosition] = building;
        }
    }

    private ICityTile? GetAt(Vector2I position) {
        return Data.GetValueSafe(position);
    }

    private bool IsEmptyAt(Vector2I position) {
        return Data.IsInBounds(position) && Data[position] == null;
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
        Vector2I cursor = path.GetCursor();
        Intersection intersection = AddIntersection(cursor);

        path.SetEnd(intersection);

        return intersection;
    }

    private void ForkPath(Path path) {
        List<Vector2I> directions = Utils.ForkDirection(path.Direction)
            .OrderBy(x => _random.Next())
            .ToList();

        var intersection = ClosePath(path);

        for (var i = 0; i < directions.Count; i++) {
            if (i < 2 || VariabilityChance(0.5f)) {
                intersection.AddOutputPath(directions[i]);
            }
        }

        Data[intersection.Position] = intersection;
    }

    private void TurnPath(Path path) {
        var intersection = ClosePath(path);
        var direction = _random.Next(Utils.TurnDirection(path.Direction));
        intersection.AddOutputPath(direction);
        Data[intersection.Position] = intersection;
    }

    private void SplitPath(Path path, Vector2I position) {
        var nodeBegIndex = Intersections.FindIndex(intersection => intersection == path.Start);
        var newIntersection = AddIntersection(position, nodeBegIndex + 1);
        var continuePath = newIntersection.AddOutputPath(path.Direction);

        if (path.End != null) {
            path.End.RemoveInputPath(path);
            continuePath.SetEnd(path.End);
        } else {
            continuePath.SetCursor(path.GetCursor());
        }

        // Refill matrix with new path
        foreach (var tilePosition in continuePath.Each()) {
            if (Data[tilePosition] is Path) {
                Data[tilePosition] = continuePath;
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
}