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
            "Betauer.DI.Tests",
            "Betauer.GameTools.Tests",
            "Betauer.GodotAction.Tests",
            "Betauer.StateMachine.Tests",
            // These two are time sensitive, it's better to run them at the end. Godot has some issues with the time
            // in the first seconds since start.
            "Betauer.Core.Tests",
            "Betauer.Animation.Tests",
        }.Select(Assembly.Load).ToArray();
        await ConsoleTestRunner.RunTests(this, assemblies);
    }
}