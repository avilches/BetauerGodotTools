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

        public void Configure(string name, CharacterBody2D body, IFlipper flippers, Marker2D marker2D, Vector2 floorUpDirection) {
            base.Configure(name, body, marker2D, floorUpDirection);
            _flippers = flippers;
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

        public bool IsJustLanded() => _isJustLanded;
        public bool IsJustTookOff() => _isJustTookOff;

        // Floor
        public bool IsOnFloor() => _isOnFloor;
        public bool IsOnSlope() => _isOnSlope;
        public bool IsOnSlopeUpRight() => IsOnSlope() && GetFloorNormal().IsUpLeft(FloorUpDirection);

        // public Vector2 GetFloorVelocity() => Body.GetFloorVelocity();
        // public bool HasFloorLateralMovement() => GetFloorVelocity().x != 0;
        // Floor collider
        public Vector2 GetFloorNormal() => _floorNormal;

        // Wall
        public bool IsOnWall() => _isOnWall;
        public bool IsOnWallRight() => IsOnWall() && GetWallNormal().IsLeft(FloorUpDirection);
        public Vector2 GetWallNormal() => _wallNormal;

        // Ceiling
        public bool IsOnCeiling() => _isOnCeiling;
        public Vector2 GetCeilingNormal() => FirstCeilingCollision()?.GetNormal() ?? Vector2.Zero;

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
            Body.Velocity = GetRotatedVelocity();
            var collide = Body.MoveAndSlide();
            Motion = RollbackRotateVelocity(Body.Velocity);
            UpdateFlags();
            return collide;
        }

        private void UpdateFlags() {
            var wasOnFloor = _isOnFloor;
            _isOnFloor = Body.IsOnFloor();
            _isOnSlope = false;
            _floorNormal = Body.GetFloorNormal();
            _floor = null;

            _isOnWall = Body.IsOnWall();
            _wallNormal = Body.GetWallNormal();
            _wall = null;

            _isOnCeiling = Body.IsOnCeiling(); 

            _isJustLanded = false;
            _isJustTookOff = false;

            if (_isOnFloor) {
                _isJustLanded = !wasOnFloor;
                _isOnSlope = _floorNormal != FloorUpDirection;
            } else {
                _isJustTookOff = wasOnFloor;
            }
        }

        private T? FirstFloorCollider<T>() where T : Object {
            return IsOnFloor() ? FindCollisions(col => col.GetCollider() is T && col.GetNormal().IsFloor(FloorUpDirection))?.GetCollider() as T : null;
        }

        private T? FirstCeilingCollider<T>() where T : Object {
            return IsOnCeiling() ? FindCollisions(col => col.GetCollider() is T && col.GetNormal().IsCeiling(FloorUpDirection))?.GetCollider() as T : null;
        }

        private T? FirstWallCollider<T>() where T : Object {
            return IsOnWall() ? FindCollisions(col => col.GetCollider() is T && col.GetNormal().IsWall(FloorUpDirection))?.GetCollider() as T : null;
        }
        
        private KinematicCollision2D? FirstFloorCollision() {
            return IsOnFloor() ? FindCollisions(col => col.GetNormal().IsFloor(FloorUpDirection)) : null;
        }

        private KinematicCollision2D? FirstCeilingCollision() {
            return IsOnCeiling() ? FindCollisions(col => col.GetNormal().IsCeiling(FloorUpDirection)) : null;
        }

        private KinematicCollision2D? FirstWallCollision() {
            return IsOnWall() ? FindCollisions(col => col.GetNormal().IsWall(FloorUpDirection)) : null;
        }

        private KinematicCollision2D? FindCollisions(Func<KinematicCollision2D, bool> predicate) {
            var slideCount = Body.GetSlideCollisionCount();
            if (slideCount == 0) return null;
            for (var i = 0; i < slideCount; i++) {
                var collision = Body.GetSlideCollision(i);
                if (predicate(collision)) return collision;
            }
            return null;
        }

        public string GetFloorCollisionInfo() {
            return IsOnFloor() 
                ? $"{(IsOnSlope()?IsOnSlopeUpRight()?"/":"\\":"flat")} {GetFloorNormal().ToString("0.0")} {Mathf.RadToDeg(GetFloorNormal().Angle()):0.0}º [{FirstFloorCollider<Node>()?.GetType().Name}] {FirstFloorCollider<Node>()?.Name}"
                : "";
        }

        public string GetCeilingCollisionInfo() {
            return IsOnCeiling()
                ? $"{FirstCeilingCollision()?.GetNormal().ToString("0.0")} {Mathf.RadToDeg(FirstCeilingCollision()?.GetNormal().Angle() ?? 0f):0.0}º [{FirstCeilingCollider<Node>()?.GetType().Name}] {FirstCeilingCollider<Node>()?.Name}"
                : "";
        }

        public string GetWallCollisionInfo() {
            return IsOnWall()
                ? $"{(IsOnWallRight()?"R":"L")} {GetWallNormal().ToString("0.0")} {Mathf.RadToDeg(GetWallNormal().Angle()):0.0}º [{FirstWallCollider<Node>()?.GetType().Name}] {FirstWallCollider<Node>()?.Name}"
                : "";
        }
    }
}