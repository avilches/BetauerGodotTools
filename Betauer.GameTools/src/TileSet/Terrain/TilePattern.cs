using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Betauer.TileSet.Terrain;

public abstract partial class TilePattern {
    private static readonly Dictionary<string, Func<int, bool>> IntRules = new Dictionary<string, Func<int, bool>>() {
        { "?", i => true },
        { "*", i => true },
        { ".", i => true },

        { "-9", i => i == -9 },
        { "-8", i => i == -8 },
        { "-7", i => i == -7 },
        { "-6", i => i == -6 },
        { "-5", i => i == -5 },
        { "-4", i => i == -4 },
        { "-3", i => i == -3 },
        { "-2", i => i == -2 },
        { "-1", i => i == -1 },
        { "-0", i => i == 0 },
        { "0", i => i == 0 },
        { "1", i => i == 1 },
        { "2", i => i == 2 },
        { "3", i => i == 3 },
        { "4", i => i == 4 },
        { "5", i => i == 5 },
        { "6", i => i == 6 },
        { "7", i => i == 7 },
        { "8", i => i == 8 },
        { "9", i => i == 9 },

        { "-9!", i => i == -9 },
        { "-8!", i => i == -8 },
        { "-7!", i => i == -7 },
        { "-6!", i => i == -6 },
        { "-5!", i => i == -5 },
        { "-4!", i => i == -4 },
        { "-3!", i => i == -3 },
        { "-2!", i => i == -2 },
        { "-1!", i => i == -1 },
        { "-0!", i => i == 0 },
        { "0!", i => i == 0 },
        { "1!", i => i == 1 },
        { "2!", i => i == 2 },
        { "3!", i => i == 3 },
        { "4!", i => i == 4 },
        { "5!", i => i == 5 },
        { "6!", i => i == 6 },
        { "7!", i => i == 7 },
        { "8!", i => i == 8 },
        { "9!", i => i == 9 },

        { "!-9", i => i != -9 },
        { "!-8", i => i != -8 },
        { "!-7", i => i != -7 },
        { "!-6", i => i != -6 },
        { "!-5", i => i != -5 },
        { "!-4", i => i != -4 },
        { "!-3", i => i != -3 },
        { "!-2", i => i != -2 },
        { "!-1", i => i != -1 },
        { "!-0", i => i != 0 },
        { "!0", i => i != 0 },
        { "!1", i => i != 1 },
        { "!2", i => i != 2 },
        { "!3", i => i != 3 },
        { "!4", i => i != 4 },
        { "!5", i => i != 5 },
        { "!6", i => i != 6 },
        { "!7", i => i != 7 },
        { "!8", i => i != 8 },
        { "!9", i => i != 9 },
    };

    /// <summary>
    /// ? = ignore (no rule in this position)
    /// . = ignore (no rule in this position)
    /// * = ignore (no rule in this position)
    /// Accepts one single digit numbers from -9 to 9
    /// To negate, use ! after or before the number, like !1 or 1! to match anything but 1. Can be used with negative numbers too, like !-1 or -1!
    /// </summary>
    /// <param name="value"></param>
    /// <param name="rules"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static TilePattern<int> Parse(string value) => Parse(value, IntRules);

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
    /// <param name="rules"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static TilePattern<T> Parse<T>(string value, Dictionary<string, Func<T, bool>> rules) {
        var lines = value.Split('\n')
            .Select(v => v.Trim())
            .Where(v => v.Length > 0)
            .ToArray();

        var gridSize = lines.Length;
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        var y = 0;
        var ruleGrid = new (string, Func<T, bool>)[gridSize, gridSize];
        foreach (var line in lines) {
            var parts = SplitWords().Split(line);
            if (parts.Length != gridSize) {
                throw new Exception($"Line has to be {gridSize} parts instead of {parts.Length}: {line}");
            }
            var x = 0;
            foreach (var part in parts) {
                ruleGrid[y, x] = (part, rules[part]);
                x++;
            }
            y++;
        }
        var tilePattern = new TilePattern<T>(rules, ruleGrid);
        return tilePattern;
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex SplitWords();
}

public class TilePattern<T> : TilePattern {
    public Dictionary<string, Func<T, bool>> Rules { get; }
    public (string, Func<T, bool>)[,] RuleGrid { get; }

    public TilePattern(Dictionary<string, Func<T, bool>> rules, (string, Func<T, bool>)[,] ruleGrid) {
        Rules = rules;
        RuleGrid = ruleGrid;
    }

    /// <summary>
    /// Data must be a grid of the same size as the pattern and indexed by [y,x] (you can use DataGrid<TT>.Data
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool Matches(T[,] data) {
        var gridSize = RuleGrid.GetLength(0);
        if (data.GetLength(0) != gridSize || data.GetLength(1) != gridSize) {
            throw new Exception($"Data size {data.GetLength(0)}x{data.GetLength(1)} doesn't match pattern size {gridSize}x{gridSize}");
        }
        for (var y = 0; y < gridSize; y++) {
            for (var x = 0; x < gridSize; x++) {
                var value = data[y, x];
                var rule = RuleGrid[y, x].Item2;
                if (!rule.Invoke(value)) return false;
            }
        }
        return true;
    }


    public string Export() {
        var gridSize = RuleGrid.GetLength(0);
        var sb = new StringBuilder(gridSize * 3 * gridSize - 1);
        var maxRuleNameLength = Rules.Keys.Max(k => k.Length);
        for (var y = 0; y < gridSize; y++) {
            for (var x = 0; x < gridSize; x++) {
                sb.Append(RuleGrid[y, x].Item1.PadRight(maxRuleNameLength));
                if (x < gridSize - 1) sb.Append(' ');
            }
            if (y < gridSize - 1) sb.Append('\n');
        }
        return sb.ToString();
    }
}