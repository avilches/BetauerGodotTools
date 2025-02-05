using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;

namespace Betauer.Core.PCG.Maze;

public static class TemplateSelector {
    /// <summary>
    /// Creates a template selector function from a TemplateSet.
    /// </summary>
    public static Func<MazeNode, Array2D<char>> Create(TemplateSet templateSet, string[]? tags = null, Random? random = null) {
        random ??= new Random();

        return (node) => {
            var nodeRequiredFlags = node.GetAttribute("tags");

            var directionFlags = GetDirectionFlags(node);
            tags = MergeTags(tags, nodeRequiredFlags);
            var templates = templateSet.FindTemplates(directionFlags, tags);
            if (templates.Count == 0) {
                throw new ArgumentException($"No templates found for node flags {DirectionFlagTools.FlagsToString(directionFlags)} with required tags {string.Join(", ", tags)}");
            }

            return templates.Select(t=>t.Body).ToArray()[random.Next(templates.Count)];
        };
    }

    public static int GetDirectionFlags(MazeNode node) {
        var directions = 0;
        if (node.Up != null) directions |= (int)DirectionFlag.Up;
        if (node.UpRight != null) directions |= (int)DirectionFlag.UpRight;
        if (node.Right != null) directions |= (int)DirectionFlag.Right;
        if (node.DownRight != null) directions |= (int)DirectionFlag.DownRight;
        if (node.Down != null) directions |= (int)DirectionFlag.Down;
        if (node.DownLeft != null) directions |= (int)DirectionFlag.DownLeft;
        if (node.Left != null) directions |= (int)DirectionFlag.Left;
        if (node.UpLeft != null) directions |= (int)DirectionFlag.UpLeft;
        return directions;
    }

    private static string[] MergeTags(object? tags1, object? tags2) {
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
        var array1 = ConvertToStringArray(tags1);
        var array2 = ConvertToStringArray(tags2);

        // If both are empty, return empty array
        if (array1.Length == 0 && array2.Length == 0) return Array.Empty<string>();

        // If one is empty, return the other
        if (array1.Length == 0) return array2;
        if (array2.Length == 0) return array1;

        // Merge both arrays and remove duplicates
        return array1.Concat(array2).Distinct().ToArray();
    }
}