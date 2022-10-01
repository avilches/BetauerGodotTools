using System;
using Betauer;
using Betauer.StateMachine;
using Godot;

namespace Veronenger.Game.Character {
    public abstract class BaseMotionBody {
        protected Logger LoggerMotion;
        protected KinematicBody2D Body;
        protected Position2D Position2D;
        
        public float SpeedX;
        public float SpeedY; 
        public Vector2 Speed {
            get => new(SpeedX, SpeedY);
            set {
                SpeedX = value.x;
                SpeedY = value.y;
            }
        }

        public Vector2 PreviousSpeed { get; private set; } = Vector2.Zero;
        public float Delta { get; private set; } = 0;

        protected void Configure(string name, KinematicBody2D body, Position2D position2D) {
            Body = body;
            Position2D = position2D;
            LoggerMotion = LoggerFactory.GetLogger($"{name}.Motion");
        }

        public void Use(StateMachine stateMachine) {
            
        }
        public virtual void StartFrame(float delta) {
            Delta = delta;
            PreviousSpeed = Speed;
        }

        public virtual void EndFrame() {
            #if DEBUG
            if (Speed != PreviousSpeed) {
                LoggerMotion.Debug($"Motion:{Speed.ToString()} (diff {(PreviousSpeed - Speed).ToString()})");
            }
            #endif
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