using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.DataMath;

public class CellularAutomata<T> {

    public delegate TT Rule<TT>(Array2D<TT> grid, Vector2I pos);
    
    private readonly List<Rule<T>> _rules = [];
    public Array2D<T> State { get; }
    private Array2D<T>? _nextState;
    
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
    public CellularAutomata<T> AddRule(Rule<T> rule) {
        _rules.Add(rule);
        return this;
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives the current grid and the position of the cell to update.
    /// So, the rule must locate the cells in the grid and return the new value for the cell.
    /// </summary>
    public CellularAutomata<T> AddRule(Func<Array2D<T>, int, int, T> updateRule) {
        _rules.Add((grid, pos) => updateRule.Invoke(grid, pos.X, pos.Y));
        return this;
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives a NxN grid with the neighbors of the cell to update.
    /// If the neighbors are out of bounds, the default value is used.
    /// </summary>
    public CellularAutomata<T> AddNeighborhoodRule(int neighborhoodSize, Func<T[,], T> updateRule, T defaultValue = default) {
        var neighbors = new T[neighborhoodSize, neighborhoodSize];
        _rules.Add((_, pos) => {
            State.CopyNeighbors(pos, neighbors, defaultValue);
            return updateRule.Invoke(neighbors);
        });
        return this;
    }
    
    /// <summary>
    /// Set the rule to update the cell. The rule receives a 3x3 grid with the neighbors of the cell to update.
    /// If the neighbors are out of bounds, the default value is used.
    /// </summary>
    public CellularAutomata<T> AddMooreNeighborhoodRule(Func<T[,], T> updateRule, T defaultValue = default) {
        return AddNeighborhoodRule(3, updateRule, defaultValue);
    }


    /// <summary>
    /// Execute the update rule for all cells in the grid. It creates a temporal state,
    /// so the changes for every rule will be visible in the next update.
    /// This is the regular behaviour of a cellular automata.
    /// </summary>
    /// <returns></returns>
    public void Update() {
        Update(0, 0, State.Width, State.Height);
    }

    public void Update(int x, int y, int width, int height) {
        _nextState ??= new Array2D<T>(State.Width, State.Height);
        foreach (var rule in _rules) {
            foreach (var (pos, value) in State) {
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
        SingleUpdate(0, 0, State.Width, State.Height);
    }
    
    /// <summary>
    /// Execute the update rule for all cells in the grid and apply the changes immediately.
    /// This is useful when you want to update the grid in a single step, like remove dead ends
    /// </summary>
    /// <returns></returns>
    public void SingleUpdate(int x, int y, int width, int height) {
        foreach (var rule in _rules) {
            foreach (var (pos, value) in State) {
                if (pos.X < x || pos.X >= width + x || pos.Y < y || pos.Y >= height + y) continue;
                var newValue = rule(State, pos);
                State[pos] = newValue;
            }
        }
    }
}