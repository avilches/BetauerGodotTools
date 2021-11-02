using Godot;

namespace Game.Tools.Platforms {
	public class FallPlatformExit : Area2D {

		public override void _EnterTree() {
			Configure();
		}

		private void Configure() {
			GameManager.Instance.PlatformManager.AddArea2DFallingPlatformExit(this);
		}
	}
}
