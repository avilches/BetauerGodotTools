using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.Maze;
using Godot;

namespace Betauer.Core.PCG.Examples;

public class MazeGraphToArray2D {
    public static Array2D<char> Convert(
        MazeGraph graph,
        MazePattern patterns,
        params string[] flags) {
        var offset = graph.GetOffset();
        var size = graph.GetSize();
        var cellSize = patterns.CellSize;

        // Crear el array final con el tamaño total necesario
        var array = new Array2D<char>(
            (size.X + 1) * cellSize,
            (size.Y + 1) * cellSize,
            '.' // caracter por defecto para espacios vacíos
        );

        // Para cada nodo en el grafo
        foreach (var node in graph.GetNodes()) {
            // Obtener el patrón apropiado según las conexiones del nodo
            var pattern = patterns.FindPatterns(node, flags)[0];

            // Calcular la posición donde colocar el patrón
            var pos = (node.Position - offset) * cellSize;

            // Copiar el patrón en la posición correcta
            CopyPattern(array, pattern, pos);
        }

        return array;
    }

    private static void CopyPattern(Array2D<char> target, Array2D<char> pattern, Vector2I position) {
        // Console.WriteLine($"Copying pattern size " +
        //                   $"({pattern.Width}x{pattern.Height}) " +
        //                   $"at position ({position.X},{position.Y})");
        for (var y = 0; y < pattern.Height; y++) {
            for (var x = 0; x < pattern.Width; x++) {
                target[position.Y + y, position.X + x] = pattern[y, x];
            }
        }
    }
}

public class PatternDefinition {
    public PatternId Id { get; }
    public Array2D<char> Pattern { get; private set; }
    public PatternId? FromPattern { get; set; }

    public PatternDefinition(PatternId id) {
        Id = id;
    }

    public void SetPattern(Array2D<char> pattern) {
        Pattern = pattern;
    }

    public bool HasExactFlags(string[] requiredFlags) {
        return Id.Flags.SetEquals(new HashSet<string>(requiredFlags));
    }

    public override string ToString() {
        return Id.ToString();
    }
}

public class MazePattern(int cellSize) {
    private readonly Dictionary<byte, List<PatternDefinition>> _patterns = new();

    public int CellSize { get; } = cellSize;

    public void LoadPatterns(string patternsFile) {
        var newPatterns = MazePatternLoader.LoadFromFile(patternsFile);
        foreach (var pattern in newPatterns) {
            var value = pattern.Key;
            if (!_patterns.ContainsKey(value)) {
                _patterns[value] = new List<PatternDefinition>();
            }
            _patterns[value].AddRange(pattern.Value);
        }
    }

    // Encuentra todos los patrones que coincidan con el nodo
    public List<Array2D<char>> FindPatterns(MazeNode node) {
        return FindPatterns(PatternId.FromNode(node));
    }

    // Encuentra todos los patrones que coincidan con el nodo y los flags
    public List<Array2D<char>> FindPatterns(MazeNode node, string[] requiredFlags, string[]? optionalFlags = null) {
        return FindPatterns(PatternId.FromNode(node), requiredFlags, optionalFlags);
    }

    // Encuentra todos los patrones para el valor dado
    public List<Array2D<char>> FindPatterns(byte value) {
        return GetPatternDefinitions(value).Select(d => d.Pattern).ToList();
    }

    // Encuentra todos los patrones que coincidan con los flags requeridos y opcionales
    public List<Array2D<char>> FindPatterns(byte value, string[] requiredFlags, string[]? optionalFlags = null) {
        return GetPatternDefinitions(value, requiredFlags, optionalFlags).Select(d => d.Pattern).ToList();
    }

    // Obtiene un único patrón que coincida exactamente con los flags requeridos
    public Array2D<char> GetPattern(byte value, string[] requiredFlags) {
        var matchingDefinitions = GetPatternDefinitionsWithExactFlags(value, requiredFlags);
        if (matchingDefinitions.Count > 1) {
            throw new ArgumentException(
                $"Multiple patterns found for value {PatternId.ByteToDirectionsString(value)} ({value}) " +
                $"with flags: {string.Join(", ", requiredFlags)}");
        }
        return matchingDefinitions[0].Pattern;
    }

    private List<PatternDefinition> GetPatternDefinitions(byte value) {
        if (!_patterns.TryGetValue(value, out var definitions)) {
            throw new ArgumentException(
                $"No pattern found for value {PatternId.ByteToDirectionsString(value)} ({value})");
        }
        return definitions;
    }

