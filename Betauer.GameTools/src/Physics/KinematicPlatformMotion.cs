using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Nodes;
using Godot;
using GodotObject = Godot.GodotObject;
using static Betauer.Physics.KinematicFormulas;

namespace Betauer.Physics; 

public class KinematicPlatformMotion {
    public CharacterBody2D CharacterBody { get; }
    public readonly List<RayCast2D>? FloorRaycasts;

    private Vector2 _floorUpDirection = Vector2.Up;
    public Vector2 FloorUpDirection {
        get => _floorUpDirection;
        set {
            _floorUpDirection = value;
            CharacterBody.UpDirection = value;
            UpRightNormal = value.Rotated(Mathf.Pi / 4); // up -> 45º

            // If FloorUpDirection has a different direction than Vector.UP, this field store the difference, so it
            // can be uses to transform original Up with regular up
            _anglesToRotateFloor = Vector2.Up.AngleTo(value);
        }
    }

    private float _anglesToRotateFloor = 0;
    
    public Vector2 UpRightNormal { get; private set; }

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

    // Floor
    private bool _isOnFloor = false;
    private bool _isOnSlope = false;
    private Vector2 _floorNormal = Vector2.Zero;
    private GodotObject? _floor = null;
    private bool _wasOnFloor = false;
    private bool _isJustOnFloor = false;
    private bool _isJustTookOff = false;

    // Wall
    private bool _isOnWall = false;
    private Vector2 _wallNormal = Vector2.Zero;
    private GodotObject? _wall = null;
    private bool _wasOnWall = false;
    private bool _isJustOnWall = false;

    // Ceiling
    private bool _isOnCeiling = false;

