using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Betauer.Core.Sudoku;

public class SudokuBoard {
    public class Cell {
        public SudokuBoard Sudoku { get; }
        public int Row { get; }
        public int Column { get; }
        public int Index { get; }
        public int GroupNo { get; }

        public int Value { get; set; }

        internal Cell(SudokuBoard sudoku, int index) {
            var (y, x) = (index % TotalColumns, index / TotalRows);
            Sudoku = sudoku;
            Value = -1;
            Index = index;
            GroupNo = x / 3 + 3 * (y / 3) + 1;
            Row = x + 1;
            Column = y + 1;
        }

        public override string ToString() {
            return $"({Column},{Row})={Value}";
        }

        /// <summary>
        /// Returns true if the cell is valid. Empty cells are invalid.
        /// </summary>
        public bool IsValidValue() {
            return AcceptValue(Value);
        }

        public bool HasValue() => Value > 0;

        /// <summary>
        /// Returns true if the value is valid for the cell (there is no conflict with the row, column or group).
        /// </summary>
        public bool AcceptValue(int val) {
            if (val is < 1 or > TotalDigits) return false;
            // Check the value whether exists in the 3x3 group.
            if (Sudoku.Cells.Where(c => c.Index != Index && c.GroupNo == GroupNo).Any(c2 => c2.Value == val))
                return false;

            // Check the value whether exists in the row.
            if (Sudoku.Cells.Where(c => c.Index != Index && c.Row == Row).Any(c2 => c2.Value == val))
                return false;

            // Check the value whether exists in the column.
            if (Sudoku.Cells.Where(c => c.Index != Index && c.Column == Column).Any(c2 => c2.Value == val))
                return false;

            return true;
        }

        /// <summary>
        /// Returns a list of possible candidates for the cell. If the cell is already filled, it returns a list with the value.
        /// </summary>
        /// <returns></returns>
        public ImmutableList<int> Candidates() {
            if (Value > 0) return new[] { Value }.ToImmutableList();
            var grpCells = Sudoku.Cells.Where(c => c.Value > 0 && c.GroupNo == GroupNo).Select(c => c.Value);
            var colCells = Sudoku.Cells.Where(c => c.Value > 0 && c.Row == Row).Select(c => c.Value);
            var rowCells = Sudoku.Cells.Where(c => c.Value > 0 && c.Column == Column).Select(c => c.Value);
            var occupied = grpCells.Concat(colCells).Concat(rowCells).ToHashSet();
            var all = Enumerable.Range(1, TotalDigits).ToList();
            all.RemoveAll(occupied.Contains);
            return all.ToImmutableList();
        }

        public void Remove() {
            Value = -1;
        }
    }

    public ImmutableList<Cell> Cells { get; }

    public int[,] CreateGrid() {
        var data = new int[TotalRows, TotalColumns];
        for (var y = 0; y < TotalRows; ++y) {
            for (var x = 0; x < TotalColumns; ++x) {
                var value = GetCell(y + 1, x + 1).Value;
                data[y, x] = value < 1 ? 0 : value;
            }
        }
        return data;
    }

    public const int TotalRows = 9;
    public const int TotalColumns = 9;
    public const int TotalCells = 81;
    public const int TotalDigits = 9;

    public SudokuBoard(SudokuBoard other) {
        Cells = InitializeCells();
        Import(other);
    }

    public SudokuBoard(string? import = null) {
        Cells = InitializeCells();
        if (import != null) Import(import);
    }

    public SudokuBoard(int[,] import) {
        Cells = InitializeCells();
        Import(import);
    }

    private ImmutableList<Cell> InitializeCells() => Enumerable.Range(0, TotalCells).Select(i => new Cell(this, i)).ToImmutableList();

    /// <summary>
    /// row and column are 1-based (1 to 9)
    /// </summary>
    public Cell GetCell(int row, int column) => GetCell((row - 1) * TotalRows + column - 1);

    /// <summary>
    /// cellIndex is 0-based (0 to 80)
    /// </summary>
    public Cell GetCell(int cellIndex) => Cells[cellIndex];

    /// <summary>
    /// Checks the board is already filled.
    /// </summary>
    public bool IsBoardFilled() => Cells.All(cell => cell.HasValue());

    /// <summary>
    /// Fills the game board with -1 which is the default for the empty state. 
    /// </summary>
    public void Clear() => Cells.ForEach(cell => cell.Remove());

    /// <summary>
    /// Returns true if all cells are valid (or empty).
    /// </summary>
    /// <returns>Returns whether is table is valid or not.</returns>
    public bool IsValid() => Cells.All(cell => !cell.HasValue() || cell.IsValidValue());

    /// <summary>
    /// Returns the first filled but invalid cell.
    /// </summary>
    /// <returns></returns>
    public Cell? GetFirstInvalidCell() => Cells.FirstOrDefault(cell => cell.HasValue() && !cell.IsValidValue());

    /// <summary>
    /// Returns all the filled but invalid cells.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Cell> GetInvalidCells() => Cells.Where(cell => cell.HasValue() && !cell.IsValidValue());


    public void Import(SudokuBoard other) {
        Enumerable.Range(0, TotalCells).ForEach(i => Cells[i].Value = other.Cells[i].Value);
    }

