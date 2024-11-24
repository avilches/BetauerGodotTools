using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.DataMath;

public class CellularAutomata<T> {
    private readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;
    private readonly List<Func<Array2D<T>, Vector2I, T>> _rules = [];
    private readonly Array2D<T> _state;
    private Array2D<T>? _nextState;
    
    public T[,] State => _state.Data;

    public CellularAutomata(Array2D<T> state) {
        _state = state;
    }

    public CellularAutomata(int width, int height, T? defaultValue = default) {
        _state = new Array2D<T>(width, height);
        if (defaultValue != null) {
            _state.Fill(defaultValue);
        }
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives the current grid and the position of the cell to update.
    /// So, the rule must locate the cells in the grid and return the new value for the cell.
    /// </summary>
    public CellularAutomata<T> AddRule(Func<Array2D<T>, Vector2I, T> updateRule) {
        _rules.Add(updateRule);
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
    /// Set the rule to update the cell. The rule receives a 3x3 grid with the neighbors of the cell to update.
    /// If the neighbors are out of bounds, the default value is used.
    /// </summary>
    public CellularAutomata<T> AddRule(int size, Func<T[,], T> updateRule, T defaultValue = default) {
        var neighbors = new T[size, size];
        _rules.Add((grid, pos) => {
            _state.CopyNeighbors(pos, neighbors, defaultValue);
            return updateRule.Invoke(neighbors);
        });
        return this;
    }
    
    public CellularAutomata<T> AddRule(Func<T[,], T> updateRule, T defaultValue = default) {
        return AddRule(3, updateRule, defaultValue);
    }


    /// <summary>
    /// Execute the update rule for all cells in the grid. It creates a temporal state, so the changes for every rule will be visible in the next update.
    /// This is the regular behaviour of a cellular automata.
    /// </summary>
    /// <returns></returns>
    public int Update() {
        var changes = 0;
        _nextState ??= new Array2D<T>(_state.Width, _state.Height);
        foreach (var rule in _rules) {
            foreach (var (pos, value) in _state) {
                var newValue = rule(_state, pos);
                if (!_comparer.Equals(newValue, value)) {
                    changes++;
                }
                _nextState[pos] = newValue;
            }
            _nextState.CopyTo(_state.Data);
        }
        return changes;
    }

    /// <summary>
    /// Execute the update rule for all cells in the grid and apply the changes immediately.
    /// This is useful when you want to update the grid in a single step, like remove dead ends
    /// </summary>
    /// <returns></returns>
    public int SingleUpdate() {
        var changes = 0;
        foreach (var rule in _rules) {
            foreach (var (pos, value) in _state) {
                var newValue = rule(_state, pos);
                if (!_comparer.Equals(newValue, value)) {
                    changes++;
                }
                _state[pos] = newValue;
            }
        }
        return changes;
    }
}