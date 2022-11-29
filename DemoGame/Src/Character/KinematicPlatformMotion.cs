using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;
using Betauer;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Veronenger.Managers;
using Object = Godot.Object;
using Vector2 = Godot.Vector2;

namespace Veronenger.Character {

    [Service(Lifetime.Transient)]
    public class KinematicPlatformMotion : BaseKinematicMotion, IFlipper {
        public float DefaultGravity { get; set; } = 0f;
        public float DefaultMaxFallingSpeed { get; set; } = 1000f;
        public float DefaultMaxFloorGravity { get; set; } = 1000f;
        public Vector2 SnapToFloorVector { get; set; }

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

        public void Configure(string name, CharacterBody2D body, IFlipper flippers, List<RayCast2D>? floorRaycast, RayCast2D? slopeRaycast, Marker2D marker2D, Vector2 snapToFloorVector, Vector2 floorUpDirection) {
            base.Configure(name, body, marker2D, floorUpDirection);
            _flippers = flippers;
            _floorRaycast = floorRaycast;
            _slopeRaycast = slopeRaycast;
            SnapToFloorVector = snapToFloorVector;
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
        // TODO Godot 4
        // public Vector2 GetFloorVelocity() => Body.GetFloorVelocity();
        // public bool HasFloorLateralMovement() => GetFloorVelocity().x != 0;
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

        public Vector2 MoveSnapping() {
            /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
            mueve una plataforma y nos queremos quedar pegados
            Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
            se para y ya no sigue a la plataforma
            */
            // TODO: Godot 4
            var stopOnSlopes = true; //!HasFloorLateralMovement();
            Body.MotionMode = CharacterBody2D.MotionModeEnum.Grounded;
            Body.Velocity = RotateSpeed();
            Body.FloorSnapLength = SnapToFloorVector.Length();
            Body.UpDirection = FloorUpDirection;
            Body.FloorStopOnSlope = stopOnSlopes;
            // Body.MaxSlides = ...
            // Body.FloorMaxAngle = ...
            Body.MoveAndSlide();
            _dirtyFlags = true;
            return RotateInertia(Body.Velocity);
        }

        public Vector2 Slide() {
            const bool stopOnSlopes = true; // true, so if the player lands in a slope, it will stick on it

            // TODO: Godot 4
            Body.MotionMode = CharacterBody2D.MotionModeEnum.Floating;
            Body.Velocity = RotateSpeed();
            Body.FloorSnapLength = 0;
            Body.UpDirection = FloorUpDirection;
            Body.FloorStopOnSlope = stopOnSlopes;

            Body.MoveAndSlide();
            _dirtyFlags = true;
            return RotateInertia(Body.Velocity);
        }

        public Vector2 Float() {
            const bool stopOnSlopes = true; // true, so if the player lands in a slope, it will stick on it

            // TODO: Godot 4
            Body.MotionMode = CharacterBody2D.MotionModeEnum.Floating;
            Body.Velocity = RotateSpeed();
            Body.FloorSnapLength = 0;
            Body.UpDirection = FloorUpDirection;
            Body.FloorStopOnSlope = stopOnSlopes;

            Body.MoveAndSlide();
            _dirtyFlags = true;
            return RotateInertia(Body.Velocity);
        }

        public void Stop(float friction, float stopIfSpeedIsLessThan) {
            if (Body.IsOnWall()) {
                MotionX = 0;
            } else {
                ApplyLateralFriction(friction, stopIfSpeedIsLessThan);
            }
            ApplyDefaultGravity();
            MoveSnapping();
        }

        public void Run(float xInput,
            float acceleration,
            float maxSpeed,
            float friction,
            float stopIfSpeedIsLessThan,
            float changeDirectionFactor) {
            
            AddLateralSpeed(xInput, acceleration, maxSpeed, friction, stopIfSpeedIsLessThan, changeDirectionFactor);
            if (IsOnSlope()) LimitMotionNormalized(maxSpeed);
            ApplyDefaultGravity();
            var pendingInertia = MoveSnapping();
            if (IsOnSlope()) {
                // Ensure the body can climb up or down slopes. Without this, the player will go down too fast
                // and go up too slow
                // And never use the pendingInertia.x when climbing a slope!!!
                MotionY = pendingInertia.y;
            }
        }

        public void FallLateral(float xInput,
            float acceleration,
            float maxSpeed,
            float airResistance,
            float stopIfSpeedIsLessThan,
            float changeDirectionFactor) {
            AddLateralSpeed(xInput, acceleration, maxSpeed, airResistance, stopIfSpeedIsLessThan, changeDirectionFactor);
            Fall();
        }

        public void Fall() {
            ApplyDefaultGravity();
            // Keep the speed from the move so if the player collides, the player could slide or stop
            Motion = Slide();
        }


        private KinematicPlatformMotion UpdateFlags() {
            if (!_dirtyFlags) return this;
            _dirtyFlags = false;
            
            var wasOnFloor = _isOnFloor;
            _isOnFloor = Body.IsOnFloor(); // Hack time: when player collides with floor, Body.IsOnFloor() is false, which is a error
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
            var slideCount = Body.GetSlideCollisionCount();
            if (slideCount == 0) return;
            for (var i = 0; i < slideCount; i++) {
                var collision = Body.GetSlideCollision(i);
                var collider = collision.GetCollider();
                var normal = collision.GetNormal();
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
            if (normal == FloorUpDirection) {
                _isOnFloor = true;
                _floorNormal = normal;
            } else if (normal.IsFloor(FloorUpDirection)) {
                _isOnFloor = true;
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
                ? $"{(IsOnSlope()?IsOnSlopeUpRight()?"/":"\\":"flat")} {GetFloorNormal().ToString("0.0")} {Mathf.RadToDeg(GetFloorNormal().Angle()):0.0}º [{GetFloor()?.GetType().Name}] {GetFloorNode()?.Name}"
                : "";
        }

        public string GetCeilingCollisionInfo() {
            return IsOnCeiling()
                ? $"{GetCeilingNormal().ToString("0.0")} {Mathf.RadToDeg(GetCeilingNormal().Angle()):0.0}º [{GetCeiling()?.GetType().Name}] {GetCeilingNode()?.Name}"
                : "";
        }

        public string GetWallCollisionInfo() {
            return IsOnWall()
                ? $"{(IsOnWallRight()?"R":"L")} {GetWallNormal().ToString("0.0")} {Mathf.RadToDeg(GetWallNormal().Angle()):0.0}º [{GetWall()?.GetType().Name}] {GetWallNode()?.Name}"
                : "";
        }
    }
}