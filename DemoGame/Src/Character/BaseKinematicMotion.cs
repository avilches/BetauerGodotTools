using System;
using Betauer;
using Godot;

namespace Veronenger.Character {
    public abstract class BaseKinematicMotion {
        public CharacterBody2D Body { get; private set; }
        public Position2D Position2D { get; private set; }

        private float _anglesToRotateFloor = 0;
        private Vector2 _floorUpDirection = Vector2.Up;
        public Vector2 FloorUpDirection {
            get => _floorUpDirection;
            set {
                _floorUpDirection = value;
                _anglesToRotateFloor = Vector2.Up.AngleTo(FloorUpDirection);
            }
        }
        
        // Motion is the desired speed to achieve. The final speed should match, but it could be different because
        // the friction or collision
        public float MotionX;
        public float MotionY;
        public Vector2 Motion {
            get => new(MotionX, MotionY);
            set {
                MotionX = value.x;
                MotionY = value.y;
            }
        }

        public float Delta { get; private set; } = 0;

        protected void Configure(string name, CharacterBody2D body, Position2D position2D, Vector2 floorUpDirection) {
            Body = body;
            Position2D = position2D;
            FloorUpDirection = floorUpDirection;
        }

        public void SetDelta(float delta) {
            Delta = delta;
        }

        protected Vector2 RotateSpeed() {
            return _anglesToRotateFloor > 0f ? Motion.Rotated(_anglesToRotateFloor) : Motion;
        }

        protected Vector2 RotateInertia(Vector2 pendingInertia) {
            return _anglesToRotateFloor > 0f ? pendingInertia.Rotated(-_anglesToRotateFloor) : pendingInertia;
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

        public void LimitMotion(Vector2 maxSpeed) {
            LimitMotionX(maxSpeed.x);
            LimitMotionY(maxSpeed.y);
        }

        public void LimitMotionX(float maxSpeed) {
            LimitMotionX(-maxSpeed, maxSpeed);
        }

        public void LimitMotionY(float maxSpeed) {
            LimitMotionY(-maxSpeed, maxSpeed);
        }

        public void LimitMotionX(float start, float end) {
            MotionX = Mathf.Clamp(MotionX, start, end);
        }

        public void LimitMotionY(float start, float end) {
            MotionY = Mathf.Clamp(MotionY, start, end);
        }

        public void LimitMotionNormalized(float maxSpeed) {
            var limited = Motion.LimitLength(maxSpeed);
            MotionX = limited.x;
            MotionY = limited.y;
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