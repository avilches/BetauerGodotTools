using System;
using System.Linq;
using System.Numerics;
using Godot;
using Vector2 = Godot.Vector2;

namespace Betauer {
    public interface IFlipper {
        public void Flip();
        public void FaceToRight(bool right);
        public void Flip(float xInput);
        public bool IsFacingRight { get; }
    }

    public class FlipperList : IFlipper {
        private IFlipper[] _flippers = Array.Empty<IFlipper>();

        public FlipperList AddSprite(Sprite2D sprite) => AddFlipper(new Sprite2DFlipH(sprite));
        public FlipperList AddArea2D(Area2D area2D) => AddFlipper(new FlipScaleX(area2D));
        public FlipperList AddRayCast2D(RayCast2D rayCast2D) => AddFlipper(new FlipScaleX(rayCast2D));

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

        public void FaceToRight(bool right) {
            Array.ForEach(_flippers, flipper => flipper.FaceToRight(right));
        }

        public void Flip(float xInput) {
            Array.ForEach(_flippers, flipper => flipper.Flip(xInput));
        }

        public bool IsFacingRight => _flippers[0].IsFacingRight;
    }

    public abstract class Flipper<T> : IFlipper {
        protected T Node { get; }
        private bool _isFacingRight = false;

        protected Flipper(T node) {
            Node = node;
            _isFacingRight = GetFacingRight();
        }

        public abstract bool GetFacingRight();
        public abstract void SetFacingRight(bool right);

        public void Flip() => IsFacingRight = !_isFacingRight;

        public void FaceToRight(bool right) => IsFacingRight = right;

        public void Flip(float xInput) {
            if (xInput != 0) IsFacingRight = xInput > 0;
        }

        public bool IsFacingRight {
            get => _isFacingRight;
            set {
                if (value == _isFacingRight) return;
                _isFacingRight = value;
                SetFacingRight(_isFacingRight);
            }
        }
    }

    public class Sprite2DFlipH : Flipper<Sprite2D> {
        public Sprite2DFlipH(Sprite2D node) : base(node) {
        }

        public override bool GetFacingRight() {
            return !Node.FlipH;
        }

        public override void SetFacingRight(bool right) {
            Node.FlipH = !right;
        }
    }

    public class FlipScaleX : Flipper<Node2D> {
        private static readonly Vector2 FlipX = new(-1, 1);

        public FlipScaleX(Node2D node) : base(node) {
        }

        public override bool GetFacingRight() {
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

        public override bool GetFacingRight() {
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