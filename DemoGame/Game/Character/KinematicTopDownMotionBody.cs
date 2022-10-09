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
    }
    
    [Service(Lifetime.Transient)]
    public class KinematicTopDownMotionBody : BaseMotionBody, IKinematicTopDownMotionBodyConfig {

        public bool DefaultSlideOnSlopes { get; set; } = true;

        public void Configure(string name, KinematicBody2D body, Position2D position2D, Action<IKinematicTopDownMotionBodyConfig> conf) {
            base.Configure(name, body, position2D);
            conf(this);
        }
        
        public void AddSpeed(float xInput, float yInput, 
            float acceleration,
            float maxSpeedX,
            float maxSpeedY,
            float friction, 
            float stopIfSpeedIsLessThan, 
            float changeDirectionFactor) {
            if (xInput != 0 && yInput != 0) {
                var input = new Vector2(xInput, yInput);
                if (input.Length() > 1) {
                    input = input.Normalized();
                    xInput = input.x;
                    yInput = input.y;
                }
            }
            Accelerate(ref SpeedX, xInput, acceleration, maxSpeedX,
                friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
            Accelerate(ref SpeedY, yInput, acceleration, maxSpeedY,
                friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
            
        }

        public void StopLateralSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            SlowDownSpeed(ref SpeedX, friction, stopIfSpeedIsLessThan);
        }

        public void StopVerticalSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            SlowDownSpeed(ref SpeedY, friction, stopIfSpeedIsLessThan);
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