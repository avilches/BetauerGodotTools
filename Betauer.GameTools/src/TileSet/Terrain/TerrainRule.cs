using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Godot;

namespace Betauer.TileSet.Terrain;

public partial class TerrainRule {

    public abstract class NeighbourRule {
        public abstract bool Check(short terrainId);
        
        public static NeighbourRule ParseRule(string neighbourRule) {
            switch (neighbourRule) {
                case "?": return NeighbourRuleIgnore.Instance;
                case ".": return NeighbourRuleEquals.EqualsEmptyInstance;
                case "X": return NeighbourRuleNotEquals.NotEqualsEmptyInstance;
            }
            if (neighbourRule.StartsWith("!")) {
                var terrainId = neighbourRule[1..];
                if (!terrainId.IsValidInt()) {
                    throw new Exception($"Rule not recognized: {neighbourRule}");
                }
                return new NeighbourRuleNotEquals(byte.Parse(terrainId));
            }
            if (!neighbourRule.IsValidInt()) {
                throw new Exception($"Rule not recognized: {neighbourRule}");
            }
            return new NeighbourRuleEquals(byte.Parse(neighbourRule));
        }
    }

    public class NeighbourRuleIgnore : NeighbourRule {

        public static NeighbourRuleIgnore Instance { get; } = new NeighbourRuleIgnore();

        private NeighbourRuleIgnore() {
        }   
        
        public override bool Check(short terrainId) => true;
    }

    public class NeighbourRuleEquals : NeighbourRule {

        public static NeighbourRuleEquals EqualsEmptyInstance { get; } = new NeighbourRuleEquals(-1);
        
        public short TerrainId { get; } = -1; // -1 == empty

        public NeighbourRuleEquals(short terrainId) {
            TerrainId = terrainId;
        }

        public override bool Check(short terrainId) => TerrainId == terrainId;
    }
    
    public class NeighbourRuleNotEquals : NeighbourRule {

        public static NeighbourRuleNotEquals NotEqualsEmptyInstance { get; } = new NeighbourRuleNotEquals(-1);

        public short TerrainId { get; } = -1; // -1 == empty

        public NeighbourRuleNotEquals(short terrainId) {
            TerrainId = terrainId;
        }

        public override bool Check(short terrainId) => TerrainId != terrainId;
    }
    
    public NeighbourRule[,] Rules;

    public TerrainRule(int size) {
        // 3 means 3x3, 5 means 5x5
        if (size is 1 or 3 or 5 or 7 or 9) {
            Rules = new NeighbourRule[size, size];
        } else {
            throw new Exception($"Size {size}x{size} is not valid: only sizes 1x1, 3x3, 5x5, 7x7 and 9x9 are supported");
        }
            
    }

    public static TerrainRule Parse(string value) {
        var lines = value.Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToArray();

        if (lines.Length is 1) {
            var tileMap = new TerrainRule((byte)lines.Length);
            var parts = SplitWords().Split(lines[0].Trim());
            if (parts.Length != 1) {
                throw new Exception($"Line 1 must have only 1 part, but it has {parts.Length}: {lines[0]}");
            }
            tileMap.Rules[0, 0] = NeighbourRule.ParseRule(lines[0].Trim());
            return tileMap;
        }
        if (lines.Length is 3 or 5 or 7 or 9) {
            var y = 0;
            var tileMap = new TerrainRule((byte)lines.Length);
            foreach (var line in lines) {
                var parts = SplitWords().Split(line);
                var x = 0;
                foreach (var neighbourRule in parts) {
                    tileMap.Rules[y, x] = NeighbourRule.ParseRule(neighbourRule);
                    x++;
                }
                y++;
            }

            return tileMap;
        }
        throw new Exception($"Size {lines.Length}x{lines.Length} is not valid: only sizes 1x1, 3x3, 5x5, 7x7 and 9x9 are supported");
    }

    public string Export() {
        var size = Rules.GetLength(0);
        var sb = new StringBuilder(size * 3 * size - 1);
        for (var y = 0; y < size; y++) {
            for (var x = 0; x < size; x++) {
                var neighbourRule = Rules[y, x];
                if (neighbourRule is NeighbourRuleEquals neighbourRuleEquals) {
                    if (neighbourRuleEquals.TerrainId == -1) {
                        sb.Append(" .");
                    } else {
                        sb.Append(' ').Append(neighbourRuleEquals.TerrainId);
                    }
                } else if (neighbourRule is NeighbourRuleNotEquals neighbourRuleNotEquals) {
                    if (neighbourRuleNotEquals.TerrainId == -1) {
                        sb.Append(" X");
                    } else {
                        sb.Append('!').Append(neighbourRuleNotEquals.TerrainId);
                    }
                } else if (neighbourRule is NeighbourRuleIgnore) {
                    sb.Append(" ?");
                } else {
                    throw new Exception($"Unknown neighbour rule: {neighbourRule}");
                }
                if (x < size - 1) sb.Append(' ');
            }
            if (y < size - 1) sb.Append('\n');
        }
        return sb.ToString();
    }


    [GeneratedRegex("\\s+")]
    private static partial Regex SplitWords();

    public bool Check(int[,] cells) {
        var size = Rules.GetLength(0);
        if (cells.GetLength(0) != size || cells.GetLength(1) != size) {
            throw new Exception($"Cells to check array must be {size}x{size}, but it is {cells.GetLength(0)}x{cells.GetLength(1)}");
        }
        for (var y = 0; y < size; y++) {
            for (var x = 0; x < size; x++) {
                if (!Rules[y, x].Check((short)cells[y, x])) return false;
            }
        }
        return true;
    }
}