using System;
using System.Text;
using Godot;
using Betauer;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using static Godot.Mathf;

namespace Veronenger.Game.Character {
    public interface IKinematicTopDownMotionBodyConfig {
        public bool DefaultSlideOnSlopes { set; } 
        public Vector2 DefaultMaxSpeed { set; }  
    }
    
    [Service(Lifetime.Transient)]
    public class KinematicTopDownMotionBody : BaseMotionBody, IKinematicTopDownMotionBodyConfig {

        public bool DefaultSlideOnSlopes { get; set; } = true;
        public Vector2 DefaultMaxSpeed { get; set; } = Vector2.One; 

        public void Configure(string name, KinematicBody2D body, Position2D position2D, Action<IKinematicTopDownMotionBodyConfig> conf) {
            base.Configure(name, body, position2D);
            conf(this);
        }
        
        public void AddSpeed(Vector2 input, float acceleration, float friction, 
            float stopIfSpeedIsLessThan, float changeDirectionFactor) {
            if (input.x != 0 && input.y != 0 && input.Length() > 1) {
                input = input.Normalized();
            }
            var xInput = input.x;
            var yInput = input.y;
            if (xInput != 0) {
                var directionChanged = SpeedX != 0 && Math.Sign(SpeedX) != Math.Sign(xInput);
                if (directionChanged) {
                    SpeedX = SpeedX * changeDirectionFactor + xInput * acceleration * Delta;
                } else {
                    SpeedX += xInput * acceleration * Delta;
                }
            } else {
                StopLateralSpeedWithFriction(friction, stopIfSpeedIsLessThan);
            }
            if (yInput != 0) {
                var directionChanged = SpeedY != 0 && Math.Sign(SpeedY) != Math.Sign(yInput);
                if (directionChanged) {
                    SpeedY = SpeedY * changeDirectionFactor + yInput * acceleration * Delta;
                } else {
                    SpeedY += yInput * acceleration * Delta;
                }
            } else {
                StopVerticalSpeedWithFriction(friction, stopIfSpeedIsLessThan);
            }
        }

        public void StopLateralSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            if (Abs(SpeedX) < stopIfSpeedIsLessThan) {
                SpeedX = 0;
            } else {
                SpeedX *= friction;
                // SetSpeedX(SpeedX - (deceleration * Delta * Math.Sign(SpeedX)));
            }
        }

        public void StopVerticalSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            if (Abs(SpeedY) < stopIfSpeedIsLessThan) {
                SpeedY = 0;
            } else {
                SpeedY *= friction;
                // SetSpeedX(SpeedX - (deceleration * Delta * Math.Sign(SpeedX)));
            }
        }

        public void LimitDefaultSpeed(float factor = 1f) {
            LimitSpeed(DefaultMaxSpeed * factor);
        }

        public void LimitSpeed(Vector2 maxSpeed) {
            SpeedX = Clamp(SpeedX, -maxSpeed.x, maxSpeed.x);
            SpeedY = Clamp(SpeedY, -maxSpeed.y, maxSpeed.y);
        }

        public void Slide() => Slide(Vector2.One, DefaultSlideOnSlopes);
        public void Slide(Vector2 slowdownVector) => Slide(slowdownVector, DefaultSlideOnSlopes);
        public void Slide(bool slideOnSlopes) => Slide(Vector2.One, slideOnSlopes);
        public void Slide(Vector2 slowdownVector, bool slideOnSlopes) {
            var remain = Body.MoveAndSlide(Speed * slowdownVector);
            if (!slideOnSlopes) {
                SpeedY = remain.y;
                SpeedX = remain.x;
            }
        }
    }
}