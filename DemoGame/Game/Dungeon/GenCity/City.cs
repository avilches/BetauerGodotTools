using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public interface ICityTile;

public class City {
    public int Width { get; }
    public int Height { get; }

    private Random _random;

    private readonly ICityTile[,] _matrix;
    private readonly List<Intersection> _nodes = [];

    private CityGenerationParameters _params = new CityGenerationParameters();

    public City(int width, int height) {
        Width = width;
        Height = height;
        _matrix = new ICityTile[Height, Width];
    }

    public List<Building> GetAllBuildings() {
        return GetAllPaths().SelectMany(path => path.GetBuildings()).ToList();
    }

    public List<Intersection> GetAllNodes() {
        return _nodes;
    }

    public List<Path> GetAllPaths() {
        return _nodes.SelectMany(node => node.GetOutputPaths()).ToList();
    }

    public void Generate(CityGenerationParameters customParams = null) {
        Reset();

        _params = customParams ?? new CityGenerationParameters();
        _random = new Random(_params.Seed);

        if (_params.StartPosition == Vector2I.Zero) {
            _params.StartPosition = new Vector2I(Width / 2, Height / 2);
        }

        if (_params.StartDirections.Count == 0) {
            _params.StartDirections = [0, 90, 180, 270];
        }

        var node = AddNode(_params.StartPosition);
        MarkAt(_params.StartPosition, node);

        foreach (var direction in _params.StartDirections) {
            node.AddOutputPath(direction);
        }

        GeneratePaths();
        GenerateBuildings();
    }

