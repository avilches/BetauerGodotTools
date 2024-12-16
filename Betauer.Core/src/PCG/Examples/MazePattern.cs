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
            size.X * cellSize,
            size.Y * cellSize,
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
        for (int y = 0; y < pattern.Height; y++) {
            for (int x = 0; x < pattern.Width; x++) {
                target[position.Y + y, position.X + x] = pattern[y, x];
            }
        }
    }
}

public class MazePattern {
    private readonly Dictionary<PatternId, PatternDefinition> _patterns;
    private readonly Random _random;

    public int CellSize { get; }

    public MazePattern(int cellSize, Random? random = null) {
        CellSize = cellSize;
        _random = random ?? new Random();
        _patterns = new Dictionary<PatternId, PatternDefinition>();
    }

    public void LoadPatterns(string patternsFile) {
        var newPatterns = MazePatternLoader.LoadFromFile(patternsFile);
        foreach (var pattern in newPatterns) {
            _patterns[pattern.Key] = pattern.Value;
        }
    }

    public Array2D<char> GetPattern(MazeNode node, params string[] requiredFlags) {
        var baseId = PatternId.FromNode(node);
        var searchId = new PatternId(baseId.Value, requiredFlags);

        if (!_patterns.TryGetValue(searchId, out var definition)) {
            throw new ArgumentException($"No pattern found for {searchId}");
        }

        if (!definition.Patterns.Any()) {
            throw new ArgumentException($"Pattern {searchId} has no variants");
        }

        return definition.Patterns[_random.Next(definition.Patterns.Count)];
    }
}

[Flags]
public enum PatternDirection {
    None = 0,
    North = 1 << 0, // 00000001
    East = 1 << 2,  // 00000100
    South = 1 << 4, // 00010000
    West = 1 << 6,  // 10000000

    // Máscara útil
    All = North | East | South | West // 1111
}

public class PatternId {
    public readonly byte Value;
    public readonly HashSet<string> Flags;

    internal PatternId(byte value, IEnumerable<string> flags) {
        Value = (byte)(value & 0x0F); // Mantenemos solo los 4 bits menos significativos
        Flags = new HashSet<string>(flags);
    }

    public static PatternId Parse(string idString) {
        var parts = idString.Split('/');
        if (parts.Length < 1) {
            throw new ArgumentException("Invalid pattern ID format. Expected 'number/flag1/flag2...'");
        }

        if (!byte.TryParse(parts[0], out byte value)) {
            throw new ArgumentException($"Invalid numeric value in pattern ID: {parts[0]}");
        }

        var flags = parts.Length > 1 ? parts[1..] : Array.Empty<string>();
        return new PatternId(value, flags);
    }

    public bool HasDirection(PatternDirection direction) {
        return (Value & (byte)direction) != 0;
    }

    public int ConnectionCount() {
        return BitOperations.PopCount(Value);
    }

    public bool IsCompatibleWith(PatternId other) {
        return ConnectionCount() == other.ConnectionCount();
    }

    public static PatternId FromNode(MazeNode node) {
        PatternDirection directions = PatternDirection.None;

        if (node.Up != null) directions |= PatternDirection.North;
        if (node.Right != null) directions |= PatternDirection.East;
        if (node.Down != null) directions |= PatternDirection.South;
        if (node.Left != null) directions |= PatternDirection.West;

        return new PatternId((byte)directions, Array.Empty<string>());
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

public class PatternVariant {
    public HashSet<string> Flags { get; } = new();
    public Array2D<char> Pattern { get; set; }

    public PatternVariant(Array2D<char> pattern, IEnumerable<string>? flags = null) {
        Pattern = pattern;
        if (flags != null) {
            Flags.UnionWith(flags);
        }
    }
}

public class PatternDefinition {
    public PatternId Id { get; }
    public List<Array2D<char>> Patterns { get; } = new();
    public PatternId? FromPattern { get; set; }

    public PatternDefinition(PatternId id) {
        Id = id;
    }

    public void AddPattern(Array2D<char> pattern) {
        Patterns.Add(pattern);
    }
}

public class MazePatternLoader {
    private const string IdPrefix = "@ID=";
    private const string FromPrefix = "@FROM=";

    public static Dictionary<PatternId, PatternDefinition> LoadFromFile(string filePath) {
        var patterns = new Dictionary<PatternId, PatternDefinition>();
        var lines = File.ReadAllLines(filePath);
        var currentPattern = new List<string>();
        PatternDefinition? current = null;

        void ProcessCurrentPattern() {
            if (current == null) return;

            if (current.FromPattern != null) {
                // Este patrón se basa en otro
                if (!patterns.TryGetValue(current.FromPattern, out var basePattern)) {
                    throw new Exception($"Base pattern not found: {current.FromPattern}");
                }

                // Verificar compatibilidad
                if (!current.Id.IsCompatibleWith(basePattern.Id)) {
                    throw new Exception($"Incompatible patterns: {current.Id} cannot inherit from {basePattern.Id} " +
                                        $"(different number of connections)");
                }

                // El patrón base debe tener al menos un patrón definido
                if (!basePattern.Patterns.Any()) {
                    throw new Exception($"Base pattern has no patterns defined: {basePattern.Id}");
                }

                foreach (var baseVariantPattern in basePattern.Patterns) {
                    var transformedPattern = TransformPattern(baseVariantPattern,
                        basePattern.Id.Value, current.Id.Value);
                    current.AddPattern(transformedPattern);
                }
            } else if (currentPattern.Count > 0) {
                // Patrón definido directamente
                var pattern = ParsePattern(currentPattern);
                current.AddPattern(pattern);
            }

            patterns[current.Id] = current;
            currentPattern.Clear();
            current = null;
        }

        foreach (var line in lines) {
            var trimmed = line.Trim();
            Console.WriteLine(trimmed);

            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) {
                if (current != null && currentPattern.Count > 0) {
                    var pattern = ParsePattern(currentPattern);
                    current.AddPattern(pattern);
                    currentPattern.Clear();
                }
                continue;
            }

            if (trimmed.StartsWith(IdPrefix)) {
                if (current != null) ProcessCurrentPattern();
                var idStr = trimmed[IdPrefix.Length..];
                var id = PatternId.Parse(idStr);
                current = new PatternDefinition(id);
            } else if (trimmed.StartsWith(FromPrefix)) {
                if (current == null) throw new Exception("From without pattern ID");
                var fromStr = trimmed[FromPrefix.Length..];
                current.FromPattern = PatternId.Parse(fromStr);
            } else {
                currentPattern.Add(trimmed);
            }
        }

        if (current != null) ProcessCurrentPattern();
        return patterns;
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