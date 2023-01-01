using System;
using System.Linq;
using Godot;
using Vector2 = Godot.Vector2;

namespace Betauer {
    public interface IFlipper {
        public void Flip();
        public void Flip(float xInput);
        public bool IsFacingRight { get; set; }
    }

    public class FlipperList : IFlipper {
        private IFlipper[] _flippers = Array.Empty<IFlipper>();

        public FlipperList AddSprite(Sprite2D sprite) => 
            AddFlipper(new Sprite2DFlipH(sprite));
        
        public FlipperList AddProperty<[MustBeVariant] TProperty>(Node node, string property, TProperty right, TProperty left) => 
            AddFlipper(new PropertyFlipper<TProperty>(node, property, right, left));
        
        public FlipperList AddArea2D(Area2D area2D) => 
            AddFlipper(new FlipScaleX(area2D));
        
        public FlipperList AddRayCast2D(RayCast2D rayCast2D) => 
            AddFlipper(new FlipScaleX(rayCast2D));

        public FlipperList AddFlipper(IFlipper flipper) {
            _flippers = _flippers.Concat(new[] { flipper }).ToArray();
            return this;
        }

        public FlipperList SetFlippers(params IFlipper[] flippers) {
            _flippers = flippers;
            return this;
        }

        public void Flip() {
            Array.ForEach(_flippers, flipper => flipper.Flip());
        }

        public void Flip(float xInput) {
            Array.ForEach(_flippers, flipper => flipper.Flip(xInput));
        }

        public bool IsFacingRight {
            get => _flippers[0].IsFacingRight;
            set => Array.ForEach(_flippers, flipper => flipper.IsFacingRight = value);
        }
    }

    public abstract class Flipper<T> : IFlipper {
        protected T Node { get; }
        private bool? _isFacingRight = null;

        protected Flipper(T node) {
            Node = node;
        }

        public abstract bool LoadIsFacingRight();
        public abstract void SetFacingRight(bool right);

        public void Flip() => IsFacingRight = !IsFacingRight;

        public void Flip(float xInput) {
            if (xInput != 0) IsFacingRight = xInput > 0;
        }

        public bool IsFacingRight {
            get {
                if (!_isFacingRight.HasValue) {
                    _isFacingRight = LoadIsFacingRight();
                    SetFacingRight(_isFacingRight.Value);
                }
                return _isFacingRight.Value;
            }
            set {
                if (_isFacingRight == value) return;
                _isFacingRight = value;
                SetFacingRight(value);
            }
        }
    }

    public class Sprite2DFlipH : Flipper<Sprite2D> {
        public Sprite2DFlipH(Sprite2D node) : base(node) {
        }

        public override bool LoadIsFacingRight() {
            return IsSprite2DFacingRight(Node);
        }

        public static bool IsSprite2DFacingRight(Sprite2D node) {
            return !node.FlipH;
        }

        public override void SetFacingRight(bool right) {
            Node.FlipH = !right;
        }
    }

    public class PropertyFlipper<[MustBeVariant] T> : Flipper<Node> {
        private readonly string _property;
        private readonly T _valueWhenRight;
        private readonly T _valueWhenLeft;

        public PropertyFlipper(Node node, string property, T valueWhenRight, T valueWhenLeft) : base(node) {
            _property = property;
            _valueWhenRight = valueWhenRight;
            _valueWhenLeft = valueWhenLeft;
        }

        public override bool LoadIsFacingRight() {
            return Node.GetIndexed(_property).As<T>()!.Equals(_valueWhenRight);
        }

        public override void SetFacingRight(bool right) {
            Node.SetIndexed(_property, Variant.From(right ? _valueWhenRight : _valueWhenLeft));
        }
    }

    public class FlipScaleX : Flipper<Node2D> {
        private static readonly Vector2 FlipX = new(-1, 1);

        public FlipScaleX(Node2D node) : base(node) {
        }

        public override bool LoadIsFacingRight() {
            return (int)Node.Scale.x == 1;
        }

        public override void SetFacingRight(bool right) {
            Node.Scale = right ? Vector2.One : FlipX;
        }
    }

    public class FlipScaleYAndRotate : Flipper<Node2D> {
        private static readonly Vector2 FlipY = new(1, -1);

        public FlipScaleYAndRotate(Node2D node) : base(node) {
        }

        public override bool LoadIsFacingRight() {
            return (int)Node.Scale.y == 1 && (Node.Transform.Rotation == 0);
        }

        public override void SetFacingRight(bool right) {
            if (right) {
                // Return to normal position
                Node.Scale = Vector2.One; // 1,1
                Node.Rotation = 0;
            } else {
                // Flip to the left = flip Y axis + rotate 180 degrees
                Node.Scale = FlipY;
                Node.Rotate(Mathf.Pi);
            }
        }
    }
}