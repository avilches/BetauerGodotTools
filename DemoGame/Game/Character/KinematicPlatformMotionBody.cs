using System;
using System.Text;
using Godot;
using Betauer;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.StateMachine;
using Veronenger.Game.Managers;
using TraceLevel = Betauer.TraceLevel;

namespace Veronenger.Game.Character {
    public interface IKinematicPlatformMotionBodyConfig {
        public float DefaultGravity { set; }
        public float DefaultMaxFallingSpeed { set; }
        public Vector2 SlopeRayCastVector { set; }
        public Vector2 FloorVector { set; }
    }
    
    [Service(Lifetime.Transient)]
    public class KinematicPlatformMotionBody : BaseMotionBody, IFlipper, IKinematicPlatformMotionBodyConfig {
        public float DefaultGravity { get; set; } = 0f;
        public float DefaultMaxFallingSpeed { get; set; } = 1000f;
        public Vector2 SlopeRayCastVector { get; set; }
        public Vector2 FloorVector { get; set; }

        private Logger _loggerCollision;
        private RayCast2D _floorDetector;
        private IFlipper _flippers;

        [Inject] private PlatformManager PlatformManager { get; set;}
        [Inject] private SlopeStairsManager SlopeStairsManager { get; set;}

        public void Configure(string name, KinematicBody2D body, IFlipper flippers, RayCast2D floorDetector, Position2D position2D, Action<IKinematicPlatformMotionBodyConfig> conf) {
            base.Configure(name, body, position2D);
            conf(this);
            _flippers = flippers;
            _loggerCollision = LoggerFactory.GetLogger($"{name}.Collision");
            _floorDetector = floorDetector;
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

        public void ApplyDefaultGravity(float factor = 1.0F) {
            ApplyGravity(DefaultGravity * factor);
        }

        public void ApplyGravity(float gravity) {
            SpeedY = Mathf.Min(SpeedY + gravity * Delta, DefaultMaxFallingSpeed);
        }

        public void AddLateralSpeed(float xInput,
            float acceleration, float maxSpeed,
            float friction, float stopIfSpeedIsLessThan,
            float changeDirectionFactor) {
            Accelerate(ref SpeedX, xInput, acceleration, maxSpeed, friction, stopIfSpeedIsLessThan, changeDirectionFactor, Delta);
        }

        public void StopLateralSpeedWithFriction(float friction, float stopIfSpeedIsLessThan) {
            SlowDownSpeed(ref SpeedX, friction, stopIfSpeedIsLessThan);
        }

        public void MoveSnapping() => MoveSnapping(Vector2.One);

        public void MoveSnapping(Vector2 slowdownVector) {
            /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
            mueve una plataforma y nos queremos quedar pegados
            Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
            se para y ya no sigue a la plataforma
            */
            var stopOnSlopes = !HasFloorLateralMovement();
            var remain = Body.MoveAndSlideWithSnap(Speed * slowdownVector, SlopeRayCastVector,
                FloorVector, stopOnSlopes);
            SpeedY = remain.y; // this line stops the gravity accumulation
            // SpeedX = remain.x:  // WARNING!! this line should be always commented, player can't climb slopes with it!!
            _dirtyGroundCollisions = true;
        }

        public void Slide() => Slide(Vector2.One);

        /*
        inertia false = se mantiene el remain.x = al chocar con la cabeza pierde toda la inercia lateral que tenia y se va para abajo. Y si choca al subir y se
        se sube, pierde tambien la inercia teniendo que alecerar desde 0

        intertia true = se pierde el remain.x = al saltar y chocar (temporalmente) con un objeto hace que al dejar de chocar, recupere
        totalmente la movilidad = si choca justo antes de subir y luego se sube, corre a tope. Si choca con la cabeza y baja un poco,
        cuando de chocar, continua hacia delante a tope.
        */
        private bool _inertia = false;
        public void Slide(Vector2 slowdownVector) {
            // stopOnSlopes debe ser true para al caer sobre una pendiente la tome comoelo
            var stopOnSlopes = true;
            var remain = Body.MoveAndSlideWithSnap(Speed * slowdownVector, Vector2.Zero,
                FloorVector,
                stopOnSlopes);
            SpeedY = remain.y; // this line stops the gravity accumulation

            if (!_inertia) {
                SpeedX = remain.x;
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

        private KinematicPlatformMotionBody UpdateFloorCollisions() {
            ResetCollisionFlags();
            if (!Body.IsOnFloor()) {
                _loggerCollision.Debug(
                    $"UpdateFloorCollisions end: floor/falling: {Body.IsOnFloor()}/{_isOnFallingPlatform} (0 checks: air?)");
                return this;
            }

            var __isOnSlope = false;
            var __isOnSlopeUpRight = false;
            var __isOnMovingPlatform = false;
            var __isOnFallingPlatform = false;
            var __isOnSlopeStairs = false;
            var __colliderNormal = Vector2.Zero;

            var slideCount = Body.GetSlideCount();
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
                    if (Body.IsOnFloor() != _floorDetector.IsColliding()) {
                        diff.Append(" Floor:" + Body.IsOnFloor() + "/" + _floorDetector.IsColliding());
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
                $"UpdateFloorCollisions({slideCount}). Floor:{Body.IsOnFloor()} Slope:{_isOnSlope} Stairs:{_isOnSlopeStairs} Falling:{_isOnFallingPlatform} Moving:{_isOnMovingPlatform} Normal:{_colliderNormal}");
            // FloorDetector.IsColliding()

            Body.Update(); // this allow to call to _draw() with the colliderNormal updated
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

            if (collisionCollider is PhysicsBody2D falling && PlatformManager.IsFallingPlatform(falling)) {
                __isOnFallingPlatform = true;
            }

            if (collisionCollider is KinematicBody2D moving && PlatformManager.IsMovingPlatform(moving)) {
                __isOnMovingPlatform = true;
            }

            if (collisionCollider is PhysicsBody2D slopeStairs && SlopeStairsManager.IsSlopeStairs(slopeStairs)) {
                __isOnSlopeStairs = true;
            }
            Body.Update(); // this allow to call to _draw() with the colliderNormal updated
            return true;
        }

        private void CheckMoveAndSlideCollisions(int slideCount) {
            if (slideCount == 0) return;
            Vector2 lastColliderNormal = Vector2.Zero;
            for (var i = 0; i < slideCount; i++) {
                var collision = Body.GetSlideCollision(i);
                if (collision == null) {
                    continue;
                }
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

        public bool HasFloorLateralMovement() => Body.GetFloorVelocity().x != 0;

        public void Fall() {
            ApplyDefaultGravity();
            Slide();
        }
    }
}