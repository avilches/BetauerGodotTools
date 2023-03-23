using System;
using System.Linq;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;

namespace Betauer {
	public partial class RunTests : SceneTree {
		public override async void _Initialize() {
			try {
				LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);
				var assemblies = new[] {
					typeof(Bus.Tests.Assembly),
					typeof(DI.Tests.Assembly),
					typeof(GameTools.Tests.Assembly),
					typeof(FSM.Tests.Assembly),
					// These two are time sensitive, it's better to run them at the end. Godot has some issues with the time
					// in the first seconds since start.
					typeof(Core.Tests.Assembly),
					typeof(Tools.Logging.Tests.Assembly),
					typeof(Tools.Reflection.Tests.Assembly),
					typeof(Animation.Tests.Assembly),
				}.Select(type => type.Assembly).ToArray();
				// await ConsoleTestRunner.RunTests(this, assemblies);
				// var assemblies = new[] { Assembly.Load("Betauer.Tools.Logging.Tests") };
				await ConsoleTestRunner.RunTests(this, assemblies);
				Quit(0);
			} catch (Exception e) {
				GD.PrintErr(e);
				Quit(1);
			}
		}
	}
}
