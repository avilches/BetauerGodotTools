using System;
using Betauer.Core.Nodes;
using Godot;

namespace Veronenger.Game.Worlds.Platform.Animation {
	public partial class PlatformFollowPathBodyNode : StaticBody2D {

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
			_original = Position;
			if (PathFollow2D == null) {
				var path2D = this.FirstNode<Path2D>();
				if (path2D != null) {
					PathFollow2D = path2D.FirstNode<PathFollow2D>();
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
