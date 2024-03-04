using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Betauer.Core.Sudoku;

// https://brutalsimplicity.github.io/2016/08/18/sudoku.html
// https://github.com/BrutalSimplicity/SudokuSolver

public class DancingLinksSolver {
    // Convenience class for tracking candidates
    class Candidate : IEnumerable {
        private readonly bool[] _values;
        private readonly int _numCandidates;

        public int Count { get; private set; }

        public Candidate(int numCandidates, bool initialValue) {
            _values = new bool[numCandidates];
            Count = 0;
            _numCandidates = numCandidates;

            for (var i = 1; i <= numCandidates; i++)
                this[i] = initialValue;
        }

        public bool this[int key] {
            // Allows candidates to be referenced by actual value (i.e. 1-9, rather than 0 - 8)
            get => _values[key - 1];

            // Automatically tracks the number of candidates
            set {
                Count += (_values[key - 1] == value) ? 0 : (value == true) ? 1 : -1;
                _values[key - 1] = value;
            }
        }

        public void SetAll(bool value) {
            for (var i = 1; i <= _numCandidates; i++)
                this[i] = value;
        }

        public override string ToString() {
            StringBuilder values = new StringBuilder();
            foreach (int candidate in this)
                values.Append(candidate);
            return values.ToString();
        }

        public IEnumerator GetEnumerator() {
            return new CandidateEnumerator(this);
        }

        // Enumerator simplifies iterating over candidates
        private class CandidateEnumerator : IEnumerator {
            private int _position;
            private readonly Candidate _candidate;

            public CandidateEnumerator(Candidate c) {
                _candidate = c;
                _position = 0;
            }

            // only iterates over valid candidates
            public bool MoveNext() {
                while (true) {
                    ++_position;
                    if (_position <= _candidate._numCandidates) {
                        if (_candidate[_position] == true)
                            return true;
                        else {
                        }
                    } else {
                        return false;
                    }
                }
            }

            public void Reset() {
                _position = 0;
            }

            public object Current => _position;
        }
    }


    // True values for row, grid, and region constraint matrices
    // mean that they contain that candidate, inversely,
    // True values in the candidate constraint matrix means that it
    // is a possible value for that cell.
    private readonly Candidate[,] _cellConstraintMatrix;
    private readonly Candidate[] _rowConstraintMatrix;
    private readonly Candidate[] _colConstraintMatrix;
    private readonly Candidate[,] _regionConstraintMatrix;

    // Actual puzzle grid (uses 0s for unsolved squares)
    public int[,] Data;

    // Another convenience structure. Easy and expressive way
    // of passing around row, column information.
    private readonly struct Cell {
        public readonly int Row;
        public readonly int Col;

        public Cell(int r, int c) {
            Row = r;
            Col = c;
        }
    }

    // helps avoid iterating over solved squares
    private readonly HashSet<Cell> _solved;
    private readonly HashSet<Cell> _unsolved;

    // Tracks the cells changed due to propagation (i.e. the rippled cells)
    private readonly Stack<HashSet<Cell>> _changed;

    // keeps cell counts by keeping them in buckets
    // this allows the cell with the least candidates
    // (minimum count) to be selected in O(1)
    private readonly HashSet<Cell>[] _bucketList;

    // Tracks the number of steps a solution takes
    public int Steps;

    private void InitializeMatrices() {
        for (int row = 0; row < 9; row++) {
            for (int col = 0; col < 9; col++) {
                // if the square is solved update the candidate list
                // for the row, column, and region
                if (Data[row, col] > 0) {
                    int candidate = Data[row, col];
                    _rowConstraintMatrix[row][candidate] = true;
                    _colConstraintMatrix[col][candidate] = true;
                    _regionConstraintMatrix[row / 3, col / 3][candidate] = true;
                }
            }
        }
    }

