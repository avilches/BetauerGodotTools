using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;
using Betauer;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Veronenger.Game.Managers;
using Object = Godot.Object;
using Vector2 = Godot.Vector2;

namespace Veronenger.Game.Character {

    [Service(Lifetime.Transient)]
    public class KinematicPlatformMotionBody : BaseMotionBody, IFlipper {
        public float DefaultGravity { get; set; } = 0f;
        public float DefaultMaxFallingSpeed { get; set; } = 1000f;
        public float DefaultMaxFloorGravity { get; set; } = 1000f;
        public Vector2 SnapToFloorVector { get; set; }
        public Vector2 FloorUpDirection { get; set; }

        private List<RayCast2D>? _floorRaycast;
        private RayCast2D? _slopeRaycast;
        private IFlipper _flippers;

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
        private Vector2 _ceilingNormal = Vector2.Zero;
        private Object? _ceiling = null;

        private bool _dirtyFlags = true;

        public void Configure(string name, KinematicBody2D body, IFlipper flippers, List<RayCast2D>? floorRaycast, RayCast2D? slopeRaycast, Position2D position2D, Vector2 snapToFloorVector, Vector2 floorUpDirection) {
            base.Configure(name, body, position2D);
            _flippers = flippers;
            _floorRaycast = floorRaycast;
            _slopeRaycast = slopeRaycast;
            SnapToFloorVector = snapToFloorVector;
            FloorUpDirection = floorUpDirection;
        }

        public void ConfigureGravity(float gravity, float maxFallingSpeed, float maxFloorGravity) {
            DefaultGravity = gravity;
            DefaultMaxFallingSpeed = maxFallingSpeed;
            DefaultMaxFloorGravity = maxFloorGravity;
        }

        public bool IsFacingRight => _flippers.IsFacingRight;
        public bool Flip() => _flippers.Flip();
        public bool Flip(bool left) => _flippers.Flip(left);
        public bool Flip(float xInput) => _flippers.Flip(xInput);
        
        /**
         * node is | I'm facing  | flip?
         * right   | right       | no
         * right   | left        | yes
         * left    | right       | yes
         * left    | left        | no
         *
         */
        public void FaceTo(Node2D node2D) {
            if (IsToTheRightOf(node2D) != _flippers.IsFacingRight) {
                _flippers.Flip();
            }
        }

        public bool IsJustLanded() => UpdateFlags()._isJustLanded;
        public bool IsJustTookOff() => UpdateFlags()._isJustTookOff;

        // Floor
        public bool IsOnFloor() => UpdateFlags()._isOnFloor;
        public bool IsOnSlope() => UpdateFlags()._isOnSlope;
        public bool IsOnSlopeUpRight() => IsOnSlope() && GetFloorNormal().IsUpLeft(FloorUpDirection);
        public Vector2 GetFloorVelocity() => Body.GetFloorVelocity();
        public bool HasFloorLateralMovement() => GetFloorVelocity().x != 0;
        // Floor collider
        public Vector2 GetFloorNormal() => UpdateFlags()._floorNormal;
        public Object? GetFloor() => UpdateFlags()._floor;
        public Node? GetFloorNode() => UpdateFlags()._floor as Node;
        public PhysicsBody2D? GetFloorPhysicsBody2D() => UpdateFlags()._floor as PhysicsBody2D;
        public TileMap? GetFloorTileMap() => UpdateFlags()._floor as TileMap;

        // Wall
        public bool IsOnWall() => UpdateFlags()._isOnWall;
        public bool IsOnWallRight() => IsOnWall() && GetWallNormal().IsLeft(FloorUpDirection);
        // Wall collider
        public Vector2 GetWallNormal() => UpdateFlags()._wallNormal;
        public Object? GetWall() => UpdateFlags()._wall;
        public Node? GetWallNode() => GetWall() as Node;
        public TileMap? GetWallTileMap() => GetWall() as TileMap;
        public PhysicsBody2D? GetWallPhysicsBody2D() => GetWall() as PhysicsBody2D;

        // Ceiling
        public bool IsOnCeiling() => UpdateFlags()._isOnCeiling;
        // Ceiling collider
        public Vector2 GetCeilingNormal() => UpdateFlags()._ceilingNormal;
        public Object? GetCeiling() => UpdateFlags()._ceiling;
        public Node? GetCeilingNode() => GetCeiling() as Node;
        public TileMap? GetCeilingTileMap() => GetCeiling() as TileMap;
        public PhysicsBody2D? GetCeilingPhysicsBody2D() => GetCeiling() as PhysicsBody2D;

        public void ApplyDefaultGravity(float factor = 1.0F) {
            ApplyGravity(DefaultGravity * factor, IsOnFloor() ? DefaultMaxFloorGravity : DefaultMaxFallingSpeed);
        }

        public void ApplyGravity(float gravity, float maxSpeed) {
            // Formula to apply gravity against floor normal, instead of just go down
            // Force += (_colliderNormal != Vector2.Zero ? _colliderNormal : Vector2.Down) * DefaultGravity * Delta;
            ForceY = Mathf.Min(ForceY + gravity * Delta, maxSpeed);
        }

