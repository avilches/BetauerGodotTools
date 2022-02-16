using System;
using System.Text;
using Godot;
using Betauer;
using Betauer.DI;
using Veronenger.Game.Managers;
using TraceLevel = Betauer.TraceLevel;

namespace Veronenger.Game.Character {
    [Transient]
    public class MotionBody : IFlipper {
        private KinematicBody2D _body;
        private string _name;
        private IFlipper _flippers;
        private MotionConfig _motionConfig;
        private Logger _loggerCollision;
        private Logger _loggerMotion;
        private RayCast2D _floorDetector;

        private Vector2 _lastMotion = Vector2.Zero;

        public float Delta { get; private set; } = 0;

        public Vector2 Motion = Vector2.Zero;

        [Inject] private PlatformManager _platformManager;
        [Inject] private SlopeStairsManager _slopeStairsManager;

        public void Configure(KinematicBody2D body, IFlipper flippers, string name, MotionConfig motionConfig) {
            _body = body;
            _name = name;
            _flippers = flippers;
            _motionConfig = motionConfig;
            _loggerCollision = LoggerFactory.GetLogger(_name, "Collision");
            _loggerMotion = LoggerFactory.GetLogger(_name, "Motion");
            _floorDetector = _body.GetNode<RayCast2D>("RayCasts/SlopeDetector");
        }

        public bool IsFacingRight => _flippers.IsFacingRight;
        public bool Flip() => _flippers.Flip();
        public bool Flip(bool left) => _flippers.Flip(left);
        public bool Flip(float xInput) => _flippers.Flip(xInput);

        public void SetMotionX(float x) => Motion.x = x;
        public void AddMotionX(float x) => Motion += new Vector2(x, 0);

        public void SetMotionY(float y) => Motion.y = y;
        public void AddMotionY(float y) => Motion += new Vector2(0, y);

        public void ApplyGravity(float factor = 1.0F) => Motion.y += _motionConfig.Gravity * factor * Delta;

        public void StartFrame(float delta) {
            Delta = delta;
            _lastMotion = Motion;
        }

        public void EndFrame() {
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
            LimitLateralMotion(maxSpeedFactor);
            LimitVerticalMotion();
        }

        public void LimitVerticalMotion(float maxSpeedFactor = 1.0F) {
            Motion.y = Mathf.Min(Motion.y,
                _motionConfig.MaxFallingSpeed); //  avoid gravity continue forever in free fall
        }

        public void LimitLateralMotion(float maxSpeedFactor = 1.0F) {
            var realMaxSpeed = _motionConfig.MaxSpeed * maxSpeedFactor;
            Motion.x = Mathf.Clamp(Motion.x, -realMaxSpeed, realMaxSpeed);
        }

        public void MoveSnapping() => MoveSnapping(Vector2.One);

        public void MoveSnapping(Vector2 slowdownVector) {
            /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
            mueve una plataforma y nos queremos quedar pegados
            Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
            se para y ya no sigue a la plataforma
            */
            var stopOnSlopes = !HasFloorLateralMovement();
            var remain = _body.MoveAndSlideWithSnap(Motion * slowdownVector, _motionConfig.SlopeRayCastVector,
                _motionConfig.FloorVector, stopOnSlopes);
            Motion.y = remain.y; // this line stops the gravity accumulation
            // motion.x = remain.x:  // WARNING!! this line should be always commented, player can't climb slopes with it!!
            _dirtyGroundCollisions = true;
        }

        public void Slide() => Slide(Vector2.One);

