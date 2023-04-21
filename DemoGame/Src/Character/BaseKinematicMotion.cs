using System;
using Betauer.Core;
using Godot;

namespace Veronenger.Character; 

public abstract class BaseKinematicMotion {
    public CharacterBody2D CharacterBody { get; }
    public Func<Vector2> GlobalPosition { get; }

    private float _anglesToRotateFloor = 0;
    private Vector2 _floorUpDirection = Vector2.Up;
    
    public Vector2 LookRightDirection { get; private set; }
    public Vector2 UpRightNormal { get; private set; }
    
    public Vector2 FloorUpDirection {
        get => _floorUpDirection;
        set {
            _floorUpDirection = value;
            CharacterBody.UpDirection = value;
            LookRightDirection = value.Rotate90Right();
            UpRightNormal = value.Rotated(Mathf.Pi / 4); // up -> 45º

            // If FloorUpDirection has a different direction than Vector.UP, this field store the difference, so it
            // can be uses to transform original Up with regular up
            _anglesToRotateFloor = Vector2.Up.AngleTo(value);
        }
    }
        
    // Motion is the desired speed to achieve. The final speed should match, but it could be different because
    // the friction or collision
    public float MotionX;
    public float MotionY;
    public Vector2 Motion {
        get => new(MotionX, MotionY);
        set {
            MotionX = value.X;
            MotionY = value.Y;
        }
    }

    protected BaseKinematicMotion(CharacterBody2D characterBody, Func<Vector2>? globalPosition, Vector2? floorUpDirection) {
        CharacterBody = characterBody;
        GlobalPosition = globalPosition ?? (() => CharacterBody.GlobalPosition);
        FloorUpDirection = floorUpDirection ?? Vector2.Up;
    }


    protected Vector2 GetRotatedVelocity() {
        return _anglesToRotateFloor > 0f ? Motion.Rotated(_anglesToRotateFloor) : Motion;
    }

    protected Vector2 RollbackRotateVelocity(Vector2 pendingInertia) {
        return _anglesToRotateFloor > 0f ? pendingInertia.Rotated(-_anglesToRotateFloor) : pendingInertia;
    }

    public void LimitMotion(Vector2 maxSpeed) {
        LimitMotionX(maxSpeed.X);
        LimitMotionY(maxSpeed.Y);
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
        MotionX = limited.X;
        MotionY = limited.Y;
    }

    /// <summary>
    /// Return true if the current body is to the right of the parameter 
    /// </summary>
    /// <param name="globalPosition"></param>
    /// <returns></returns>
    public bool IsToTheRightOf(Vector2 globalPosition) => LookRightDirection.IsOppositeDirection(DirectionTo(globalPosition));

    public float DistanceTo(Vector2 globalPosition) => GlobalPosition().DistanceTo(globalPosition);
    
    public Vector2 DirectionTo(Vector2 globalPosition) => GlobalPosition().DirectionTo(globalPosition);

    public float AngleTo(Vector2 globalPosition) => LookRightDirection.AngleTo(DirectionTo(globalPosition));
}