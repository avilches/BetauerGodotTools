using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Betauer;
using Betauer.Core;
using Betauer.Core.Nodes;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Object = Godot.Object;

namespace Veronenger.Character; 

public class KinematicPlatformMotion : BaseKinematicMotion, IFlipper {
    private readonly IFlipper _flippers;
    protected readonly List<RayCast2D>? FloorRaycasts;

    private bool _isJustLanded = false;
    private bool _isJustTookOff = false;

    // Floor
    private bool _isOnFloor = false;
    private bool _isOnSlope = false;
    private Vector2 _floorNormal = Vector2.Zero;
    private Object? _floor = null;

    // Wall
    private bool _isOnWall = false;
    private Vector2 _wallNormal = Vector2.Zero;
    private Object? _wall = null;

    // Ceiling
    private bool _isOnCeiling = false;


    public KinematicPlatformMotion(CharacterBody2D characterBody, IFlipper flippers, Marker2D marker2D, Vector2 floorUpDirection, List<RayCast2D>? floorRaycasts = null) :
        base(characterBody, marker2D, floorUpDirection) {
        _flippers = flippers;
        FloorRaycasts = floorRaycasts;
    }

    public int FacingRight => IsFacingRight ? 1 : -1; 

    public bool IsFacingRight {
        get => _flippers.IsFacingRight;
        set => _flippers.IsFacingRight = value;
    }

    public void Flip() => _flippers.Flip();
    public void Flip(float xInput) => _flippers.Flip(xInput);

    public void FaceTo(Node2D node2D) => FaceTo(node2D.GlobalPosition);
    public void FaceOppositeTo(Node2D node2D) => FaceOppositeTo(node2D.GlobalPosition);
    public bool IsFacingTo(Node2D node2D) => IsFacingTo(node2D.GlobalPosition);

    public void FaceTo(Vector2 globalPosition) {
        if (!IsFacingTo(globalPosition)) _flippers.Flip();
    }

    public void FaceOppositeTo(Vector2 globalPosition) {
        if (IsFacingTo(globalPosition)) _flippers.Flip();
    }

    /*
     *  IsToTheRightOf | IsFacingRight | 
     *  true           | true          |   globalPosition  -   Body:)
     *  true           | false         |   globalPosition  - (:Body
     *  false          | true          |   Body:)  -  globalPosition
     *  false          | false         | (:Body    -  globalPosition
     */
    public bool IsFacingTo(Vector2 globalPosition) => IsToTheRightOf(globalPosition) != _flippers.IsFacingRight;

    public bool IsJustLanded() => _isJustLanded;
    public bool IsJustTookOff() => _isJustTookOff;

    // Floor flags
    public bool IsOnFloor() => _isOnFloor;
    public bool IsOnSlope() => _isOnSlope;
    public bool IsOnSlopeUpRight() => IsOnSlope() && GetFloorNormal().IsSameDirection(FloorUpDirection) && GetFloorNormal().IsRight(FloorUpDirection);
    // Floor collider
    public Vector2 GetFloorNormal() => _floorNormal;
    public T? GetFloorCollider<T>() where T : Node => GetFloorColliders<T>().FirstOrDefault();
    // public Vector2 GetFloorVelocity() => Body.GetFloorVelocity();
    // public bool HasFloorLateralMovement() => GetFloorVelocity().x != 0;

    // Wall flags
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

    public void ApplyGravity(float gravity, float maxSpeed) {
        MotionY = Mathf.Min(MotionY + gravity * Delta, maxSpeed);
    }

    public void AddLateralSpeed(float xInput,
        float acceleration,
        float maxSpeed,
        float friction,
        float stopIfSpeedIsLessThan,
        float changeDirectionFactor) {
            
        Accelerate(ref MotionX, xInput, acceleration, maxSpeed, 
            friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
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
        Accelerate(ref MotionX, xInput, acceleration, maxSpeedX,
            friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
        Accelerate(ref MotionY, yInput, acceleration, maxSpeedY,
            friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
    }

    public void ApplyLateralFriction(float friction, float stopIfSpeedIsLessThan) {
        SlowDownSpeed(ref MotionX, friction, stopIfSpeedIsLessThan);
    }

    public void ApplyVerticalFriction(float friction, float stopIfSpeedIsLessThan) {
        SlowDownSpeed(ref MotionY, friction, stopIfSpeedIsLessThan);
    }

    public void Stop(float friction, float stopIfSpeedIsLessThan) {
        if (IsOnWall()) {
            MotionX = 0;
        } else {
            ApplyLateralFriction(friction, stopIfSpeedIsLessThan);
        }
        Move();
    }

    public void Lateral(float xInput,
        float acceleration,
        float maxSpeed,
        float friction,
        float stopIfSpeedIsLessThan,
        float changeDirectionFactor) {
            
        AddLateralSpeed(xInput, acceleration, maxSpeed, friction, stopIfSpeedIsLessThan, changeDirectionFactor);
        Move();
    }

    public bool Move() {
        CharacterBody.Velocity = GetRotatedVelocity();
        var collide = CharacterBody.MoveAndSlide();
        Motion = RollbackRotateVelocity(CharacterBody.Velocity);
        UpdateFlags();
        return collide;
    }

    private void UpdateFlags() {
        var wasOnFloor = _isOnFloor;
        _isOnFloor = CharacterBody.IsOnFloor();
        _isOnSlope = false;
        _floorNormal = CharacterBody.GetFloorNormal();
        _floor = null;

        _isOnWall = CharacterBody.IsOnWall();
        _wallNormal = CharacterBody.GetWallNormal();
        _wall = null;

        _isOnCeiling = CharacterBody.IsOnCeiling(); 

        _isJustLanded = false;
        _isJustTookOff = false;

        if (_isOnFloor) {
            _isJustLanded = !wasOnFloor;
            _isOnSlope = _floorNormal != FloorUpDirection;
        } else {
            _isJustTookOff = wasOnFloor;
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
                   .Do(r => r.ForceRaycastUpdate())
                   .Where(r => r.IsColliding())
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