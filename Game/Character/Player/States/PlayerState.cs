using Godot;
using Betauer;
using Betauer.DI;
using Betauer.Input;
using Betauer.Statemachine;
using Veronenger.Game.Controller.Character;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Character.Player.States {
    public abstract class PlayerState : BaseState {
        public const string StateIdle = nameof(GroundStateIdle);
        public const string StateRun = nameof(GroundStateRun);
        public const string StateFallShort = nameof(AirStateFallShort);
        public const string StateFallLong = nameof(AirStateFallLong);
        public const string StateJump = nameof(AirStateJump);

        public const string CoyoteJumpEnabledKey = nameof(CoyoteJumpEnabledKey);


        [Inject] public PlatformManager PlatformManager;
        [Inject] public InputManager InputManager;

        protected readonly PlayerController Player;
        protected MotionBody Body => Player.MotionBody;
        private readonly Logger _loggerJumpHelper = LoggerFactory.GetLogger("Player", "JumpHelper");
        private readonly Logger _loggerCoyoteJump = LoggerFactory.GetLogger("Player", "CoyoteJump");
        private readonly Logger _loggerJumpVelocity = LoggerFactory.GetLogger("Player", "JumpVelocity");

        protected PlayerState(string name, PlayerController player) : base(name) {
            Player = player;
        }

        // Input from the player
        protected float XInput => InputManager.LateralMotion.Strength;
        protected float YInput => InputManager.VerticalMotion.Strength;
        protected ActionState Jump => InputManager.Jump;
        protected ActionState Attack => InputManager.Attack;
        protected bool IsRight => XInput > 0;
        protected bool IsLeft => XInput < 0;
        protected bool IsUp => YInput < 0;
        protected bool IsDown => YInput > 0;

        protected Vector2 Motion => Player.MotionBody.Motion;
        protected PlayerConfig PlayerConfig => Player.PlayerConfig;
        protected MotionConfig MotionConfig => Player.PlayerConfig.MotionConfig;

        protected void DebugJumpHelper(string message) => _loggerJumpHelper.Debug(message);
        protected void DebugCoyoteJump(string message) => _loggerCoyoteJump.Debug(message);
        protected void DebugJump(string message) => _loggerJumpVelocity.Debug(message);
    }
}