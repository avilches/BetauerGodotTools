using System;
using System.Linq;
using System.Reflection;
using Betauer;
using Betauer.Tools.Logging;
using Betauer.TestRunner;
using Godot;

namespace Betauer {
	public partial class RunTests : SceneTree {
		public override async void _Initialize() {
			try {
				LoggerFactory.SetDefaultTraceLevel(TraceLevel.All);
				var assemblies = new[] {
					// "Betauer.Bus.Tests",
					// "Betauer.DI.Tests",
					// "Betauer.GameTools.Tests",
					// "Betauer.StateMachine.Tests",
					// These two are time sensitive, it's better to run them at the end. Godot has some issues with the time
					// in the first seconds since start.
					typeof(Betauer.Core.Tests.Assembly),
					typeof(Betauer.Tools.Logging.Tests.Assembly),
					// "Betauer.Animation.Tests",
				}.Select(type => type.Assembly).ToArray();
				// await ConsoleTestRunner.RunTests(this, assemblies);
				// var assemblies = new[] { Assembly.Load("Betauer.Tools.Logging.Tests") };
				await ConsoleTestRunner.RunTests(this, assemblies);
			} catch (Exception e) {
				GD.PrintErr(e);
				Quit(1);
			}
		}
	}
}
