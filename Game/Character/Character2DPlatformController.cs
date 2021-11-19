using System;
using System.Text;
using Godot;
using Tools;
using Veronenger.Game.Managers;
using Veronenger.Game.Managers.Autoload;
using TraceLevel = Tools.TraceLevel;

namespace Veronenger.Game.Character {
    public class SpriteFlipper {
        private Sprite Sprite { get; }
        private bool _isFacingRight = false;

        public SpriteFlipper(Sprite sprite) {
            Sprite = sprite;
            _isFacingRight = !Sprite.FlipH; // FlipH is true when looks left
        }

        public void Flip() {
            IsFacingRight = !IsFacingRight;
        }

        public void Flip(float xInput) {
            if (xInput == 0) return;
            bool shouldFaceRight = xInput > 0;
            IsFacingRight = shouldFaceRight;
        }

        public bool IsFacingRight {
            get => _isFacingRight;
            set {
                if (value != _isFacingRight) {
                    _isFacingRight = value;
                    // TODO: use scale instead of flip the sprite, so collider areas will flip together
                    Sprite.FlipH = !_isFacingRight;
                }
            }
        }
    }

    public abstract class Character2DPlatformController : KinematicBody2D {
        private Logger _loggerMotion;
        private Logger _loggerCollision;
        private Vector2 _lastMotion = Vector2.Zero;

        protected Sprite MainSprite { get; private set; }
        protected Label Label { get; private set; }
        protected Node2D Parent { get; private set; }
        protected SpriteFlipper _spriteFlipper;

        public Vector2 Motion = Vector2.Zero;
        public PlatformManager PlatformManager => GameManager.Instance.PlatformManager;
        public SlopeStairsManager SlopeStairsManager => GameManager.Instance.SlopeStairsManager;
        public CharacterManager CharacterManager => GameManager.Instance.CharacterManager;
        public float Delta { get; private set; } = 0;

        protected abstract Platform2DCharacterConfig Platform2DCharacterConfig { get; }
        protected abstract string GetName();
        protected abstract void PhysicsProcess();
        protected virtual void EnterTree() {
        }

        public sealed override void _EnterTree() {
            _loggerCollision = LoggerFactory.GetLogger(GetName(),"Collision");
            _loggerMotion = LoggerFactory.GetLogger(GetName(),"Motion");

            MainSprite = GetNode<Sprite>("Sprite");
            Parent = GetParent<Node2D>();
            Label = Parent.GetNode<Label>("Label");
            _spriteFlipper = new SpriteFlipper(MainSprite);
            EnterTree();
        }

        public bool IsFacingRight => _spriteFlipper.IsFacingRight;
        public void Flip() => _spriteFlipper.Flip();
        public void Flip(float xInput) => _spriteFlipper.Flip(xInput);
        public void SetMotionX(float x) => Motion.x = x;
        public void SetMotionY(float y) => Motion.y = y;
        public void ApplyGravity(float factor = 1.0F) => Motion.y += Platform2DCharacterConfig.Gravity * factor * Delta;

        public sealed override void _PhysicsProcess(float delta) {
            Delta = delta;
            _lastMotion = Motion;
            PhysicsProcess();
            if (Motion != _lastMotion) {
                _loggerMotion.Debug($"Motion:{Motion} (diff {(_lastMotion - Motion)})");
            }
        }

        public void AddLateralMotion(float xInput, float acceleration, float friction, float stopIfSpeedIsLessThan,
            float changeDirectionFactor) {
            if (xInput != 0) {
                bool directionChanged = Motion.x != 0 && Math.Sign(Motion.x) != Math.Sign(xInput);
                if (directionChanged) {
                    SetMotionX((Motion.x * changeDirectionFactor) + xInput * acceleration * Delta);
                } else {
                    SetMotionX(Motion.x + xInput * acceleration * Delta);
                }
            } else {
                StopLateralMotionWithFriction(friction, stopIfSpeedIsLessThan);
            }
        }

        public void StopLateralMotionWithFriction(float friction, float stopIfSpeedIsLessThan) {
            if (Mathf.Abs(Motion.x) < stopIfSpeedIsLessThan) {
                SetMotionX(0);
            } else {
                SetMotionX(Motion.x * friction);
                // SetMotionX(Motion.x - (deceleration * Delta * Math.Sign(Motion.x)));
            }
        }

        public void LimitMotion(float maxSpeedFactor = 1.0F) {
            var realMaxSpeed = Platform2DCharacterConfig.MaxSpeed * maxSpeedFactor;
            Motion.x = Mathf.Clamp(Motion.x, -realMaxSpeed, realMaxSpeed);
            Motion.y = Mathf.Min(Motion.y,
                Platform2DCharacterConfig.MaxFallingSpeed); //  avoid gravity continue forever in free fall
        }

        public void MoveSnapping() => MoveSnapping(Vector2.One);

