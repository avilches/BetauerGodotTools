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
    [Betauer.TestRunner.Test]
    public void DancingLinkTest() {
        foreach (var sudokuString in FromFile("Betauer.Core.Tests/test/Sudokus.txt")) {
            SudokuBoard sudoku = new SudokuBoard(sudokuString);
            Assert.That(sudoku.IsBoardFilled(), Is.False);
            Assert.That(sudoku.IsValid(), Is.True);

            Console.WriteLine(sudoku.Export());
            Assert.That(sudoku.SolveDancingLinks(), Is.True);

            Assert.That(sudoku.IsBoardFilled(), Is.True);
            Assert.That(sudoku.IsValid(), Is.True);
        }
    }

    [Betauer.TestRunner.Test]
    public void DancingLinkMultipleSolutionTest() {
        SudokuBoard sudoku = new SudokuBoard(".......2.8.......6.1.2.5...9.54....8.........3....85.1...3.2.8.4.......9.7..6....");
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

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

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic, no seed, fixed next number")]
    public void Invalid() {
        Assert.False(new SudokuBoard("110000000000000000000000000000000000000000000000000000000000000000000000000000000").IsValid()); // same row
        Assert.False(new SudokuBoard("100000000000000000000000000000000000000000000000000000000000000000000000100000000").IsValid()); // same col
        Assert.False(new SudokuBoard("100000010000000000000000000000000000000000000000000000000000000000000000000000000").IsValid()); // same group
        Assert.False(new SudokuBoard("113456789456789123789123456214365897365897214897214365531642978642978531978531642").IsValid());
    }

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic, no seed, fixed next number")]
    public void GenerateBacktrackingDeterministicNoSeed() {
        var import = "".PadRight(81,'0');
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

        Assert.That(sudoku.SolveBacktrack());
        Console.WriteLine(sudoku.Export());

        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.IsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(), "123456789456789123789123456214365897365897214897214365531642978642978531978531642");
        
        sudoku.RemoveCells(1331, 30);
        Console.WriteLine(sudoku.Export());
        Assert.AreEqual(sudoku.Export(), "003450089406009003089100406210305007365897214807014000001040078642978531078500642");
        Assert.AreEqual(sudoku.Export().Count("0"), 30);
    }

    [Betauer.TestRunner.Test]
    public void TestGridImportExport() {
        var import = "953187642624539178817462395589713264132654789476928513745296831268341957391875426";
        var sudoku = new SudokuBoard(import);
        var other = new SudokuBoard(sudoku.CreateGrid());
        Assert.AreEqual(other.Export(), import);
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
        sudoku.Shuffle(123, 100);
        Assert.True(sudoku.IsValid());
    }

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic, seed, random next number")]
    public void GenerateBacktrackingDeterministicRandom() {
        var import = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

        Assert.That(sudoku.SolveBacktrack(902));
        Console.WriteLine(sudoku.Export());

        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.IsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(), "953187642624539178817462395589713264132654789476928513745296831268341957391875426");
    }

    [Betauer.TestRunner.Test(Description = "Generate using dancing links deterministic")]
    public void GenerateDancingLinks() {
        var import = "120000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.IsValid(), Is.True);

        sudoku.SolveDancingLinks();
        Console.WriteLine(sudoku.Export());

        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.IsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(), "123456789789123456456789123312845967697312845845697312231574698968231574574968231");
    }

    [Betauer.TestRunner.Test]
    public void CompareAlgorithms() {
        foreach (var sudokuString in FromFile("Betauer.Core.Tests/test/Sudokus-compare.txt")) {
            var sudokuDancing = new SudokuBoard(sudokuString);
            Assert.That(sudokuDancing.IsBoardFilled(), Is.False);
            Assert.That(sudokuDancing.IsValid(), Is.True);
            Assert.True(sudokuDancing.SolveDancingLinks());
            Assert.That(sudokuDancing.IsBoardFilled(), Is.True);
            Assert.That(sudokuDancing.IsValid(), Is.True);

            var sudokuBacktrack = new SudokuBoard(sudokuString);
            Assert.That(sudokuBacktrack.IsBoardFilled(), Is.False);
            Assert.That(sudokuBacktrack.IsValid(), Is.True);
            Assert.True(sudokuBacktrack.SolveBacktrack());
            Assert.That(sudokuBacktrack.IsBoardFilled(), Is.True);
            Assert.That(sudokuBacktrack.IsValid(), Is.True);
            Console.WriteLine(sudokuString);
            try {
                Assert.AreEqual(sudokuBacktrack.Export(), sudokuDancing.Export());
            } catch (Exception e) {
                Console.WriteLine("BackTracking: " + sudokuBacktrack.Export());
                Console.WriteLine("DancingLinks: " + sudokuDancing.Export());
                Console.WriteLine(e);
                throw;
            }
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