using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Betauer.TileSet.Terrain;

public partial class TilePattern {
    public int GridSize { get; init;  }

    public const string NotEqualsToPrefix = "!";
    public const string Ignore = "?";


    /// <summary>
    /// ? = ignore (no rule in this position)
    /// 0 = equals to 0
    /// 1 = equals to 1
    /// 2 = equals to 2
    /// ...
    /// !1 = not equals to 1
    /// !2 = not equals to 2
    /// ...
    /// </summary>
    /// <param name="value"></param>
    /// <param name="extraRules"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static TilePattern Parse(string value, Dictionary<string, NeighborRule>? extraRules = null) {
        var lines = value.Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToArray();

        var gridSize = lines.Length;
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        var y = 0;
        var rules = new List<NeighborRulePos>();
        foreach (var line in lines) {
            var parts = SplitWords().Split(line);
            if (parts.Length != gridSize) {
                throw new Exception($"Line has to be {gridSize} parts instead of {parts.Length}: {line}");
            }
            var x = 0;
            foreach (var neighbourRule in parts) {
                if (extraRules != null && extraRules.TryGetValue(neighbourRule, out var extraRule)) {
                    rules.Add(new NeighborRulePos(extraRule, x, y));
                } else {
                    var tileRule = NeighborRule.ParseRule(neighbourRule);
                    if (tileRule.HasValue) {
                        rules.Add(new NeighborRulePos(tileRule.Value, x, y));
                    }
                }
                x++;
            }
            y++;
        }
        var tilePattern = new TilePattern(gridSize, rules.ToArray());
        return tilePattern;
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex SplitWords();

    public readonly NeighborRulePos[] Rules;

    internal TilePattern(int gridSize, NeighborRulePos[] rules) {
        // 3 means 3x3, 5 means 5x5
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        GridSize = gridSize;
        Rules = rules;
    }
    

    public NeighborRulePos? FindRuleAt(int x, int y) {
        foreach (var rule in Rules) {
            if (rule.X == x && rule.Y == y) return rule;
        }
        return null;
    }

    public string Export() {
        var sb = new StringBuilder(GridSize * 3 * GridSize - 1);
        for (var y = 0; y < GridSize; y++) {
            for (var x = 0; x < GridSize; x++) {
                var tileRule = FindRuleAt(x, y);
                if (tileRule == null) {
                    sb.Append(' ').Append(Ignore);
                    continue;
                }
                if (tileRule.EqualsTo) {
                    sb.Append(' ').Append(tileRule.Value);
                } else {
                    sb.Append(NotEqualsToPrefix).Append(tileRule.Value);
                }
                if (x < GridSize - 1) sb.Append(' ');
            }
            if (y < GridSize - 1) sb.Append('\n');
        }
        return sb.ToString();
    }

    public bool Matches(int[,] data) {
        if (data.GetLength(0) != GridSize || data.GetLength(1) != GridSize) {
            throw new Exception($"Data size {data.GetLength(0)}x{data.GetLength(1)} doesn't match pattern size {GridSize}x{GridSize}");
        }
        foreach (var rule in Rules) {
            var value = data[rule.X, rule.Y];
            var matches = rule.EqualsTo ? value == rule.Value : value != rule.Value;
            if (!matches) return false; // All rules must match
        }
        return true;
    }
}