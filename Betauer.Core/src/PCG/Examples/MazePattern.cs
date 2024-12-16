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
            var pattern = patterns.GetPattern(node, flags);

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
    public byte Value { get; }
    public Array2D<char> Pattern { get; private set; }
    public PatternId? FromPattern { get; set; }
    public HashSet<string> Flags { get; }

    public PatternDefinition(byte value, IEnumerable<string> flags) {
        Value = value;
        Flags = new HashSet<string>(flags);
    }

    public void SetPattern(Array2D<char> pattern) {
        Pattern = pattern;
    }

    public bool HasAnyFlag(string[] requiredFlags) {
        return requiredFlags.Length == 0 || requiredFlags.Any(flag => Flags.Contains(flag));
    }
}

public class MazePattern {
    private readonly Dictionary<byte, List<PatternDefinition>> _patterns;
    private readonly Random _random;

    public int CellSize { get; }

    public MazePattern(int cellSize, Random? random = null) {
        CellSize = cellSize;
        _random = random ?? new Random();
        _patterns = new Dictionary<byte, List<PatternDefinition>>();
    }

    public void LoadPatterns(string patternsFile) {
        var newPatterns = MazePatternLoader.LoadFromFile(patternsFile);
        foreach (var pattern in newPatterns) {
            var value = pattern.Key; // Ahora pattern.Key es directamente el byte
            if (!_patterns.ContainsKey(value)) {
                _patterns[value] = new List<PatternDefinition>();
            }
            _patterns[value].AddRange(pattern.Value);
        }
    }

    public Array2D<char> GetPattern(MazeNode node) {
        return FindPattern(PatternId.FromNode(node));
    }

    public Array2D<char> GetPattern(MazeNode node, params string[] requiredFlags) {
        return FindPattern(PatternId.FromNode(node), requiredFlags);
    }

    // Encuentra todos los patrones que coincidan con el valor y los flags
    public Array2D<char> FindPattern(byte value, params string[] requiredFlags) {
        var matchingDefinitions = GetMatchingDefinitions(value, requiredFlags);
        return matchingDefinitions[_random.Next(matchingDefinitions.Count)].Pattern;
    }

    public Array2D<char> GetPattern(byte value, params string[] requiredFlags) {
        var matchingDefinitions = GetMatchingDefinitions(value, requiredFlags);
        if (matchingDefinitions.Count > 1) {
            throw new ArgumentException($"Multiple patterns found for value {value} with flags: {string.Join(", ", requiredFlags)}");
        }
        return matchingDefinitions[0].Pattern;
    }

    private List<PatternDefinition> GetMatchingDefinitions(byte value, string[] requiredFlags) {
        if (!_patterns.TryGetValue(value, out var definitions)) {
            var directions = GetDirectionsDescription(value);
            throw new ArgumentException(
                $"No pattern found for value {value} " +
                $"(bits: {Convert.ToString(value, 2).PadLeft(8, '0')}, " +
                $"connections: {directions})");
        }

        var matchingDefinitions = requiredFlags.Length == 0
            ? definitions
            : definitions.Where(d => d.HasAnyFlag(requiredFlags)).ToList();

        if (!matchingDefinitions.Any()) {
            throw new ArgumentException($"No pattern found for value {value} with specified flags: {string.Join(", ", requiredFlags)}");
        }

        return matchingDefinitions;
    }

    private static string GetDirectionsDescription(byte value) {
        var directions = new List<string>();
        if ((value & (byte)PatternDirection.North) != 0) directions.Add("North");
        if ((value & (byte)PatternDirection.East) != 0) directions.Add("East");
        if ((value & (byte)PatternDirection.South) != 0) directions.Add("South");
        if ((value & (byte)PatternDirection.West) != 0) directions.Add("West");
        return directions.Any() ? string.Join("+", directions) : "None";
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
        var baseString = Value.ToString();
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
            Console.WriteLine("Processing pattern " + current.Value+ " with flags: " + string.Join(", ", current.Flags));

            // Primero comprobamos si ya existe un patrón con el mismo value y flags
            if (patterns.TryGetValue(current.Value, out var existingPatterns)) {
                var duplicatePattern = existingPatterns.FirstOrDefault(p =>
                    p.Flags.SetEquals(current.Flags));

                if (duplicatePattern != null) {
                    throw new ArgumentException(
                        $"Duplicate pattern found for value {current.Value} " +
                        $"with flags: {string.Join(", ", current.Flags)}");
                }
            } else {
                patterns[current.Value] = new List<PatternDefinition>();
            }

            if (current.FromPattern != null) {
                var baseArray2D = GetPatternFromDictionary(patterns, current.FromPattern.Value, current.FromPattern.Flags.ToArray());
                current.SetPattern(TransformPattern(baseArray2D, current.FromPattern.Value, current.Value));
            } else if (currentPattern.Count > 0) {
                current.SetPattern(ParsePattern(currentPattern));
            }

            patterns[current.Value].Add(current);
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
                current = new PatternDefinition(id.Value, id.Flags);

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

    private static Array2D<char> GetPatternFromDictionary(Dictionary<byte, List<PatternDefinition>> patterns, byte value, string[] flags) {
        if (!patterns.TryGetValue(value, out var definitions)) {
            throw new ArgumentException($"Base pattern with value {value} not found");
        }

        var matchingDefinitions = definitions.Where(d => d.HasAnyFlag(flags)).ToList();

        if (!matchingDefinitions.Any()) {
            throw new ArgumentException($"No base pattern found with value {value} and flags: {string.Join(", ", flags)}");
        }

        if (matchingDefinitions.Count > 1) {
            throw new ArgumentException($"Multiple base patterns found with value {value} and flags: {string.Join(", ", flags)}. Base pattern must be unique.");
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