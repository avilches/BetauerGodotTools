using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Betauer.Core.Sudoku;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[Betauer.TestRunner.Test]
public class SudokuTests {

    [Betauer.TestRunner.Test]
    public void DancingLinkTest() {
        foreach (var sudokuString in FromFile("Betauer.Core.Tests/test/Sudokus.txt")) {
            SudokuBoard sudoku = new SudokuBoard(sudokuString);
            Assert.That(sudoku.IsBoardFilled(), Is.False);
            Assert.That(sudoku.CheckTableStateIsValid(), Is.True);
            
            Console.WriteLine(sudoku.Export(true));
            Assert.That(sudoku.SolveDancingLinks(), Is.True);
            
            Assert.That(sudoku.IsBoardFilled(), Is.True);
            Assert.That(sudoku.CheckTableStateIsValid(), Is.True);
        }
    }

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic, no seed, fixed next number")]
    public void Invalid() {
        Assert.False(new SudokuBoard("110000000000000000000000000000000000000000000000000000000000000000000000000000000").CheckTableStateIsValid()); // same row
        Assert.False(new SudokuBoard("100000000000000000000000000000000000000000000000000000000000000000000000100000000").CheckTableStateIsValid()); // same col
        Assert.False(new SudokuBoard("100000010000000000000000000000000000000000000000000000000000000000000000000000000").CheckTableStateIsValid()); // same group
        Assert.False(new SudokuBoard("113456789456789123789123456214365897365897214897214365531642978642978531978531642").CheckTableStateIsValid());
    }

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic, no seed, fixed next number")]
    public void GenerateBacktrackingDeterministicNoSeed() {
        var import = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.CheckTableStateIsValid(), Is.True);

        Assert.That(sudoku.SolveBacktrack());
        Console.WriteLine(sudoku.Export(true));
        
        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.CheckTableStateIsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(true), "123456789456789123789123456214365897365897214897214365531642978642978531978531642");

    }

    [Betauer.TestRunner.Test(Description = "Generate using backtracking deterministic, seed, random next number")]
    public void GenerateBacktrackingDeterministicRandom() {
        var import = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.CheckTableStateIsValid(), Is.True);

        Assert.That(sudoku.SolveBacktrack(902));
        Console.WriteLine(sudoku.Export(true));
        
        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.CheckTableStateIsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(true), "953187642624539178817462395589713264132654789476928513745296831268341957391875426");
    }

    [Betauer.TestRunner.Test(Description = "Generate using dancing links deterministic")]
    public void GenerateDancingLinks() {
        var import = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sudoku = new SudokuBoard(import);
        Assert.That(sudoku.IsBoardFilled(), Is.False);
        Assert.That(sudoku.CheckTableStateIsValid(), Is.True);

        sudoku.SolveDancingLinks();
        Console.WriteLine(sudoku.Export(true));
        
        Assert.That(sudoku.IsBoardFilled(), Is.True);
        Assert.That(sudoku.CheckTableStateIsValid(), Is.True);
        Assert.AreEqual(sudoku.Export(true), "126437985435986271897152643742598316358261497619374852561829734284713569973645128");
    }


    [Betauer.TestRunner.Test]
    public void CompareAlgorithms() {
        foreach (var sudokuString in FromFile("Betauer.Core.Tests/test/Sudokus-compare.txt")) {
            var sudokuDancing = new SudokuBoard(sudokuString);
            Assert.That(sudokuDancing.IsBoardFilled(), Is.False);
            Assert.That(sudokuDancing.CheckTableStateIsValid(), Is.True);
            Assert.True(sudokuDancing.SolveDancingLinks());
            Assert.That(sudokuDancing.IsBoardFilled(), Is.True);
            Assert.That(sudokuDancing.CheckTableStateIsValid(), Is.True);

            var sudokuBacktrack = new SudokuBoard(sudokuString);
            Assert.That(sudokuBacktrack.IsBoardFilled(), Is.False);
            Assert.That(sudokuBacktrack.CheckTableStateIsValid(), Is.True);
            Assert.True(sudokuBacktrack.SolveBacktrack());
            Assert.That(sudokuBacktrack.IsBoardFilled(), Is.True);
            Assert.That(sudokuBacktrack.CheckTableStateIsValid(), Is.True);
            Console.WriteLine(sudokuString);
            try {
                Assert.AreEqual(sudokuBacktrack.Export(true), sudokuDancing.Export(true));
            } catch (Exception e) {
                Console.WriteLine("BackTracking: "+sudokuBacktrack.Export(true));
                Console.WriteLine("DancingLinks: "+sudokuDancing.Export(true));
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
        Assert.AreEqual(sudokuModel.Export(true), line);
        
        Assert.AreEqual(new SudokuBoard(sudokuModel.Export(true)).Export(true), line);
        Assert.AreEqual(new SudokuBoard(sudokuModel.Export()).Export(true), line);
    }
}


