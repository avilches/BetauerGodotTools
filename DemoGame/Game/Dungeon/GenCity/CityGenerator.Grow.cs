using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Geometry;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public static class CityGrowExtension {
    public static CityGenerator CreateGenerator(this City city, CityGenerationOptions? options = null) {
        return new CityGenerator(city) {
            Options = options ?? new CityGenerationOptions()
        };
    }
}

public partial class CityGenerator(City city) {
    public City City { get; } = city;
    const float HeadingBorderPathLimit = 3;
    const float HeadingBorderPathProbability = 0.3f;


    public CityGenerationOptions Options { get; set; } = new();
    public int Seed => Options.Seed;

    public void Start() {
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
        for (var n = 0; n < Options.SeedOffset; n++) {
            random.NextDouble();
        }

        var deleted = new HashSet<Path>();
        while (!isCompleted) {
            var paths = City.GetIncompletePaths().ToArray();
            foreach (var path in paths) {
                if (!deleted.Contains(path) && !path.IsCompleted()) {
                    // A path could be completed by other path when joining
                    GrowPath(path);
                }
            }
            isCompleted = !City.GetIncompletePaths().Any();
        }
        return;

        void GrowPath(Path path) {
            var currentPos = path.GetCursor();
            var nextPos = currentPos + path.Direction;

            if (!City.Data.IsInBounds(nextPos)) {
                // The path is heading to the border, close it
                CreateDeadEnd(path);
            } else {
                var tile = City.Data[nextPos];

                if (tile is Intersection intersection) {
                    // The path collides with an intersection, join to it
                    JoinPathToIntersection(path, intersection);
                } else if (tile is Path perpendicularPath && path.IsPerpendicular(perpendicularPath)) {
                    // The other path is perpendicular, split the path creating a new intersection in the middle and joint to it
                    JoinPathToPerpendicularPath(path, perpendicularPath, nextPos);
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
                    JoinSameLinePaths(path, sameLinePath);
                } else if (tile != null) {
                    CreateDeadEnd(path);
                } else if (random.NextBool(Options.ProbabilityStreetEnd) || StopPathHeadingToBorder(path)) {
                    path.SetCursor(nextPos);
                    CreateDeadEnd(path);
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
            City.FlatIntersection(path.Start);
            if (path.End != null) City.FlatIntersection(path.End);
            City.OnUpdate?.Invoke(path.GetCursor());
        }

        Intersection CreateDeadEnd(Path path) {
            if (path.GetLength() == 0) {
                // A zero-length path is just an intersection with a fresh new output path that has not even started yet
                RemovePath(path);
                return null;
            }
            var cursor = path.GetCursor();
            City.Data[cursor] = null;
            var intersection = City.AddIntersection(cursor);
            path.SetEnd(intersection);
            City.FlatIntersection(path.Start);
            City.OnUpdate?.Invoke(intersection.Position);
            return intersection;
        }

        void JoinPathToPerpendicularPath(Path path, Path perpendicularPath, Vector2I position) {
            var splitJoin = City.SplitPath(perpendicularPath, position);
            JoinPathToIntersection(path, splitJoin);
            City.FlatIntersection(perpendicularPath.Start);
        }

        void JoinPathToIntersection(Path path, Intersection intersection) {
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

        void JoinSameLinePaths(Path path, Path sameLinePath) {
            var join = CreateDeadEnd(path);
            sameLinePath.SetEnd(join);
            City.FlatIntersection(join);
            City.FlatIntersection(path.Start);
            City.FlatIntersection(sameLinePath.Start);
            City.OnUpdate?.Invoke(join.Position);
        }

        void CreateCrossPath(Path path) {
            var directions = ForkDirection(path.Direction).ToList();
            var intersection = CreateDeadEnd(path);
            foreach (var direction in directions) {
                intersection.CreatePathTo(direction);
            }
            City.OnUpdate?.Invoke(intersection.Position);
        }

        void CreateForkPath(Path path) {
            List<Vector2I> directions = ForkDirection(path.Direction)
                .OrderBy(x => random.Next())
                .ToList();
            var intersection = CreateDeadEnd(path);
            foreach (var direction in directions.Take(directions.Count - 1)) {
                intersection.CreatePathTo(direction);
            }
            City.OnUpdate?.Invoke(intersection.Position);
        }

        void CreateTurnPath(Path path) {
            var direction = random.Next(BothTurnDirections(path.Direction));
            var intersection = CreateDeadEnd(path);
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

    public static IList<Vector2I> BothTurnDirections(Vector2I direction) {
        return [direction.Rotate90Left(), direction.Rotate90Right()];
    }

    public static IList<Vector2I> ForkDirection(Vector2I direction) {
        return [direction, direction.Rotate90Left(), direction.Rotate90Right()];
    }

    /// <summary>
    /// Generates a valid city and tries to make it have the desired density. If it does not achieve the desired density, it will keep
    /// the one with the highest density that it has achieved during all the retries.
    /// Returns true if it achieves the desired density. In any case, it will always generate a valid city, no matter how many tries
    ///
    /// Una desiredDensity de 0 significa que se quedará con la primera ciudad generada que sea valida, sin necesidad de intentar añadir mas caminos.
    /// Una desiredDensity de 1 significa que consumirá todos los tries y se quedará con la que tenga mayor densidad. 
    /// </summary>
    /// <param name="desiredDensity"></param>
    /// <param name="validate"></param>
    /// <param name="tries"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool Generate(Func<bool> validate, float desiredDensity = 0, int tries = 20, float timeout = 10f) {
        var best = (Offset: -1, Density: 0f);
        var count = 0;

        Options.SeedOffset = 0;
        var startTime = DateTime.Now;
        while (count < tries || best.Offset == -1) {
            City.RemoveAllPaths();
            Start();
            Grow();
            FillGaps(desiredDensity);
            if (validate()) {
                var density = City.GetPathDensity();
                if (density >= desiredDensity) {
                    // Console.WriteLine(":) Dense enough! Offset: "+Options.SeedOffset);
                    return true;
                }
                if (density > best.Density) {
                    // Console.WriteLine($"  No dense enough: {density} Offset : "+Options.SeedOffset+ " has more density "+density+" than the best one "+best.Density);
                    best = (Offset: Options.SeedOffset, Density: density);
                } else {
                    // Console.WriteLine($"  No dense enough: {density} Offset : "+Options.SeedOffset);
                }
                count++;
            } else {
                Console.WriteLine("Invalid Offset: " + Options.SeedOffset);
            }
            if ((DateTime.Now - startTime).TotalSeconds > timeout) {
                return false;
            }
            Options.SeedOffset++;
        }

        Options.SeedOffset = best.Offset;
        City.RemoveAllPaths();
        Start();
        Grow();
        FillGaps(desiredDensity);
        return false;
    }

    /// <summary>
    /// It will stop when the density reaches the minDensity, or no more paths can be added
    /// </summary>
    /// <param name="minDensity"></param>
    public bool FillGaps(float minDensity = 1f) {
        var density = City.GetPathDensity();
        while (density < minDensity) {
            var gaps = FindGaps().ToList();
            if (gaps.Count == 0) {
                return false;
            }

            // Ordenar los huecos por tamaño (de mayor a menor)
            gaps.Sort((a, b) => b.Area.CompareTo(a.Area));

            var addedAnyRoads = false;
            foreach (var gap in gaps) {
                if (TryAddRoadsToGap(gap).Count > 0) {
                    addedAnyRoads = true;
                    break;
                }
            }
            if (!addedAnyRoads) {
                return false;
            }
            Grow();
            density = City.GetPathDensity();
        }
        return true;
    }

    public IEnumerable<Rect2I> FindGaps() {
        var visited = new bool[City.Height, City.Width];
        foreach (var ((x, y), item) in City.Data.GetIndexedValues()) {
            if (!visited[y, x] && item is null) {
                var gap = FillRect(new Vector2I(x, y), visited);
                yield return gap;
            }
        }
    }

    public Dictionary<Vector2I, Rect2I> GetGaps(IEnumerable<Vector2I> positions) {
        var visited = new bool[City.Height, City.Width];
        Dictionary<Vector2I, Rect2I> result = [];
        foreach (var pos in positions) {
            var gap = FillRect(pos, visited);
            result[pos] = gap;
        }
        return result;
    }

    private Rect2I FillRect(Vector2I start, bool[,] visited) {
        // Buscar los límites del área vacía en las cuatro direcciones
        var minX = start.X;
        var maxX = start.X;
        var minY = start.Y;
        var maxY = start.Y;

        if (!IsPositionValid(start, visited)) {
            return new Rect2I(start, Vector2I.Zero);
        }

        // Marcar el punto inicial como visitado
        visited[start.Y, start.X] = true;

        // Intentar expandir en todas las direcciones
        var expandedAny = true;
        while (expandedAny) {
            expandedAny = false;

            // Intentar expandir hacia la derecha
            if (CanExpandHorizontally(maxX + 1, minY, maxY)) {
                for (var y = minY; y <= maxY; y++) {
                    visited[y, maxX + 1] = true;
                }
                maxX++;
                expandedAny = true;
            }

            // Intentar expandir hacia la izquierda
            if (CanExpandHorizontally(minX - 1, minY, maxY)) {
                for (var y = minY; y <= maxY; y++) {
                    visited[y, minX - 1] = true;
                }
                minX--;
                expandedAny = true;
            }

            // Intentar expandir hacia abajo
            if (CanExpandVertically(minX, maxX, maxY + 1)) {
                for (var x = minX; x <= maxX; x++) {
                    visited[maxY + 1, x] = true;
                }
                maxY++;
                expandedAny = true;
            }

            // Intentar expandir hacia arriba
            if (CanExpandVertically(minX, maxX, minY - 1)) {
                for (var x = minX; x <= maxX; x++) {
                    visited[minY - 1, x] = true;
                }
                minY--;
                expandedAny = true;
            }
        }
        return new Rect2I(minX, minY, maxX - minX + 1, maxY - minY + 1);

        // Función auxiliar para verificar si se puede expandir horizontalmente (columna)
        bool CanExpandHorizontally(int x, int yStart, int yEnd) {
            if (x < 0 || x >= City.Width) return false;
            for (var y = yStart; y <= yEnd; y++) {
                if (!IsPositionValid(new Vector2I(x, y), visited)) return false;
            }
            return true;
        }

        // Función auxiliar para verificar si se puede expandir verticalmente (fila)
        bool CanExpandVertically(int xStart, int xEnd, int y) {
            if (y < 0 || y >= City.Height) return false;
            for (var x = xStart; x <= xEnd; x++) {
                if (!IsPositionValid(new Vector2I(x, y), visited)) return false;
            }
            return true;
        }

        // Método auxiliar para comprobar si una posición es válida y está disponible
        bool IsPositionValid(Vector2I pos, bool[,] visitedMap) {
            return City.Data.IsInBounds(pos) && !visitedMap[pos.Y, pos.X] && City.Data[pos] == null;
        }
    }

    private List<Path> TryAddRoadsToGap(Rect2I gap) {
        // Encontrar caminos existentes cerca del hueco
        List<Path> nearbyPaths = FindPathsNearGap(gap);
        if (nearbyPaths.Count == 0) return [];

        HashSet<Vector2I> visited = [];
        List<Path> newPaths = [];

        // Intentar diferentes posibles direcciones para nuevos caminos
        foreach (var sourcePath in nearbyPaths) {
            foreach (var direction in BothTurnDirections(sourcePath.Direction)) {
                // Verificar si podemos añadir un camino perpendicular desde este camino existente
                if (!CanAddPathInDirection(sourcePath, direction, gap)) continue;

                // Intentar encontrar una posición adecuada a lo largo del camino existente
                var startPoint = FindSuitableStartPoint(sourcePath, direction, gap);
                if (!startPoint.HasValue || !visited.Add(startPoint.Value)) continue;

                // Crear una nueva intersección y camino
                var intersection = City.GetOrCreateIntersectionAt(startPoint.Value);
                var newPath = intersection.CreatePathTo(direction);
                newPaths.Add(newPath);
                break;
            }
        }
        return newPaths;

        List<Path> FindPathsNearGap(Rect2I gap) {
            HashSet<Path> addedPaths = [];
            var minX = gap.Position.X;
            var minY = gap.Position.Y;
            var maxX = gap.Position.X + gap.Size.X - 1;
            var maxY = gap.Position.Y + gap.Size.Y - 1;

            // Top edge
            for (var x = minX; x <= maxX; x++) {
                CheckNeighborForPath(new Vector2I(x, minY) + Vector2I.Up);
            }

            // Bottom edge
            for (var x = minX; x <= maxX; x++) {
                CheckNeighborForPath(new Vector2I(x, maxY) + Vector2I.Down);
            }

            // Left edge (excluding corners already checked)
            for (var y = minY + 1; y < maxY; y++) {
                CheckNeighborForPath(new Vector2I(minX, y) + Vector2I.Left);
            }

            // Right edge (excluding corners already checked)
            for (var y = minY + 1; y < maxY; y++) {
                CheckNeighborForPath(new Vector2I(maxX, y) + Vector2I.Right);
            }

            return addedPaths.ToList();

            void CheckNeighborForPath(Vector2I pos) {
                if (City.Data.IsInBounds(pos) && City.Data[pos] is Path path) {
                    addedPaths.Add(path);
                }
            }
        }

        bool CanAddPathInDirection(Path sourcePath, Vector2I direction, Rect2I gap) {
            // Verificar si podemos añadir un camino en la dirección dada
            foreach (var pathPos in sourcePath.GetAllPositions()) {
                // El punto de inicio de la nueva carretera sería desde el camino existente
                // Verificamos si desde este punto es posible crear un camino perpendicular
                if (City.Data.IsInBounds(pathPos)) {
                    // Comprobar el punto de inicio
                    var nextPos = pathPos + direction;

                    // Verificar que el primer paso esté dentro del gap
                    if (City.Data.IsInBounds(nextPos) && City.Data[nextPos] == null && gap.GetPositions().Contains(nextPos)) {
                        // Verificar que tengamos espacio para un camino de longitud mínima
                        if (HasEnoughSpaceInGap(pathPos, direction, gap)) {
                            // Verificar que no haya caminos paralelos cercanos
                            if (!HasParallelPathsNearby(pathPos, direction)) {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        Vector2I? FindSuitableStartPoint(Path sourcePath, Vector2I direction, Rect2I gap) {
            List<Vector2I> candidates = [];

            foreach (var pos in sourcePath.GetAllPositions()) {
                // Verificar si es posible empezar un camino desde aquí
                var nextPos = pos + direction;

                // La primera celda debe estar libre y dentro del gap
                if (!City.Data.IsInBounds(nextPos) ||
                    City.Data[nextPos] != null ||
                    !gap.GetPositions().Contains(nextPos)) continue;

                // Verificar espacio y caminos paralelos
                if (HasEnoughSpaceInGap(pos, direction, gap) &&
                    !HasParallelPathsNearby(pos, direction)) {
                    candidates.Add(pos);
                }
            }

            if (candidates.Count == 0) return null;

            // Elegir un punto que esté aproximadamente en el centro del camino,
            // para evitar crear intersecciones demasiado cerca de los extremos
            return candidates[candidates.Count / 2];
        }

        bool HasEnoughSpaceInGap(Vector2I start, Vector2I direction, Rect2I gap) {
            var count = 0;
            var pos = start;

            // Necesitamos contar al menos StreetMinLength celdas libres
            for (var i = 1; i <= Options.StreetMinLength * 2; i++) { // Verificamos el doble para tener margen
                pos += direction;

                // Verificar si la posición está fuera de los límites
                if (!City.Data.IsInBounds(pos)) {
                    break;
                }

                // Verificar si la posición tiene algo
                if (City.Data[pos] != null) {
                    break;
                }

                // Verificar si la posición está en el gap
                if (!gap.GetPositions().Contains(pos)) {
                    break;
                }

                count++;

                // Si ya tenemos suficiente longitud, devolver true
                if (count >= Options.StreetMinLength) {
                    return true;
                }
            }

            return false;
        }

        bool HasParallelPathsNearby(Vector2I start, Vector2I direction) {
            // La dirección perpendicular para verificar caminos paralelos
            var perpendicular = direction.Rotate90Left();

            // Comprobar en ambas direcciones perpendiculares hasta StreetMinLength de distancia
            for (var dist = 1; dist <= Options.StreetMinLength; dist++) {
                // Comprobar en ambas direcciones perpendiculares
                if (HasParallelPathInLine(start + perpendicular * dist, direction) ||
                    HasParallelPathInLine(start - perpendicular * dist, direction)) {
                    return true;
                }
            }

            return false;
        }

        bool HasParallelPathInLine(Vector2I pos, Vector2I direction) {
            // Verificar si esta posición está dentro de los límites
            if (!City.Data.IsInBounds(pos)) {
                return false;
            }

            // Verificar si hay un camino en esta posición
            if (City.Data[pos] is Path path) {
                // Si es un camino paralelo, devolver true
                if (path.Direction == direction || path.Direction == -direction) {
                    return true;
                }
            }

            // Comprobar a lo largo de la línea en ambas direcciones
            for (var offset = 1; offset <= Options.StreetMinLength; offset++) {
                // Verificar hacia adelante
                Vector2I checkPos = pos + direction * offset;
                if (City.Data.IsInBounds(checkPos) && City.Data[checkPos] is Path forwardPath) {
                    if (forwardPath.Direction == direction || forwardPath.Direction == -direction) {
                        return true;
                    }
                }

                // Verificar hacia atrás
                checkPos = pos - direction * offset;
                if (City.Data.IsInBounds(checkPos) && City.Data[checkPos] is Path backwardPath) {
                    if (backwardPath.Direction == direction || backwardPath.Direction == -direction) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}