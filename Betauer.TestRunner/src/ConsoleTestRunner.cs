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
        testRunner.OnResult +=  PrintConsoleResult;
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
        Normal($"{GetTestMethodLine(testReport, testMethod)}: Executing...");
    }

    private static void PrintConsoleResult(TestReport testReport, TestRunner.TestMethod testMethod) {
        var testPasses = testMethod.Result == TestRunner.Result.Passed;

        if (testPasses) {
            Green($"{GetTestMethodLine(testReport, testMethod)}: Passed ({Elapsed(testMethod.Stopwatch.Elapsed)})");
        } else {
            MethodError(testReport.TestsFailed, testReport, testMethod);
        }
    }

    private static void PrintConsoleFinish(TestReport testReport, Stopwatch stopwatch) {
        if (testReport.TestsFailed > 0) {
            Red($"- Passed: {testReport.TestsPassed}/{testReport.TestsTotal}, {testReport.TestsFailed} ({Elapsed(stopwatch.Elapsed)}) -");

            var x = 1;
            testReport.TestsFailedResults.ForEach(testMethod => {
                MethodError(x, testReport, testMethod);
                Console.WriteLine();
                x++;
            });

            Red($"- Passed: {testReport.TestsPassed}/{testReport.TestsTotal}, {testReport.TestsFailed} failed ({Elapsed(stopwatch.Elapsed)}) -");
        } else {
            Green($"- Passed: {testReport.TestsPassed}/{testReport.TestsTotal} :-) ({Elapsed(stopwatch.Elapsed)}) -");
        }
    }

    private static void MethodError(int error, TestReport testReport, TestRunner.TestMethod testMethod) {
        Red($"{GetTestMethodLine(testReport, testMethod)}: Failed ({Elapsed(testMethod.Stopwatch.Elapsed)})");
        Red($"Error: {error}/{testReport.TestsFailed}");
        Red($"{testMethod.Exception.GetType()}");
        Red(testMethod.Exception.Message);
        Normal(testMethod.Exception.StackTrace.Split("\n"), it => it.Contains(testMethod.TestClass.Type.Name));
    }

    private static string Elapsed(TimeSpan span) {
        return span.TotalSeconds switch {
            < 10 => $"{span.TotalSeconds:N3}s",
            < 60 => $"{span.TotalSeconds:N0}s",
            _ => $"{(int)span.TotalMinutes}m {span.Seconds}s"
        };
    }

    private static void Normal(string[] split, Func<string, bool> redPredicate) {
        foreach (var line in split) {
            if (redPredicate(line)) Red(line);
            else Normal(line);
        }
    }

    private static string GetTestMethodLine(TestReport testReport, TestRunner.TestMethod testMethod) {
        var line = $"Test: {testMethod.Id}/{testReport.TestsTotal} {testMethod.TestClass.Type.Name}.{testMethod.Name}";
        if (testMethod.Description != null) {
            line += " \"" + testMethod.Description + "\"";
        }
        return line;
    }

    private static void Banner(string[] lines, ConsoleColor color) {
        if (color == ConsoleColor.Gray) {
            Console.ResetColor();
        } else {
            Console.ForegroundColor = color;
        }
        var max = lines.Max(i => i.Length);
        var fill = new string('─', max);
        Console.WriteLine($"┌─{fill}─┐");
        foreach (var line in lines) Console.WriteLine($"│ {line.PadRight(max)} │");
        Console.WriteLine($"└─{fill}─┘");
    }

    private static void GreenBanner(params string[] print) {
        Banner(print, ConsoleColor.Green);
    }

    private static void RedBanner(params string[] print) {
        Banner(print, ConsoleColor.Red);
    }

    private static void Normal(string print) {
        Console.ResetColor();
        Console.WriteLine(print);
    }

    private static void Green(string print) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(print);
        Console.ResetColor();
    }

    private static void Red(string print) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(print);
        Console.ResetColor();
    }

}