    private void PopulateCandidates() {
        //Add possible candidates by checking
        //the rows, columns and grid
        for (int row = 0; row < 9; row++) {
            for (int col = 0; col < 9; col++) {
                //if solved, then there are no possible candidates
                if (Data[row, col] > 0) {
                    _cellConstraintMatrix[row, col].SetAll(false);
                    _solved.Add(new Cell(row, col));
                } else {
                    // populate each cell with possible candidates
                    // by checking the row, col, and grid associated 
                    // with that cell
                    foreach (int candidate in _rowConstraintMatrix[row])
                        _cellConstraintMatrix[row, col][candidate] = false;
                    foreach (int candidate in _colConstraintMatrix[col])
                        _cellConstraintMatrix[row, col][candidate] = false;
                    foreach (int candidate in _regionConstraintMatrix[row / 3, col / 3])
                        _cellConstraintMatrix[row, col][candidate] = false;

                    Cell c = new Cell(row, col);
                    _bucketList[_cellConstraintMatrix[row, col].Count].Add(c);
                    _unsolved.Add(c);
                }
            }
        }
    }

    private Cell NextCell() {
        if (_unsolved.Count == 0)
            return new Cell(-1, -1); // easy way to signal a solved puzzle

        foreach (var t in _bucketList)
            if (t.Count > 0)
                return t.First();

        // should never execute
        return new Cell(99, 99);
    }

    // Backtracking method. Undoes the specified selection
    private void UnselectCandidate(Cell aCell, int candidate) {
        // 1) Remove selected candidate from grid
        Data[aCell.Row, aCell.Col] = 0;

        // 2) Add that candidate back to the cell constraint matrix.
        //    Since it wasn't selected, it can still be selected in the 
        //    future
        _cellConstraintMatrix[aCell.Row, aCell.Col][candidate] = true;

        // 3) Put cell back in the bucket list
        _bucketList[_cellConstraintMatrix[aCell.Row, aCell.Col].Count].Add(aCell);

        // 4) Remove the candidate from the row, col, and region constraint matrices
        _rowConstraintMatrix[aCell.Row][candidate] = false;
        _colConstraintMatrix[aCell.Col][candidate] = false;
        _regionConstraintMatrix[aCell.Row / 3, aCell.Col / 3][candidate] = false;

        // 5) Add the candidate back to any cells that changed from
        //    its selection (propagation).
        foreach (Cell c in _changed.Pop()) {
            // shift affected cells up the bucket list
            _bucketList[_cellConstraintMatrix[c.Row, c.Col].Count].Remove(c);
            _bucketList[_cellConstraintMatrix[c.Row, c.Col].Count + 1].Add(c);
            _cellConstraintMatrix[c.Row, c.Col][candidate] = true;
        }

        // 6) Add the cell back to the list of unsolved
        _solved.Remove(aCell);
        _unsolved.Add(aCell);
    }

    private void SelectCandidate(Cell aCell, int candidate) {
        HashSet<Cell> changedCells = new HashSet<Cell>();

        // place candidate on grid
        Data[aCell.Row, aCell.Col] = candidate;

        // remove from bucket list
        _bucketList[_cellConstraintMatrix[aCell.Row, aCell.Col].Count].Remove(aCell);

        // remove candidate from cell constraint matrix
        _cellConstraintMatrix[aCell.Row, aCell.Col][candidate] = false;

        // add the candidate to the cell, row, col, region constraint matrices
        _colConstraintMatrix[aCell.Col][candidate] = true;
        _rowConstraintMatrix[aCell.Row][candidate] = true;
        _regionConstraintMatrix[aCell.Row / 3, aCell.Col / 3][candidate] = true;

        /**** RIPPLE ACROSS COL, ROW, REGION ****/

        // (propagation)
        // remove candidates across unsolved cells in the same
        // row and col.
        for (var i = 0; i < 9; i++) {
            // only change unsolved cells containing the candidate
            if (Data[aCell.Row, i] == 0) {
                if (_cellConstraintMatrix[aCell.Row, i][candidate] == true) {
                    // shift affected cells down the bucket list
                    _bucketList[_cellConstraintMatrix[aCell.Row, i].Count].Remove(new Cell(aCell.Row, i));
                    _bucketList[_cellConstraintMatrix[aCell.Row, i].Count - 1].Add(new Cell(aCell.Row, i));

                    // remove the candidate
                    _cellConstraintMatrix[aCell.Row, i][candidate] = false;

                    //update changed cells (for backtracking)
                    changedCells.Add(new Cell(aCell.Row, i));
                }
            }
            // only change unsolved cells containing the candidate
            if (Data[i, aCell.Col] == 0) {
                if (_cellConstraintMatrix[i, aCell.Col][candidate] == true) {
                    // shift affected cells down the bucket list
                    _bucketList[_cellConstraintMatrix[i, aCell.Col].Count].Remove(new Cell(i, aCell.Col));
                    _bucketList[_cellConstraintMatrix[i, aCell.Col].Count - 1].Add(new Cell(i, aCell.Col));

                    // remove the candidate
                    _cellConstraintMatrix[i, aCell.Col][candidate] = false;

                    //update changed cells (for backtracking)
                    changedCells.Add(new Cell(i, aCell.Col));
                }
            }
        }

        // (propagation)
        // remove candidates across unsolved cells in the same
        // region.
        var grid_row_start = aCell.Row / 3 * 3;
        var grid_col_start = aCell.Col / 3 * 3;
        for (var row = grid_row_start; row < grid_row_start + 3; row++)
            for (var col = grid_col_start; col < grid_col_start + 3; col++)
                // only change unsolved cells containing the candidate
                if (Data[row, col] == 0) {
                    if (_cellConstraintMatrix[row, col][candidate] == true) {
                        // shift affected cells down the bucket list
                        _bucketList[_cellConstraintMatrix[row, col].Count].Remove(new Cell(row, col));
                        _bucketList[_cellConstraintMatrix[row, col].Count - 1].Add(new Cell(row, col));

                        // remove the candidate
                        _cellConstraintMatrix[row, col][candidate] = false;

                        //update changed cells (for backtracking)
                        changedCells.Add(new Cell(row, col));
                    }
                }

        // add cell to solved list
        _unsolved.Remove(aCell);
        _solved.Add(aCell);
        _changed.Push(changedCells);
    }

