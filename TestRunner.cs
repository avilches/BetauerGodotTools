using System;
using Betauer;
using Betauer.DI;
using Betauer.TestRunner;
using Godot;

public class TestRunner : SceneTree {
    public override async void _Initialize() {
        LoggerFactory.LoadFrames(GetFrame);
        await ConsoleTestRunner.RunTests(this);
    }
}