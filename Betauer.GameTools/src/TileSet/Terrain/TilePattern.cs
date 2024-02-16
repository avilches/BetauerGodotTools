using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Betauer.TileSet.Terrain;

public enum ConditionType {
    // Values are sorted by priority. Higher value will be evaluated first.
    // The theory behind is, in a random distribution or tiles in the map, "equals" comparision
    // has a higher chance to be false than "not equals". So, the sooner it fails, the better because it can be discarded.
    EqualsTo = 5,
    TemplateEqualsTo = 4,
    NotEqualsTo = 3,
    TemplateNotEqualsTo = 2,
    UserDefined = 1,
    UserDefinedPosition = 0,
}

public abstract partial class TilePattern {
    public int GridSize { get; init;  }

    public bool Matches(int[,] data, int centerX, int centerY, int? templateTerrain = null) {
        return Matches((x, y) => x < 0 || y < 0 || x >= data.GetLength(0) || y >= data.GetLength(1) ? -1 : data[x, y], centerX, centerY, templateTerrain);
    }

    public abstract bool Matches(Func<int, int, int> loader, int centerX, int centerY, int? templateTerrain = null);
    
    public static FunctionTilePattern Create(int gridSize, Func<int[,], int, bool> function) => new FunctionTilePattern(gridSize, function);
    
    public static FunctionTilePattern Create(int gridSize, Func<int[,], bool> function) => new FunctionTilePattern(gridSize, function);


    /// <summary>
    /// ? = ignore (no rule in this position)
    /// . = empty (terrain = -1)
    /// X = not empty (terrain != -1)
    /// 1 = equals to 1
    /// 2 = equals to 2
    /// ...
    /// !1 = not equals to 1
    /// !2 = not equals to 2
    /// ...
    ///
    /// using WithTerrain(t)
    /// ! = not equals to t
    /// # = equals to t
    /// </summary>
    /// <param name="value"></param>
    /// <param name="extraRules"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static RulesTilePattern Parse(string value, Dictionary<string, NeighborRule>? extraRules = null) {
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
                    if (tileRule != null) {
                        rules.Add(new NeighborRulePos(tileRule, x, y));
                    }
                }
                x++;
            }
            y++;
        }
        rules.Sort((a, b) => ((int)b.NeighborRule.ConditionType).CompareTo((int)a.NeighborRule.ConditionType));
        var tilePattern = new RulesTilePattern(gridSize, rules.ToArray());
        return tilePattern;
    }

    [GeneratedRegex("\\s+")]
    private static partial Regex SplitWords();

}

public class FunctionTilePattern : TilePattern {
    public readonly Func<int[,], int, bool> Function;
    private int[,] _grid;

    internal FunctionTilePattern(int gridSize, Func<int[,], int, bool> function) {
        GridSize = gridSize;
        Function = function;
        _grid = new int[GridSize, GridSize];
    }

    internal FunctionTilePattern(int gridSize, Func<int[,], bool> function) {
        GridSize = gridSize;
        Function = (grid,_) => function(grid);
        _grid = new int[GridSize, GridSize];
    }


    public override bool Matches(Func<int, int, int> loader, int centerX, int centerY, int? templateTerrain = null) {
        var halfSize = GridSize / 2;
        for (var x = 0; x < GridSize; x++) {
            for (var y = 0; y < GridSize; y++) {
                var i = centerX + x - halfSize;
                var j = centerY + y - halfSize;
                _grid[x, y] = loader(i, j);
            }
        }
        return Function(_grid, templateTerrain ?? -1);
    }
}

public class RulesTilePattern : TilePattern {
    public const string NotEqualsToPrefix = "!";

    public const string Ignore = "?";
    public const string IsEmpty = ".";
    public const string IsNotEmpty = "X";
    public const string TemplateNotEqualsTo = "!";
    public const string TemplateEqualsTo = "#";

    public readonly NeighborRulePos[] Rules;

    public bool HasTemplateRules => Rules.Any(rule => rule.NeighborRule.IsTemplate);

