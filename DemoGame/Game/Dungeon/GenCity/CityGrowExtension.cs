using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public static class CityGrowExtension {
    public static CityGenerator CreateGenerator(this City city, CityGenerationOptions? options = null) {
        return new CityGenerator(city);
    }
}

public class CityGenerator(City city) {
    public City City { get; } = city;
    const float HeadingBorderPathLimit = 3;
    const float HeadingBorderPathProbability = 0.3f;


    private CityGenerationOptions _options = new();
    public int Seed => _options.Seed;

    public void Configure(CityGenerationOptions? options = null) {
        _options = options ?? _options;
    }

    public void Start() {
        City.Reset();
        var startPosition = _options.StartPosition ?? new Vector2I(City.Width / 2, City.Height / 2);
        var intersection = City.GetOrCreateIntersectionAt(startPosition);
        foreach (var direction in _options.StartDirections) {
            intersection.CreatePathTo(direction);
            City.OnUpdate?.Invoke(intersection.Position);
        }
    }

    public void Grow() {
        var isCompleted = false;
        var random = new Random(_options.Seed);
        while (!isCompleted) {
            var paths = City.GetAllPaths().ToArray();
            foreach (var path in paths) {
                if (!path.IsCompleted()) {
                    // A path could be completed by other path when joining
                    GrowPath(path);
                }
            }
            isCompleted = City.GetAllPaths().All(path => path.IsCompleted());
        }
        return;

        void GrowPath(Path path) {
            var newPos = path.GetCursor() + path.Direction;
            if (!City.Data.IsInBounds(newPos)) {
                if (path.GetLength() == 0) {
                    City.RemovePath(path);
                } else {
                    ClosePath(path);
                }
                return;
            }

            var tile = City.Data[newPos];
            if (tile != null) {
                if (tile is Intersection intersection) {
                    // The path collides with an intersection, join to it
                    path.SetEnd(intersection);
                } else if (tile is Path crossPath) {
                    // The path collides with another path
                    City.SplitPath(crossPath, newPos);
                }
                return;
            }

            path.SetCursor(newPos);

            if (random.NextBool(_options.ProbabilityStreetEnd) || StopPathHeadingToBorder(path)) {
                // Console.WriteLine("Closing path randomly");
                ClosePath(path);
                return;
            }

            City.Data[newPos] = path;
            City.OnUpdate?.Invoke(newPos);

            var streetLength = _options.StreetMinLength;
            if (path.GetLength() > streetLength &&
                GetNextTile(path.GetCursor(), path.Direction, streetLength) == null &&
                !random.NextBool(_options.ProbabilityExtend)) {
                random.Pick(new (Action<Path>, float)[] {
                    (CreateCrossPath, _options.ProbabilityCross),
                    (CreateForkPath, _options.ProbabilityFork),
                    (CreateTurnPath, _options.ProbabilityTurn)
                })(path);
            }
        }

        Intersection ClosePath(Path path) {
            var cursor = path.GetCursor();
            Intersection intersection = City.GetOrCreateIntersectionAt(cursor);
            path.SetEnd(intersection);
            City.OnUpdate?.Invoke(intersection.Position);
            return intersection;
        }

        void CreateCrossPath(Path path) {
            var directions = ForkDirection(path.Direction).ToList();
            var intersection = ClosePath(path);
            foreach (var direction in directions) {
                intersection.CreatePathTo(direction);
                City.OnUpdate?.Invoke(intersection.Position);
            }
        }

        void CreateForkPath(Path path) {
            List<Vector2I> directions = ForkDirection(path.Direction)
                .OrderBy(x => random.Next())
                .ToList();
            var intersection = ClosePath(path);
            foreach (var direction in directions.Take(directions.Count - 1)) {
                intersection.CreatePathTo(direction);
                City.OnUpdate?.Invoke(intersection.Position);
            }
        }

        void CreateTurnPath(Path path) {
            var direction = random.Next(TurnDirection(path.Direction));
            var intersection = ClosePath(path);
            intersection.CreatePathTo(direction);
            City.OnUpdate?.Invoke(intersection.Position);
        }

        bool StopPathHeadingToBorder(Path path) {
            var projectedX = path.GetCursor().X + path.Direction.X * HeadingBorderPathLimit;
            var projectedY = path.GetCursor().Y + path.Direction.Y * HeadingBorderPathLimit;
            var headingToBorder = projectedX >= City.Width || projectedX < 0 || projectedY >= City.Height || projectedY < 0;
            return headingToBorder && random.NextBool(HeadingBorderPathProbability);
        }

        Vector2I? GetNextTile(Vector2I start, Vector2I direction, int length) {
            for (var i = 1; i <= length; i++) {
                var position = start + direction * i;
                if (City.Data.IsInBounds(position) && City.Data[position] != null) {
                    return position;
                }
            }
            return null;
        }
    }

    public void GenerateBuildings() {
        var random = new Random(_options.Seed);
        foreach (var path in City.GetAllPaths()) {
            var nextPos = path.Start.Position + path.Direction;

            foreach (var direction in TurnDirection(path.Direction)) {
                var stepOffset = _options.BuildingOffset;

                while (path.GetLength() > stepOffset) {
                    var stepShift = path.Direction * stepOffset;
                    var shiftFromPath = direction * (_options.BuildingOffset + 1);
                    var startPosition = nextPos + stepShift + shiftFromPath;

                    int[] size = [
                        random.Next(_options.BuildingMinSize, _options.BuildingMaxSize + 1),
                        random.Next(_options.BuildingMinSize, _options.BuildingMaxSize + 1)
                    ];

                    if (stepOffset + size[0] + _options.BuildingOffset > path.GetLength()) {
                        break;
                    }
                    ProcessingBuilding(path, startPosition, size, [path.Direction, direction]);

                    var spaceBetweenBuildings = random.Next(_options.BuildingMinSpace, _options.BuildingMaxSpace + 1);
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

                    if (City.Data.IsInBounds(tilePosition) && City.Data[tilePosition] == null) {
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
            City.Buildings.Add(building);

            foreach (Vector2I tilePosition in tiles) {
                City.Data[tilePosition] = building;
                City.OnUpdate?.Invoke(tilePosition);
            }
        }
    }


    public static IList<Vector2I> TurnDirection(Vector2I direction) {
        return [direction.Rotate90Left(), direction.Rotate90Right()];
    }

    public static IList<Vector2I> ForkDirection(Vector2I direction) {
        return [direction, direction.Rotate90Left(), direction.Rotate90Right()];
    }
}