using Godot;

namespace Betauer.Nodes.Property {
    public class PositionBySizeX : ComputedProperty<float> {
        private readonly float _initialValue;

        public PositionBySizeX(Node node) : base(node, Properties.PositionX) {
            if (IsCompatibleWith(node)) {
                _initialValue = Properties.PositionX.GetValue(node);
            }
        }

        protected override float ComputeValue(float percent) {
            var size = Node switch {
                Sprite sprite => sprite.GetSpriteSize(),
                Control control => control.RectSize,
            };
            return _initialValue + (size.x * percent);
        }

        public sealed override bool IsCompatibleWith(Node node) {
            return node is Sprite || node is Control;
        }

        public override string ToString() {
            return $"PositionBySizeX.Computed({Property})";
        }
    }
}