    private bool SolveRecurse(Cell nextCell) {
        // Our base case: No more unsolved cells to select, 
        // thus puzzle solved
        if (nextCell.Row == -1)
            return true;

        // Loop through all candidates in the cell
        foreach (int candidate in _cellConstraintMatrix[nextCell.Row, nextCell.Col]) {
            // writer.WriteLine("{4} -> ({0}, {1}) : {2} ({3})", nextCell.row, nextCell.col,
                // cellConstraintMatrix[nextCell.row, nextCell.col], cellConstraintMatrix[nextCell.row, nextCell.col].Count, steps++);

            SelectCandidate(nextCell, candidate);

            // Move to the next cell.
            // if it returns false backtrack
            if (SolveRecurse(NextCell()) == false) {
                ++Steps;
                // writer.WriteLine("{0} -> BACK", steps);
                UnselectCandidate(nextCell, candidate);
                continue;
            } else // if we receive true here this means the puzzle was solved earlier
                return true;
        }

        // return false if path is unsolvable
        return false;
    }

    public bool Solve() {
        Steps = 1;
        return SolveRecurse(NextCell());
    }

    public DancingLinksSolver(int[,] initialGrid) {
        Data = new int[9, 9];
        _cellConstraintMatrix = new Candidate[9, 9];
        _rowConstraintMatrix = new Candidate[9];
        _colConstraintMatrix = new Candidate[9];
        _regionConstraintMatrix = new Candidate[9, 9];
        _solved = new HashSet<Cell>();
        _unsolved = new HashSet<Cell>();
        _changed = new Stack<HashSet<Cell>>();
        _bucketList = new HashSet<Cell>[10];
        Steps = 0;

        // initialize constraints

        for (var row = 0; row < 9; row++) {
            for (var col = 0; col < 9; col++) {
                // copy grid, and turn on all Candidates for every cell
                Data[row, col] = initialGrid[row, col];
                _cellConstraintMatrix[row, col] = new Candidate(9, true);
            }
        }

        for (var i = 0; i < 9; i++) {
            _rowConstraintMatrix[i] = new Candidate(9, false);
            _colConstraintMatrix[i] = new Candidate(9, false);
            _bucketList[i] = new HashSet<Cell>();
        }
        _bucketList[9] = new HashSet<Cell>();

        for (var row = 0; row < 3; row++)
            for (var col = 0; col < 3; col++)
                _regionConstraintMatrix[row, col] = new Candidate(9, false);

        InitializeMatrices();
        PopulateCandidates();
    }
}