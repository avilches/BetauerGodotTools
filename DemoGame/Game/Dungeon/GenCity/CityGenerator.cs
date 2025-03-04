using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public static class CityGrowExtension {
    public static CityGenerator CreateGenerator(this City city, CityGenerationOptions? options = null) {
        return new CityGenerator(city) {
            Options = options ?? new CityGenerationOptions()
        };
    }
}

public class CityGenerator(City city) {
    public City City { get; } = city;
    const float HeadingBorderPathLimit = 3;
    const float HeadingBorderPathProbability = 0.3f;


    public CityGenerationOptions Options { get; set; } = new();
    public int Seed => Options.Seed;

    public void Start() {
        City.Reset();
        var startPosition = Options.StartPosition ?? new Vector2I(City.Width / 2, City.Height / 2);
        var intersection = City.GetOrCreateIntersectionAt(startPosition);
        foreach (var direction in Options.StartDirections) {
            intersection.CreatePathTo(direction);
            City.OnUpdate?.Invoke(intersection.Position);
        }
    }

    public void Grow() {
        var isCompleted = false;
        var random = new Random(Options.Seed);
        var deleted = new HashSet<Path>();
        while (!isCompleted) {
            var paths = City.GetAllPaths().ToArray();
            foreach (var path in paths) {
                if (!deleted.Contains(path) && !path.IsCompleted()) {
                    // A path could be completed by other path when joining
                    GrowPath(path);
                }
            }
            isCompleted = City.GetAllPaths().All(path => path.IsCompleted());
        }
        return;

        void GrowPath(Path path) {
            var currentPos = path.GetCursor();
            var nextPos = currentPos + path.Direction;

            if (path.Id == "121-0" || path.Id == "103-1") {
            }
            if (!City.Data.IsInBounds(nextPos)) {
                // The path is heading to the border, close it
                if (path.GetLength() == 0) {
                    // A zero-length path is just an intersection with a fresh new output path that hasn’t even started yet.
                    RemovePath(path);
                } else {
                    // Close the path creating a new intersection
                    ClosePath(path);
                }
            } else {
                var tile = City.Data[nextPos];

                if (tile is Intersection intersection) {
                    // The path collides with an intersection, join to it
                    JoinIntersection(path, intersection);

                } else if (tile is Path perpendicularPath && path.IsPerpendicular(perpendicularPath)) {
                    // The other path is perpendicular, split the path creating a new intersection in the middle and joint to it
                    var splitJoin = City.SplitPath(perpendicularPath, nextPos);
                    JoinIntersection(path, splitJoin);

                } else if (tile is Path otherPath && path.GetLength() == 0) {
                    // (since now, colliding paths ARE NOT PERPENDICULAR)
                    // There is other path coming from the same direction
                    // - And the current path is just starting, so remove it
                    RemovePath(path);

                } else if (tile is Path sameLinePath) {
                    // (since now, colliding paths ARE NOT PERPENDICULAR and HAVE A LENGTH > 0)

                    // There is other path coming from the same direction (not perpendicular)
                    // - We use the current position (not new position, which is already used by the other path)
                    // to create a new intersection
                    JoinPaths(path, sameLinePath);

                } else if (random.NextBool(Options.ProbabilityStreetEnd) || StopPathHeadingToBorder(path)) {
                    path.SetCursor(nextPos);
                    ClosePath(path);

                } else {
                    path.SetCursor(nextPos);
                    City.Data[nextPos] = path;
                    City.OnUpdate?.Invoke(nextPos);

                    var streetLength = Options.StreetMinLength;
                    if (path.GetLength() > streetLength &&
                        GetFirstOccupiedPosition(nextPos, path.Direction, streetLength) == null &&
                        random.NextBool(Options.ProbabilityIntersection)) {

                        // Randomly end path with an intersection and create a new path/fork/turn
                        var action = random.Pick(new (Action<Path>, float)[] {
                            (CreateCrossPath, Options.ProbabilityCross),
                            (CreateForkPath, Options.ProbabilityFork),
                            (CreateTurnPath, Options.ProbabilityTurn)
                        });
                        action.Invoke(path);

                    }
                }
            }
        }

        void RemovePath(Path path) {
            City.RemovePath(path);
            City.OnUpdate?.Invoke(path.GetCursor());
        }

        Intersection ClosePath(Path path) {
            var cursor = path.GetCursor();
            City.Data[cursor] = null;
            var intersection = City.AddIntersection(cursor);
            path.SetEnd(intersection);
            City.FlatIntersection(path.Start);
            City.OnUpdate?.Invoke(intersection.Position);
            return intersection;
        }

        void JoinIntersection(Path path, Intersection intersection) {
            // If the intersection has an output path in the opposite direction, remove it
            var other = intersection.GetOutputPaths().FirstOrDefault(p => p.Direction == -path.Direction && !p.IsCompleted() && p.GetLength() == 0);
            if (other != null) {
                intersection.RemoveOutputPath(other);
                // We need mark the other path deleted to make sure it's not processed again in the same loop in Grow
                deleted.Add(other);
            }
            path.SetEnd(intersection);
            City.FlatIntersection(path.Start);
            City.FlatIntersection(path.End!);
            City.OnUpdate?.Invoke(intersection.Position);
        }

        void JoinPaths(Path path, Path sameLinePath) {
            var join = ClosePath(path);
            if (City.FlatIntersection(path.Start)) {
            }
            sameLinePath.SetEnd(join);
            City.FlatIntersection(sameLinePath.Start);
            City.FlatIntersection(join);
            City.OnUpdate?.Invoke(join.Position);
        }

        void CreateCrossPath(Path path) {
            var directions = ForkDirection(path.Direction).ToList();
            var intersection = ClosePath(path);
            foreach (var direction in directions) {
                intersection.CreatePathTo(direction);
            }
            City.OnUpdate?.Invoke(intersection.Position);
        }

        void CreateForkPath(Path path) {
            List<Vector2I> directions = ForkDirection(path.Direction)
                .OrderBy(x => random.Next())
                .ToList();
            var intersection = ClosePath(path);
            foreach (var direction in directions.Take(directions.Count - 1)) {
                intersection.CreatePathTo(direction);
            }
            City.OnUpdate?.Invoke(intersection.Position);
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

        Vector2I? GetFirstOccupiedPosition(Vector2I start, Vector2I direction, int length) {
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
        var random = new Random(Options.Seed);
        foreach (var path in City.GetAllPaths()) {
            var nextPos = path.Start.Position + path.Direction;

            foreach (var direction in TurnDirection(path.Direction)) {
                var stepOffset = Options.BuildingOffset;

                while (path.GetLength() > stepOffset) {
                    var stepShift = path.Direction * stepOffset;
                    var shiftFromPath = direction * (Options.BuildingOffset + 1);
                    var startPosition = nextPos + stepShift + shiftFromPath;

                    int[] size = [
                        random.Next(Options.BuildingMinSize, Options.BuildingMaxSize + 1),
                        random.Next(Options.BuildingMinSize, Options.BuildingMaxSize + 1)
                    ];

                    if (stepOffset + size[0] + Options.BuildingOffset > path.GetLength()) {
                        break;
                    }
                    ProcessingBuilding(path, startPosition, size, [path.Direction, direction]);

                    var spaceBetweenBuildings = random.Next(Options.BuildingMinSpace, Options.BuildingMaxSpace + 1);
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