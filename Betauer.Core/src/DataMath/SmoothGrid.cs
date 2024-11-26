using System;

namespace Betauer.Core.DataMath;

public abstract class SmoothGrid {
    public static SmoothGrid<bool> Create(Array2D<bool> grid, int deleteIfLessThan = 5, int addIfMoreThan = 5) {
        return Create(grid, true, false, deleteIfLessThan, addIfMoreThan);
    }

    public static SmoothGrid<TT> Create<TT>(Array2D<TT> grid, TT fill, TT empty, int deleteIfLessThan = 5, int addIfMoreThan = 5) {
        return new SmoothGrid<TT>(grid, v => Equals(v, fill), (v, b) => b ? fill : empty, empty, deleteIfLessThan, addIfMoreThan);
    }
}

public class SmoothGrid<T> : SmoothGrid {
    private readonly CellularAutomata<T> _automata;
    private readonly Func<T, bool> _isEnabled;
    private readonly Func<T, bool, T> _setValue;
    private readonly T _defaultValue;
    private readonly Array2D<T> _grid;
    private readonly int _deleteIfLessThan;
    private readonly int _addIfMoreThan;

    public int FilledCells { get; private set; }
    public int EmptyCells { get; private set; }
    public int RemovedCells { get; private set; }
    public int AddedCells { get; private set; }

    public SmoothGrid(T[,] grid, Func<T, bool> isEnabled, Func<T, bool, T> setValue, T defaultValue, int deleteIfLessThan, int addIfMoreThan) :
        this(new Array2D<T>(grid), isEnabled, setValue, defaultValue, deleteIfLessThan, addIfMoreThan) {
    }

    public SmoothGrid(Array2D<T> grid, Func<T, bool> isEnabled, Func<T, bool, T> setValue, T defaultValue, int deleteIfLessThan, int addIfMoreThan) {
        _grid = grid;
        _isEnabled = isEnabled;
        _setValue = setValue;
        _defaultValue = defaultValue;
        _deleteIfLessThan = deleteIfLessThan;
        _addIfMoreThan = addIfMoreThan;

        // Inicializar contadores una sola vez
        for (var y = 0; y < grid.Height; y++) {
            for (var x = 0; x < grid.Width; x++) {
                if (isEnabled(grid[y, x])) FilledCells++;
                else EmptyCells++;
            }
        }

        _automata = new CellularAutomata<T>(grid);
        _automata.AddMooreNeighborhoodRule(RemoveRule, defaultValue);
        _automata.AddMooreNeighborhoodRule(AddRule, defaultValue);
    }

    private T RemoveRule(T[,] neighbors) {
        var value = neighbors[1, 1];
        if (!_isEnabled(value)) return value; // ignorar celdas muertas
        var liveNeighbors = Automatas.CountMooreNeighborhood(neighbors, _isEnabled);
        var newValue = _setValue(value, liveNeighbors >= _deleteIfLessThan);
        if (!_isEnabled(newValue) && _isEnabled(value)) {
            RemovedCells++;
            FilledCells--;
            EmptyCells++;
        }
        return newValue;
    }

    private T AddRule(T[,] neighbors) {
        var value = neighbors[1, 1];
        if (_isEnabled(value)) return value; // ignorar celdas vivas
        var liveNeighbors = Automatas.CountMooreNeighborhood(neighbors, _isEnabled);
        var newValue = _setValue(value, liveNeighbors > _addIfMoreThan);
        if (_isEnabled(newValue) && !_isEnabled(value)) {
            AddedCells++;
            FilledCells++;
            EmptyCells--;
        }
        return newValue;
    }

    public void Update() {
        RemovedCells = 0;
        AddedCells = 0;
        _automata.Update();
    }

    public (int removed, int added) UpdateAndGetChanges() {
        Update();
        return (RemovedCells, AddedCells);
    }
}