    public void Import(int[,] grid) {
        for (var y = 0; y < TotalRows; ++y) {
            for (var x = 0; x < TotalColumns; ++x) {
                var cell = GetCell(y + 1, x + 1);
                cell.Value = grid[y, x];
            }
        }
    }

    public void Import(string import) {
        if (import.Length > TotalCells) {
            // remove all non-digits and non-dots
            import = new string(import.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }
        Enumerable.Range(0, TotalCells).ForEach(pos => {
            var (x, y) = (pos % TotalColumns, pos / TotalRows);
            var c = import[pos];
            var value = c != '.' && c != '0' ? c - '0' : -1;
            var cell = GetCell(y + 1, x + 1);
            cell.Value = value;
        });
    }

    public string Export() {
        var sb = new StringBuilder(TotalCells);
        for (var y = 0; y < TotalRows; ++y) {
            for (var x = 0; x < TotalColumns; ++x) {
                var value = GetCell(y + 1, x + 1).Value;
                if (value < 1) sb.Append('0');
                else sb.Append(value);
            }
        }
        return sb.ToString();
    }

    public bool Solve() {
        var solver = GetSolutions(1).FirstOrDefault();
        if (solver == null) return false;
        Import(solver);
        return true;
    }

    public IEnumerable<SudokuBoard> GetSolutions(int maxSolutions) {
        return GetSolutions(_ => --maxSolutions == 0);
    }

    public IEnumerable<SudokuBoard> GetSolutions(Predicate<SudokuBoard> predicate) {
        return DlxSudokuSolver.Resolve(this, predicate);
    }

    public bool HasUniqueSolution() {
        return GetSolutions(2).Count() == 1;
    }

    public bool HasSolutions() {
        return GetSolutions(1).Count() == 1;
    }

    public bool Generate(int seed = -1) {
        return new BacktrackSolver(this, seed).Solve();
    }

    private static readonly ImmutableList<Func<int[,], int[,]>> Transformations = new Func<int[,], int[,]>[] {
        null, // this is for a random relabel
        data => data.YxFlipDiagonal(),
        data => data.YxFlipDiagonalSecondary(),
        data => data.YxRotate90(),
        data => data.YxRotate180(),
        data => data.YxRotateMinus90(),
        data => data.YxFlipH(),
        data => data.YxFlipV(),
        data => data.YxSwapColumns(0, 1),
        data => data.YxSwapColumns(0, 2),
        data => data.YxSwapColumns(1, 2),
        data => data.YxSwapColumns(3, 4),
        data => data.YxSwapColumns(3, 5),
        data => data.YxSwapColumns(4, 5),
        data => data.YxSwapColumns(6, 7),
        data => data.YxSwapColumns(6, 8),
        data => data.YxSwapColumns(7, 8),
        data => data.YxSwapRows(0, 1),
        data => data.YxSwapRows(0, 2),
        data => data.YxSwapRows(1, 2),
        data => data.YxSwapRows(3, 4),
        data => data.YxSwapRows(3, 5),
        data => data.YxSwapRows(4, 5),
        data => data.YxSwapRows(6, 7),
        data => data.YxSwapRows(6, 8),
        data => data.YxSwapRows(7, 8),
        data => data.YxSwapRows(0, 3, 3),
        data => data.YxSwapRows(0, 6, 3),
        data => data.YxSwapRows(3, 6, 3),
        data => data.YxSwapColumns(0, 3, 3),
        data => data.YxSwapColumns(0, 6, 3),
        data => data.YxSwapColumns(3, 6, 3)
    }.ToImmutableList();

    public void Relabel(Random rnd) {
        var map = Enumerable.Range(1, TotalDigits).ToArray();
        rnd.Shuffle(map);
        Relabel(map);
    }

    public void RemoveCells(int seed, int hints) {
        var toRemove = TotalCells - hints;
        new Random(seed)
            .Extract(Enumerable.Range(0, TotalCells).ToArray(), toRemove)
            .ForEach(pos => GetCell(pos).Remove());
    }

    public void RemoveCells(string mask) {
        if (mask.Length > TotalCells) {
            // remove all non-digits and non-dots
            mask = new string(mask.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }
        Enumerable.Range(0, TotalCells).ForEach(pos => {
            var (x, y) = (pos % TotalColumns, pos / TotalRows);
            var c = mask[pos];
            var value = c != '.' && c != '0' ? c - '0' : -1;
            if (value == -1) GetCell(y + 1, x + 1).Remove();
        });
    }

    public void Relabel(int[] map) {
        // validate d is a permutation of 1..9
        if (map.Length != 9 || map.Distinct().Count() != TotalDigits || map.Min() != 1 || map.Max() != TotalDigits) {
            throw new ArgumentException("Invalid permutation");
        }
        Enumerable.Range(0, TotalCells).ForEach(index => {
            var cell = GetCell(index);
            var value = cell.Value;
            if (value > 0) {
                cell.Value = map[value - 1];
            }
        });
    }

    public void Shuffle(int seed, int steps = 30) {
        var data = CreateGrid();
        var rnd = new Random(seed);
        for (var i = 0; i < steps; ++i) {
            var func = rnd.Next(Transformations);
            if (func == null) {
                Import(data);
                Relabel(rnd);
                data = CreateGrid();
            } else {
                func.Invoke(data);
            }
        }
        Import(data);
    }
}