    private List<PatternDefinition> GetPatternDefinitions(byte value, string[] requiredFlags, string[]? optionalFlags = null) {
        if (!_patterns.TryGetValue(value, out var definitions)) {
            throw new ArgumentException(
                $"No pattern found for value {PatternId.ByteToDirectionsString(value)} ({value})");
        }

        // Primero filtramos por flags requeridos
        var matchingDefinitions = definitions
            .Where(d => HasRequiredFlags(d, requiredFlags))
            .ToList();

        if (!matchingDefinitions.Any()) {
            throw new ArgumentException(
                $"No pattern found for value {PatternId.ByteToDirectionsString(value)} ({value}) " +
                $"with required flags: {string.Join(", ", requiredFlags)}");
        }

        // Si hay flags opcionales, ordenamos por cantidad de coincidencias
        if (optionalFlags != null && optionalFlags.Length > 0) {
            matchingDefinitions = matchingDefinitions
                .OrderByDescending(d => CountMatchingOptionalFlags(d, optionalFlags))
                .ThenBy(d => d.Id.Flags.Count) // Preferimos patrones con menos flags adicionales
                .ToList();
        }

        return matchingDefinitions;
    }

    private List<PatternDefinition> GetPatternDefinitionsWithExactFlags(byte value, string[] requiredFlags) {
        if (!_patterns.TryGetValue(value, out var definitions)) {
            throw new ArgumentException(
                $"No pattern found for value {PatternId.ByteToDirectionsString(value)} ({value})");
        }

        var matchingDefinitions = definitions
            .Where(d => d.HasExactFlags(requiredFlags))
            .ToList();

        if (!matchingDefinitions.Any()) {
            throw new ArgumentException(
                $"No pattern found for value {PatternId.ByteToDirectionsString(value)} ({value}) " +
                $"with exact flags: {string.Join(", ", requiredFlags)}");
        }

        return matchingDefinitions;
    }

    private bool HasRequiredFlags(PatternDefinition pattern, string[] requiredFlags) {
        return requiredFlags.All(flag => pattern.Id.Flags.Contains(flag));
    }

    private int CountMatchingOptionalFlags(PatternDefinition pattern, string[] optionalFlags) {
        return optionalFlags.Count(flag => pattern.Id.Flags.Contains(flag));
    }
}

[Flags]
public enum PatternDirection {
    None = 0,
    North = 1 << 0, // 00000001
    East = 1 << 2, // 00000100
    South = 1 << 4, // 00010000
    West = 1 << 6, // 10000000

    // Máscara útil
    All = North | East | South | West // 1111
}

public class PatternId {
    public readonly byte Value;
    public readonly HashSet<string> Flags;

    internal PatternId(byte value, IEnumerable<string> flags) {
        Value = value;
        Flags = new HashSet<string>(flags);
    }

    public static PatternId Parse(string idString) {
        var parts = idString.Split('/');
        if (parts.Length < 1) {
            throw new ArgumentException("Invalid pattern ID format. Expected 'directions/flag1/flag2...'");
        }

        var value = ParseDirections(parts[0]);
        var flags = parts.Length > 1 ? parts[1..] : Array.Empty<string>();
        return new PatternId(value, flags);
    }

    public static string ByteToDirectionsString(byte value) {
        var directions = new List<string>();
        if ((value & (byte)PatternDirection.North) != 0) directions.Add("N");
        if ((value & (byte)PatternDirection.East) != 0) directions.Add("E");
        if ((value & (byte)PatternDirection.South) != 0) directions.Add("S");
        if ((value & (byte)PatternDirection.West) != 0) directions.Add("W");
        return string.Join("", directions);
    }

    private static byte ParseDirections(string directions) {
        byte value = 0;
        directions = directions.Trim().ToUpper();

        // Si es un número, lo parseamos directamente
        if (byte.TryParse(directions, out byte numericValue)) {
            return numericValue;
        }

        // Si no, lo interpretamos como combinación de letras
        foreach (char c in directions) {
            value |= c switch {
                'N' => (byte)PatternDirection.North, // 1
                'E' => (byte)PatternDirection.East, // 4
                'S' => (byte)PatternDirection.South, // 16
                'W' => (byte)PatternDirection.West, // 64
                _ => throw new ArgumentException($"Invalid direction character: {c}")
            };
        }
        return value;
    }

    public static byte FromNode(MazeNode node) {
        byte directions = 0;

        if (node.Up != null) directions |= (byte)PatternDirection.North;
        if (node.Right != null) directions |= (byte)PatternDirection.East;
        if (node.Down != null) directions |= (byte)PatternDirection.South;
        if (node.Left != null) directions |= (byte)PatternDirection.West;

        return directions;
    }

