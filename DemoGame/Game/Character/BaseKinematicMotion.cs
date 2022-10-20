using System;
using Betauer;
using Godot;

namespace Veronenger.Game.Character {
    public abstract class BaseKinematicMotion {
        protected KinematicBody2D Body;
        protected Position2D Position2D;
        
        // Speed is the desired speed to achieve. The final speed should match, but it could be different because
        // the friction or collision
        public float SpeedX;
        public float SpeedY; 
        public Vector2 Speed {
            get => new(SpeedX, SpeedY);
            set {
                SpeedX = value.x;
                SpeedY = value.y;
            }
        }

        public float Delta { get; private set; } = 0;

        protected void Configure(string name, KinematicBody2D body, Position2D position2D) {
            Body = body;
            Position2D = position2D;
        }

        public virtual void StartFrame(float delta) {
            Delta = delta;
        }

        public virtual void EndFrame() {
        }

        public static void Accelerate(
            ref float speed, // The current speed to update 
            float amount, // From 0 to 1. Use XInput or YInput. Or 0 to stop and 1 to full throttle 100%
            
            // Acceleration rate in pixels/second
            float acceleration,
            float maxSpeed,
            
            // Slow down if amount is 0
            float friction, // The negative acceleration to slow down, in pixels/second
            float stopIfSpeedIsLessThan, // If new speed is less than this value, the speed will be 0
            
            // If amount is changing direction, apply this factor to slow down first the current speed. So, 0.5 means
            // the speed will be the half
            float changeDirectionFactor,
            
            float delta) {
            if (amount != 0) {
                var directionChanged = speed != 0 && Math.Sign(speed) != Math.Sign(amount);
                var frameAcceleration = amount * acceleration * delta;
                if (directionChanged) {
                    speed = speed * changeDirectionFactor + frameAcceleration;
                } else {
                    speed = Mathf.Clamp(speed + frameAcceleration, -maxSpeed, maxSpeed);;
                }
            } else {
                SlowDownSpeed(ref speed, friction, stopIfSpeedIsLessThan);
            }
        }

        public static void SlowDownSpeed(ref float speed, float friction, float stopIfSpeedIsLessThan) {
            if (Mathf.Abs(speed) < stopIfSpeedIsLessThan) {
                speed = 0;
            } else {
                speed *= friction;
            }
        }

        public static void Decelerate(ref float speed, float deceleration, float stopIfSpeedIsLessThan, float delta) {
            var absSpeed = Math.Abs(speed);
            if (absSpeed < stopIfSpeedIsLessThan) {
                speed = 0;
            } else {
                speed = Math.Max(0, absSpeed - deceleration * delta) * Math.Sign(speed);
            }
        }

        public void LimitSpeed(Vector2 maxSpeed) {
            LimitSpeedX(maxSpeed.x);
            LimitSpeedY(maxSpeed.y);
        }

        public void LimitSpeedX(float maxSpeed) {
            LimitSpeedX(-maxSpeed, maxSpeed);
        }

        public void LimitSpeedY(float maxSpeed) {
            LimitSpeedY(-maxSpeed, maxSpeed);
        }

        public void LimitSpeedX(float start, float end) {
            SpeedX = Mathf.Clamp(SpeedX, start, end);
        }

        public void LimitSpeedY(float start, float end) {
            SpeedY = Mathf.Clamp(SpeedY, start, end);
        }

        public void LimitSpeedNormalized(float maxSpeed) {
            var limited = Speed.LimitLength(maxSpeed);
            SpeedX = limited.x;
            SpeedY = limited.y;
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