        public void MoveSnapping(Vector2 slowdownVector) {
            /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
            mueve una plataforma y nos queremos quedar pegados
            Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
            se para y ya no sigue a la plataforma
            */
            var stopOnSlopes = !HasFloorLateralMovement();
            var remain = MoveAndSlideWithSnap(Motion * slowdownVector, Platform2DCharacterConfig.SlopeRayCastVector,
                Platform2DCharacterConfig.FloorVector, stopOnSlopes);
            Motion.y = remain.y; // this line stops the gravity accumulation
            // motion.x = remain.x:  // WARNING!! this line should be always commented, player can't climb slopes with it!!
            _dirtyGroundCollisions = true;
        }

        public void Slide() => Slide(Vector2.One);

        public void Slide(Vector2 slowdownVector) {
            // stopOnSlopes debe ser true para al caer sobre una pendiente la tome comoelo
            var stopOnSlopes = true;
            var remain = MoveAndSlideWithSnap(Motion * slowdownVector, Vector2.Zero,
                Platform2DCharacterConfig.FloorVector,
                stopOnSlopes);
            Motion.y = remain.y; // this line stops the gravity accumulation
            /*
        inertia false = se mantiene el remain.x = al chocar con la cabeza pierde toda la inercia lateral que tenia y se va para abajo. Y si choca al subir y se
        se sube, pierde tambien la inercia teniendo que alecerar desde 0

        intertia true = se pierde el remain.x = al saltar y chocar (temporalmente) con un objeto hace que al dejar de chocar, recupere
        totalmente la movilidad = si choca justo antes de subir y luego se sube, corre a tope. Si choca con la cabeza y baja un poco,
        cuando de chocar, continua hacia delante a tope.
        */
            var inertia = false;

            if (!inertia) {
                Motion.x = remain.x;
            }

            _dirtyGroundCollisions = true;
        }

        private bool _dirtyGroundCollisions = true;

        private bool _isOnSlope = false;
        private bool _isOnSlopeUpRight = false;
        private bool _isOnMovingPlatform = false;
        private bool _isOnFallingPlatform = false;
        private bool _isOnSlopeStairs = false;
        private Vector2 _colliderNormal = Vector2.Zero;

