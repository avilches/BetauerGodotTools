using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze;

public static class DirectionSelectors {
    /// <summary>
    /// Returns a function that selects a random direction from the available ones (ignoring the last direction).
    /// </summary>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateRandom(Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) => available[rng.Next(available.Count)];
    }

    /// <summary>
    /// Returns a function that selects the last direction with the given bias: the bigger the bias (1), the more chances it will return the last direction.
    /// In other case, it will select a random direction.
    ///
    /// This method works if the Backtracker cell selector is used. Only the Backtracker cell selector will provide a consistent lastDir parameter.
    ///  
    /// </summary>
    /// <param name="directionalBias"></param>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateWindy(float directionalBias, Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) =>
            lastDir.HasValue && rng.NextSingle() <= directionalBias
                ? lastDir.Value
                : available[rng.Next(available.Count)];
    }

    /// <summary>
    /// Returns a function that selects a vertical direction (if any) with the given bias: the bigger the bias (1), the more vertical directions
    /// will be selected. If no vertical directions are available, it will select a random direction.
    /// </summary>
    /// <param name="horizontalBias"></param>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateHorizontalSelector(float horizontalBias, Random? rng = null) {
        rng ??= new Random();
        return (_, available) => {
            var horizontal = available.Where(d => d.Y == 0).ToList();
            return horizontal.Count > 0 && rng.NextSingle() <= horizontalBias ? horizontal[rng.Next(horizontal.Count)] : available[rng.Next(available.Count)];
        };
    }

    /// <summary>
    /// Returns a function that selects a vertical direction (if any) with a given bias: the bigger the bias (1), the more vertical directions
    /// will be selected. If no vertical directions are available, it will select a random direction.
    /// </summary>
    /// <param name="verticalBias"></param>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateVerticalSelector(float verticalBias, Random? rng = null) {
        rng ??= new Random();
        return (_, available) => {
            var vertical = available.Where(d => d.X == 0).ToList();
            return vertical.Count > 0 && rng.NextSingle() <= verticalBias ? vertical[rng.Next(vertical.Count)] : available[rng.Next(available.Count)];
        };
    }

    /// <summary>
    /// Returns a function that selects the next clockwise position (if it's possible) based on the last position and with a given bias: the bigger the bias (1),
    /// the more clockwise directions will be selected. If no vertical directions are available, it will select a random direction.
    ///
    /// This method works if the Backtracker cell selector is used. Only the Backtracker cell selector will provide a consistent lastDir parameter. 
    /// </summary>
    /// <param name="clockwiseBias"></param>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateClockwiseSelector(float clockwiseBias, Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) => {
            if (lastDir.HasValue && rng.NextSingle() <= clockwiseBias) {
                var clockwise = lastDir.Value.Clockwise();
                while (!available.Contains(clockwise)) {
                    clockwise = clockwise.CounterClockwise();
                }
                return clockwise;
            }
            return available[rng.Next(available.Count)];
        };
    }

    /// <summary>
    /// Returns a function that selects the next counter clockwise position (if it's possible) based on the last position and with a given bias: the bigger the bias (1),
    /// the more counter clockwise directions will be selected. If no vertical directions are available, it will select a random direction.
    ///
    /// This method works if the Backtracker cell selector is used. Only the Backtracker cell selector will provide a consistent lastDir parameter. 
    /// </summary>
    /// <param name="counterClockwiseBias"></param>
    /// <param name="rng"></param>
    /// <returns></returns>
    public static Func<Vector2I?, IList<Vector2I>, Vector2I> CreateCounterClockwiseSelector(float counterClockwiseBias, Random? rng = null) {
        rng ??= new Random();
        return (lastDir, available) => {
            if (lastDir.HasValue && rng.NextSingle() <= counterClockwiseBias) {
                var clockwise = lastDir.Value.CounterClockwise();
                while (!available.Contains(clockwise)) {
                    clockwise = clockwise.Clockwise();
                }
                return clockwise;
            }
            return available[rng.Next(available.Count)];
        };
    }

}