        public void AddLateralSpeed(float xInput,
            float acceleration, float maxSpeed,
            float friction, float stopIfSpeedIsLessThan,
            float changeDirectionFactor) {
            Accelerate(ref ForceX, xInput, acceleration, maxSpeed, friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
        }

        public void StopLateralSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            SlowDownSpeed(ref ForceX, friction, stopIfSpeedIsLessThan);
        }

        public Vector2 MoveSnapping() {
            /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
            mueve una plataforma y nos queremos quedar pegados
            Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
            se para y ya no sigue a la plataforma
            */
            var stopOnSlopes = !HasFloorLateralMovement();
            var pendingInertia = Body.MoveAndSlideWithSnap(Force, SnapToFloorVector, FloorUpDirection, stopOnSlopes);
            _dirtyFlags = true;
            return pendingInertia;
        }

        public Vector2 MoveSlide() {
            const bool stopOnSlopes = true; // true, so if the player lands in a slope, it will stick on it
            var remain = Body.MoveAndSlideWithSnap(Force, Vector2.Zero, FloorUpDirection, stopOnSlopes);
            _dirtyFlags = true;
            return remain;
        }
        
        private KinematicPlatformMotionBody UpdateFlags() {
            if (!_dirtyFlags) return this;
            
            var wasOnFloor = _isOnFloor;
            _isOnFloor = false; // Hack time: when player collides with floor, Body.IsOnFloor() is false, which is a error
            _isOnSlope = false;
            _floorNormal = Vector2.Zero;
            _floor = null;

            _isOnWall = false; // Hack time: when player collides with ceiling, Body.IsOnWall() is true, which is a error
            _wallNormal = Vector2.Zero;
            _wall = null;

            _isOnCeiling = false;  // Hack time: when player collides with ceiling, Body.IsOnCeiling() is false, which is a error 
            _ceilingNormal = Vector2.Zero;
            _ceiling = null;

            _isJustLanded = false;
            _isJustTookOff = false;

            CheckMoveAndSlideCollisions();

            if (_isOnFloor) {
                _isJustLanded = !wasOnFloor;
                if (!_isOnSlope) {
                    // Just in case the move_and_slide collisions didn't detect the slope, use the raycast as backup
                    CheckSlopeRaycast();
                }
            } else {
                _isJustTookOff = wasOnFloor;
            }

            return this;
        }

        private void CheckMoveAndSlideCollisions() {
            var slideCount = Body.GetSlideCount();
            if (slideCount == 0) return;
            for (var i = 0; i < slideCount; i++) {
                var collision = Body.GetSlideCollision(i);
                var collider = collision.Collider;
                var normal = collision.Normal;
                if (collision == null || normal == Vector2.Zero) continue;
                if (normal == FloorUpDirection) {
                    _isOnFloor = true;
                    if (!_isOnSlope) {
                        _floorNormal = normal;
                        _floor = collider;
                    }
                } else if (normal.IsFloor(FloorUpDirection)) {
                    _isOnFloor = true;
                    if (!_isOnSlope) {
                        _isOnSlope = true;
                        _floorNormal = normal;
                        _floor = collider;
                    }
                } else if (normal.IsCeiling(FloorUpDirection)) {
                    _isOnCeiling = true;
                    _ceilingNormal = normal;
                    _ceiling = collider;
                } else {
                    _isOnWall = true;
                    _wallNormal = normal;
                    _wall = collider;
                }
            }
        }

        private void CheckSlopeRaycast() {
            if (_slopeRaycast == null) return;
            _slopeRaycast.ForceRaycastUpdate();
            var collisionCollider = _slopeRaycast.GetCollider();
            if (collisionCollider == null) return;
            var normal = _slopeRaycast.GetCollisionNormal();
            if (normal != FloorUpDirection && normal.IsFloor(FloorUpDirection)) {
                _isOnSlope = true;
                _floorNormal = normal;
            }
        }

        private void CheckFloorRaycast() {
            if (_floorRaycast == null) return;
            foreach (var floorRaycast2D in _floorRaycast) {
                floorRaycast2D.ForceRaycastUpdate();
                var collisionCollider = floorRaycast2D.GetCollider();
                if (collisionCollider == null) return;
                _isOnFloor = true;
                _floorNormal = floorRaycast2D.GetCollisionNormal();
            }
        }

        public string GetFloorCollisionInfo() {
            return IsOnFloor()
                ? $"{GetFloorNormal().ToString("0.0")} {Mathf.Rad2Deg(GetFloorNormal().Angle()):0.0}º [{GetFloor()?.GetType().Name}] {GetFloorNode()?.Name}"
                : "";
        }

        public string GetCeilingCollisionInfo() {
            return IsOnCeiling()
                ? $"{GetCeilingNormal().ToString("0.0")} {Mathf.Rad2Deg(GetCeilingNormal().Angle()):0.0}º [{GetCeiling()?.GetType().Name}] {GetCeilingNode()?.Name}"
                : "";
        }

        public string GetWallCollisionInfo() {
            return IsOnWall()
                ? $"{(IsOnWallRight()?"R":"L")} {GetWallNormal().ToString("0.0")} {Mathf.Rad2Deg(GetWallNormal().Angle()):0.0}º [{GetWall()?.GetType().Name}] {GetWallNode()?.Name}"
                : "";
        }
    }
}