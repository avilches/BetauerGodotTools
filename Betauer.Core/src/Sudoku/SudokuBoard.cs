using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Betauer.Core.Sudoku;

public class SudokuBoard {
    public class Cell {
        public int Row { get; }
        public int Column { get; }
        public int Value { get; internal set; }
        public int Index { get; }
        public int GroupNo { get; }
        public SudokuBoard Sudoku { get; }

        public Cell(SudokuBoard sudoku, int value, int index, int groupNo, int row, int column) {
            Sudoku = sudoku;
            Value = value;
            Index = index;
            GroupNo = groupNo;
            Row = row;
            Column = column;
        }

        public override string ToString() {
            return $"({Column},{Row})={Value}";
        }

        /// <summary>
        /// Checks the specified cell has a valid value
        /// </summary>
        public bool IsValidValue() {
            return IsValidValue(Value);
        }
        
        public bool IsFilled() => Value > 0;

        /// <summary>
        /// Checks the specified cell can accept the specified value.
        /// </summary>
        public bool IsValidValue(int val) {
            if (val is < 1 or > 9) return false;
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
    }

    internal List<Cell> Cells { get; }

    public int[,] CreateGrid() {
        var data = new int[9, 9];
        for (var y = 0; y < 9; ++y) {
            for (var x = 0; x < 9; ++x) {
                var value = GetCell(y + 1, x + 1).Value;
                data[y, x] = value < 1 ? 0 : value;
            }
        }
        return data;
    }

    public const int TotalRows = 9;
    public const int TotalColumns = 9;
    public const int TotalCells = 81;

    private void InitializeCells() {
        for (var x = 0; x < TotalRows; x++) {
            for (var y = 0; y < TotalColumns; y++) {
                Cells.Add(new Cell(
                    sudoku: this,
                    value: -1,
                    index: x * TotalRows + y,
                    groupNo: (x / 3) + 3 * (y / 3) + 1,
                    row: x + 1,
                    column: y + 1
                ));
            }
        }
    }

    public SudokuBoard(SudokuBoard other) {
        Cells = new List<Cell>(TotalCells);
        InitializeCells();
        Import(other);
    }

    public SudokuBoard(string? import = null) {
        Cells = new List<Cell>(TotalCells);
        InitializeCells();
        if (import != null) Import(import);
    }

    public SudokuBoard(int[,] import) {
        Cells = new List<Cell>(TotalCells);
        InitializeCells();
        Import(import);
    }

    /// <summary>
    /// row and column are 1-based (1 to 9)
    /// </summary>
    public Cell GetCell(int row, int column) => GetCell((row - 1) * TotalRows + column - 1);
    /// <summary>
    /// cellIndex is 0-based (0 to 80)
    /// </summary>
    public Cell GetCell(int cellIndex) => Cells[cellIndex];

    /// <summary>
    /// cellIndex is 0-based (0 to 80)
    /// </summary>
    public void SetCellValue(int value, int row, int column) => SetCellValue(value, (row - 1) * TotalRows + column - 1);
    
    /// <summary>
    /// row and column are 1-based (1 to 9)
    /// </summary>
    public void SetCellValue(int value, int cellIndex) => Cells[cellIndex].Value = value < 1 ? -1 : value;

    /// <summary>
    /// cellIndex is 0-based (0 to 80)
    /// </summary>
    public void RemoveCell(int row, int column) => SetCellValue(-1, (row - 1) * TotalRows + column - 1);
    
    /// <summary>
    /// row and column are 1-based (1 to 9)
    /// </summary>
    public void RemoveCell(int cellIndex) => Cells[cellIndex].Value = -1;

    /// <summary>
    /// Checks the board is already filled.
    /// </summary>
    public bool IsBoardFilled() => Cells.All(cell => cell.IsFilled());

    /// <summary>
    /// Fills the game board with -1 which is the default for the empty state. 
    /// </summary>
    public void Clear() => Cells.ForEach(cell => cell.Value = -1);

