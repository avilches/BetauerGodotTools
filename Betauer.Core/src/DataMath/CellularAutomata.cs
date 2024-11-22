using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Core.DataMath;

public class CellularAutomata<T> {
    private readonly EqualityComparer<T> _comparer = EqualityComparer<T>.Default;
    private Func<Array2D<T>, Vector2I, T> _updateRule;
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
    public CellularAutomata<T> OnUpdate(Func<Array2D<T>, Vector2I, T> updateRule) {
        _updateRule = updateRule;
        return this;
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives the current grid and the position of the cell to update.
    /// So, the rule must locate the cells in the grid and return the new value for the cell.
    /// </summary>
    public CellularAutomata<T> OnUpdate(Func<Array2D<T>, int, int, T> updateRule) {
        _updateRule = (grid, pos) => updateRule.Invoke(grid, pos.X, pos.Y);
        return this;
    }

    /// <summary>
    /// Set the rule to update the cell. The rule receives a 3x3 grid with the neighbors of the cell to update.
    /// If the neighbors are out of bounds, the default value is used.
    /// </summary>
    public CellularAutomata<T> OnUpdate(Func<T[,], T> updateRule, T defaultValue = default) {
        var neighbors = new T[3, 3];
        _updateRule = (grid, pos) => {
            _state.CopyNeighbors(pos, neighbors, defaultValue);
            return updateRule.Invoke(neighbors);
        };
        return this;
    }

    /// <summary>
    /// Execute the update rule for all cells in the grid. It creates a temporal state, so the changes for every rule will be visible in the next update.
    /// This is the regular behaviour of a cellular automata.
    /// </summary>
    /// <returns></returns>
    public int Update() {
        var changes = 0;
        _nextState ??= new Array2D<T>(_state.Width, _state.Height);
        foreach (var cell in _state) {
            var oldValue = cell.Value;
            var newValue = _updateRule.Invoke(_state, cell.Position);
            if (!_comparer.Equals(newValue, oldValue)) {
                changes++;
            }
            _nextState[cell.Position] = newValue;
        }
        _nextState.CopyTo(_state.Data);
        return changes;
    }

    /// <summary>
    /// Execute the update rule for all cells in the grid and apply the changes immediately.
    /// This is useful when you want to update the grid in a single step, like remove dead ends
    /// </summary>
    /// <returns></returns>
    public int SingleUpdate() {
        var changes = 0;
        foreach (var cell in _state) {
            var oldValue = cell.Value;
            var newValue = _updateRule.Invoke(_state, cell.Position);
            if (!_comparer.Equals(newValue, oldValue)) {
                changes++;
            }
            _state[cell.Position] = newValue;
        }
        return changes;
    }
}