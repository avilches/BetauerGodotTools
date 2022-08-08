using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace Betauer.TestRunner {
    public class ConsoleTestRunner {
        public static async Task RunTests(SceneTree tree, Assembly[]? assemblies = null) {
            var stopwatch = Stopwatch.StartNew();

            TestRunner testRunner = new TestRunner(assemblies);
            await testRunner.Run(tree,
                testMethod => PrintConsoleStart(testRunner, testMethod),
                testMethod => PrintConsoleResult(testRunner, testMethod));
            
            PrintConsoleFinish(testRunner);

            if (testRunner.TestsFailed == 0) {
                Green($"exit(0) - {stopwatch.Elapsed.Seconds}s");
                tree.Quit(0);
            } else {
                Red($"exit(1) - {stopwatch.Elapsed.Seconds}s");
                tree.Quit(1);
            }
        }

        private static void PrintConsoleStart(TestRunner testRunner, TestRunner.TestMethod testMethod) {
            Banner($"{GetTestMethodLine(testRunner, testMethod)}: Executing...");
        }

        private static void PrintConsoleResult(TestRunner testRunner, TestRunner.TestMethod testMethod) {
            var testPasses = testMethod.Result == TestRunner.Result.Passed;

            if (testPasses) {
                GreenBanner($"{GetTestMethodLine(testRunner, testMethod)}: Passed ({testMethod.Stopwatch.ElapsedMilliseconds}ms)");
            } else {
                Red($"┌─────────────────────────────────────────────────────────────────────────────────");
                Red($"│ {GetTestMethodLine(testRunner, testMethod)}: Failed ({testMethod.Stopwatch.ElapsedMilliseconds}ms)");
                Red(testMethod.Exception.GetType().ToString());
                Red(testMethod.Exception.Message);
                var line = testMethod.Exception.StackTrace.Split("\n").ToList().Find(it => it.Contains(testMethod.FixtureType.Name));
                if (line != null) {
                    Red("Error");
                    Normal(line);
                    Red("Stacktrace");
                }
                Normal(testMethod.Exception.StackTrace);
                Red($"└──────────────────────────────────────────────────────────────────────────────────");
            }
        }

        private static void PrintConsoleFinish(TestRunner testRunner) {
            if (testRunner.TestsFailed > 0) {
                Red($"┌─────────────────────────────────────────────────────────────────────────────────");
                Green($"| Passed: {testRunner.TestsPassed}/{testRunner.TestsTotal}");
                Red($"| Failed: {testRunner.TestsFailed}/{testRunner.TestsTotal}");
                Red($"└─────────────────────────────────────────────────────────────────────────────────");
                Red($"┌─────────────────────────────────────────────────────────────────────────────────");

                var x = 1;
                testRunner.TestsFailedResults.ForEach(testMethod => {
                    Red($"│ [Error: #{x}/{testRunner.TestsFailed}]");
                    Normal($"│  {GetTestMethodLine(testRunner, testMethod)}: Failed");
                    Red(testMethod.Exception.GetType().ToString());
                    Red(testMethod.Exception.Message);
                    var line = testMethod.Exception.StackTrace.Split("\n").ToList().Find(it => it.Contains(testMethod.FixtureType.Name));
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

                Red($"│ Total failed: {testRunner.TestsFailed}/{testRunner.TestsTotal}");
                Green($"│ Total passed: {testRunner.TestsPassed}/{testRunner.TestsTotal}");
                Red($"└─────────────────────────────────────────────────────────────────────────────────");
            } else {
                GreenBanner($"All tests passed: {testRunner.TestsPassed}/{testRunner.TestsTotal} :-)");
            }
        }

        private static string GetTestMethodLine(TestRunner testRunner, TestRunner.TestMethod testMethod) {
            var line = $"[Test: #{testMethod.Id}/{testRunner.TestsTotal}] {testMethod.FixtureType.Name}.{testMethod.Name}";
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
}