    /// <summary>
    /// Returns true if all cells are valid (or empty).
    /// </summary>
    /// <returns>Returns whether is table is valid or not.</returns>
    public bool IsValid() => Cells.All(cell => !cell.IsFilled() || cell.IsValidValue());

    /// <summary>
    /// Returns the first filled but invalid cell.
    /// </summary>
    /// <returns></returns>
    public Cell? GetFirstInvalidCell() => Cells.FirstOrDefault(cell => cell.IsFilled() && !cell.IsValidValue());

    /// <summary>
    /// Returns all the filled but invalid cells.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Cell> GetInvalidCells() => Cells.Where(cell => cell.IsFilled() && !cell.IsValidValue());


    public void Import(SudokuBoard other) {
        for (var i = 0; i < TotalCells; ++i) {
            SetCellValue(other.GetCell(i).Value, i);
        }
    }

    public void Import(int[,] grid) {
        for (var y = 0; y < 9; ++y) {
            for (var x = 0; x < 9; ++x) {
                SetCellValue(grid[y, x], y + 1, x + 1);
            }
        }
    }

    public void Import(string import) {
        if (import.Length > TotalCells) {
            // remove all non-digits and non-dots
            import = new string(import.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }
        for (var pos = 0; pos < TotalCells; ++pos) {
            var (x, y) = (pos % 9, pos / 9);
            var c = import[pos];
            var value = c != '.' && c != '0' ? c - '0' : -1;
            SetCellValue(value, y + 1, x + 1);
        }
    }

    public string Export() {
        var sb = new StringBuilder(TotalCells);
        for (var y = 0; y < 9; ++y) {
            for (var x = 0; x < 9; ++x) {
                var value = GetCell(y + 1, x + 1).Value;
                if (value < 1) sb.Append('0');
                else sb.Append(value);
            }
        }
        return sb.ToString();
    }

    public bool SolveDancingLinks() {
        var solver = GetSolutions(1).FirstOrDefault();
        if (solver == null) return false;
        Import(solver);
        return true;
    }

    public IEnumerable<SudokuBoard> GetSolutions(int maxSolutions) {
        return DlxSudokuSolver.Resolve(this, (_) => --maxSolutions == 0);
    }

    public IEnumerable<SudokuBoard> GetSolutions(Predicate<SudokuBoard> predicate) {
        return DlxSudokuSolver.Resolve(this, predicate);
    }

    public bool SolveBacktrack(int seed = -1) {
        return new BacktrackSolver(this, seed).Solve();
    }

    private static readonly IList<Func<int[,], int[,]>> Transformations = new Func<int[,], int[,]>[]{
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
        var map = new int[9];
        for (var i = 0; i < 9; ++i) map[i] = i + 1;
        rnd.Shuffle(map);
        Relabel(map);
    }

    public void RemoveCells(int seed, int hints) {
        var indexes = new Random(seed).Extract(Enumerable.Range(0, TotalCells).ToArray(), TotalCells - hints);
        foreach (var index in indexes) {
            RemoveCell(index);
        }
    }

    public void RemoveCells(string mask) {
        if (mask.Length > TotalCells) {
            // remove all non-digits and non-dots
            mask = new string(mask.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }
        for (var pos = 0; pos < TotalCells; ++pos) {
            var (x, y) = (pos % 9, pos / 9);
            var c = mask[pos];
            var value = c != '.' && c != '0' ? c - '0' : -1;
            if (value == -1) RemoveCell(y + 1, x + 1);
        }
    }

    public void Relabel(int[] map) {
        // validate d is a permutation of 1..9
        if (map.Length != 9 || map.Distinct().Count() != 9 || map.Min() != 1 || map.Max() != 9) {
            throw new ArgumentException("Invalid permutation");
        }
        for (var index = 0; index < TotalCells; ++index) {
            var value = GetCell(index).Value;
            if (value > 0) {
                SetCellValue(map[value - 1], index);
            }
        }
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