    private void Reset() {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                _matrix[y, x] = null;
            }
        }
        _nodes.Clear();
    }

    private void GeneratePaths() {
        var isCompleted = false;
        while (!isCompleted) {
            var paths = GetAllPaths();
            foreach (var p in paths) {
                ProcessingPath(p);
            }
            isCompleted = paths.All(path => path.IsCompleted());
        }
    }

    private void ProcessingPath(Path path) {
        if (path.IsCompleted()) {
            return;
        }

        var nextCursor = path.GetNextCursor();

        if (GetAt(nextCursor) == null && IsOutOfBounds(nextCursor) // Out of matrix
            || VariabilityChance(_params.ProbabilityStreetEnd)) { // Street end
            ClosePath(path);
            return;
        }

        // Crossed another street
        var cross = GetCross(path);

        if (cross.tile != null) {
            Intersection? intersection = cross.tile switch {
                Intersection crossNode => crossNode,
                Path crossPath => SplitPath(crossPath, cross.position),
                _ => null
            };
            if (intersection != null) {
                path.SetNodeEnd(intersection);
            }
            return;
        }

        // Continuation path
        path.SetCursor(nextCursor);
        MarkAt(path.GetCursor(), path);

        var streetLength = _params.StreetMinLength;

        if (path.GetLength() > streetLength && GetCross(path, streetLength).tile == null) {
            if (VariabilityChance(_params.ProbabilityIntersection)) {
                ForkPath(path);
            } else if (VariabilityChance(_params.ProbabilityTurn)) {
                TurnPath(path);
            }
        }
    }

    private void GenerateBuildings() {
        foreach (var path in GetAllPaths()) {
            var (dirX, dirY) = Utils.GetShift(path.Direction);
            var x = path.GetStart().Position.X + dirX;
            var y = path.GetStart().Position.Y + dirY;

            foreach (var direction in Utils.TurnDirection(path.Direction)) {
                var stepOffset = _params.BuildingOffset;

                while (path.GetLength() > stepOffset) {
                    Vector2I stepShift = Utils.GetShift(path.Direction, stepOffset);
                    Vector2I shiftFromPath = Utils.GetShift(direction, _params.BuildingOffset + 1);
                    Vector2I startPosition = new Vector2I(
                        x + stepShift.X + shiftFromPath.X,
                        y + stepShift.Y + shiftFromPath.Y
                    );

                    int[] size = [
                        VariabilityRange(_params.BuildingMinSize, _params.BuildingMaxSize),
                        VariabilityRange(_params.BuildingMinSize, _params.BuildingMaxSize)
                    ];

                    if (stepOffset + size[0] + _params.BuildingOffset > path.GetLength()) {
                        break;
                    }
                    ProcessingBuilding(path, startPosition, size, [path.Direction, direction]);

                    var spaceBetweenBuildings = VariabilityRange(_params.BuildingMinSpace, _params.BuildingMaxSpace);
                    stepOffset += size[0] + spaceBetweenBuildings;
                }
            }
        }
    }

    private void ProcessingBuilding(Path path, Vector2I position, int[] size, List<int> directions) {
        List<Vector2I> tiles = [];
        List<Vector2I> vertices = [];

        for (var i = 0; i < size[0]; i++) {
            Vector2I shiftParallel = Utils.GetShift(directions[0], i);

            Vector2I startFromPathPosition = new Vector2I(position.X + shiftParallel.X, position.Y + shiftParallel.Y);

            for (var j = 0; j < size[1]; j++) {
                Vector2I shiftPerpendicular = Utils.GetShift(directions[1], j);
                Vector2I tilePosition = new Vector2I(
                    startFromPathPosition.X + shiftPerpendicular.X,
                    startFromPathPosition.Y + shiftPerpendicular.Y
                );

                if (IsEmptyAt(tilePosition)) {
                    tiles.Add(tilePosition);

                    if (
                        (i == 0 || i == size[0] - 1)
                        && (j == 0 || j == size[1] - 1)
                    ) {
                        vertices.Add(tilePosition);
                    }
                } else {
                    return;
                }
            }
        }

        var building = path.AddBuilding(vertices);

        foreach (Vector2I tilePosition in tiles) {
            MarkAt(tilePosition, building);
        }
    }

    private ICityTile? GetAt(Vector2I position) {
        if (IsOutOfBounds(position)) return null;
        var (x, y) = position;
        return _matrix[y, x];
    }

    private bool IsOutOfBounds(Vector2I position) {
        var (x, y) = position;
        return x < 0 || y < 0 || x >= Width || y >= Height;
    }

    private void MarkAt(Vector2I position, ICityTile tile) {
        var (x, y) = position;
        _matrix[y, x] = tile;
    }

    private bool IsEmptyAt(Vector2I position) {
        return !IsOutOfBounds(position) && GetAt(position) == null;
    }

    private Intersection AddNode(Vector2I position, int? index = null) {
        var node = new Intersection(_nodes.Count, position);

        if (!index.HasValue) {
            _nodes.Add(node);
        } else {
            _nodes.Insert(index.Value, node);
        }

        return node;
    }

    private Intersection ClosePath(Path path) {
        Vector2I cursor = path.GetCursor();
        Intersection intersection = AddNode(cursor);

        path.SetNodeEnd(intersection);
        MarkAt(cursor, intersection);

        return intersection;
    }

    private void ForkPath(Path path) {
        List<int> directions = Utils.ForkDirection(path.Direction)
            .OrderBy(x => VariabilityChance(0.5f) ? 1 : -1)
            .ToList();

        directions = FilterDirections(path, directions);

        if (directions.Count == 0 || (directions.Count == 1 && directions[0] == path.Direction)) {
            return;
        }

        var intersection = ClosePath(path);

        for (var i = 0; i < directions.Count; i++) {
            if (i < 2 || VariabilityChance(0.5f)) {
                intersection.AddOutputPath(directions[i]);
            }
        }

        MarkAt(intersection.Position, intersection);
    }

    private void TurnPath(Path path) {
        var directions = Utils.TurnDirection(path.Direction)
            .OrderBy(x => VariabilityChance(0.5f) ? 1 : -1)
            .ToList();

        directions = FilterDirections(path, directions);

        if (directions.Count == 0) {
            return;
        }

        var intersection = ClosePath(path);
        intersection.AddOutputPath(directions[0]);
        MarkAt(intersection.Position, intersection);
    }

    private Intersection SplitPath(Path path, Vector2I position) {
        var nodeBeg = path.GetStart();
        var nodeBegIndex = _nodes.FindIndex(node => node == nodeBeg);
        var nodeNew = AddNode(position, nodeBegIndex + 1);
        var continuePath = nodeNew.AddOutputPath(path.Direction);

        MarkAt(position, nodeNew);

        var nodeEnd = path.GetNodeEnd();
        if (nodeEnd != null) {
            continuePath.SetNodeEnd(nodeEnd);
        } else {
            continuePath.SetCursor(path.GetCursor());
        }

        // Refill matrix with new path
        foreach (var tilePosition in continuePath.Each()) {
            if (GetAt(tilePosition) is Path) {
                MarkAt(tilePosition, continuePath);
            }
        }
        path.SetNodeEnd(nodeNew);
        return nodeNew;
    }

    private (object? tile, Vector2I position) GetCross(Path path, int length = 1) {
        Vector2I cursor = path.GetCursor();

        for (var i = 1; i <= length; i++) {
            var (x, y) = Utils.GetShift(path.Direction, i);
            var position = new Vector2I(cursor.X + x, cursor.Y + y);

            if (!IsEmptyAt(position)) {
                return (GetAt(position), position);
            }
        }

        return (null, Vector2I.Zero);
    }

    private List<int> FilterDirections(Path path, List<int> directions) {
        return directions.Where(direction => {
            var (x, y) = Utils.GetShift(direction);
            var (cx, cy) = path.GetCursor();
            var nextCursor = new Vector2I(cx + x, cy + y);

            // Primero verificamos si la posición inmediata está vacía
            if (!IsEmptyAt(nextCursor)) return false;

            // Verificar si hay carreteras paralelas cercanas
            var minDistanceToRoad = 3; // Distancia mínima a otra carretera
            var isHorizontal = Math.Abs(direction) == 0 || Math.Abs(direction) == 180;

            // Verificar en perpendicular a la dirección de la carretera
            for (var dist = 1; dist <= minDistanceToRoad; dist++) {
                Vector2I checkPos1, checkPos2;
                if (isHorizontal) {
                    // Para carreteras horizontales, verificar arriba y abajo
                    checkPos1 = new Vector2I(nextCursor.X, nextCursor.Y + dist);
                    checkPos2 = new Vector2I(nextCursor.X, nextCursor.Y - dist);
                } else {
                    // Para carreteras verticales, verificar izquierda y derecha
                    checkPos1 = new Vector2I(nextCursor.X + dist, nextCursor.Y);
                    checkPos2 = new Vector2I(nextCursor.X - dist, nextCursor.Y);
                }

                // Si encontramos una carretera paralela demasiado cerca, rechazar esta dirección
                if (!IsOutOfBounds(checkPos1) && GetAt(checkPos1) is Path nearPath) {
                    var isParallel = (isHorizontal && (Math.Abs(nearPath.Direction) == 0 || Math.Abs(nearPath.Direction) == 180)) ||
                                     (!isHorizontal && (Math.Abs(nearPath.Direction) == 90 || Math.Abs(nearPath.Direction) == 270));
                    if (isParallel) return false;
                }
                if (!IsOutOfBounds(checkPos2) && GetAt(checkPos2) is Path nearPath2) {
                    var isParallel = (isHorizontal && (Math.Abs(nearPath2.Direction) == 0 || Math.Abs(nearPath2.Direction) == 180)) ||
                                     (!isHorizontal && (Math.Abs(nearPath2.Direction) == 90 || Math.Abs(nearPath2.Direction) == 270));
                    if (isParallel) return false;
                }
            }

            // También verificar un poco hacia adelante para evitar convergencias
            var forwardCheck = 3;
            for (var i = 1; i <= forwardCheck; i++) {
                Vector2I forwardPos = new Vector2I(
                    nextCursor.X + x * i,
                    nextCursor.Y + y * i
                );

                if (!IsOutOfBounds(forwardPos) && GetAt(forwardPos) is Path) {
                    return false;
                }
            }

            return true;
        }).ToList();
    }

    public bool VariabilityChance(float value) {
        return _random.NextDouble() > 1.0 - value;
    }

    public int VariabilityRange(int min, int max) {
        return (int)(min + _random.NextDouble() * (max - min + 1));
    }
}