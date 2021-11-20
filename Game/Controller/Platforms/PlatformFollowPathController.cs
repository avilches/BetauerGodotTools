using System;
using Godot;
using Tools;

namespace Veronenger.Game.Controller.Platforms {
	public class PlatformFollowPathController : PlatformController {

		[Export] public float Speed = 10;
		[Export] public PathFollow2D PathFollow2D;
		[Export] public bool Enabled = true;

		private Vector2 _original;

		public override void _EnterTree() {
			Configure();
		}

		public override void _PhysicsProcess(float delta) {
			if (!Enabled) return;
			UpdatePosition(delta);
		}

		private void Configure() {
			PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);
			_original = Position;
			if (PathFollow2D == null) {
				var path2D = GodotTools.FindChild<Path2D>(this);
				if (path2D != null) {
					PathFollow2D = GodotTools.FindChild<PathFollow2D>(path2D);
				}
			}

			if (PathFollow2D == null) {
				throw new Exception("PlatformFollowPath " + Name + " doesn't have a valid PathFollow2D");
			}
		}

		private void UpdatePosition(float delta) {
			Position = _original + PathFollow2D.Position;
			PathFollow2D.Offset += Speed * delta;
		}
	}
}
