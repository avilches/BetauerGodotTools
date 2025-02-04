using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;

namespace Betauer.Core.PCG.Maze;

public class TemplateSelectorOptions {
    public string[]? RequiredFlags { get; init; }
    public string[]? OptionalFlags { get; init; }
    public Random? Random { get; init; }
}

public static class TemplateSelector {
    /// <summary>
    /// Creates a template selector function from a TemplateSet.
    /// </summary>
    public static Func<MazeNode, Array2D<char>> Create(TemplateSet templateSet, TemplateSelectorOptions? options = null) {
        var random = options?.Random ?? new Random();

        return (node) => {
            var nodeRequiredFlags = node.GetAttribute("requiredFlags");
            var nodeOptionalFlags = node.GetAttribute("optionalFlags");

            var type = GetNodeType(node);
            var requiredFlags = MergeFlags(options?.RequiredFlags, nodeRequiredFlags);
            var optionalFlags = MergeFlags(options?.OptionalFlags, nodeOptionalFlags);
            var templates = templateSet.FindTemplates(type, requiredFlags, optionalFlags);

            return templates[random.Next(templates.Count)];
        };
    }

    public static int GetNodeType(MazeNode node) {
        var directions = 0;
        if (node.Up != null) directions |= (int)DirectionFlags.Up;
        if (node.UpRight != null) directions |= (int)DirectionFlags.UpRight;
        if (node.Right != null) directions |= (int)DirectionFlags.Right;
        if (node.DownRight != null) directions |= (int)DirectionFlags.DownRight;
        if (node.Down != null) directions |= (int)DirectionFlags.Down;
        if (node.DownLeft != null) directions |= (int)DirectionFlags.DownLeft;
        if (node.Left != null) directions |= (int)DirectionFlags.Left;
        if (node.UpLeft != null) directions |= (int)DirectionFlags.UpLeft;
        return directions;
    }

    private static string[] MergeFlags(object? flags1, object? flags2) {
        // Helper method to convert an object to string[]
        string[] ConvertToStringArray(object? obj) {
            // if (obj == null) return Array.Empty<string>();
            return obj switch {
                string[] array => array,
                IList<string> list => list.ToArray(),
                IEnumerable<string> enumerable => enumerable.ToArray(),
                string str when !string.IsNullOrEmpty(str) => str.Split(',').Select(s => s.Trim()).ToArray(),
                _ => Array.Empty<string>()
            };
        }

        // Convert both objects to string arrays
        var array1 = ConvertToStringArray(flags1);
        var array2 = ConvertToStringArray(flags2);

        // If both are empty, return empty array
        if (array1.Length == 0 && array2.Length == 0) return Array.Empty<string>();

        // If one is empty, return the other
        if (array1.Length == 0) return array2;
        if (array2.Length == 0) return array1;

        // Merge both arrays and remove duplicates
        return array1.Concat(array2).Distinct().ToArray();
    }
}