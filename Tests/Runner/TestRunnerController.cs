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
        int passed = 0;
        int failed = 0;
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
            passed = failed = 0;
            treeItems.Clear();
            itemSelections.Clear();
            buttonFailedOnly.Disabled = buttonRepeat.Disabled = true;
            Clear(tree.GetRoot(), false);
            stackTraceLabel.BbcodeText = "";

            TestRunner testRunner = new TestRunner(GetTree());
            await testRunner.Run((TestResult testResult) => {
                bool didFail = testResult.result == TestResult.Result.Failed;

                if (didFail) {
                    failed++;
                } else {
                    passed++;
                }

                string classType = testResult.classType.ToString();

                if (!treeItems.ContainsKey(classType)) {
                    treeItems[classType] = CreateTreeItemsForClassType(classType, rootItem);
                }

                // treeItems[classType].SetText(0, testResult.classType.ToString());

                TreeItem testItem = tree.CreateItem(treeItems[classType]);
                testItem.SetText(0, testResult.testMethod.Name);
                testItem.SetIcon(0, didFail ? failedIcon : passedIcon);

                if (didFail) {
                    TreeItem element = treeItems[classType];
                    while (element != null) {
                        element.SetIcon(0, failedIcon);
                        element.Collapsed = false;
                        element = element.GetParent();
                    }

                    GD.Print(testResult.classType.Name + "." + testResult.testMethod.Name + "\n" +
                             testResult.exception.Message + "\n" + testResult.exception.StackTrace);
                }

                itemSelections[testItem] = testResult;
                if (failed > 0) {
                    overallStatusLabel.BbcodeText =
                        $"Running tests: {passed + failed} of {testRunner.testCount}\t\t[color=green]Passed: {passed}[/color]\t\t[color=red]Failed: {failed}[/color]";
                } else {
                    overallStatusLabel.BbcodeText =
                        $"Running tests: {passed + failed} of {testRunner.testCount}\t\t[color=green]Passed: {passed}[/color]";
                }
            });
            overallStatusLabel.BbcodeText =
                $"Done: {passed + failed}\t\t[color=green]Passed: {passed}[/color]\t\t[color=red]Failed: {failed}[/color]";
            buttonFailedOnly.Disabled = false;
            buttonRepeat.Disabled = false;
        }

        TreeItem CreateTreeItemsForClassType(string classType, TreeItem rootItem) {
            string[] treeItemParts = classType.Split('.');
            var currentClassType = "";
            TreeItem nextItem = rootItem;
            for (var i = 0; i < treeItemParts.Length; i++) {
                if (i > 0) {
                    currentClassType += ".";
                }

                bool isLast = i == treeItemParts.Length - 1;
                currentClassType += treeItemParts[i];
                if (!treeItems.ContainsKey(currentClassType)) {
                    var newItem = tree.CreateItem(nextItem, 0);
                    newItem.SetText(0, treeItemParts[i]);
                    newItem.Collapsed = isLast;
                    newItem.SetIcon(0, passedIcon);
                    treeItems[currentClassType] = newItem;
                }

                nextItem = treeItems[currentClassType];
            }

            return nextItem;
        }

        void ShowOnlyFailed() {
            Clear(tree.GetRoot(), true);
            tree.Update();
        }

        void Clear(TreeItem parent, bool onlyPassed) {
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


        void OnCellSelected() {
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