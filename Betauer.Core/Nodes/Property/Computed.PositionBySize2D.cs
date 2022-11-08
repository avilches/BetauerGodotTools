using Godot;

namespace Betauer.Nodes.Property {
    public class PositionBySize2D : ComputedProperty<Vector2> {
        private readonly Vector2 _initialValue;

        public PositionBySize2D(Node node) : base(node, Properties.Position2D) {
            if (IsCompatibleWith(node)) {
                _initialValue = Properties.Position2D.GetValue(node);
            }
        }

        protected override Vector2 ComputeValue(Vector2 percent) {
            var size = Node switch {
                Sprite2D sprite => sprite.GetSpriteSize(),
                Control control => control.Size,
            };
            return _initialValue + size * percent;
        }

        public sealed override bool IsCompatibleWith(Node node) {
            return node is Sprite2D || node is Control;
        }

        public override string ToString() {
            return $"PositionBySize2D.Computed({Property})";
        }
    }
}