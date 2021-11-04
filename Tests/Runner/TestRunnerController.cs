using System.Collections.Generic;
using Godot;

namespace Veronenger.Tests.Runner {
    public class TestRunnerController : Node {
        [Export] bool enabled = true;
        [Export] Texture passedIcon;
        [Export] Texture failedIcon;

        Dictionary<string, TreeItem> treeItems = new Dictionary<string, TreeItem>();
        Dictionary<TreeItem, TestResult> itemSelections = new Dictionary<TreeItem, TestResult>();
        Panel panel;
        RichTextLabel overallStatusLabel;
        RichTextLabel stackTraceLabel;
        Button buttonFailedOnly;
        Button buttonRepeat;
        int testsPassed = 0;
        int testsFailed = 0;
        private Tree tree;
        private TreeItem rootItem;


        public override void _Ready() {
            if (!enabled) return;
            buttonFailedOnly = GetNode<Button>("ButtonFailedOnly");
            buttonRepeat = GetNode<Button>("ButtonRepeat");
            panel = GetNode<Panel>("Panel");
            overallStatusLabel = panel.GetNode<RichTextLabel>("OverallStatus");
            stackTraceLabel = panel.GetNode<RichTextLabel>("Stacktrace");
            tree = panel.GetNode<Tree>("Tree");
            buttonFailedOnly.Connect("pressed", this, nameof(ShowOnlyFailed));
            buttonRepeat.Connect("pressed", this, nameof(RunTests));
            tree.Connect("cell_selected", this, nameof(OnCellSelected));
            rootItem = tree.CreateItem(tree);
            RunTests();
        }

        private async void RunTests() {
            testsPassed = testsFailed = 0;
            treeItems.Clear();
            itemSelections.Clear();
            buttonFailedOnly.Disabled = buttonRepeat.Disabled = true;
            Clear(tree.GetRoot(), false);
            stackTraceLabel.BbcodeText = "";

            TestRunner testRunner = new TestRunner(GetTree());
            await testRunner.Run((TestResult testResult) => {
                bool testPasses = testResult.result == TestResult.Result.Passed;
                string classType = testResult.classType.ToString();

                if (!treeItems.ContainsKey(classType)) {
                    treeItems[classType] = CreateTreeItemsForClassType(classType);
                }

                TreeItem testItem = tree.CreateItem(treeItems[classType]);
                testItem.SetText(0, testResult.testMethod.Name);
                testItem.SetIcon(0, testPasses ? passedIcon: failedIcon);

                if (testPasses) {
                    testsPassed++;
                } else {
                    testsFailed++;
                    TreeItem element = treeItems[classType];
                    while (element != null) {
                        element.SetIcon(0, failedIcon);
                        element.Collapsed = false;
                        element = element.GetParent();
                    }

                    GD.Print("* Failed test: " + testResult.classType.Name + "." + testResult.testMethod.Name + "\n" +
                             testResult.exception.Message + "\n" + testResult.exception.StackTrace);
                }

                itemSelections[testItem] = testResult;
                overallStatusLabel.BbcodeText =
                    $"Running tests: {testsPassed + testsFailed} of {testRunner.testCount}\t\t[color=green]Passed: {testsPassed}[/color]";
                if (testsFailed > 0) {
                    overallStatusLabel.BbcodeText += $"\t\t[color=red]Failed: {testsFailed}[/color]";
                }
            });
            overallStatusLabel.BbcodeText = $"[color=green]Passed: {testsPassed}[/color]";
            if (testsFailed > 0) {
                overallStatusLabel.BbcodeText += $"\t\t[color=red]Failed: {testsFailed}[/color]";
                GD.Print("* Passed: " + testsPassed + " | Failed: " + testsFailed);
            } else {
                GD.Print("* All passed: " + testsPassed + "!");
            }

            buttonFailedOnly.Disabled = false;
            buttonRepeat.Disabled = false;
        }

        private TreeItem CreateTreeItemsForClassType(string classType) {
            string[] treeItemParts = classType.Split('.');
            var canonicalTestName = "";
            TreeItem nextItem = rootItem;
            for (var i = 0; i < treeItemParts.Length; i++) {
                if (i > 0) {
                    canonicalTestName += ".";
                }

                canonicalTestName += treeItemParts[i];
                bool isLast = i == treeItemParts.Length - 1;
                if (!treeItems.ContainsKey(canonicalTestName)) {
                    var newItem = tree.CreateItem(nextItem, 0);
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
            Clear(tree.GetRoot(), true);
            tree.Update();
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
            var itemSelected = tree.GetSelected();
            if (!itemSelections.ContainsKey(itemSelected)) {
                stackTraceLabel.BbcodeText = "";
                return;
            }

            TestResult itemSelection = itemSelections[itemSelected];
            if (itemSelection.exception != null) {
                stackTraceLabel.BbcodeText =
                    itemSelection.exception.Message + "\n" + itemSelection.exception.StackTrace;
            } else {
                stackTraceLabel.BbcodeText = "";
            }
        }
    }
}