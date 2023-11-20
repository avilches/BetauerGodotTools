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
            Green($"exit(0)");
            tree.Quit(0);
        } else {
            Red($"exit(1)");
            tree.Quit(1);
        }
    }

    private static void PrintConsoleStart(TestReport testReport, TestRunner.TestMethod testMethod) {
        Banner($"{GetTestMethodLine(testReport, testMethod)}: Executing...");
    }

    private static void PrintConsoleResult(TestReport testReport, TestRunner.TestMethod testMethod) {
        var testPasses = testMethod.Result == TestRunner.Result.Passed;

        if (testPasses) {
            GreenBanner($"{GetTestMethodLine(testReport, testMethod)}: Passed ({testMethod.Stopwatch.ElapsedMilliseconds}ms)");
        } else {
            Red($"┌─────────────────────────────────────────────────────────────────────────────────");
            Red($"│ {GetTestMethodLine(testReport, testMethod)}: Failed ({testMethod.Stopwatch.ElapsedMilliseconds}ms)");
            Red($"│ [Error: #{testReport.TestsFailed}/?]");
            Red($"| {testMethod.Exception.GetType()}");
            RedIndent(testMethod.Exception.Message.Split("\n"));
            var line = testMethod.Exception.StackTrace.Split("\n").Reverse().ToList().Find(it => it.Contains(testMethod.TestClass.Type.Name));
            if (line != null) {
                Red("| Error:");
                Red($"| {line}");
            }
            Red("| Stacktrace:");
            NormalIndent(testMethod.Exception.StackTrace.Split("\n"));
            Red($"└──────────────────────────────────────────────────────────────────────────────────");
        }
    }

    private static void PrintConsoleFinish(TestReport testReport, Stopwatch stopwatch) {
        if (testReport.TestsFailed > 0) {
            RedBanner($"Failed: {testReport.TestsFailed}/{testReport.TestsTotal}. Passed: {testReport.TestsPassed}/{testReport.TestsTotal}", $"Elapsed: {stopwatch.Elapsed.Seconds}s");

            var x = 1;
            testReport.TestsFailedResults.ForEach(testMethod => {
                Red($"│ {GetTestMethodLine(testReport, testMethod)}: Failed");
                Red($"│ [Error: #{x}/{testReport.TestsFailed}]");
                Red($"| {testMethod.Exception.GetType()}");
                RedIndent(testMethod.Exception.Message.Split("\n"));
                var line = testMethod.Exception.StackTrace.Split("\n").ToList().Find(it => it.Contains(testMethod.TestClass.Type.Name));
                if (line != null) {
                    Red("| Error:");
                    Normal($"| {line}");
                } else {
                    Red("| Stacktrace:");
                    NormalIndent(testMethod.Exception.StackTrace.Split("\n"));
                }
                Red("├─────────────────────────────────────────────────────────────────────────────────");
                x++;
            });

            RedBanner($"Failed: {testReport.TestsFailed}/{testReport.TestsTotal}. Passed: {testReport.TestsPassed}/{testReport.TestsTotal}", $"Elapsed: {stopwatch.Elapsed.Seconds}s");
        } else {
            GreenBanner($"Passed: {testReport.TestsPassed}/{testReport.TestsTotal}! :-)", $"Time: {stopwatch.Elapsed.Seconds}s");
        }
    }

    private static void RedIndent(string[] split) {
        foreach (var line in split) Red($"| {line}");
    }

    private static void NormalIndent(string[] split) {
        foreach (var line in split) Normal($"| {line}");
    }

    private static string GetTestMethodLine(TestReport testReport, TestRunner.TestMethod testMethod) {
        var line = $"[Test : #{testMethod.Id}/{testReport.TestsTotal}] {testMethod.TestClass.Type.Name}.{testMethod.Name}";
        if (testMethod.Description != null) {
            line += " \"" + testMethod.Description + "\"";
        }
        return line;
    }

    private static void Banner(params string[] lines) {
        Banner(lines, ConsoleColor.Gray);
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