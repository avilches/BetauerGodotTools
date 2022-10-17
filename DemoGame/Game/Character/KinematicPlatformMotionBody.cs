using System;
using System.Text;
using Godot;
using Betauer;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Veronenger.Game.Character.Player;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character {

    [Service(Lifetime.Transient)]
    public class KinematicPlatformMotionBody : BaseMotionBody, IFlipper {
        public float DefaultGravity { get; set; } = 0f;
        public float DefaultMaxFallingSpeed { get; set; } = 1000f;
        public float DefaultMaxFloorGravity { get; set; } = 1000f;
        public Vector2 SnapToFloorVector { get; set; }
        public Vector2 FloorVector { get; set; }

        private Logger _loggerCollision;
        private RayCast2D _floorRaycast;
        private IFlipper _flippers;

        [Inject] private PlatformManager PlatformManager { get; set;}
        [Inject] private SlopeStairsManager SlopeStairsManager { get; set;}

        public void Configure(string name, KinematicBody2D body, IFlipper flippers, RayCast2D floorRaycast, Position2D position2D, Vector2 snapToFloorVector, Vector2 floorVector) {
            base.Configure(name, body, position2D);
            _flippers = flippers;
            _loggerCollision = LoggerFactory.GetLogger($"{name}.Collision");
            _floorRaycast = floorRaycast;
            SnapToFloorVector = snapToFloorVector;
            FloorVector = floorVector;
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

        public void MoveSnapping() {
            /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
            mueve una plataforma y nos queremos quedar pegados
            Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
            se para y ya no sigue a la plataforma
            */
            var stopOnSlopes = !HasFloorLateralMovement();
            var remain = Body.MoveAndSlideWithSnap(Force, SnapToFloorVector, FloorVector, stopOnSlopes);
            if (remain.y != 0f) {
                // remain.y means the user was climbing
                var onFloor = Body.GetSlideCount() > 0;
                if (onFloor) {
                    // Ensure the body can climb up or down slopes. Without this, the player will go down too fast
                    // and go up too slow
                    ForceY = remain.y;
                } else {
                    if (ForceY < 0) {
                        // User was climbing up and now it's in the air -> stop the speed.
                        // This is needed when the player is climbing a slope, and the slope ends
                        // if the remain.y speed is kept, the player will end the slope with a little jump
                        ForceY = 0;
                    }
                }
            }
            _dirtyFlags = true;
        }

        /*
        inertia false = se mantiene el remain.x = al chocar con la cabeza pierde toda la inercia lateral que tenia y se va para abajo. Y si choca al subir y se
        se sube, pierde tambien la inercia teniendo que alecerar desde 0

        intertia true = se pierde el remain.x = al saltar y chocar (temporalmente) con un objeto hace que al dejar de chocar, recupere
        totalmente la movilidad = si choca justo antes de subir y luego se sube, corre a tope. Si choca con la cabeza y baja un poco,
        cuando de chocar, continua hacia delante a tope.
        */
        private bool _inertia = false;
        public void MoveSlide(float speedYLimit = 0f) {
            // stopOnSlopes debe ser true para al caer sobre una pendiente la tome comoelo
            var stopOnSlopes = true;
            var remain = Body.MoveAndSlideWithSnap(Force, Vector2.Zero, FloorVector, stopOnSlopes);
            ForceY = remain.y; 
            if (speedYLimit != 0 && remain.y < 0) {
                // Ensure that the speed of a fast moving up platform is not added the jump speed.
                // The trick is just limit the remaining speed with the jump limit
                ForceY = Math.Max(remain.y, -Math.Abs(speedYLimit));
            } else {
                ForceY = remain.y;
            }

            if (!_inertia) {
                ForceX = remain.x;
            }

            _dirtyFlags = true;
        }

        private bool _dirtyFlags = true;

        private bool _isOnSlope = false;
        private bool _isJustLanded = false;
        private bool _isJustTookOff = false;
        private bool _isOnFloor = false;
        private bool _isOnSlopeUpRight = false;
        private bool _isOnMovingPlatform = false;
        private bool _isOnFallingPlatform = false;
        private bool _isOnSlopeStairs = false;
        private Vector2 _colliderNormal = Vector2.Zero;
        private PhysicsBody2D? _platform = null;

        public bool IsOnFloor() => UpdateFlags()._isOnFloor;
        public bool IsJustLanded() => UpdateFlags()._isJustLanded;
        public bool IsJustTookOff() => UpdateFlags()._isJustTookOff;
        public bool IsOnMovingPlatform() => UpdateFlags()._isOnMovingPlatform;
        public bool IsOnFallingPlatform() => UpdateFlags()._isOnFallingPlatform;

        public Vector2 GetColliderNormal() => UpdateFlags()._colliderNormal;
        public PhysicsBody2D? GetPlatform() => UpdateFlags()._platform;

        public bool IsOnSlope() => UpdateFlags()._isOnSlope;
        public bool IsOnSlopeUpRight() => IsOnSlope() && _isOnSlopeUpRight;
        public bool IsOnSlopeDownLeft() => IsOnSlope() && _isOnSlopeUpRight;
        public bool IsOnSlopeDownRight() => IsOnSlope() && !_isOnSlopeUpRight;
        public bool IsOnSlopeUpLeft() => IsOnSlope() && !_isOnSlopeUpRight;
        public bool IsOnSlopeStairs() => UpdateFlags()._isOnSlopeStairs;

        private KinematicPlatformMotionBody UpdateFlags() {
            if (!_dirtyFlags) return this;
            var wasOnFloor = _isOnFloor;
            _isOnFloor = Body.IsOnFloor();
            _isJustLanded = false;
            _isJustTookOff = false;
            _isOnSlope = false;
            _isOnSlopeUpRight = false;
            _isOnMovingPlatform = false;
            _isOnFallingPlatform = false;
            _isOnSlopeStairs = false;
            _colliderNormal = Vector2.Zero;
            _dirtyFlags = false;
            _platform = null;

            if (!_isOnFloor) {
                _isJustTookOff = wasOnFloor;
                // No floor, rest of the flags are false too
                return this;
            }
            _isJustLanded = !wasOnFloor;
            var slideCount = Body.GetSlideCount();
            if (slideCount > 0) {
                CheckCollisionFromMoveAndSlide(slideCount);
            }
            if (!_isOnSlope) {
                CheckCollisionFromRaycast();
            }
            return this;
        }

        private void CheckCollisionFromMoveAndSlide(int slideCount) {
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
                if (_platform == null && PlatformManager.IsPlatform(collisionCollider)) {
                    _platform = collisionCollider as PhysicsBody2D;
                }
                _isOnFallingPlatform = PlatformManager.IsFallingPlatform(collisionCollider);
                _isOnMovingPlatform = PlatformManager.IsMovingPlatform(collisionCollider);
                _isOnSlopeStairs = SlopeStairsManager.IsSlopeStairs(collisionCollider);
            }
            _colliderNormal = lastColliderNormal;
        }

        private void CheckCollisionFromRaycast() {
            _floorRaycast.ForceRaycastUpdate();
            var collisionCollider = _floorRaycast.GetCollider();
            if (collisionCollider == null) return;
            _isOnFloor = true;
            _colliderNormal = _floorRaycast.GetCollisionNormal();

            if (_colliderNormal != Vector2.Zero && Mathf.Abs(_colliderNormal.y) < 1) {
                _isOnSlope = true;
                _isOnSlopeUpRight = _colliderNormal.x < 0;
            }

            if (!_isOnFallingPlatform) _isOnFallingPlatform = PlatformManager.IsFallingPlatform(collisionCollider);
            if (!_isOnMovingPlatform) _isOnMovingPlatform = PlatformManager.IsMovingPlatform(collisionCollider);
            if (!_isOnSlopeStairs) _isOnSlopeStairs = SlopeStairsManager.IsSlopeStairs(collisionCollider);
        }

        public bool HasFloorLateralMovement() => Body.GetFloorVelocity().x != 0;

    }
}