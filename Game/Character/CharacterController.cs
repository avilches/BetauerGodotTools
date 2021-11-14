using System;
using Godot;
using Tools;
using Tools.Statemachine;
using Veronenger.Game.Managers;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Character {
    public abstract class CharacterController : KinematicBody2D, IFrameAware {
        protected CharacterConfig CharacterConfig { get;}
        protected StateMachine StateMachine { get;  }
        protected Sprite MainSprite { get; private set; }
        protected AnimationStack AnimationStack { get; private set; }
        protected Label Label { get; private set; }

        public float Delta { get; private set; } = 0.16f;
        public Vector2 Motion = Vector2.Zero;
        private Vector2 _lastMotion = Vector2.Zero;
        public PlatformManager PlatformManager => GameManager.Instance.PlatformManager;
        public SlopeStairsManager SlopeStairsManager => GameManager.Instance.SlopeStairsManager;
        public CharacterManager CharacterManager => GameManager.Instance.CharacterManager;
        private long _frame = 0;

        protected abstract StateMachine CreateStateMachine();
        protected abstract CharacterConfig CreateCharacterConfig();
        protected abstract AnimationStack CreateAnimationStack(AnimationPlayer animationPlayer);

        protected CharacterController() {
            CharacterConfig = CreateCharacterConfig();
            StateMachine = CreateStateMachine();
        }

        public override void _EnterTree() {
            AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            MainSprite = GetNode<Sprite>("Sprite");
            AnimationStack = CreateAnimationStack(animationPlayer);
            Label = GetNode<Label>("Label");
        }

        public long GetFrame() {
            return _frame;
        }

        public void SetNextConfig(System.Collections.Generic.Dictionary<string, object> config) {
            StateMachine.SetNextConfig(config);
        }

        public void SetNextConfig(string key, object value) {
            StateMachine.SetNextConfig(key, value);
        }

        public System.Collections.Generic.Dictionary<string, object> GetNextConfig() {
            return StateMachine.GetNextConfig();
        }

        public void SetNextState(Type nextStateType, bool immediate = false) {
            StateMachine.SetNextState(nextStateType, immediate);
        }

        public void Flip(float xInput) {
            if (xInput == 0) return;
            // TODO: write the FlipH on every frame (cost 1) is cheaper (in terms of inter-op cost) than read the FlipH
            // and, if it's different, change it (read 1 op + write: 2 ops in the worst case).
            // If the FlipH value is cached, the inter-op cost will be 0 if no change, 1 if change
            Flip(xInput < 0);
        }

        public void Flip(bool left) {
            MainSprite.FlipH = left;
        }


        public void Debug(bool flag, string message) {
            if (flag) {
                Debug(message);
            }
        }

        public void Debug(string message) {
            GD.Print($"#{GetFrame()}: {GetType().Name} | {message}");
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
                GD.Print($"Motion:{Motion} (diff {(_lastMotion - Motion)})");
            }
        }

        protected abstract void PhysicsProcess();

        public void AddLateralMotion(float xInput, float acceleration, float friction, float stopIfSpeedIsLessThan,
            float changeDirectionFactor) {
            if (xInput != 0) {
                var directionChanged = Math.Sign(Motion.x) != Math.Sign(xInput);
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
            var remain = MoveAndSlideWithSnap(Motion * slowdownVector, Vector2.Zero, CharacterConfig.FLOOR,
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

        void ResetCollisionFlags() {
            _dirtyGroundCollisions = false;

            _isOnSlope = false;
            _isOnSlopeUpRight = false;
            _isOnMovingPlatform = false;
            _isOnFallingPlatform = false;
            _isOnSlopeStairs = false;
            _colliderNormal = Vector2.Zero;
        }

        public CharacterController UpdateFloorCollisions() {
            ResetCollisionFlags();
            if (!IsOnFloor()) {
                // GD.Print("UpdateFloorCollisions end: floor/falling:", IsOnFloor(),"/"+_isOnFallingPlatform, " (0 checks: air?)");
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
            CheckRaycastGroundCollisions(ref __colliderNormal, ref __isOnSlope, ref __isOnFallingPlatform,
                ref __isOnMovingPlatform, ref __isOnSlopeStairs, ref __isOnSlopeUpRight);

            if (CharacterConfig.DEBUG_COLLISION_RAYSCAST_VS_SLIDECOUNT) {
                if (_isOnSlope != __isOnSlope ||
                    _isOnSlopeStairs != __isOnSlopeStairs ||
                    _isOnFallingPlatform != __isOnFallingPlatform ||
                    _isOnMovingPlatform != __isOnMovingPlatform ||
                    _colliderNormal != __colliderNormal) {
                    string diff = "UpdateFloorCollisions(" + slideCount + ")";
                    if (IsOnFloor() != FloorDetector.IsColliding()) {
                        diff += " Floor:" + IsOnFloor() + "/" + FloorDetector.IsColliding();
                    }
                    if (_isOnSlope != __isOnSlope) {
                        diff += " Slope:" + _isOnSlope + "/" + __isOnSlope;
                    }
                    if (_isOnSlopeStairs != __isOnSlopeStairs) {
                        diff += " Stairs:" + _isOnSlopeStairs + "/" + __isOnSlopeStairs;
                    }
                    if (_isOnFallingPlatform != __isOnFallingPlatform) {
                        diff += " Falling:" + _isOnFallingPlatform + "/" + __isOnFallingPlatform;
                    }
                    if (_isOnMovingPlatform != __isOnMovingPlatform) {
                        diff += " Moving:" + _isOnMovingPlatform + "/" + __isOnMovingPlatform;
                    }
                    if (_colliderNormal != __colliderNormal) {
                        diff += " Normal:" + _colliderNormal + "/" + __colliderNormal;
                    }
                    GD.Print(diff);
                }
            }

            _isOnSlope |= __isOnSlope;
            _isOnSlopeUpRight |= __isOnSlopeUpRight;
            _isOnMovingPlatform |= __isOnMovingPlatform;
            _isOnFallingPlatform |= __isOnFallingPlatform;
            _isOnSlopeStairs |= __isOnSlopeStairs;
            _colliderNormal = _colliderNormal != Vector2.Zero ? _colliderNormal : __colliderNormal;

            if (CharacterConfig.DEBUG_COLLISION) {
                GD.Print("UpdateFloorCollisions. Floor:", IsOnFloor(),
                    // " Falling:"+_isOnFallingPlatform,"/",__isOnFallingPlatform,
                    " Slope:", _isOnSlope,
                    " Stairs:", _isOnSlopeStairs,
                    " Falling:", _isOnFallingPlatform,
                    " Moving:", _isOnMovingPlatform,
                    " Normal:", _colliderNormal,
                    " (", slideCount, "/", FloorDetector.IsColliding(), ")");
            }

            Update(); // this allow to call to _draw() with the colliderNormal updated
            return this;
        }

        private bool CheckRaycastGroundCollisions(ref Vector2 __colliderNormal, ref bool __isOnSlope,
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

        public bool HasFloorLateralMovement() {
            return GetFloorVelocity().x != 0;
        }

        // public override void _Draw() {
            // DrawLine(slopeDetector.Position, slopeDetector.Position + slopeDetector.CastTo, Colors.Red, 3F);
        // }
    }
}