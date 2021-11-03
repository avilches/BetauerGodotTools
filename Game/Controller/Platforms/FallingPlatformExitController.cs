using Godot;
using Veronenger.Game.Tools;

namespace Veronenger.Game.Controller.Platforms {
	public class FallingPlatformExit : Area2D {

		public override void _EnterTree() {
			Configure();
		}

		private void Configure() {
			GameManager.Instance.PlatformManager.AddArea2DFallingPlatformExit(this);
		}
	}
}
