using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Betauer.Core.Sudoku;

public class SudokuBoard {
    public class Cell {
        public int Row { get; internal set; }
        public int Column { get; internal set; }
        public int Value { get; internal set; }
        public int Index { get; }
        public int GroupNo { get; }
        public Cell(int value, int index, int groupNo, int row, int column) {
            Value = value;
            Index = index;
            GroupNo = groupNo;
            Row = row;
            Column = column;
        }
    }

    internal List<Cell> Cells { get; }
    public const int TotalRows = 9;
    public const int TotalColumns = 9;
    public const int TotalCells = 81;

    private void InitializeCells() {
        for (var x = 0; x < TotalRows; x++) {
            for (var y = 0; y < TotalColumns; y++) {
                Cells.Add(new Cell(
                    value: -1,
                    index: x * TotalRows + y,
                    groupNo: (x / 3) + 3 * (y / 3) + 1,
                    row: x + 1,
                    column: y + 1
                ));
            }
        }
    }

    /// <summary>
    /// Creates empty Sudoku game object.
    /// </summary>
    public SudokuBoard(string? import = null) {
        Cells = new List<Cell>(TotalCells);
        InitializeCells();
        if (import != null) Import(import);
    }

    public Cell GetCell(int row, int column) => GetCell((row - 1) * TotalRows + column - 1);

    public Cell GetCell(int cellIndex) => Cells[cellIndex];

    public void SetCellValue(int value, int row, int column) => SetCellValue(value, (row - 1) * TotalRows + column - 1);

    public void SetCellValue(int value, int cellIndex) => Cells[cellIndex].Value = value == 0 ? -1 : value;

    /// <summary>
    /// Checks the board is already filled.
    /// </summary>
    public bool IsBoardFilled() => Cells.FirstOrDefault(cell => cell.Value == -1) == null;

    /// <summary>
    /// Returns whether table is empty.
    /// </summary>
    public bool IsTableEmpty() => Cells.FirstOrDefault(cell => cell.Value != -1) == null;

    /// <summary>
    /// Fills the game board with -1 which is the default for the empty state. 
    /// </summary>
    public void Clear() => Cells.ForEach(cell => SetCellValue(-1, cell.Index));
    
    /// <summary>
    /// Checks the specified cell can accept the specified value.
    /// </summary>
    public bool IsValidValueForTheCell(int val, Cell cell) {
        // Check the value whether exists in the 3x3 group.
        if (Cells.Where(c => c.Index != cell.Index && c.GroupNo == cell.GroupNo).FirstOrDefault(c2 => c2.Value == val) != null)
            return false;

        // Check the value whether exists in the row.
        if (Cells.Where(c => c.Index != cell.Index && c.Row == cell.Row).FirstOrDefault(c2 => c2.Value == val) != null)
            return false;

        // Check the value whether exists in the column.
        if (Cells.Where(c => c.Index != cell.Index && c.Column == cell.Column).FirstOrDefault(c2 => c2.Value == val) != null)
            return false;

        return true;
    }

    
    /// <summary>
    /// Check current state of the table is valid.
    /// </summary>
    /// <returns>Returns whether is table is valid or not.</returns>
    public bool CheckTableStateIsValid(bool ignoreEmptyCells = false) =>
        Cells
            .Where(cell => !ignoreEmptyCells || cell.Value != -1)
            .FirstOrDefault(cell => cell.Value != -1 && !IsValidValueForTheCell(cell.Value, cell)) == null;


    public void Import(string import) {
        if (import.Length > 81) {
            // remove all non-digits and non-dots
            import = new string(import.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }
        for (var i = 0; i < 81; ++i) {
            var (x, y) = (i % 9, i / 9);
            var c = import[i];
            var value = c != '.' && c != '0' ? c - '0' : -1;
            SetCellValue(value, y + 1, x + 1);
        }
    }

    public string Export(bool onlyLine = false) {
        var sb = new StringBuilder();
        for (var i = 0; i < 9; ++i) {
            if (!onlyLine && i > 0) sb.AppendLine();
            for (var j = 0; j < 9; ++j) {
                var value = GetCell(i + 1, j + 1).Value;
                if (value < 1) sb.Append('0');
                else sb.Append(value);
            }
        }
        return sb.ToString();
    }

    public bool SolveDancingLinks() {
        var data = new int[9, 9];
        for (var i = 0; i < 9; ++i) {
            for (var j = 0; j < 9; ++j) {
                var value = GetCell(i + 1, j + 1).Value;
                data[i, j] = value < 1 ? 0 : value;
            }
        }
        var solver = new DancingLinksSolver(data);
        if (!solver.Solve()) return false;
        for (var i = 0; i < 9; ++i) {
            for (var j = 0; j < 9; ++j) {
                SetCellValue(solver.Data[i, j], i + 1, j + 1);
            }
        }
        return true;
    }
   
    public bool SolveBacktrack(int seed = -1) {
        return new BacktrackSolver(this, seed).Solve();
    }
}