        public void Slide(Vector2 slowdownVector) {
            // stopOnSlopes debe ser true para al caer sobre una pendiente la tome comoelo
            var stopOnSlopes = true;
            var remain = _body.MoveAndSlideWithSnap(Motion * slowdownVector, Vector2.Zero,
                _motionConfig.FloorVector,
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

        private void ResetCollisionFlags() {
            _dirtyGroundCollisions = false;

            _isOnSlope = false;
            _isOnSlopeUpRight = false;
            _isOnMovingPlatform = false;
            _isOnFallingPlatform = false;
            _isOnSlopeStairs = false;
            _colliderNormal = Vector2.Zero;
        }

        private MotionBody UpdateFloorCollisions() {
            ResetCollisionFlags();
            if (!_body.IsOnFloor()) {
                _loggerCollision.Debug(
                    $"UpdateFloorCollisions end: floor/falling: {_body.IsOnFloor()}/{_isOnFallingPlatform} (0 checks: air?)");
                return this;
            }

            var __isOnSlope = false;
            var __isOnSlopeUpRight = false;
            var __isOnMovingPlatform = false;
            var __isOnFallingPlatform = false;
            var __isOnSlopeStairs = false;
            var __colliderNormal = Vector2.Zero;

            var slideCount = _body.GetSlideCount();
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
                    if (_body.IsOnFloor() != _floorDetector.IsColliding()) {
                        diff.Append(" Floor:" + _body.IsOnFloor() + "/" + _floorDetector.IsColliding());
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
                $"UpdateFloorCollisions({slideCount}). Floor:{_body.IsOnFloor()} Slope:{_isOnSlope} Stairs:{_isOnSlopeStairs} Falling:{_isOnFallingPlatform} Moving:{_isOnMovingPlatform} Normal:{_colliderNormal}");
            // FloorDetector.IsColliding()

            _body.Update(); // this allow to call to _draw() with the colliderNormal updated
            return this;
        }

        private bool CheckCollisionsWithFloorDetector(ref Vector2 __colliderNormal, ref bool __isOnSlope,
            ref bool __isOnFallingPlatform, ref bool __isOnMovingPlatform, ref bool __isOnSlopeStairs,
            ref bool __isOnSlopeUpRight) {
            _floorDetector.ForceRaycastUpdate();
            var collisionCollider = _floorDetector.GetCollider();
            if (collisionCollider == null) return false;

            __colliderNormal = _floorDetector.GetCollisionNormal();

            if (__colliderNormal != Vector2.Zero && Mathf.Abs(__colliderNormal.y) < 1) {
                __isOnSlope = true;
                __isOnSlopeUpRight = __colliderNormal.x < 0;
            }

            if (collisionCollider is PhysicsBody2D falling && _platformManager.IsFallingPlatform(falling)) {
                __isOnFallingPlatform = true;
            }

            if (collisionCollider is KinematicBody2D moving && _platformManager.IsMovingPlatform(moving)) {
                __isOnMovingPlatform = true;
            }

            if (collisionCollider is PhysicsBody2D slopeStairs && _slopeStairsManager.IsSlopeStairs(slopeStairs)) {
                __isOnSlopeStairs = true;
            }
            _body.Update(); // this allow to call to _draw() with the colliderNormal updated
            return true;
        }

        private void CheckMoveAndSlideCollisions(int slideCount) {
            if (slideCount == 0) return;
            Vector2 lastColliderNormal = Vector2.Zero;
            for (var i = 0; i < slideCount; i++) {
                var collision = _body.GetSlideCollision(i);
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

                if (collisionCollider is PhysicsBody2D falling && _platformManager.IsFallingPlatform(falling)) {
                    _isOnFallingPlatform = true;
                }

                if (collisionCollider is KinematicBody2D moving && _platformManager.IsMovingPlatform(moving)) {
                    _isOnMovingPlatform = true;
                }

                if (collisionCollider is PhysicsBody2D slopeStairs && _slopeStairsManager.IsSlopeStairs(slopeStairs)) {
                    _isOnSlopeStairs = true;
                }
                // TODO: is this really needed? check if the Reference is disposed/GC
                collision.Unreference();
            }

            if (_colliderNormal == null) {
                _colliderNormal = lastColliderNormal;
            }
        }

        public bool HasFloorLateralMovement() => _body.GetFloorVelocity().x != 0;

        public void Fall() {
            ApplyGravity();
            LimitMotion();
            Slide();
        }
    }
}