        public bool IsOnSlope() => (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnSlope;
        public bool IsOnSlopeUpRight() => IsOnSlope() && _isOnSlopeUpRight;
        public bool IsOnSlopeDownLeft() => IsOnSlope() && _isOnSlopeUpRight;

        public bool IsOnSlopeDownRight() => IsOnSlope() && !_isOnSlopeUpRight;
        public bool IsOnSlopeUpLeft() => IsOnSlope() && !_isOnSlopeUpRight;

        public bool IsOnMovingPlatform() =>
            (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnMovingPlatform;

        public bool IsOnFallingPlatform() =>
            (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnFallingPlatform;

        public bool IsOnSlopeStairs() => (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnSlopeStairs;
        public Vector2 GetColliderNormal() => (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._colliderNormal;

        private RayCast2D _floorDetector;

        public RayCast2D FloorDetector {
            get {
                if (_floorDetector == null) {
                    _floorDetector = GetNode("RayCasts").GetNode<RayCast2D>("SlopeDetector");
                }

                return _floorDetector;
            }
        }

        private void ResetCollisionFlags() {
            _dirtyGroundCollisions = false;

            _isOnSlope = false;
            _isOnSlopeUpRight = false;
            _isOnMovingPlatform = false;
            _isOnFallingPlatform = false;
            _isOnSlopeStairs = false;
            _colliderNormal = Vector2.Zero;
        }

        private Character2DPlatformController UpdateFloorCollisions() {
            ResetCollisionFlags();
            if (!IsOnFloor()) {
                _loggerCollision.Debug(
                    $"UpdateFloorCollisions end: floor/falling: {IsOnFloor()}/{_isOnFallingPlatform} (0 checks: air?)");
                return this;
            }

            var __isOnSlope = false;
            var __isOnSlopeUpRight = false;
            var __isOnMovingPlatform = false;
            var __isOnFallingPlatform = false;
            var __isOnSlopeStairs = false;
            var __colliderNormal = Vector2.Zero;

            var slideCount = GetSlideCount();
            CheckMoveAndSlideCollisions(slideCount);
            CheckCollisionsWithFloorDetector(ref __colliderNormal, ref __isOnSlope, ref __isOnFallingPlatform,
                ref __isOnMovingPlatform, ref __isOnSlopeStairs, ref __isOnSlopeUpRight);

            if (_loggerCollision.IsEnabled(TraceLevel.Debug)) {
                if (_isOnSlope != __isOnSlope ||
                    _isOnSlopeStairs != __isOnSlopeStairs ||
                    _isOnFallingPlatform != __isOnFallingPlatform ||
                    _isOnMovingPlatform != __isOnMovingPlatform ||
                    _colliderNormal != __colliderNormal) {
                    StringBuilder diff = new StringBuilder();
                    if (IsOnFloor() != FloorDetector.IsColliding()) {
                        diff.Append(" Floor:" + IsOnFloor() + "/" + FloorDetector.IsColliding());
                    }
                    if (_isOnSlope != __isOnSlope) {
                        diff.Append(" Slope:" + _isOnSlope + "/" + __isOnSlope);
                    }
                    if (_isOnSlopeStairs != __isOnSlopeStairs) {
                        diff.Append(" Stairs:" + _isOnSlopeStairs + "/" + __isOnSlopeStairs);
                    }
                    if (_isOnFallingPlatform != __isOnFallingPlatform) {
                        diff.Append(" Falling:" + _isOnFallingPlatform + "/" + __isOnFallingPlatform);
                    }
                    if (_isOnMovingPlatform != __isOnMovingPlatform) {
                        diff.Append(" Moving:" + _isOnMovingPlatform + "/" + __isOnMovingPlatform);
                    }
                    // if (_colliderNormal != __colliderNormal) {
                    // diff.Append(" Normal:" + _colliderNormal + "/" + __colliderNormal);
                    // }
                    if (diff.Length > 0) {
                        _loggerCollision.Debug("Diff in collisions:" + diff.ToString());
                    }
                }
            }

            _isOnSlope |= __isOnSlope;
            _isOnSlopeUpRight |= __isOnSlopeUpRight;
            _isOnMovingPlatform |= __isOnMovingPlatform;
            _isOnFallingPlatform |= __isOnFallingPlatform;
            _isOnSlopeStairs |= __isOnSlopeStairs;
            _colliderNormal = _colliderNormal != Vector2.Zero ? _colliderNormal : __colliderNormal;

            _loggerCollision.Debug(
                $"UpdateFloorCollisions({slideCount}). Floor:{IsOnFloor()} Slope:{_isOnSlope} Stairs:{_isOnSlopeStairs} Falling:{_isOnFallingPlatform} Moving:{_isOnMovingPlatform} Normal:{_colliderNormal}");
            // FloorDetector.IsColliding()

            Update(); // this allow to call to _draw() with the colliderNormal updated
            return this;
        }

        private bool CheckCollisionsWithFloorDetector(ref Vector2 __colliderNormal, ref bool __isOnSlope,
            ref bool __isOnFallingPlatform, ref bool __isOnMovingPlatform, ref bool __isOnSlopeStairs,
            ref bool __isOnSlopeUpRight) {
            FloorDetector.ForceRaycastUpdate();
            var collisionCollider = FloorDetector.GetCollider();
            if (collisionCollider == null) return false;

            __colliderNormal = FloorDetector.GetCollisionNormal();

            if (__colliderNormal != Vector2.Zero && Mathf.Abs(__colliderNormal.y) < 1) {
                __isOnSlope = true;
                __isOnSlopeUpRight = __colliderNormal.x < 0;
            }

            if (collisionCollider is PhysicsBody2D falling && PlatformManager.IsFallingPlatform(falling)) {
                __isOnFallingPlatform = true;
            }

            if (collisionCollider is KinematicBody2D moving && PlatformManager.IsMovingPlatform(moving)) {
                __isOnMovingPlatform = true;
            }

            if (collisionCollider is PhysicsBody2D slopeStairs && SlopeStairsManager.IsSlopeStairs(slopeStairs)) {
                __isOnSlopeStairs = true;
            }
            Update(); // this allow to call to _draw() with the colliderNormal updated
            return true;
        }

        private void CheckMoveAndSlideCollisions(int slideCount) {
            if (slideCount == 0) return;
            Vector2 lastColliderNormal = Vector2.Zero;
            for (var i = 0; i < slideCount; i++) {
                var collision = GetSlideCollision(i);
                var collisionCollider = collision.Collider;
                var currentColliderNormal = collision.Normal;
                if (currentColliderNormal != Vector2.Zero) {
                    lastColliderNormal = currentColliderNormal;
                    if (!_isOnSlope) {
                        _colliderNormal = lastColliderNormal;
                        if (Mathf.Abs(_colliderNormal.y) < 1) {
                            _isOnSlope = true;
                            _isOnSlopeUpRight = _colliderNormal.x < 0;
                        }
                    }
                }

                if (collisionCollider is PhysicsBody2D falling && PlatformManager.IsFallingPlatform(falling)) {
                    _isOnFallingPlatform = true;
                }

                if (collisionCollider is KinematicBody2D moving && PlatformManager.IsMovingPlatform(moving)) {
                    _isOnMovingPlatform = true;
                }

                if (collisionCollider is PhysicsBody2D slopeStairs && SlopeStairsManager.IsSlopeStairs(slopeStairs)) {
                    _isOnSlopeStairs = true;
                }
            }

            if (_colliderNormal == null) {
                _colliderNormal = lastColliderNormal;
            }
        }

        public bool HasFloorLateralMovement() => GetFloorVelocity().x != 0;

        // public override void _Draw() {
        // DrawLine(slopeDetector.Position, slopeDetector.Position + slopeDetector.CastTo, Colors.Red, 3F);
        // }
    }
}