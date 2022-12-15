using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Godot;
using Betauer;
using Betauer.Core;
using Betauer.Core.Nodes;
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

        protected List<RayCast2D>? FloorRaycasts;

        public void Configure(string name, CharacterBody2D body, IFlipper flippers, Marker2D marker2D, Vector2 floorUpDirection, List<RayCast2D>? floorRaycasts = null) {
            base.Configure(name, body, marker2D, floorUpDirection);
            _flippers = flippers;
            FloorRaycasts = floorRaycasts;
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

        // Floor flags
        public bool IsOnFloor() => _isOnFloor;
        public bool IsOnSlope() => _isOnSlope;
        public bool IsOnSlopeUpRight() => IsOnSlope() && GetFloorNormal().IsUpLeft(FloorUpDirection);
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
        
        public IEnumerable<T> GetFloorColliders<T>(Func<T, bool>? predicate = null) where T : Node => Body.GetFloorColliders(FloorUpDirection, predicate); 
        public IEnumerable<T> GetWallColliders<T>(Func<T, bool>? predicate = null) where T : Node => Body.GetWallColliders(FloorUpDirection, predicate); 
        public IEnumerable<T> GetCeilingColliders<T>(Func<T, bool>? predicate = null) where T : Node => Body.GetCeilingColliders(FloorUpDirection, predicate);
        
        public IEnumerable<KinematicCollision2D> GetFloorCollisions(Func<KinematicCollision2D, bool>? predicate = null) => Body.GetFloorCollisions(FloorUpDirection, predicate); 
        public IEnumerable<KinematicCollision2D> GetWallCollisions(Func<KinematicCollision2D, bool>? predicate = null) => Body.GetWallCollisions(FloorUpDirection, predicate); 
        public IEnumerable<KinematicCollision2D> GetCeilingCollisions(Func<KinematicCollision2D, bool>? predicate = null) => Body.GetCeilingCollisions(FloorUpDirection, predicate); 

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
}