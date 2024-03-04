using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Sudoku;

// https://github.com/firateski/SudokuLibrary/tree/master
// https://github.com/firateski/SudokuGameWindowsForms

public class BacktrackSolver {
    private readonly SudokuBoard _sudokuBoard;
    private static readonly int[] Numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    /// <summary>
    /// Cell indexes that excludes from backtracking because they already have a initial value (the hints)
    /// </summary>
    private readonly List<int> _filledCells;

    /// <summary>
    /// The list to use for backtracking while solving the processes. Each list of specified index represents the blacklist of the cell.
    /// </summary>
    private readonly List<List<int>> _blackListsCells;
    
    private readonly Random? _random;

    public BacktrackSolver(SudokuBoard sudoku, int seed = -1) {
        _sudokuBoard = sudoku;
        _random = seed == -1 ? null : new Random(seed);
        _blackListsCells = new List<List<int>>(SudokuBoard.TotalCells);
        for (var index = 0; index < SudokuBoard.TotalCells; index++) {
            _blackListsCells.Add(new List<int>());
        }
        _filledCells = _sudokuBoard.Cells.FindAll(cell => cell.Value != -1).Select(cell => cell.Index).ToList();
    }

    /// <summary>
    /// Creates solved state to the game board and returns whether the puzzle solved.
    /// </summary>
    /// <param name="seed">Set it to true to see a different result for each solution.</param>
    /// <returns>Returns whether the board solved.</returns>
    public bool Solve() {
        if (!_sudokuBoard.IsValid()) return false;

        var currentCellIndex = 0;
        while (currentCellIndex < SudokuBoard.TotalCells) {
            // If the current cell index is protected (which means it was inner the current state of the board), pass it.
            if (_filledCells.Contains(currentCellIndex)) {
                ++currentCellIndex;
                continue;
            }

            // Clear blacklists of the indexes after the current index.
            ClearBlackList(startCleaningFromThisIndex: currentCellIndex + 1);
            var currentCell = _sudokuBoard.GetCell(currentCellIndex);
            var nextValidNumber = GetNextValidNumber(currentCellIndex);

            // No valid number found for the cell. Let's backtrack.
            if (nextValidNumber == 0) {
                // Let's backtrack
                currentCellIndex = BacktrackTo(currentCellIndex);
            } else {
                // Set found valid value to current cell.
                _sudokuBoard.SetCellValue(nextValidNumber, currentCell.Index);
                ++currentCellIndex;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Backtracking operation for the cell specified with index.
    /// </summary>
    private int BacktrackTo(int index) {
        // Pass over the protected cells.
        while (_filledCells.Contains(--index)) ;

        // Get the back-tracked Cell.
        SudokuBoard.Cell backTrackedCell = _sudokuBoard.GetCell(index);

        // Add the value to the black-list of the back-tracked cell.
        AddToBlacklist(backTrackedCell.Value, cellIndex: index);

        // Reset the back-tracked cell value.
        backTrackedCell.Value = -1;

        // Reset the blacklist starting from the next one of the current tracking cell.
        ClearBlackList(startCleaningFromThisIndex: index + 1);

        return index;
    }

    /// <summary>
    /// Returns a valid number for the specified cell index, excluding (and updating) the blacklisted numbers.
    /// It can return 0 if no valid number found.
    /// </summary>
    private int GetNextValidNumber(int cellIndex) {
        var validNumbers = Numbers.Where(x => !_blackListsCells[cellIndex].Contains(x)).ToArray();

        if (validNumbers.Length == 0) return 0;
        var pos = _random?.Next(validNumbers.Length) ?? 0;
        var nextValidNumber = validNumbers[pos];

        // Try to get valid (random) value for the current cell, if no any valid value break the loop.
        while (nextValidNumber != 0) {
            SudokuBoard.Cell currentCell = _sudokuBoard.GetCell(cellIndex);

            if (currentCell.IsValidValue(nextValidNumber)) {
                // Valid number found!
                break;
            }
            AddToBlacklist(nextValidNumber, cellIndex);
            nextValidNumber = GetNextValidNumber(cellIndex);
        }
        return nextValidNumber;
    }

    /// <summary>
    /// Add given value into the specified index of the blacklist. 
    /// </summary>
    private void AddToBlacklist(int value, int cellIndex) => _blackListsCells[cellIndex].Add(value);

    /// <summary>
    /// Initializes the black lists of the cells.
    /// </summary>
    /// <param name="startCleaningFromThisIndex">Clear the rest of the blacklist starting from the index.</param>
    private void ClearBlackList(int startCleaningFromThisIndex = 0) {
        for (var index = startCleaningFromThisIndex; index < _blackListsCells.Count; index++)
            _blackListsCells[index].Clear();
    }
}