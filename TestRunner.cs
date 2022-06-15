using Betauer;
using Betauer.Animation.Tests;
using Betauer.DI.Tests;
using Betauer.StateMachine.Tests;
using Betauer.TestRunner;
using Betauer.Tests;
using Godot;

public class TestRunner : SceneTree {
    public override async void _Initialize() {
        LoggerFactory.LoadFrames(GetFrame);
        LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);
        typeof(ActionTweenTests).GetType();
        typeof(VariantHelperTests).GetType();
        typeof(ScannerBasicTests).GetType();
        typeof(GodotTopicTests).GetType();
        typeof(StateMachineTests).GetType();
        await ConsoleTestRunner.RunTests(this);
    }
}