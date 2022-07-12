using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Memory;
using TraceLevel = Betauer.TraceLevel;

namespace DemoAnimation.Game.Managers.Autoload {
    public class Bootstrap : GodotContainer /* needed to be instantiated as an Autoload from Godot */ {
        public Bootstrap() {
            AutoConfigure();
        }

        public override void _Ready() {
            AddChild(new ObjectWatcherNode());
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Error);
            // LoggerFactory.SetTraceLevel(typeof(PropertyTweener), TraceLevel.All);
            LoggerFactory.SetTraceLevel(typeof(ObjectWatcher), TraceLevel.All);
            DisposeTools.ShowMessageOnDispose = true;
            DisposeTools.ShowWarningOnShutdownDispose = true;
        }
    }
}