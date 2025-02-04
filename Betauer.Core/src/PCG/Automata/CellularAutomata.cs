using System;
using System.Collections.Generic;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.Automata;

public class CellularAutomata<T> {

    public delegate TT Rule<TT>(Array2D<TT> grid, Vector2I pos);
   
    private readonly List<Rule<T>> _rules = [];
    public Array2D<T> State { get; }
    private Array2D<T>? _nextState;
    public int Width => State.Width;
    public int Height => State.Height;
    
    public CellularAutomata(T[,] state) {
        State = new Array2D<T>(state);
    }

    public CellularAutomata(Array2D<T> state) {
        State = state;
    }

    public CellularAutomata(int width, int height, T? defaultValue = default) {
        State = new Array2D<T>(width, height);
        if (defaultValue != null) {
            State.Fill(defaultValue);
        }
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives the current grid and the position of the cell to update.
    /// So, the rule must locate the cells in the grid and return the new value for the cell.
    /// </summary>
    public void AddRule(Rule<T> rule) {
        _rules.Add(rule);
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives the current grid and the position of the cell to update.
    /// So, the rule must locate the cells in the grid and return the new value for the cell.
    /// </summary>
    public void AddRule(Func<Array2D<T>, int, int, T> updateRule) {
        _rules.Add((grid, pos) => updateRule.Invoke(grid, pos.X, pos.Y));
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives a NxN grid with the neighbors of the cell to update.
    /// If the neighbors are out of bounds, the default value is used.
    /// </summary>
    public void AddNeighborhoodRule(int neighborhoodSize, Func<T[,], T> updateRule, T defaultValue = default) {
        var neighbors = new T[neighborhoodSize, neighborhoodSize];
        _rules.Add((_, pos) => {
            State.CopyChebyshevRegion(pos, neighbors, defaultValue);
            return updateRule.Invoke(neighbors);
        });
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives a 3x3 grid with the neighbors of the cell to update.
    /// If the neighbors are out of bounds, the default value is used.
    /// </summary>
    public void AddMooreNeighborhoodRule(Func<T[,], T> updateRule, T defaultValue = default) {
        AddNeighborhoodRule(3, updateRule, defaultValue);
    }


    /// <summary>
    /// Execute the update rule for all cells in the grid. It creates a temporal state,
    /// so the changes for every rule will be visible in the next update.
    /// This is the regular behaviour of a cellular automata.
    /// </summary>
    /// <returns></returns>
    public void Update() {
        Update(0, 0, Width, Height);
    }

    public void Update(int x, int y, int width, int height) {
        _nextState ??= new Array2D<T>(State.Width, State.Height);
        foreach (var rule in _rules) {
            foreach (var (pos, value) in State.GetIndexedValues()) {
                if (pos.X < x || pos.X >= width + x || pos.Y < y || pos.Y >= height + y) {
                    _nextState[pos] = value;
                } else {
                    var newValue = rule(State, pos);
                    _nextState[pos] = newValue;
                }
            }
            _nextState.CopyTo(State.Data);
        }
    }

    public void SingleUpdate() {
        SingleUpdate(0, 0, Width, Height);
    }

    /// <summary>
    /// Execute the update rule for all cells in the grid and apply the changes immediately.
    /// This is useful when you want to update the grid in a single step, like remove dead ends
    /// </summary>
    /// <returns></returns>
    public void SingleUpdate(int x, int y, int width, int height) {
        foreach (var rule in _rules) {
            foreach (var pos in State.GetPositions()) {
                if (pos.X < x || pos.X >= width + x || pos.Y < y || pos.Y >= height + y) continue;
                var newValue = rule(State, pos);
                State[pos] = newValue;
            }
        }
    }
}