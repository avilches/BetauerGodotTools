using System;
using Godot;
using Betauer.Core.Nodes;

namespace Veronenger.Controller.Platforms {
	public partial class PlatformFollowPathController : PlatformController {

		[Export] public float Speed = 10;
		[Export] public PathFollow2D? PathFollow2D;
		[Export] public bool Enabled = true;

		private Vector2 _original;

		public override void _Ready() {
			Configure();
		}

		public override void _PhysicsProcess(double delta) {
			if (!Enabled) return;
			UpdatePosition(delta);
		}

		private void Configure() {
			PlatformManager.ConfigurePlatform(this, IsFallingPlatform, true);
			_original = Position;
			if (PathFollow2D == null) {
				var path2D = this.GetNode<Path2D>();
				if (path2D != null) {
					PathFollow2D = path2D.GetNode<PathFollow2D>();
				}
			}

			if (PathFollow2D == null) {
				throw new Exception("PlatformFollowPath " + Name + " doesn't have a valid PathFollow2D");
			}
		}

		private void UpdatePosition(double delta) {
			Position = _original + PathFollow2D.Position;
			PathFollow2D.Progress += Speed * (float)delta;
		}
	}
}
