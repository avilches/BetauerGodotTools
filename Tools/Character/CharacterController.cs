using System;
using Betauer.Tools.Platforms;
using Godot;

namespace Betauer.Tools.Character {
    public abstract class CharacterController : KinematicBody2D, IFrameAware {
        protected CharacterConfig CharacterConfig;
        public float Delta { get; private set; } = 0.16f;
        public Vector2 Motion = Vector2.Zero;
        private Vector2 _lastMotion = Vector2.Zero;
        public PlatformManager PlatformManager => PlatformManager.Instance;
        private long _frame = 0;
        public long GetFrame() {
            return _frame;
        }

        public void Debug(bool flag, string message) {
            if (flag) {
                Debug(message);
            }
        }

        public void Debug(string message) {
            GD.Print("#" + GetFrame() + ": " + GetType().Name + " | " + message);
        }

        public void SetMotion(Vector2 motion) {
            Motion.x = motion.x;
            Motion.y = motion.y;
        }

        public void AddMotion(Vector2 motion) {
            Motion.x += motion.x;
            Motion.y += motion.y;
        }

        public void SetMotionX(float x) {
            Motion.x = x;
        }

        public void SetMotionY(float y) {
            Motion.y = y;
        }

        public void ApplyGravity(float factor = 1.0F) {
            Motion.y += CharacterConfig.GRAVITY * factor * Delta;
        }

        public sealed override void _PhysicsProcess(float delta) {
            Delta = delta;
            _frame++;
            _lastMotion = Motion;
            PhysicsProcess();
            if (CharacterConfig.DEBUG_MOTION && Motion != _lastMotion) {
                GD.Print("Motion:" + Motion + " (diff " + (_lastMotion - Motion) + ")");
            }
        }

        protected abstract void PhysicsProcess();

        /**
     *
     */
        public void AddLateralMovement(float xInput, float acceleration, float friction, float stopIfSpeedIsLessThan,
            float changeDirectionFactor) {
            if (xInput != 0) {
                var directionChanged = Math.Sign(Motion.x) != Math.Sign(xInput);
                if (directionChanged) {
                    SetMotionX((Motion.x * changeDirectionFactor) + xInput * acceleration * Delta);
                } else {
                    SetMotionX(Motion.x + xInput * acceleration * Delta);
                }
            } else {
                if (Mathf.Abs(Motion.x) < stopIfSpeedIsLessThan) {
                    SetMotionX(0);
                } else {
                    SetMotionX(Motion.x * friction);
                }
            }
        }

        public void LimitMotion(float maxSpeedFactor = 1.0F) {
            var realMaxSpeed = CharacterConfig.MAX_SPEED * maxSpeedFactor;
            Motion.x = Mathf.Clamp(Motion.x, -realMaxSpeed, realMaxSpeed);
            Motion.y = Mathf.Min(Motion.y,
                CharacterConfig.MAX_FALLING_SPEED); //  avoid gravity continue forever in free fall
        }

        public void MoveSnapping() {
            MoveSnapping(Vector2.One);
        }

        public void MoveSnapping(Vector2 slowdownVector) {
            /* stopOnSlopes debe ser true para que no se resbale en las pendientes, pero tiene que ser false si se
            mueve una plataforma y nos queremos quedar pegados
            Hay un bug conocido: si la plataforma que se mueve tiene slope, entonces para que detected el slope como suelo,
            se para y ya no sigue a la plataforma
            */
            var stopOnSlopes = !HasFloorLateralMovement();
            var remain = MoveAndSlideWithSnap(Motion * slowdownVector, CharacterConfig.SLOPE_RAYCAST_VECTOR,
                CharacterConfig.FLOOR, stopOnSlopes);
            Motion.y = remain.y; // this line stops the gravity accumulation
            // motion.x = remain.x:  // WARNING!! this line should be always commented, player can't climb slopes with it!!
            _dirtyGroundCollisions = true;
        }

        public void Slide() {
            Slide(Vector2.One);
        }

        public void Slide(Vector2 slowdownVector) {
            // stopOnSlopes debe ser true para al caer sobre una pendiente la tome comoelo
            var stopOnSlopes = true;
            var remain = MoveAndSlideWithSnap(Motion * slowdownVector, Vector2.Zero, CharacterConfig.FLOOR, stopOnSlopes);
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
        private bool _isOnMovingPlatform = false;
        private bool _isOnFallingPlatform = false;
        private bool _isOnSlopeStairs = false;

        public bool IsOnSlope() => (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnSlope;
        public bool IsOnMovingPlatform() => (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnMovingPlatform;
        public bool IsOnFallingPlatform() => (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnFallingPlatform;
        public bool IsOnSlopeStairs() => (_dirtyGroundCollisions ? UpdateFloorCollisions() : this)._isOnSlopeStairs;
        public Vector2 ColliderNormal = Vector2.Zero;

        public CharacterController UpdateFloorCollisions() {
            _dirtyGroundCollisions = false;
            _isOnSlope = false;
            _isOnMovingPlatform = false;
            _isOnFallingPlatform = false;
            _isOnSlopeStairs = false;
            ColliderNormal = Vector2.Zero;
            if (!IsOnFloor()) return this;
            // if (GetSlideCount() == 0) {
                // GD.Print("Ground but no colliders??");
            // }

            var slideCount = GetSlideCount();
            for (var i = 0; i < slideCount; i++) {
                var collision = GetSlideCollision(i);
                ColliderNormal = collision.Normal;

                // This is like a callback: if the objet has method collide_with, it's called
                // if (collision.Collider.has_method("collide_with"):
                // collision.collider.collide_with(self, collision)

                if (Mathf.Abs(collision.Normal.y) < 1) {
                    _isOnSlope = true;
                }

                if (collision.Collider is KinematicBody2D falling && PlatformManager.IsFallingPlatform(falling)) {
                    _isOnFallingPlatform = true;
                }

                if (collision.Collider is KinematicBody2D moving && PlatformManager.IsMovingPlatform(moving)) {
                    _isOnMovingPlatform = true;
                }

                if (collision.Collider is KinematicBody2D slopeStairs && PlatformManager.IsSlopeStairs(slopeStairs)) {
                    _isOnSlopeStairs = true;
                }
            }
            Update();  // this allow to call to _draw() with the colliderNormal updated
            return this;
        }

        public bool HasFloorLateralMovement() {
            return GetFloorVelocity().x != 0;
        }
    }
}