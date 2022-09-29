using System;
using Betauer;
using Godot;

namespace Veronenger.Game.Character {
    public class BaseMotionBody  : IFlipper {
        protected Logger LoggerMotion;
        protected KinematicBody2D Body;
        protected Position2D Position2D;
        
        public Vector2 Motion { get; private set; } = Vector2.Zero;
        public Vector2 LastMotion { get; private set; } = Vector2.Zero;
        public float Delta { get; private set; } = 0;

        private IFlipper _flippers;

        protected void Configure(string name, KinematicBody2D body, IFlipper flippers, Position2D position2D) {
            Body = body;
            _flippers = flippers;
            Position2D = position2D;
            LoggerMotion = LoggerFactory.GetLogger($"{name}.Motion");
        }

        public void SetMotionX(float x) => Motion = new Vector2(x, Motion.y);
        public void AddMotionX(float x) => Motion += new Vector2(x, 0);
        public void SetMotionY(float y) => Motion = new Vector2(Motion.x, y);
        public void AddMotionY(float y) => Motion += new Vector2(0, y);
        public bool IsFacingRight => _flippers.IsFacingRight;
        public bool Flip() => _flippers.Flip();
        public bool Flip(bool left) => _flippers.Flip(left);
        public bool Flip(float xInput) => _flippers.Flip(xInput);

        public virtual void StartFrame(float delta) {
            Delta = delta;
            LastMotion = Motion;
        }

        public virtual void EndFrame() {
            #if DEBUG
            if (Motion != LastMotion) {
                LoggerMotion.Debug($"Motion:{Motion.ToString()} (diff {(LastMotion - Motion).ToString()})");
            }
            #endif
        }
        
        /**
         * node is | I'm facing  | flip?
         * right   | right       | no
         * right   | left        | yes
         * left    | right       | yes
         * left    | left        | no
         *
         */
        public void FaceTo(Node2D node2D) {
            if (IsToTheRightOf(node2D) != _flippers.IsFacingRight) {
                _flippers.Flip();
            }
        }

        public bool IsToTheLeftOf(Node2D node2D) {
            return Math.Abs(DegreesTo(node2D) - 180f) < 0.1f;
        }

        public bool IsToTheRightOf(Node2D node2D) {
            return DegreesTo(node2D) == 0f;
        }

        public float DegreesTo(Node2D node2D) {
            return Mathf.Rad2Deg(Body.ToLocal(node2D.GlobalPosition).AngleTo(Position2D.Position));
        }


    }
}