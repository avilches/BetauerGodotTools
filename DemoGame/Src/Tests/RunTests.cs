using System.Linq;
using System.Reflection;
using Betauer.TestRunner;
using Godot;

namespace Veronenger.Tests; 

public partial class RunTests : SceneTree {
    public override async void _Initialize() {
        while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
        // This is needed because https://github.com/godotengine/godot/issues/62101
        var assemblies = new[] {
            "Tests",
        }.Select(Assembly.Load).ToArray();
        // Console.WriteLine("Assemblies "+string.Join(assemblies.Select(a => a.Location.ToList())));
        await ConsoleTestRunner.RunTests(this, assemblies);
    }
}