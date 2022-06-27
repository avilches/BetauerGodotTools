using Betauer;
using Betauer.Animation;
using Betauer.DI;
using TraceLevel = Betauer.TraceLevel;

namespace Veronenger.Game.Managers.Autoload {
    public class Bootstrap : GodotContainer /* needed to be instantiated as an Autoload from Godot */ {
        public Bootstrap() {
            AutoConfigure();
        }

        public override void _Ready() {
            Name = nameof(Bootstrap); // This name is shown in the remote editor
            LoggerFactory.LoadFrames(GetTree().GetFrame);
            LoggerFactory.SetDefaultTraceLevel(TraceLevel.Error);
            LoggerFactory.SetTraceLevel(typeof(PropertyTweener), TraceLevel.All);
        }

    }
}