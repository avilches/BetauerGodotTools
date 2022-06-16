using System.Linq;
using System.Reflection;
using Betauer;
using Betauer.TestRunner;
using Godot;

public class TestRunner : SceneTree {
    public override async void _Initialize() {
        LoggerFactory.LoadFrames(GetFrame);
        LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);
        var assemblies = new[] {
            "Betauer.Animation.Tests",
            "Betauer.Core.Tests",
            "Betauer.DI.Tests",
            "Betauer.GameTools.Tests",
            "Betauer.StateMachine.Tests",
        }.Select(Assembly.Load).ToArray();
        await ConsoleTestRunner.RunTests(this, assemblies);
    }
}