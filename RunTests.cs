using System.Linq;
using System.Reflection;
using Betauer;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;

public class RunTests : SceneTree {
    public override async void _Initialize() {
        LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);
        var assemblies = new[] {
            "Betauer.Bus.Tests",
            "Betauer.DI.Tests",
            "Betauer.GameTools.Tests",
            "Betauer.StateMachine.Tests",
            // These two are time sensitive, it's better to run them at the end. Godot has some issues with the time
            // in the first seconds since start.
            "Betauer.Core.Tests",
            "Betauer.Animation.Tests",
        }.Select(Assembly.Load).ToArray();
        await ConsoleTestRunner.RunTests(this, assemblies);
    }
}