    public override string ToString() {
        var baseString = ByteToDirectionsString(Value);
        if (Flags.Count == 0) return baseString;
        return baseString + "/" + string.Join("/", Flags);
    }

    public override bool Equals(object? obj) {
        if (obj is not PatternId other) return false;
        return Value == other.Value && Flags.SetEquals(other.Flags);
    }

    public override int GetHashCode() {
        var hash = Value.GetHashCode();
        foreach (var flag in Flags.OrderBy(f => f)) {
            hash = HashCode.Combine(hash, flag.GetHashCode());
        }
        return hash;
    }
}

public class MazePatternLoader {
    private const string IdPrefix = "@ID=";

    private static void PrintArray2D(Array2D<char> array2D) {
        for (var y = 0; y < array2D.Height; y++) {
            for (var x = 0; x < array2D.Width; x++) {
                Console.Write(array2D[y, x]);
            }
            Console.WriteLine();
        }
    }

    public static Dictionary<byte, List<PatternDefinition>> LoadFromFile(string filePath) {
        var patterns = new Dictionary<byte, List<PatternDefinition>>();
        var lines = File.ReadAllLines(filePath);
        var currentPattern = new List<string>();
        PatternDefinition? current = null;

        void ProcessCurrentPattern() {
            if (current == null) return;
            Console.WriteLine($"Processing pattern {current}");

            // Primero comprobamos si ya existe un patrón con el mismo value y flags
            if (patterns.TryGetValue(current.Id.Value, out var existingPatterns)) {
                var duplicatePattern = existingPatterns.FirstOrDefault(p =>
                    p.HasExactFlags(current.Id.Flags.ToArray()));

                if (duplicatePattern != null) {
                    throw new ArgumentException(
                        $"Duplicate pattern found: {current}");
                }
            } else {
                patterns[current.Id.Value] = new List<PatternDefinition>();
            }

            if (current.FromPattern != null) {
                var baseArray2D = GetPatternFromDictionary(patterns, current.FromPattern);
                current.SetPattern(TransformPattern(baseArray2D, current.FromPattern.Value, current.Id.Value));
            } else if (currentPattern.Count > 0) {
                current.SetPattern(ParsePattern(currentPattern));
            }

            patterns[current.Id.Value].Add(current);
            currentPattern.Clear();
            current = null;
        }

        foreach (var line in lines) {
            var trimmed = line.Trim();

            // Ignorar líneas vacías o comentarios
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) {
                if (current != null && currentPattern.Count > 0) {
                    current.SetPattern(ParsePattern(currentPattern));
                    currentPattern.Clear();
                }
                continue;
            }

            // Procesar línea @ID
            if (trimmed.StartsWith(IdPrefix)) {
                if (current != null) ProcessCurrentPattern();

                // Eliminar comentarios al final de la línea
                var withoutComments = trimmed.Split('#')[0].Trim();

                // Parsear la línea completa
                var parts = withoutComments[IdPrefix.Length..].Split(new[] { "from" }, StringSplitOptions.TrimEntries);

                var id = PatternId.Parse(parts[0]);
                current = new PatternDefinition(id);

                if (parts.Length > 1) {
                    current.FromPattern = PatternId.Parse(parts[1]);
                }
            } else {
                currentPattern.Add(trimmed);
            }
        }

