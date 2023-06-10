using System.Linq;
using System.Reflection;
using Betauer.TestRunner;
using Godot;

namespace Veronenger.Tests; 

public partial class RunTests : SceneTree {
    public override async void _Initialize() {
        while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
        // This is needed because https://github.com/godotengine/godot/issues/62101
        await ConsoleTestRunner.RunTests(this, GetType().Assembly);
    }
}