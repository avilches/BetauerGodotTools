using System;
using System.Collections.Immutable;
using System.Linq;

namespace Betauer.Core.Sudoku;

public static class SudokuShuffleExtensions {
    
    public static readonly ImmutableList<Func<int[,], int, int[,]>> ColumnTransformationsInGroup = new Func<int[,], int, int[,]>[] {
        (data, group) => data,                                                            // 123 -> 123                                             
        (data, group) => data.YxSwapColumns(0 + group * 3, 1 + group * 3), // 123 -> 213                       
        (data, group) => data.YxSwapColumns(0 + group * 3, 2 + group * 3), // 123 -> 321                       
        (data, group) => data.YxSwapColumns(1 + group * 3, 2 + group * 3), // 123 -> 132                      
        (data, group) => {                                                                 // 123 -> 321 -> 231
            data = data.YxSwapColumns(0 + group * 3, 2 + group * 3); // 123 -> 321
            return data.YxSwapColumns(0 + group * 3, 1 + group * 3);        // 321 -> 231
        },
        (data, group) => {                                                                 // 123 -> 213 -> 312
            data = data.YxSwapColumns(0 + group * 3, 1 + group * 3); // 123 -> 213
            return data.YxSwapColumns(0 + group * 3, 2 + group * 3); //        213 -> 312
        }
    }.ToImmutableList();                       
    
    public static readonly ImmutableList<Func<int[,], int, int[,]>> RowTransformationsInGroup = new Func<int[,], int, int[,]>[] {
        (data, group) => data,                                                           // 123 -> 123                                             
        (data, group) => data.YxSwapRows(0 + group * 3, 1 + group * 3), // 123 -> 213                       
        (data, group) => data.YxSwapRows(0 + group * 3, 2 + group * 3), // 123 -> 321                       
        (data, group) => data.YxSwapRows(1 + group * 3, 2 + group * 3), // 123 -> 132                      
        (data, group) => {                                                         // 123 -> 321 -> 231
            data = data.YxSwapRows(0 + group * 3, 2 + group * 3); // 123 -> 321
            return data.YxSwapRows(0 + group * 3, 1 + group * 3); //        321 -> 231
        },
        (data, group) => {                                                         // 123 -> 213 -> 312
            data = data.YxSwapRows(0 + group * 3, 1 + group * 3); // 123 -> 213
            return data.YxSwapRows(0 + group * 3, 2 + group * 3); //        213 -> 312
        }
    }.ToImmutableList();                       

    
    public static readonly ImmutableList<Func<int[,], int[,]>> ColumnGroupTransformations = new Func<int[,], int[,]>[] {
        (data) => data,                                                  // 111222333 -> 111222333                                             
        (data) => data.YxSwapColumns(0, 3, 3), // 111222333 -> 222111333                       
        (data) => data.YxSwapColumns(0, 6, 3), // 111222333 -> 333222111                       
        (data) => data.YxSwapColumns(3, 6, 3), // 111222333 -> 111333222                      
        (data) => {                                                      // 111222333 -> 333222111 -> 222333111
            data = data.YxSwapColumns(0, 6, 3); // 111222333 -> 333222111
            return data.YxSwapColumns(0, 3, 3); //              333222111 -> 222333111
        },
        (data) => {                                                       // 111222333 -> 222111333 -> 333111222
            data = data.YxSwapColumns(0, 3, 3); // 111222333 -> 222111333
            return data.YxSwapColumns(0, 6, 3); //              222111333 -> 333111222
        }
    }.ToImmutableList();

    public static readonly ImmutableList<Func<int[,], int[,]>> RowGroupTransformations = new Func<int[,], int[,]>[] {
        (data) => data,                                           // 111222333 -> 111222333                                             
        (data) => data.YxSwapRows(0, 3, 3), // 111222333 -> 222111333                       
        (data) => data.YxSwapRows(0, 6, 3), // 111222333 -> 333222111                       
        (data) => data.YxSwapRows(3, 6, 3), // 111222333 -> 111333222                      
        (data) => {                                               // 111222333 -> 333222111 -> 222333111
            data = data.YxSwapRows(0, 6, 3); // 111222333 -> 333222111
            return data.YxSwapRows(0, 3, 3); //              333222111 -> 222333111
        },
        (data) => {                                               // 111222333 -> 222111333 -> 333111222
            data = data.YxSwapRows(0, 3, 3); // 111222333 -> 222111333
            return data.YxSwapRows(0, 6, 3); //              222111333 -> 333111222
        }
    }.ToImmutableList();

    public static readonly ImmutableList<Func<int[,], int[,]>> GlobalTransformations = new Func<int[,], int[,]>[] {
        data => data, 
        data => data.YxRotate90(),
        data => data.YxRotate180(),
        data => data.YxRotateMinus90(),
        data => data.YxFlipDiagonal(),
        data => data.YxFlipDiagonalSecondary(),
        data => data.YxFlipH(),
        data => data.YxFlipV(),
    }.ToImmutableList();

    public static void Relabel(this SudokuBoard sudoku, int seed) {
        var mapping = Enumerable.Range(1, SudokuBoard.TotalDigits).ToArray();
        new Random(seed).Shuffle(mapping);
        Relabel(sudoku, mapping);
    }

    public static void Relabel(this SudokuBoard sudoku, int[] map) {
        // validate d is a permutation of 1..9
        if (map.Length != 9 || map.Distinct().Count() != SudokuBoard.TotalDigits || map.Min() != 1 || map.Max() != SudokuBoard.TotalDigits) {
            throw new ArgumentException("Invalid permutation");
        }
        Enumerable.Range(0, SudokuBoard.TotalCells).ForEach(index => {
            var cell = sudoku.GetCell(index);
            var value = cell.Value;
            if (value > 0) {
                cell.Value = map[value - 1];
            }
        });
    }

    public static void Shuffle(this SudokuBoard sudoku, int seed) {
        var rnd = new Random(seed);
        var data = sudoku.CreateGrid();
        // Mix the columns inside every column group
        data = rnd.Next(ColumnTransformationsInGroup).Invoke(data, 0);
        data = rnd.Next(ColumnTransformationsInGroup).Invoke(data, 1);
        data = rnd.Next(ColumnTransformationsInGroup).Invoke(data, 2);
        // Mix the rows inside every row group
        data = rnd.Next(RowTransformationsInGroup).Invoke(data, 0);
        data = rnd.Next(RowTransformationsInGroup).Invoke(data, 1);
        data = rnd.Next(RowTransformationsInGroup).Invoke(data, 2);
        // Mix column groups
        data = rnd.Next(ColumnGroupTransformations).Invoke(data);
        // Mix row groups
        data = rnd.Next(RowGroupTransformations).Invoke(data);
        // Apply only one global transformation (rotation, flip or none)
        data = rnd.Next(GlobalTransformations).Invoke(data);
        sudoku.Import(data);
    }
}