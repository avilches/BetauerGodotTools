using System;
using Godot;

namespace Veronenger.Character; 

public static class KinematicFormulas {
    /// Update the speed with a constant acceleration, taking into account the friction and the max speed
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
            var frameAcceleration = Math.Sign(amount) * acceleration * delta;
            if (directionChanged) {
                speed = speed * changeDirectionFactor + frameAcceleration;
            } else {
                maxSpeed *= Math.Abs(amount);
                speed = Mathf.Clamp(speed + frameAcceleration, -maxSpeed, maxSpeed);;
            }
        } else {
            SlowDownSpeed(ref speed, friction, stopIfSpeedIsLessThan);
        }
    }

    /// <summary>
    ///  Update the speed with a constant deceleration, taking into account the friction and the max speed
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="deceleration"></param>
    /// <param name="stopIfSpeedIsLessThan"></param>
    /// <param name="delta"></param>
    public static void Decelerate(ref float speed, float deceleration, float stopIfSpeedIsLessThan, float delta) {
        var absSpeed = Math.Abs(speed);
        if (absSpeed < stopIfSpeedIsLessThan) {
            speed = 0;
        } else {
            speed = Math.Max(0, absSpeed - deceleration * delta) * Math.Sign(speed);
        }
    }

    public static void SlowDownSpeed(ref float speed, float friction, float stopIfSpeedIsLessThan) {
        if (Mathf.Abs(speed) < stopIfSpeedIsLessThan) {
            speed = 0;
        } else {
            speed *= friction;
        }
    }
}