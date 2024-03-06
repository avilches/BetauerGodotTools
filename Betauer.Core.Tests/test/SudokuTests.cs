using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Betauer.Core.Sudoku;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[Betauer.TestRunner.Test]
// [Only]
public class SudokuTests {
    [Betauer.TestRunner.Test(Description = "IsValid() tests")]
    public void NoSolution() {
        var sudoku = new SudokuBoard("110000000000000000000000000000000000000000000000000000000000000000000000000000000");
        Assert.False(sudoku.IsValid());
        Assert.False(sudoku.Solve());
        Assert.False(sudoku.HasUniqueSolution());
        Assert.False(sudoku.HasSolutions());
    }

    [Betauer.TestRunner.Test(Description = "IsValid() tests")]
    public void InvalidTest() {
        Assert.False(new SudokuBoard("110000000000000000000000000000000000000000000000000000000000000000000000000000000").IsValid()); // same row
        Assert.False(new SudokuBoard("100000000000000000000000000000000000000000000000000000000000000000000000100000000").IsValid()); // same col
        Assert.False(new SudokuBoard("100000010000000000000000000000000000000000000000000000000000000000000000000000000").IsValid()); // same group
        Assert.False(new SudokuBoard("113456789456789123789123456214365897365897214897214365531642978642978531978531642").IsValid());
    }

    [Betauer.TestRunner.Test(Description = "Test the valid values from every cell")]
    public void BasicValidValuesTest() {
        // No candidate
        var sudoku1 = new SudokuBoard("953187642624539178817462395589713264132654789476928513745296831268341957391875426");
        CollectionAssert.AreEqual(sudoku1.GetCell(0).Candidates(), new List<int> { 9 });
        CollectionAssert.AreEqual(sudoku1.GetCell(10).Candidates(), new List<int> { 2 });
        CollectionAssert.AreEqual(sudoku1.GetCell(80).Candidates(), new List<int> { 6 });

        // One candidate
        var sudoku2 = new SudokuBoard("003187642624539178817462395589713264132654789476928513745296831268341957391875426");
        CollectionAssert.AreEqual(sudoku2.GetCell(0).Candidates(), new List<int> { 9 });
        CollectionAssert.AreEqual(sudoku2.GetCell(1).Candidates(), new List<int> { 5 });
    }

    [Betauer.TestRunner.Test(Description = "Test the candidates changes after setting a value")]
    public void ValidValuesTest() {
        var sudoku = new SudokuBoard(".....7.95.....1...86..2.....2..73..85......6...3..49..3.5...41724................");
        CollectionAssert.AreEqual(sudoku.GetCell(0).Candidates(), new List<int> { 1, 4 });
        CollectionAssert.AreEqual(sudoku.GetCell(4).Candidates(), new List<int> { 3, 4, 6, 8 });
        CollectionAssert.AreEqual(sudoku.GetCell(9).Candidates(), new List<int> { 4, 7, 9 });
        CollectionAssert.AreEqual(sudoku.GetCell(11).Candidates(), new List<int> { 2, 4, 7, 9 });

        // The 4 disappear from the candidates of the same row, col and group
        sudoku.GetCell(0).Value = 4;
        CollectionAssert.AreEqual(sudoku.GetCell(0).Candidates(), new List<int> { 4 });
        CollectionAssert.AreEqual(sudoku.GetCell(4).Candidates(), new List<int> { 3, 6, 8 }); // Same row
        CollectionAssert.AreEqual(sudoku.GetCell(9).Candidates(), new List<int> { 7, 9 }); // Same col
        CollectionAssert.AreEqual(sudoku.GetCell(11).Candidates(), new List<int> { 2, 7, 9 }); // Same grup

        sudoku.GetCell(0).Remove();
        CollectionAssert.AreEqual(sudoku.GetCell(0).Candidates(), new List<int> { 1, 4 });
        CollectionAssert.AreEqual(sudoku.GetCell(4).Candidates(), new List<int> { 3, 4, 6, 8 });
        CollectionAssert.AreEqual(sudoku.GetCell(9).Candidates(), new List<int> { 4, 7, 9 });
        CollectionAssert.AreEqual(sudoku.GetCell(11).Candidates(), new List<int> { 2, 4, 7, 9 });
    }

