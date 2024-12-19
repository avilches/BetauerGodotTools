using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace Betauer.TestRunner;

public class ConsoleTestRunner {
    public static async Task RunTests(SceneTree tree, params Assembly[]? assemblies) {
        var stopwatch = Stopwatch.StartNew();

        TestRunner testRunner = new TestRunner();
        testRunner.OnStart += PrintConsoleStart;
        testRunner.OnResult += PrintConsoleResult;
        testRunner.Load(assemblies);
        var testReport = await testRunner.Run(tree);

        stopwatch.Stop();
        PrintConsoleFinish(testReport, stopwatch);

        if (testReport.TestsFailed == 0) {
            tree.Quit(0);
            await tree.AwaitProcessFrame();
        } else {
            tree.Quit(1);
            await tree.AwaitProcessFrame();
        }
    }

    private static void PrintConsoleStart(TestReport testReport, TestRunner.TestMethod testMethod) {
        // Normal($"{GetTestMethodLine(testReport, testMethod)}: Executing...");
    }

    private static void PrintConsoleResult(TestReport testReport, TestRunner.TestMethod testMethod) {
        var testPasses = testMethod.Result == TestRunner.Result.Passed;

        if (testPasses) {
            Print($@"\{(int)ConsoleColor.White}{GetTestMethodLine(testReport, testMethod)} \{(int)ConsoleColor.Green}passed ({Elapsed(testMethod.Stopwatch.Elapsed)})");
        } else {
            MethodError(testReport.TestsFailed, testReport, testMethod);
        }
    }

    private static void PrintConsoleFinish(TestReport testReport, Stopwatch stopwatch) {
        if (testReport.TestsFailed > 0) {
            Print($@"\{(int)ConsoleColor.White}Passed: {testReport.TestsPassed}/{testReport.TestsTotal}\{(int)ConsoleColor.Red} {testReport.TestsFailed} failed ({Elapsed(stopwatch.Elapsed)})");
            Console.WriteLine();
            Console.WriteLine($"{testReport.TestsFailed} failed tests:");
            Console.WriteLine();
            var x = 1;
            testReport.TestsFailedResults.ForEach(testMethod => {
                MethodError(x, testReport, testMethod);
                Console.WriteLine();
                x++;
            });

            Print($@"\{(int)ConsoleColor.White}Passed: {testReport.TestsPassed}/{testReport.TestsTotal}\{(int)ConsoleColor.Red} {testReport.TestsFailed} failed ({Elapsed(stopwatch.Elapsed)})");
        } else {
            Print($@"\{(int)ConsoleColor.White}Passed: {testReport.TestsPassed}/{testReport.TestsTotal}\{(int)ConsoleColor.Green} 0 failed ({Elapsed(stopwatch.Elapsed)})");
        }
    }

    private static void MethodError(int error, TestReport testReport, TestRunner.TestMethod testMethod) {
        Print($@"\{(int)ConsoleColor.White}Error {error}/{testReport.TestsFailed} {GetTestMethodLine(testReport, testMethod)}\{(int)ConsoleColor.Red} failed ({Elapsed(testMethod.Stopwatch.Elapsed)})");
        Print($"{testMethod.Exception.GetType()}");
        Print(testMethod.Exception.Message);
        Stacktrace(testMethod.Exception.StackTrace.Split("\n"), it => it.Contains(testMethod.TestClass.Type.Name));
    }

    private static string Elapsed(TimeSpan span) {
        return span.TotalSeconds switch {
            < 10 => $"{span.TotalSeconds:N3}s",
            < 60 => $"{span.TotalSeconds:N0}s",
            _ => $"{(int)span.TotalMinutes}m {span.Seconds}s"
        };
    }

    private static void Stacktrace(string[] split, Func<string, bool> redPredicate) {
        foreach (var line in split) {
            if (redPredicate(line)) Print($@"\{(int)ConsoleColor.Red}{line}");
            else Console.WriteLine(line);
        }
    }

    private static string GetTestMethodLine(TestReport testReport, TestRunner.TestMethod testMethod) {
        var line = $"Test {testMethod.Id}/{testReport.TestsTotal}: {testMethod.TestClass.Type.Name} {testMethod.Name}";
        if (testMethod.Description != null) {
            line += " \"" + testMethod.Description + "\"";
        }
        return line;
    }

    public static void Print(string text) {
        var parts = text.Split('\\');

        for (var i = 0; i < parts.Length; i++) {
            var part = parts[i];
            if (i == 0) {
                Console.Write(part);
                continue;
            }

            if (part.Length > 1 && char.IsDigit(part[0])) {
                var j = 1;
                if (char.IsDigit(part[j])) {
                    j++;
                }

                var colorCode = int.Parse(part[0..j]);
                Console.ForegroundColor = (ConsoleColor)colorCode;
                Console.Write(part[j..]);
            } else {
                Console.Write("\\" + part);
            }
        }
        Console.WriteLine();

        Console.ResetColor();
    }
}