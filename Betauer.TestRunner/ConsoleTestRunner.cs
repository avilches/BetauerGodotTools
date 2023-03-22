using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace Betauer.TestRunner; 

public class ConsoleTestRunner {
    public static async Task RunTests(SceneTree tree, Assembly[]? assemblies = null) {
        var stopwatch = Stopwatch.StartNew();

        TestRunner testRunner = new TestRunner();
        testRunner.OnStart += PrintConsoleStart;
        testRunner.OnResult +=  PrintConsoleResult;
        testRunner.Load(assemblies);
        var testReport = await testRunner.Run(tree);
            
        PrintConsoleFinish(testReport);

        if (testReport.TestsFailed == 0) {
            Green($"exit(0) - {stopwatch.Elapsed.Seconds}s");
            tree.Quit(0);
        } else {
            Red($"exit(1) - {stopwatch.Elapsed.Seconds}s");
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
            Red(testMethod.Exception.GetType().ToString());
            Red(testMethod.Exception.Message);
            var line = testMethod.Exception.StackTrace.Split("\n").ToList().Find(it => it.Contains(testMethod.TestClass.Type.Name));
            if (line != null) {
                Red("Error");
                Normal(line);
                Red("Stacktrace");
            }
            Normal(testMethod.Exception.StackTrace);
            Red($"└──────────────────────────────────────────────────────────────────────────────────");
        }
    }

    private static void PrintConsoleFinish(TestReport testReport) {
        if (testReport.TestsFailed > 0) {
            Red($"┌─────────────────────────────────────────────────────────────────────────────────");
            Green($"| Passed: {testReport.TestsPassed}/{testReport.TestsTotal}");
            Red($"| Failed: {testReport.TestsFailed}/{testReport.TestsTotal}");
            Red($"└─────────────────────────────────────────────────────────────────────────────────");
            Red($"┌─────────────────────────────────────────────────────────────────────────────────");

            var x = 1;
            testReport.TestsFailedResults.ForEach(testMethod => {
                Red($"│ [Error: #{x}/{testReport.TestsFailed}]");
                Normal($"│  {GetTestMethodLine(testReport, testMethod)}: Failed");
                Red(testMethod.Exception.GetType().ToString());
                Red(testMethod.Exception.Message);
                var line = testMethod.Exception.StackTrace.Split("\n").ToList().Find(it => it.Contains(testMethod.TestClass.Type.Name));
                if (line != null) {
                    Red("Error");
                    Normal(line);
                } else {
                    Red("Stacktrace");
                    Normal(testMethod.Exception.StackTrace);
                }
                Red("├─────────────────────────────────────────────────────────────────────────────────");
                x++;
            });

            Red($"│ Total failed: {testReport.TestsFailed}/{testReport.TestsTotal}");
            Green($"│ Total passed: {testReport.TestsPassed}/{testReport.TestsTotal}");
            Red($"└─────────────────────────────────────────────────────────────────────────────────");
        } else {
            GreenBanner($"All tests passed: {testReport.TestsPassed}/{testReport.TestsTotal} :-)");
        }
    }

    private static string GetTestMethodLine(TestReport testReport, TestRunner.TestMethod testMethod) {
        var line = $"[Test: #{testMethod.Id}/{testReport.TestsTotal}] {testMethod.TestClass.Type.Name}.{testMethod.Name}";
        if (testMethod.Description != null) {
            line += " \"" + testMethod.Description + "\"";
        }
        return line;
    }

    private static void Banner(string line, ConsoleColor color = ConsoleColor.Gray) {
        if (color == ConsoleColor.Gray) {
            Console.ResetColor();
        } else {
            Console.ForegroundColor = color;
        }
        var fill = new string('─', line.Length);
        GD.Print($"┌─{fill}─┐");
        GD.Print($"│ {line} │");
        GD.Print($"└─{fill}─┘");
    }

    private static void GreenBanner(string print) {
        Banner(print, ConsoleColor.Green);
    }

    private static void Normal(string print) {
        Console.ResetColor();
        GD.Print(print);
    }

    private static void Green(string print) {
        Console.ForegroundColor = ConsoleColor.Green;
        GD.Print(print);
        Console.ResetColor();
    }

    private static void Red(string print) {
        Console.ForegroundColor = ConsoleColor.Red;
        GD.Print(print);
        Console.ResetColor();
    }

}