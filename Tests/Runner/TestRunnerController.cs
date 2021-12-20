using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Veronenger.Tests.Runner {
    public class TestRunnerController : Node {
        [Export] bool enabled = true;
        [Export] Texture passedIcon;
        [Export] Texture failedIcon;

        private readonly Dictionary<string, TreeItem> treeItems = new Dictionary<string, TreeItem>();

        private readonly Dictionary<TreeItem, TestRunner.TestMethod> _itemSelections =
            new Dictionary<TreeItem, TestRunner.TestMethod>();

        private Panel _panel;
        private RichTextLabel _overallStatusLabel;
        private RichTextLabel _stackTraceLabel;
        private Button _buttonFailedOnly;
        private Button _buttonRepeat;
        private Tree _tree;
        private TreeItem _rootItem;


        public override void _Ready() {
            if (!enabled) return;
            _buttonFailedOnly = GetNode<Button>("ButtonFailedOnly");
            _buttonRepeat = GetNode<Button>("ButtonRepeat");
            _panel = GetNode<Panel>("Panel");
            _overallStatusLabel = _panel.GetNode<RichTextLabel>("OverallStatus");
            _stackTraceLabel = _panel.GetNode<RichTextLabel>("Stacktrace");
            _tree = _panel.GetNode<Tree>("Tree");
            _buttonFailedOnly.Connect("pressed", this, nameof(ShowOnlyFailed));
            _buttonRepeat.Connect("pressed", this, nameof(RunTests));
            _tree.Connect("cell_selected", this, nameof(OnCellSelected));
            _rootItem = _tree.CreateItem(_tree);
            RunTests();
        }

        private async void RunTests() {
            treeItems.Clear();
            _itemSelections.Clear();
            _buttonFailedOnly.Disabled = _buttonRepeat.Disabled = true;
            Clear(_tree.GetRoot(), false);
            _stackTraceLabel.BbcodeText = "";

            TestRunner testRunner = new TestRunner();
            await testRunner.Run(GetTree(),
                (testMethod) => {
                    PrintConsoleStart(testRunner, testMethod);
                    _overallStatusLabel.BbcodeText += "\n\nExecuting test " + testRunner.TestsExecuted + "/" +
                                                      testRunner.TestsTotal + ": " + testMethod.Name + "...";
                },
                (testMethod) => {
                    PrintConsoleResult(testRunner, testMethod);
                    UpdateTreeWithTestResult(testMethod);
                    _overallStatusLabel.BbcodeText =
                        $"[color=green]Passed: {testRunner.TestsPassed}[/color]";
                    if (testRunner.TestsFailed > 0) {
                        _overallStatusLabel.BbcodeText += $"\t\t[color=red]Failed: {testRunner.TestsFailed}[/color]";
                    }
                });
            _overallStatusLabel.BbcodeText = $"[color=green]Passed: {testRunner.TestsPassed}[/color]";
            if (testRunner.TestsFailed > 0) {
                _overallStatusLabel.BbcodeText += $"\t\t[color=red]Failed: {testRunner.TestsFailed}[/color]";
            }

            _buttonFailedOnly.Disabled = false;
            _buttonRepeat.Disabled = false;

            PrintConsoleFinish(testRunner);

            if (testRunner.TestsFailed == 0) {
                await Task.Delay(50);
                Green("exit(0)");
                GetTree().Quit(0);
            } else {
                if (OS.GetCmdlineArgs().Contains("--no-window")) {
                    Red("exit(1)");
                    GetTree().Quit(1);
                }
            }
        }

        private static void PrintConsoleStart(TestRunner testRunner, TestRunner.TestMethod testMethod) {
            GD.Print(
                $"#{testRunner.TestsExecuted}/{testRunner.TestsTotal}: {testMethod.Type.Name}.{testMethod.Name} \"{testMethod.Description}\" ...");
        }

        private static string GetTestMethodLine(TestRunner testRunner, TestRunner.TestMethod testMethod) {
            var line = $"#{testMethod.Id}/{testRunner.TestsTotal} {testMethod.Type.Name}.{testMethod.Name}";
            if (testMethod.Description != null) {
                line += "\"" + testMethod.Description + "\"";
            }
            return line;
        }
        private static void PrintConsoleResult(TestRunner testRunner, TestRunner.TestMethod testMethod) {
            var testPasses = testMethod.Result == TestRunner.Result.Passed;

            if (testPasses) {
                Green($"{GetTestMethodLine(testRunner, testMethod)}: Passed");
            } else {
                Red($"{GetTestMethodLine(testRunner, testMethod)}: Failed");
                Red(testMethod.Exception.Message);
                Normal(testMethod.Exception.StackTrace);
            }
        }

        private static void PrintConsoleFinish(TestRunner testRunner) {
            if (testRunner.TestsFailed > 0) {
                Green($"Passed: {testRunner.TestsPassed}/{testRunner.TestsTotal}");
                Red("--------------------------------------------------------------------------------");
                Red($"Failed: {testRunner.TestsFailed}/{testRunner.TestsTotal}");

                testRunner.TestsFailedResults.ForEach(testMethod => {
                    Red($"{GetTestMethodLine(testRunner, testMethod)}: Failed");
                    Red(testMethod.Exception.Message);
                    var stackTraceFiltered = testMethod.Exception.StackTrace.Split("\n").ToList()
                        .FindAll(line => !line.Trim().EndsWith(":0"));
                    Normal(string.Join("\n", stackTraceFiltered));
                });

                Red("--------------------------------------------------------------------------------");
                Green($"Passed: {testRunner.TestsPassed}/{testRunner.TestsTotal}");
                Red($"Failed: {testRunner.TestsFailed}/{testRunner.TestsTotal}");
            } else {
                Green($"All passed: {testRunner.TestsPassed}/{testRunner.TestsTotal}");
            }
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

        private void UpdateTreeWithTestResult(TestRunner.TestMethod testMethod) {
            var testPasses = testMethod.Result == TestRunner.Result.Passed;
            var classType = testMethod.Type.ToString();
            if (!treeItems.ContainsKey(classType)) {
                treeItems[classType] = CreateTreeItemsForClassType(classType);
            }

            TreeItem testItem = _tree.CreateItem(treeItems[classType]);
            testItem.SetText(0, testMethod.Name);
            testItem.SetIcon(0, testPasses ? passedIcon : failedIcon);

            if (!testPasses) {
                TreeItem element = treeItems[classType];
                while (element != null) {
                    element.SetIcon(0, failedIcon);
                    element.Collapsed = false;
                    element = element.GetParent();
                }
            }

            _itemSelections[testItem] = testMethod;
        }

        private TreeItem CreateTreeItemsForClassType(string classType) {
            var treeItemParts = classType.Split('.');
            var canonicalTestName = "";
            TreeItem nextItem = _rootItem;
            for (var i = 0; i < treeItemParts.Length; i++) {
                if (i > 0) {
                    canonicalTestName += ".";
                }

                canonicalTestName += treeItemParts[i];
                bool isLast = i == treeItemParts.Length - 1;
                if (!treeItems.ContainsKey(canonicalTestName)) {
                    var newItem = _tree.CreateItem(nextItem, 0);
                    newItem.SetText(0, treeItemParts[i]);
                    newItem.Collapsed = isLast;
                    newItem.SetIcon(0, passedIcon);
                    treeItems[canonicalTestName] = newItem;
                }

                nextItem = treeItems[canonicalTestName];
            }

            return nextItem;
        }

        private void ShowOnlyFailed() {
            Clear(_tree.GetRoot(), true);
            _tree.Update();
        }

        private void Clear(TreeItem parent, bool onlyPassed) {
            TreeItem treeItem = parent.GetChildren();
            while (treeItem != null) {
                if (!onlyPassed || treeItem.GetIcon(0) == passedIcon) {
                    parent.RemoveChild(treeItem);
                } else {
                    Clear(treeItem, onlyPassed);
                }

                treeItem = treeItem.GetNext();
            }
        }

        private void OnCellSelected() {
            var itemSelected = _tree.GetSelected();
            if (!_itemSelections.ContainsKey(itemSelected)) {
                _stackTraceLabel.BbcodeText = "";
                return;
            }

            TestRunner.TestMethod itemSelection = _itemSelections[itemSelected];
            if (itemSelection.Exception != null) {
                _stackTraceLabel.BbcodeText =
                    itemSelection.Exception.Message + "\n" + itemSelection.Exception.StackTrace;
            } else {
                _stackTraceLabel.BbcodeText = "";
            }
        }
    }
}