        if (current != null) ProcessCurrentPattern();
        return patterns;
    }

    private static Array2D<char> GetPatternFromDictionary(Dictionary<byte, List<PatternDefinition>> patterns, PatternId patternId) {
        if (!patterns.TryGetValue(patternId.Value, out var definitions)) {
            throw new ArgumentException($"Base pattern not found: {patternId}");
        }

        var matchingDefinitions = definitions.Where(d => d.HasExactFlags(patternId.Flags.ToArray())).ToList();

        if (!matchingDefinitions.Any()) {
            throw new ArgumentException($"No base pattern found: {patternId}");
        }

        if (matchingDefinitions.Count > 1) {
            throw new ArgumentException($"Multiple base patterns found: {patternId}. Base pattern must be unique.");
        }

        return matchingDefinitions[0].Pattern;
    }


    /// <summary>
    /// Transforma un patrón base en otro patrón con diferente número de conexiones.
    /// El método calcula la rotación necesaria basándose en los bits de conexión.
    /// 
    /// Por ejemplo:
    /// - Si el patrón base (1 conexión) tiene conexión norte (0001)
    /// - Y queremos un patrón de 2 conexiones norte-este (0011)
    /// - No necesita rotación porque la conexión norte ya está en su sitio
    /// 
    /// Pero:
    /// - Si el patrón base (1 conexión) tiene conexión norte (0001)
    /// - Y queremos un patrón de 2 conexiones este-sur (0110)
    /// - Necesita rotar 90 grados a la derecha
    /// </summary>
    private static Array2D<char> TransformPattern(Array2D<char> basePattern, int baseConnections, int targetConnections) {
        // Primero verificamos que el número de conexiones del patrón base es menor o igual al objetivo
        if (BitOperations.PopCount((uint)baseConnections) > BitOperations.PopCount((uint)targetConnections)) {
            throw new ArgumentException($"Base pattern has more connections ({baseConnections}) than target ({targetConnections})");
        }

        // Si el patrón base tiene solo una conexión, es más simple porque solo necesitamos
        // encontrar la primera conexión del patrón objetivo y rotar hacia ella
        if (BitOperations.PopCount((uint)baseConnections) == 1) {
            // Encuentra la posición del bit en el patrón base (0-3)
            int basePos = BitOperations.TrailingZeroCount((uint)baseConnections);

            // Encuentra la primera conexión en el patrón objetivo
            int targetPos = BitOperations.TrailingZeroCount((uint)targetConnections);

            // Calcula cuántas rotaciones de 90 grados necesitamos
            int rotations = (targetPos - basePos + 4) % 4;

            // Aplica las rotaciones
            return rotations switch {
                0 => basePattern.Clone(),
                1 => new Array2D<char>(basePattern.Data.Rotate90()),
                2 => new Array2D<char>(basePattern.Data.Rotate180()),
                3 => new Array2D<char>(basePattern.Data.RotateMinus90()),
                _ => throw new ArgumentException("Invalid rotation calculated")
            };
        }

        // Para patrones con más de una conexión, necesitamos encontrar la mejor rotación
        // que minimice el número de transformaciones adicionales necesarias
        int bestRotation = FindBestRotation(baseConnections, targetConnections);

        return bestRotation switch {
            0 => basePattern.Clone(),
            1 => new Array2D<char>(basePattern.Data.Rotate90()),
            2 => new Array2D<char>(basePattern.Data.Rotate180()),
            3 => new Array2D<char>(basePattern.Data.RotateMinus90()),
            _ => throw new ArgumentException("Invalid rotation calculated")
        };
    }

    /// <summary>
    /// Encuentra la mejor rotación comparando todas las posibles rotaciones del patrón base
    /// con el patrón objetivo. La mejor rotación es la que requiere menos transformaciones
    /// adicionales para alcanzar el patrón objetivo.
    /// </summary>
    private static int FindBestRotation(int baseConnections, int targetConnections) {
        int bestRotation = 0;
        int bestScore = -1;

        // Prueba todas las rotaciones posibles (0, 90, 180, 270 grados)
        for (int rotation = 0; rotation < 4; rotation++) {
            // Rota el patrón base
            int rotatedBase = RotateConnections(baseConnections, rotation);

            // Cuenta cuántos bits coinciden con el patrón objetivo
            int score = CountMatchingBits(rotatedBase, targetConnections);

            // Actualiza la mejor rotación si encontramos una mejor coincidencia
            if (score > bestScore) {
                bestScore = score;
                bestRotation = rotation;
            }
        }

        return bestRotation;
    }

    /// <summary>
    /// Rota las conexiones un número específico de veces 90 grados en sentido horario.
    /// Las conexiones se representan como bits: Norte=1, Este=2, Sur=4, Oeste=8
    /// </summary>
    private static int RotateConnections(int connections, int rotations) {
        // Normaliza las rotaciones a 0-3
        rotations = rotations & 3;

        // Si no hay rotación, devuelve el original
        if (rotations == 0) return connections;

        // Rota los bits el número especificado de veces
        // Cada rotación: Norte->Este->Sur->Oeste->Norte (1->2->4->8->1)
        int result = connections;
        for (int i = 0; i < rotations; i++) {
            // Mueve todos los bits una posición y maneja el bit que se sale del patrón
            result = ((result << 1) | (result >> 3)) & 0xF;
        }

        return result;
    }

    /// <summary>
    /// Cuenta cuántos bits coinciden entre dos patrones de conexiones
    /// </summary>
    private static int CountMatchingBits(int pattern1, int pattern2) {
        int matching = pattern1 & pattern2;
        return BitOperations.PopCount((uint)matching);
    }

    private static Array2D<char> ParsePattern(List<string> lines) {
        int height = lines.Count;
        int width = lines[0].Length;
        var pattern = new Array2D<char>(width, height);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                pattern[y, x] = lines[y][x];
            }
        }

        return pattern;
    }
}