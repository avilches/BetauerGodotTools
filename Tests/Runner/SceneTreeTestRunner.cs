using System;
using Betauer.TestRunner;
using Godot;

namespace Veronenger.Tests.Runner {
    public class SceneTreeTestRunner : SceneTree {
        public override async void _Initialize() {
            while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
            await ConsoleTestRunner.RunTests(this);
        }
    }
}