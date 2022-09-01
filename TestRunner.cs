using System.Linq;
using System.Reflection;
using Betauer;
using Betauer.TestRunner;
using Godot;

public class TestRunner : SceneTree {
    public override async void _Initialize() {
        LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);
        SceneTreeHolder.SceneTree = this;
        var assemblies = new[] {
            "Betauer.Animation.Tests",
            "Betauer.Core.Tests",
            "Betauer.DI.Tests",
            "Betauer.GameTools.Tests",
            "Betauer.GodotAction.Tests",
            "Betauer.StateMachine.Tests",
        }.Select(Assembly.Load).ToArray();
        await ConsoleTestRunner.RunTests(this, assemblies);
    }
}