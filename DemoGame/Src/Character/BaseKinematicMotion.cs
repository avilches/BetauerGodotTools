using System;
using Betauer.Core;
using Godot;

namespace Veronenger.Character; 

public abstract class BaseKinematicMotion {
    public CharacterBody2D Body { get; private set; }
    public Marker2D Marker2D { get; private set; }

    private float _anglesToRotateFloor = 0;
    private Vector2 _floorUpDirection = Vector2.Up;
    private Vector2 _lookRightDirection = Vector2.Right;
    public Vector2 FloorUpDirection {
        get => _floorUpDirection;
        set {
            _floorUpDirection = value;
            _anglesToRotateFloor = Vector2.Up.AngleTo(FloorUpDirection);
            _lookRightDirection = _floorUpDirection.Right();
            Body.UpDirection = FloorUpDirection;
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

    protected void Configure(string name, CharacterBody2D body, Marker2D marker2D, Vector2 floorUpDirection) {
        Body = body;
        Marker2D = marker2D;
        FloorUpDirection = floorUpDirection;
    }

    public void SetDelta(double delta) {
        Delta = (float)delta;
    }

    public void SetDelta(float delta) {
        Delta = delta;
    }

    protected Vector2 GetRotatedVelocity() {
        return _anglesToRotateFloor > 0f ? Motion.Rotated(_anglesToRotateFloor) : Motion;
    }

    protected Vector2 RollbackRotateVelocity(Vector2 pendingInertia) {
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

    /// <summary>
    /// Return true if the current body is to the right of the parameter 
    /// </summary>
    /// <param name="node2D"></param>
    /// <returns></returns>
    public bool IsToTheRightOf(Node2D node2D) {
        var angle = AngleTo(node2D);
        return Mathf.Abs(angle) > Mathf.Pi / 2;
    }

    public float AngleTo(Node2D node2D) {
        return _lookRightDirection.AngleTo(node2D.GlobalPosition - Marker2D.GlobalPosition);
    }
}