    [Betauer.TestRunner.Test(Description = "Test the solution values match the original candidates")]
    public void ValidValuesMatchesAfterSolvingTest() {
        var sudoku = new SudokuBoard(".....7.95.....1...86..2.....2..73..85......6...3..49..3.5...41724................");

        // Validate every candidate is valid
        sudoku.Cells.ForEach(cell =>
            cell.Candidates().ForEach(c => Assert.That(cell.AcceptValue(c)))
        );

        var solved = new SudokuBoard(sudoku);
        solved.Solve();

        // Test the solution values match the original candidates
        solved.Cells.ForEach((cell, idx) => Assert.That(sudoku.GetCell(idx).Candidates().Contains(cell.Value)));
    }

    [Betauer.TestRunner.Test]
    public void TestGridImportExport() {
        var import = "953187642624539178817462395589713264132654789476928513745296831268341957391875426";
        var sudoku = new SudokuBoard(import);
        var other = new SudokuBoard(sudoku.CreateGrid());
        Assert.AreEqual(other.Export(), import);
    }

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic (fixed next number)")]
    public void GenerateBacktrackingDeterministicNoSeed() {
        var import = "".PadRight(81, '0');
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

        Assert.That(sudoku.Generate());
        Console.WriteLine(sudoku.Export());

        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.IsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(), "123456789456789123789123456214365897365897214897214365531642978642978531978531642");
    }

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic, seed, random next number")]
    public void GenerateBacktrackingDeterministicRandom() {
        var import = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

        Assert.That(sudoku.Generate(902));
        Console.WriteLine(sudoku.Export());

        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.IsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(), "953187642624539178817462395589713264132654789476928513745296831268341957391875426");
    }

    [Betauer.TestRunner.Test]
    public void CompareSolveAlgorithmsTest() {
        foreach (var sudokuString in FromFile("Betauer.Core.Tests/test/Sudokus-compare.txt")) {
            Console.WriteLine(sudokuString);

            var sudokuDancing = new SudokuBoard(sudokuString);
            Assert.That(sudokuDancing.IsBoardFilled(), Is.False);
            Assert.That(sudokuDancing.IsValid(), Is.True);
            Assert.True(sudokuDancing.Solve());
            Assert.That(sudokuDancing.IsBoardFilled(), Is.True);
            Assert.That(sudokuDancing.IsValid(), Is.True);

            var sudokuBacktrack = new SudokuBoard(sudokuString);
            Assert.That(sudokuBacktrack.IsBoardFilled(), Is.False);
            Assert.That(sudokuBacktrack.IsValid(), Is.True);
            Assert.True(sudokuBacktrack.Generate());
            Assert.That(sudokuBacktrack.IsBoardFilled(), Is.True);
            Assert.That(sudokuBacktrack.IsValid(), Is.True);

            Assert.AreEqual(sudokuBacktrack.Export(), sudokuDancing.Export());
        }
    }

    [Betauer.TestRunner.Test(Description = "Generate using dancing links deterministic")]
    public void SolveDancingLinks() {
        var import = "120000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

        sudoku.Solve();
        Console.WriteLine(sudoku.Export());

        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.IsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(), "123456789789123456456789123312845967697312845845697312231574698968231574574968231");
    }

    [Betauer.TestRunner.Test]
    public void SolveDancingLinksTest() {
        foreach (var sudokuString in FromFile("Betauer.Core.Tests/test/Sudokus.txt")) {
            SudokuBoard sudoku = new SudokuBoard(sudokuString);
            Assert.That(sudoku.IsBoardFilled(), Is.False);
            Assert.That(sudoku.IsValid(), Is.True);

            Console.WriteLine(sudoku.Export());
            Assert.That(sudoku.Solve(), Is.True);

            Assert.That(sudoku.IsBoardFilled(), Is.True);
            Assert.That(sudoku.IsValid(), Is.True);
        }
    }

    [Betauer.TestRunner.Test]
    public void SolveDancingLinksMultipleSolutionTest() {
        SudokuBoard sudoku = new SudokuBoard(".......2.8.......6.1.2.5...9.54....8.........3....85.1...3.2.8.4.......9.7..6....");
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

        Assert.That(sudoku.HasUniqueSolution(), Is.False);
        Assert.That(sudoku.HasSolutions(), Is.True);

        Console.WriteLine(sudoku.Export());
        var solutions10 = sudoku.GetSolutions(10).ToList();
        Assert.That(solutions10.Count, Is.EqualTo(10));
        var solutions = sudoku.GetSolutions(0).ToList();
        Assert.That(solutions.Count, Is.EqualTo(1204));

        solutions.ForEach(s => {
            Assert.That(s.IsBoardFilled(), Is.True);
            Assert.That(s.IsValid(), Is.True);
        });
    }

    [Betauer.TestRunner.Test(Description = "Remove cells by mask")]
    public void RemoveCellsByMaskTest() {
        var import = "".PadRight(81, '1');
        var sudoku = new SudokuBoard(import);
        sudoku.RemoveCells("""
                           003.2.6..
                           9..3.5..1
                           ..18.64..
                           ..81.29..
                           7.......8
                           ..67.82..
                           ..26.95..
                           8..2.3..9
                           ..5.1.3..
                           """);
        Console.WriteLine(sudoku.Export());
        Assert.AreEqual(sudoku.Export(), "001010100100101001001101100001101100100000001001101100001101100100101001001010100");
    }

    [Betauer.TestRunner.Test(Description = "Remove cells random")]
    public void RemoveCellsRandom() {
        var import = "".PadRight(81, '1');
        var sudoku = new SudokuBoard(import);
        sudoku.RemoveCells(1331, 51);
        Console.WriteLine(sudoku.Export());
        Assert.AreEqual(sudoku.Export().Count("0"), 30);
        Assert.AreEqual(sudoku.Export(), "111010001110100111111110111111000001111101011111101001010011111010111110100101000");
    }

    [Betauer.TestRunner.Test]
    public void TransformationTests() {
        var import = "953187642624539178817462395589713264132654789476928513745296831268341957391875426";
        var sudoku = new SudokuBoard(import);
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxFlipDiagonal()).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxFlipDiagonalSecondary()).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxRotate90()).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxRotate180()).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxRotateMinus90()).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxFlipH()).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxFlipV()).IsValid());

        // Swap columns in the same group
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(0, 1)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(0, 2)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(1, 2)).IsValid());

        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(3, 4)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(3, 5)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(4, 5)).IsValid());

        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(6, 7)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(6, 8)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(7, 8)).IsValid());

        // Swap rows in the same row group
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(0, 1)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(0, 2)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(1, 2)).IsValid());

        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(3, 4)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(3, 5)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(4, 5)).IsValid());

        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(6, 7)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(6, 8)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(7, 8)).IsValid());

        // Swap the 3 rows group to another group
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(0, 3, 3)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(0, 6, 3)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapRows(3, 6, 3)).IsValid());

        // Swap the 3 rows group to another group
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(0, 3, 3)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(0, 6, 3)).IsValid());
        Assert.True(new SudokuBoard(sudoku.CreateGrid().YxSwapColumns(3, 6, 3)).IsValid());
    }

    [Betauer.TestRunner.Test]
    public void Relabel1Tests() {
        var import = "000011111111111111111111111111111111111112222222222222222222222222222222223456789";
        var sudoku = new SudokuBoard(import);
        sudoku.Relabel(new[] { 2, 1, 3, 4, 5, 6, 7, 8, 9 });
        Console.WriteLine(sudoku.Export());
        Assert.That(sudoku.Export(), Is.EqualTo("000022222222222222222222222222222222222221111111111111111111111111111111113456789"));
    }

    [Betauer.TestRunner.Test]
    public void Relabel2Tests() {
        var import = "000087642624539178817462395589713264132654789476928513745296831268341957391875426";
        var sudoku = new SudokuBoard(import);
        sudoku.Relabel(new[] { 2, 3, 4, 5, 6, 7, 8, 9, 1 });
        Console.WriteLine(sudoku.Export());
        Assert.That(sudoku.Export(), Is.EqualTo("000098753735641289928573416691824375243765891587139624856317942379452168412986537"));
        Assert.True(sudoku.IsValid());
    }

    [Betauer.TestRunner.Test]
    public void ShuffleTests() {
        var import = "953187642624539178817462395589713264132654789476928513745296831268341957391875426";
        var sudoku = new SudokuBoard(import);
        for (var seed = 0; seed < 100; seed++) {
            sudoku.Shuffle(seed);
            Assert.True(sudoku.IsValid());
            sudoku.Relabel(seed);
            Assert.True(sudoku.IsValid());
        }
    }

    [Betauer.TestRunner.Test]
    public void Shuffle1Tests() {
        // var import = "953187642624539178817462395589713264132654789476928513745296831268341957391875426";
        // var import = "1111111111111111111111111111111111111112222222222222222222222222222222222222222222";
        // var import = "1111111112222222223333333334444444445555555555666666666777777777888888888999999999";
        var sudoku = new SudokuBoard();
        Enumerable.Range(0, 81).ForEach(x => sudoku.GetCell(x).Value = x);

        
        for (var y = 0; y < 9; y++) {
            for (var x = 0; x < 9; x++) {
                var value = y * 10 + x;
                sudoku.GetCell(y * 9 + x).Value = value;
                Console.Write(value.ToString().PadLeft(2, '0') + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("---------------------------");

        sudoku.Shuffle(0);
        
        // show the result in lines of 9 length each one
        for (var y = 0; y < 9; y++) {
            for (var x = 0; x < 9; x++) {
                Console.Write(sudoku.GetCell(y * 9 + x).Value.ToString().PadLeft(2, '0') + " ");
            }
            Console.WriteLine();
        }
    }

    public static IEnumerable<string> FromFile(string filename) {
        var buffer = new StringBuilder();
        Regex digits = new Regex("\\d+");
        var lineCount = 0;
        var lineNumber = 0;
        foreach (var s in File.ReadAllLines(filename)) {
            ++lineNumber;
            // Console.WriteLine($"line {lineNumber}: {s}");
            // read different styles:
            var line = s.Trim().Replace('.', '0');
            if (line.StartsWith("#"))
                continue;
            if (line.Length == 81) {
                AssertOneLine(line);
                yield return line;
            } else if (line.Length == 9 && digits.IsMatch(line)) {
                buffer.Append(line);
                ++lineCount;
                if (lineCount == 9) {
                    lineCount = 0;
                    var sudokuMultiLine = buffer.ToString();
                    buffer.Clear();
                    AssertOneLine(sudokuMultiLine);
                    yield return sudokuMultiLine;
                }
            } else {
                Console.WriteLine($"ERROR: unknown format at line number {lineNumber}, starts with {line.Substring(0, Math.Min(line.Length, 10))}");
            }
        }
    }

    private static void AssertOneLine(string line) {
        Assert.AreEqual(81, line.Length);
        Assert.IsTrue(Regex.IsMatch(line, "^[0-9.]+$"));

        var sudokuModel = new SudokuBoard(line);
        Assert.AreEqual(sudokuModel.Export(), line);

        Assert.AreEqual(new SudokuBoard(sudokuModel.Export()).Export(), line);
        Assert.AreEqual(new SudokuBoard(sudokuModel.Export()).Export(), line);
    }
}