using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Checks the specified cell can accept the specified value.
        /// </summary>
        public bool IsValidValue(int val) {
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
        for (var i = 0; i < TotalCells; ++i) {
            SetCellValue(other.GetCell(i).Value, i);
        }
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

    public Cell GetCell(int row, int column) => GetCell((row - 1) * TotalRows + column - 1);

    public Cell GetCell(int cellIndex) => Cells[cellIndex];

    public void SetCellValue(int value, int row, int column) {
        SetCellValue(value, (row - 1) * TotalRows + column - 1);
    }

    public void SetCellValue(int value, int cellIndex) {
        Cells[cellIndex].Value = value == 0 ? -1 : value;
    }

    /// <summary>
    /// Checks the board is already filled.
    /// </summary>
    public bool IsBoardFilled() => Cells.All(cell => cell.Value != -1);

    /// <summary>
    /// Fills the game board with -1 which is the default for the empty state. 
    /// </summary>
    public void Clear() => Cells.ForEach(cell => SetCellValue(-1, cell.Index));

    /// <summary>
    /// Check current state of the table is valid.
    /// </summary>
    /// <returns>Returns whether is table is valid or not.</returns>
    public bool IsValid(bool ignoreEmptyCells = false) =>
        Cells.All(cell => cell.Value == -1 || cell.IsValidValue()) ;

    public Cell? GetFirstInvalidCell() =>
        Cells.FirstOrDefault(cell => cell.Value != -1 && !cell.IsValidValue());


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

    public string Export(bool onlyLine = false) {
        var sb = new StringBuilder();
        for (var y = 0; y < 9; ++y) {
            if (!onlyLine && y > 0) sb.AppendLine();
            for (var x = 0; x < 9; ++x) {
                var value = GetCell(y + 1, x + 1).Value;
                if (value < 1) sb.Append('0');
                else sb.Append(value);
            }
        }
        return sb.ToString();
    }

    public bool SolveDancingLinks() {
        var solver = new DancingLinksSolver(CreateGrid());
        if (!solver.Solve()) return false;
        Import(solver.Data);
        return true;
    }

    public bool SolveBacktrack(int seed = -1) {
        return new BacktrackSolver(this, seed).Solve();
    }

    private static Func<int[,], int[,]>[] Trans = {
        null, // this is for a random relabel
        (data) => data.YxFlipDiagonal(),
        (data) => data.YxFlipDiagonalSecondary(),
        (data) => data.YxRotate90(),
        (data) => data.YxRotate180(),
        (data) => data.YxRotateMinus90(),
        (data) => data.YxFlipH(),
        (data) => data.YxFlipV(),
        (data) => data.YxSwapColumns(0, 1),
        (data) => data.YxSwapColumns(0, 2),
        (data) => data.YxSwapColumns(1, 2),
        (data) => data.YxSwapColumns(3, 4),
        (data) => data.YxSwapColumns(3, 5),
        (data) => data.YxSwapColumns(4, 5),
        (data) => data.YxSwapColumns(6, 7),
        (data) => data.YxSwapColumns(6, 8),
        (data) => data.YxSwapColumns(7, 8),
        (data) => data.YxSwapRows(0, 1),
        (data) => data.YxSwapRows(0, 2),
        (data) => data.YxSwapRows(1, 2),
        (data) => data.YxSwapRows(3, 4),
        (data) => data.YxSwapRows(3, 5),
        (data) => data.YxSwapRows(4, 5),
        (data) => data.YxSwapRows(6, 7),
        (data) => data.YxSwapRows(6, 8),
        (data) => data.YxSwapRows(7, 8),
        (data) => data.YxSwapRows(0, 3, 3),
        (data) => data.YxSwapRows(0, 6, 3),
        (data) => data.YxSwapRows(3, 6, 3),
        (data) => data.YxSwapColumns(0, 3, 3),
        (data) => data.YxSwapColumns(0, 6, 3),
        (data) => data.YxSwapColumns(3, 6, 3)
    };

    public void Relabel(Random rnd) {
        var map = new int[9];
        for (var i = 0; i < 9; ++i) map[i] = i + 1;
        rnd.Shuffle(map);
        Relabel(map);
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
            var func = rnd.Next(Trans);
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