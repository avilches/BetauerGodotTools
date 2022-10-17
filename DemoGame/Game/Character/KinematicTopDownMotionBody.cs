using System;
using System.Text;
using Godot;
using Betauer;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using static Godot.Mathf;

namespace Veronenger.Game.Character {
    [Service(Lifetime.Transient)]
    public class KinematicTopDownMotionBody : BaseMotionBody {

        public bool DefaultSlideOnSlopes;

        public void Configure(string name, KinematicBody2D body, Position2D position2D, bool defaultSlideOnSlopes) {
            base.Configure(name, body, position2D);
            DefaultSlideOnSlopes = defaultSlideOnSlopes;
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
            Accelerate(ref ForceX, xInput, acceleration, maxSpeedX,
                friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
            Accelerate(ref ForceY, yInput, acceleration, maxSpeedY,
                friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
            
        }

        public void StopLateralSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            SlowDownSpeed(ref ForceX, friction, stopIfSpeedIsLessThan);
        }

        public void StopVerticalSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            SlowDownSpeed(ref ForceY, friction, stopIfSpeedIsLessThan);
        }

        public void LimitSpeed(Vector2 maxSpeed) {
            ForceX = Clamp(ForceX, -maxSpeed.x, maxSpeed.x);
            ForceY = Clamp(ForceY, -maxSpeed.y, maxSpeed.y);
        }

        public void Slide() => Slide(Vector2.One, DefaultSlideOnSlopes);
        public void Slide(Vector2 slowdownVector) => Slide(slowdownVector, DefaultSlideOnSlopes);
        public void Slide(bool slideOnSlopes) => Slide(Vector2.One, slideOnSlopes);
        public void Slide(Vector2 slowdownVector, bool slideOnSlopes) {
            var remain = Body.MoveAndSlide(Force * slowdownVector);
            if (!slideOnSlopes) {
                ForceY = remain.y;
                ForceX = remain.x;
            }
        }
    }
}