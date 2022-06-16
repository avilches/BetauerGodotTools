using Betauer;
using Betauer.TestRunner;
using Godot;

namespace Veronenger.Tests {
    public class TestRunner : SceneTree {
        public override async void _Initialize() {
            while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
            // This is needed because https://github.com/godotengine/godot/issues/62101
            LoggerFactory.LoadFrames(GetFrame);
            await ConsoleTestRunner.RunTests(this);
        }
    }
}