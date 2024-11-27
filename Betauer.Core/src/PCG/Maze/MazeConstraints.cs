using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class MazeConstraints(Func<Vector2I?, IList<Vector2I>, Vector2I> directionSelector) {
    private static readonly Lazy<Random> DefaultRandom = new Lazy<Random>(() => new Random());

    public Func<Vector2I?, IList<Vector2I>, Vector2I> DirectionSelector = directionSelector;
    
    public int MaxPaths { get; set; } = -1;
    public int MaxDepth { get; set; } = -1;
    public int MaxTotalCells { get; set; } = -1;
    public int MaxCellsPerPath { get; set; } = -1;

    public MazeConstraints With(Action<MazeConstraints> configure) {
        configure(this);
        return this;
    }

    public static MazeConstraints CreateRandom(Random? rng = null) {
        return new MazeConstraints(DirectionSelectors.CreateRandom(rng ?? DefaultRandom.Value));
    }

    public static MazeConstraints CreateWindy(float directionalBias, Random? rng = null) {
        return directionalBias == 0 
            ? CreateRandom(rng) 
            : new MazeConstraints(DirectionSelectors.CreateWindy(directionalBias, rng ?? DefaultRandom.Value));
    }

    public static MazeConstraints CreateVerticalBias(float verticalBias, Random? rng = null) {
        return new MazeConstraints(DirectionSelectors.CreateVerticalSelector(verticalBias, rng ?? DefaultRandom.Value));
    }

    public static MazeConstraints CreateHorizontalBias(float horizontalBias, Random? rng = null) {
        return new MazeConstraints(DirectionSelectors.CreateHorizontalSelector(horizontalBias, rng ?? DefaultRandom.Value));
    }

    public static MazeConstraints CreateClockwiseBias(float bias, Random? rng = null) {
        return new MazeConstraints(DirectionSelectors.CreateClockwiseSelector(bias, rng ?? DefaultRandom.Value));
    }

    public static MazeConstraints CreateCounterClockwiseBias(float bias, Random? rng = null) {
        return new MazeConstraints(DirectionSelectors.CreateCounterClockwiseSelector(bias, rng ?? DefaultRandom.Value));
    }
}