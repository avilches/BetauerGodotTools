using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class BacktrackConstraints {
    private static readonly Lazy<Random> DefaultRandom = new Lazy<Random>(() => new Random());

    public Func<Vector2I?, IList<Vector2I>, Vector2I> DirectionSelector;
    public int MaxPaths { get; set; } = -1;
    public int MaxTotalCells { get; set; } = -1;
    public int MaxCellsPerPath { get; set; } = -1;

    public BacktrackConstraints With(Action<BacktrackConstraints> configure) {
        configure(this);
        return this;
    }

    public static BacktrackConstraints CreateRandom(Random? rng = null) {
        return new BacktrackConstraints {
            DirectionSelector = DirectionSelectors.CreateRandom(rng ?? DefaultRandom.Value)
        };
    }

    public static BacktrackConstraints CreateWindy(float directionalBias, Random? rng = null) {
        return directionalBias == 0
            ? CreateRandom(rng)
            : new BacktrackConstraints {
                DirectionSelector = DirectionSelectors.CreateWindy(directionalBias, rng ?? DefaultRandom.Value)
            };
    }

    public static BacktrackConstraints CreateVerticalBias(float verticalBias, Random? rng = null) {
        return new BacktrackConstraints {
            DirectionSelector = DirectionSelectors.CreateVerticalSelector(verticalBias, rng ?? DefaultRandom.Value)
        };
    }

    public static BacktrackConstraints CreateHorizontalBias(float horizontalBias, Random? rng = null) {
        return new BacktrackConstraints {
            DirectionSelector = DirectionSelectors.CreateHorizontalSelector(horizontalBias, rng ?? DefaultRandom.Value)
        };
    }

    public static BacktrackConstraints CreateClockwiseBias(float bias, Random? rng = null) {
        return new BacktrackConstraints {
            DirectionSelector = DirectionSelectors.CreateClockwiseSelector(bias, rng ?? DefaultRandom.Value)
        };
    }

    public static BacktrackConstraints CreateCounterClockwiseBias(float bias, Random? rng = null) {
        return new BacktrackConstraints {
            DirectionSelector = DirectionSelectors.CreateCounterClockwiseSelector(bias, rng ?? DefaultRandom.Value)
        };
    }
}