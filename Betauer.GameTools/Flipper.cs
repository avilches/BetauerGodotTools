using System;
using System.Linq;
using Godot;

namespace Betauer {
    public interface IFlipper {
        public bool Flip();

        public bool Flip(bool left);

        public bool Flip(float xInput);

        public bool IsFacingRight { get; }
    }

    public class FlipperList : IFlipper {
        private IFlipper _main;
        private IFlipper[] _flippers = Array.Empty<IFlipper>();

        public FlipperList() {
        }

        public FlipperList(params IFlipper[] flippers) {
            SetFlippers(flippers);
        }

        public FlipperList AddSprite(Sprite2D sprite) {
            AddFlipper(new SpriteFlipper(sprite));
            return this;
        }

        public FlipperList AddNode2D(Node2D node2D) {
            AddFlipper(new Node2DFlipper(node2D));
            return this;
        }

        public FlipperList AddFlipper(IFlipper flipper) {
            if (_main == null) {
                _main = flipper;
            } else {
                _flippers = _flippers.Concat(new[] { flipper }).ToArray();
            }
            return this;
        }

        public void SetFlippers(params IFlipper[] flippers) {
            _main = flippers[0];
            if (flippers.Length <= 1) return;
            _flippers = new IFlipper[flippers.Length - 1];
            Array.Copy(flippers, 1, _flippers, 0, flippers.Length - 1);
        }

        public bool Flip() {
            Array.ForEach(_flippers, flipper => flipper.Flip());
            return _main.Flip();
        }

        public bool Flip(bool left) {
            Array.ForEach(_flippers, flipper => flipper.Flip(left));
            return _main.Flip(left);
        }

        public bool Flip(float xInput) {
            Array.ForEach(_flippers, flipper => flipper.Flip(xInput));
            return _main.Flip(xInput);
        }

        public bool IsFacingRight => _main.IsFacingRight;
    }

    public abstract class Flipper<T> : IFlipper {
        protected T Node2D { get; }
        private bool _isFacingRight = false;

        protected Flipper(T node2D) {
            Node2D = node2D;
            _isFacingRight = LoadIsFacingRight();
        }

        public abstract bool LoadIsFacingRight();
        public abstract void ChangeFacingRight(bool right);

        public bool Flip() {
            IsFacingRight = !_isFacingRight;
            return _isFacingRight;
        }

        public bool Flip(bool left) {
            IsFacingRight = !left;
            return _isFacingRight;
        }

        public bool Flip(float xInput) {
            if (xInput != 0) {
                bool shouldFaceRight = xInput > 0;
                IsFacingRight = shouldFaceRight;
            }
            return _isFacingRight;
        }

        public bool IsFacingRight {
            get => _isFacingRight;
            set {
                if (value == _isFacingRight) return;
                _isFacingRight = value;
                ChangeFacingRight(_isFacingRight);
            }
        }
    }

    public class SpriteFlipper : Flipper<Sprite2D> {
        public SpriteFlipper(Sprite2D node2D) : base(node2D) {
        }

        public override bool LoadIsFacingRight() {
            return !Node2D.FlipH;
        }

        public override void ChangeFacingRight(bool right) {
            Node2D.FlipH = !right;
        }
    }

    public class Node2DFlipper : Flipper<Node2D> {
        public Node2DFlipper(Node2D node2D) : base(node2D) {
        }

        public override bool LoadIsFacingRight() {
            return (int)Node2D.Scale.y == 1 && (Node2D.Transform.Rotation == 0);
        }

        public override void ChangeFacingRight(bool right) {
            if (right) {
                // Return to normal position
                Node2D.Scale = Vector2.One; // 1,1
                Node2D.Rotation = 0;
            } else {
                // Flip to the left: scale y -1 and rotate 180 degrees
                Node2D.Scale = new Vector2(1, -1);
                Node2D.Rotate(Mathf.Pi);
            }
        }
    }
}