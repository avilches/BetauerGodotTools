using Betauer;
using Betauer.Application;
using Betauer.DI;
using Betauer.Core.Memory;
using TraceLevel = Betauer.TraceLevel;

namespace DemoAnimation.Managers.Autoload {
    public class Bootstrap : AutoConfiguration /* needed to be instantiated as an Autoload from Godot */ {
        
        public override void _Ready() {
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Error);
            DisposeTools.ShowMessageOnDispose = true;
            DisposeTools.ShowWarningOnShutdownDispose = true;
        }
    }
}