    public KinematicPlatformMotion(CharacterBody2D characterBody, Vector2? floorUpDirection, List<RayCast2D>? floorRaycasts = null) {
        CharacterBody = characterBody;
        FloorUpDirection = floorUpDirection ?? Vector2.Up;
        FloorRaycasts = floorRaycasts;
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

    // Floor flags
    public bool WasOnFloor() => _wasOnFloor;
    public bool IsJustOnFloor() => _isJustOnFloor;
    public bool IsJustTookOff() => _isJustTookOff;
    public bool IsOnFloor() => _isOnFloor;
    public bool IsOnSlope() => _isOnSlope;

    public bool IsOnSlopeUpRight() {
        // UpRightNormal is a 45º angle. IsSameDirectionAngle uses 45º by default, it means a 90º cone
        return IsOnSlope() && GetFloorNormal().IsSameDirectionAngle(UpRightNormal);
    }

    // Floor collider
    public Vector2 GetFloorNormal() => _floorNormal;
    public T? GetFloorCollider<T>() where T : Node => GetFloorColliders<T>().FirstOrDefault();
    // public Vector2 GetFloorVelocity() => Body.GetFloorVelocity();
    // public bool HasFloorLateralMovement() => GetFloorVelocity().X != 0;

    // Wall flags
    public bool WasOnWall() => _wasOnWall;
    public bool IsJustOnWall() => _isJustOnWall;
    public bool IsOnWall() => _isOnWall;
    public bool IsOnWallRight() => IsOnWall() && GetWallNormal().IsLeft(FloorUpDirection);
    // Wall collider
    public Vector2 GetWallNormal() => _wallNormal;
    public T? GetWallCollider<T>() where T : Node => GetWallColliders<T>().FirstOrDefault();

    // Ceiling flags
    public bool IsOnCeiling() => _isOnCeiling;
    // Ceiling collider
    public Vector2 GetCeilingNormal() => GetCeilingCollisions().FirstOrDefault()?.GetNormal() ?? Vector2.Zero;
    public T? GetCeilingCollider<T>() where T : Node => GetCeilingColliders<T>().FirstOrDefault();

    public void ApplyGravity(float gravity, float maxSpeed, float delta) {
        MotionY = Mathf.Min(MotionY + gravity * delta, maxSpeed);
    }

    public void ApplyConstantAcceleration(float xInput, float yInput, 
        float acceleration,
        float maxSpeedX,
        float maxSpeedY,
        float friction, 
        float stopIfSpeedIsLessThan, 
        float changeDirectionFactor, 
        float delta) {
            
        if (xInput != 0 && yInput != 0) {
            var input = new Vector2(xInput, yInput);
            if (input.Length() > 1) {
                input = input.Normalized();
                xInput = input.X;
                yInput = input.Y;
            }
        }
        Accelerate(ref MotionX, xInput, acceleration, maxSpeedX,
            friction, stopIfSpeedIsLessThan, changeDirectionFactor, delta);
        Accelerate(ref MotionY, yInput, acceleration, maxSpeedY,
            friction, stopIfSpeedIsLessThan, changeDirectionFactor, delta);
    }

    public void ApplyLateralFriction(float friction, float stopIfSpeedIsLessThan) {
        SlowDownSpeed(ref MotionX, friction, stopIfSpeedIsLessThan);
    }

    public void ApplyVerticalFriction(float friction, float stopIfSpeedIsLessThan) {
        SlowDownSpeed(ref MotionY, friction, stopIfSpeedIsLessThan);
    }

    public void ApplyLateralConstantAcceleration(float xInput,
        float acceleration,
        float maxSpeed,
        float friction,
        float stopIfSpeedIsLessThan,
        float changeDirectionFactor,
        float delta) {
            
        Accelerate(ref MotionX, xInput, acceleration, maxSpeed, 
            friction, stopIfSpeedIsLessThan, changeDirectionFactor, delta);
    }

    public bool Move() {
        CharacterBody.Velocity = GetRotatedVelocity();
        var collide = CharacterBody.MoveAndSlide();
        Motion = RollbackRotateVelocity(CharacterBody.Velocity);
        UpdateFlags();
        return collide;
    }

    private void UpdateFlags() {
        _wasOnFloor = _isOnFloor;
        _wasOnWall = _isOnWall;
        _isOnFloor = CharacterBody.IsOnFloor();
        _isOnSlope = false;
        _floorNormal = CharacterBody.GetFloorNormal();
        _floor = null;

        _isOnWall = CharacterBody.IsOnWall();
        _wallNormal = CharacterBody.GetWallNormal();
        _wall = null;

        _isOnCeiling = CharacterBody.IsOnCeiling(); 

        _isJustOnFloor = false;
        _isJustTookOff = false;

        _isJustOnWall = _isOnWall && !_wasOnWall;
        if (_isOnFloor) {
            _isJustOnFloor = !_wasOnFloor;
            _isOnSlope = _floorNormal != FloorUpDirection;
        } else {
            _isJustTookOff = _wasOnFloor;
        }
    }

    /// <summary>
    /// This is needed because:
    /// - If player collides against a wall with a tilemap, only one collision is returned withe the tilemap and the
    /// wall normal. No floor normal collision is returned.
    /// - If player walks through a slope, sometimes there is no collision. Increase the gravity reduces the amount
    /// of times the slope floor collision is missing, but not at 100%... and too much gravity avoid climb the slope.
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<RayCast2D> GetFloorRaycastColliding() {
        return FloorRaycasts?
                   .Where(r => {
                       r.ForceRaycastUpdate();
                       return r.IsColliding();
                   })
               ?? Enumerable.Empty<RayCast2D>();
    }

    public IEnumerable<Node> GetFloorRaycastColliders() =>
        GetFloorRaycastColliding().Where(r => r.GetCollisionNormal().IsFloor(FloorUpDirection))
            .Select(r => (Node)r.GetCollider());

    public IEnumerable<Node> GetFloorColliders(Func<Node, bool>? predicate = null) => GetFloorColliders<Node>(predicate); 
    public IEnumerable<Node> GetWallColliders(Func<Node, bool>? predicate = null) => GetWallColliders<Node>(predicate); 
    public IEnumerable<Node> GetCeilingColliders(Func<Node, bool>? predicate = null) => GetCeilingColliders<Node>(predicate); 
        
    public IEnumerable<T> GetFloorColliders<T>(Func<T, bool>? predicate = null) where T : Node => CharacterBody.GetFloorColliders(FloorUpDirection, predicate); 
    public IEnumerable<T> GetWallColliders<T>(Func<T, bool>? predicate = null) where T : Node => CharacterBody.GetWallColliders(FloorUpDirection, predicate); 
    public IEnumerable<T> GetCeilingColliders<T>(Func<T, bool>? predicate = null) where T : Node => CharacterBody.GetCeilingColliders(FloorUpDirection, predicate);
        
    public IEnumerable<KinematicCollision2D> GetFloorCollisions(Func<KinematicCollision2D, bool>? predicate = null) => CharacterBody.GetFloorCollisions(FloorUpDirection, predicate); 
    public IEnumerable<KinematicCollision2D> GetWallCollisions(Func<KinematicCollision2D, bool>? predicate = null) => CharacterBody.GetWallCollisions(FloorUpDirection, predicate); 
    public IEnumerable<KinematicCollision2D> GetCeilingCollisions(Func<KinematicCollision2D, bool>? predicate = null) => CharacterBody.GetCeilingCollisions(FloorUpDirection, predicate); 

    public string GetFloorCollisionInfo() {
        if (!IsOnFloor()) return "";
        var collider = GetFloorColliders().FirstOrDefault();
        if (collider == null) {
            collider = GetFloorRaycastColliders().FirstOrDefault();
        }
        return $"{(IsOnSlope() ? IsOnSlopeUpRight() ? "/" : "\\" : "flat")} {GetFloorNormal().ToString("0.0")} {Mathf.RadToDeg(GetFloorNormal().Angle()):0.0}º [{collider?.GetType().Name}] {collider?.Name}";
    }

    public string GetWallCollisionInfo() {
        if (!IsOnWall()) return "";
        var collider = (Node)GetWallCollisions().FirstOrDefault()!.GetCollider();
        return $"{(IsOnWallRight() ? "R" : "L")} {GetWallNormal().ToString("0.0")} {Mathf.RadToDeg(GetWallNormal().Angle()):0.0}º [{collider.GetType().Name}] {collider.Name}";
    }

    public string GetCeilingCollisionInfo() {
        if (!IsOnCeiling()) return "";
        var firstCeilingCollision = GetCeilingCollisions().FirstOrDefault();
        var collider = (Node)firstCeilingCollision!.GetCollider();
        return $"{firstCeilingCollision.GetNormal().ToString("0.0")} {Mathf.RadToDeg(firstCeilingCollision.GetNormal().Angle()):0.0}º [{collider.GetType().Name}] {collider.Name}";
    }
}