using System;
using System.Collections.Generic;
using System.Linq;

namespace Veronenger.Game.Dungeon.GenCity;

public static class CityValidationExtensions {

    public static void ValidateIntersections(this City city, bool checkFlattenIntersections = false) {
        var errors = new List<string>();
        foreach (var intersection in city.Intersections) {
            if (checkFlattenIntersections && intersection.CanBeFlatten(out var path1, out var path2)) {
                errors.Add($"Intersection {intersection.Id} at {intersection.Position} can be flattened: {path1.Id} and {path2.Id}");
            }
            if (city.Data[intersection.Position] != intersection) {
                errors.Add($"Wrong intersection position: {intersection.Position}");
            }
        }
        foreach (var (pos, intersection) in city.Data.GetIndexedValues<Intersection>()) {
            if (!city.Intersections.Contains(intersection)) {
                errors.Add($"Intersection in {pos} doesn't belong to the city");
            }
            if (intersection.Position != pos) {
                errors.Add($"Wrong intersection position in {pos}");
            }
        }
        if (errors.Count > 0) {
            throw new Exception("City intersections are invalid:\n" + string.Join("\n", errors));
        }
    }

    public static void ValidateIntersectionPaths(this City city) {
        var output = city.Intersections.SelectMany(intersection => intersection.GetOutputPaths().Where(p => p.IsCompleted())).ToHashSet();
        var input = city.Intersections.SelectMany(intersection => intersection.GetInputPaths()).ToHashSet();
        if (!output.SetEquals(input)) {
            var outputOnly = output.Except(input).ToList();
            var inputOnly = input.Except(output).ToList();
            throw new Exception($"Output paths and input paths do not contain the same elements. " +
                                $"Only in output: {outputOnly.Count}, Only in input: {inputOnly.Count}");
        }
    }

    public static void ValidatePaths(this City city) {
        var errors = new List<string>();
        var output = city.Intersections.SelectMany(intersection => intersection.GetOutputPaths()).ToHashSet();
        foreach (var path in output) {
            var start = path.Start.Position;
            var end = path.End?.Position ?? path.GetCursor();

            foreach (var pos in path.GetAllPositions()) {
                var tile = city.Data[pos];
                if (pos == start) {
                    if (tile != path.Start) {
                        errors.Add($"Wrong start position {start} for path {path}. Expected {path.Start.Id} but got {tile}");
                    }
                } else if (pos == end) {
                    if (path.End == null) {
                        if (tile != path) {
                            errors.Add($"Wrong end position (cursor) for path {path} at {pos}. Expected {path} but got {tile}");
                        }
                    } else {
                        if (tile != path.End) {
                            errors.Add($"Wrong end position for path {path} at {pos}. Expected {path.End} but got {tile}");
                        }
                    }
                } else {
                    if (tile != path) {
                        errors.Add($"Expected at {pos}. Expected {path} but got {tile}");
                    }
                }
            }
        }
        foreach (var (pos, tile) in city.Data.GetIndexedValues()) {
            if (tile is not Path path) continue;
            if (!output.Contains(path)) {
                errors.Add($"Path in {pos} doesn't belong to the city");
            }
        }
        if (errors.Count > 0) {
            throw new Exception("Invalid paths:\n" + string.Join("\n", errors));
        }
    }
}