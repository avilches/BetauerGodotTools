using System;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath;

public abstract class DeadEndRemover {
    public static DeadEndRemover<TT> Create<TT>(Array2D<TT> grid, TT fill, TT empty) {
        return new DeadEndRemover<TT>(grid, v => Equals(v, fill), (v, b) => b ? fill : empty, empty);
    }

    public static DeadEndRemover<bool> Create(Array2D<bool> grid) {
        return Create(grid, true, false);
    }

    public static void RemoveAllDeadEnds(Array2D<bool> grid) {
        Create(grid).RemoveAll();
    }

    public static void RemoveAllDeadEnds<T>(Array2D<T> grid, T fill, T empty) {
        Create(grid, fill, empty).RemoveAll();
    }

    public static void RemoveAllDeadEnds<T>(Array2D<T> grid, Func<T, bool> isEnabled, Func<T, bool, T> update, T defaultValue) {
        new DeadEndRemover<T>(grid, isEnabled, update, defaultValue).RemoveAll();
    }

    public static DeadEndRemover<TT> Create<TT>(Array2D<TT> grid, Func<TT, bool> isEnabled, Func<TT, bool, TT> update, TT defaultValue) {
        return new DeadEndRemover<TT>(grid, isEnabled, update, defaultValue);
    }
}

/// <summary>
/// Remove cells that are dead ends: a cell with only one neighbor.
/// Every call to Update() will remove one cell of every dead end in order from top to bottom and
/// left to right, so branches to top left will be erased faster than branches to down/right, which
/// will be erased one cell per call to Update.
///
/// ··#··##·      ·····##·
/// ·#######      ··#####·
/// ····#···      ····#···
/// ····###·      ····##··
///
/// Calling RemoveAll() will remove all dead ends.
/// </summary>
public class DeadEndRemover<T> : DeadEndRemover {
    private readonly CellularAutomata<T> _automata;
    private int _changes;
    private readonly Func<T, bool> _isEnabled;
    private readonly Func<T, bool, T> _update;

    public DeadEndRemover(Array2D<T> grid, Func<T, bool> isEnabled, Func<T, bool, T> update, T defaultValue) {
        _isEnabled = isEnabled;
        _update = update;
        _automata = new CellularAutomata<T>(grid);
        _automata.AddRule(UpdateRule);
        _changes = 0;
    }

    private T UpdateRule(Array2D<T> grid, Vector2I pos) {
        var value = grid.GetValueSafe(pos);
        if (!_isEnabled(value)) return value;
        var pathNeighbors = grid.GetValidUpDownLeftRightPositions(pos, _isEnabled).Count();
        value = _update(value, pathNeighbors > 1);
        if (!_isEnabled(value)) _changes++;
        return value;
    }

    public int Update() {
        _changes = 0;
        _automata.SingleUpdate();
        return _changes;
    }

    public int RemoveAll() {
        var totalChanges = 0;
        int changes;
        while ((changes = Update()) > 0) {
            totalChanges += changes;
        }
        return totalChanges;
    }
}