    internal RulesTilePattern(int gridSize, NeighborRulePos[] rules) {
        // 3 means 3x3, 5 means 5x5
        if (gridSize % 2 == 0) {
            throw new Exception($"Size {gridSize}x{gridSize} is not valid: only odd sizes are allowed: 1x1, 3x3, 5x5...");
        }
        GridSize = gridSize;
        Rules = rules;
    }

    /// <summary>
    /// Creates a new TilePattern without templates, but with the same rules.
    /// If the TilePattern doesn't have any template rules, it returns the same TilePattern.
    /// </summary>
    /// <param name="terrainId"></param>
    /// <returns></returns>
    public RulesTilePattern WithTerrain(int terrainId) {
        if (!HasTemplateRules) return this;
        var rules = Rules.Select(rule => rule.NeighborRule.ConditionType switch {
            ConditionType.TemplateEqualsTo => new NeighborRulePos(NeighborRule.CreateEqualsTo(terrainId), rule.X, rule.Y),
            ConditionType.TemplateNotEqualsTo => new NeighborRulePos(NeighborRule.CreateNotEqualsTo(terrainId), rule.X, rule.Y),
            _ => rule
        }).ToArray();
        return new RulesTilePattern(GridSize, rules);
    }

    public NeighborRule? FindRuleAt(int x, int y) {
        foreach (var rule in Rules) {
            if (rule.X == x && rule.Y == y) return rule.NeighborRule;
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
                var conditionType = tileRule.ConditionType;
                var tileRuleExpectedTerrain = tileRule.ExpectedTerrain;
                if (conditionType == ConditionType.EqualsTo) {
                    if (tileRuleExpectedTerrain == -1) {
                        sb.Append(' ').Append(IsEmpty);
                    } else {
                        sb.Append(' ').Append(tileRuleExpectedTerrain);
                    }
                } else if (conditionType == ConditionType.NotEqualsTo) {
                    if (tileRuleExpectedTerrain == -1) {
                        sb.Append(' ').Append(IsNotEmpty);
                    } else {
                        sb.Append(NotEqualsToPrefix).Append(tileRuleExpectedTerrain);
                    }
                } else if (conditionType == ConditionType.TemplateEqualsTo) {
                    sb.Append(' ').Append(TemplateEqualsTo);
                } else if (conditionType == ConditionType.TemplateNotEqualsTo) {
                    sb.Append(' ').Append(TemplateNotEqualsTo);
                } else {
                    throw new Exception($"Unknown neighbour rule: {tileRule}");
                }
                if (x < GridSize - 1) sb.Append(' ');
            }
            if (y < GridSize - 1) sb.Append('\n');
        }
        return sb.ToString();
    }


    public override bool Matches(Func<int, int, int> loader, int centerX, int centerY, int? templateTerrain = null) {
        var center = GridSize / 2;
        foreach (var rule in Rules) {
            var x = centerX + rule.X - center;
            var y = centerY + rule.Y - center;
            var terrain = loader(x, y);
            if (rule.NeighborRule.ConditionType is ConditionType.TemplateEqualsTo or ConditionType.TemplateNotEqualsTo && !templateTerrain.HasValue) {
                throw new Exception("templateTerrain is needed to evaluate a "+Enum.GetName(rule.NeighborRule.ConditionType)+" condition");
            }
            var matches = rule.NeighborRule.ConditionType switch {
                ConditionType.EqualsTo => terrain == rule.NeighborRule.ExpectedTerrain,
                ConditionType.NotEqualsTo => terrain != rule.NeighborRule.ExpectedTerrain,
                ConditionType.TemplateEqualsTo => terrain == templateTerrain,
                ConditionType.TemplateNotEqualsTo => terrain != templateTerrain,
                ConditionType.UserDefined => rule.NeighborRule.UserCondition(terrain),
                ConditionType.UserDefinedPosition => rule.NeighborRule.UserConditionPosition(x, y),
                _ => throw new Exception($"ConditionType not implemented: {Enum.GetName(rule.NeighborRule.ConditionType)}")
            };
            if (!matches) return false; // All rules must match
        }
        return true;
    }
}