using System;
using Betauer.Characters.Player.States;
using Betauer.Tools;
using Betauer.Tools.Character;
using Betauer.Tools.Events;
using Betauer.Tools.Input;
using Betauer.Tools.Statemachine;
using Godot;

namespace Betauer.Characters.Player {
    public class PlayerController : CharacterController {
        public PlayerConfig PlayerConfig => (PlayerConfig) CharacterConfig;
        private readonly StateMachine _stateMachine;
        public readonly MyPlayerActions PlayerActions;
        private AnimationPlayer _animationPlayer;
        private Sprite _sprite;
        private Label _label;


        public PlayerController() {
            CharacterConfig = new PlayerConfig();
            PlayerActions = new MyPlayerActions(-1);

            // State Machine
            _stateMachine = new StateMachine(PlayerConfig, this)
                .AddState(new GroundStateIdle(this))
                .AddState(new GroundStateRun(this))
                .AddState(new AirStateFall(this))
                .AddState(new AirStateJump(this));

            // Mapping
            PlayerActions.ConfigureMapping();
        }

        public override void _EnterTree() {
            _sprite = GetNode<Sprite>("Sprite");
            _animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
            _stateMachine.SetNextState(typeof(GroundStateIdle));
            _label = GetNode<Label>("Label");
        }

        public override void _Ready() {
            GameManager.Instance.RegisterPlayerController(this);
            PlatformManager.SubscribeFallingPlatformOut(new Area2DEnterListenerDelegate(this, _OnFallingPlatformOut));
            PlatformManager.SubscribeSlopeStairsUp(new Area2DEnterListenerDelegate(this, _OnSlopeStairsUpIn),
                new Area2DEnterListenerDelegate(this, _OnSlopeStairsUpOut));
            PlatformManager.SubscribeSlopeStairsDown(new Area2DEnterListenerDelegate(this, _OnSlopeStairsDownIn),
                new Area2DEnterListenerDelegate(this, _OnSlopeStairsDownOut));
            PlatformManager.SubscribeSlopeStairsEnabler(new Area2DEnterListenerDelegate(this, _OnSlopeStairsEnablerIn));
            PlatformManager.SubscribeSlopeStairsDisabler(
                new Area2DEnterListenerDelegate(this, _OnSlopeStairsDisablerIn));

            _animationPlayer.Connect("animation_finished", this, nameof(OnAnimationFinished));
        }

        public void EnableSlopeStairs() {
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "Enabling slope stairs");
            PlatformManager.DisableSlopeStairsCoverForBody(this);
            PlatformManager.EnableSlopeStairsForBody(this);
        }

        public void DisableSlopeStairs() {
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "Disabling slope stairs");
            PlatformManager.EnableSlopeStairsCoverForBody(this);
            PlatformManager.DisableSlopeStairsForBody(this);
        }

        public bool IsOnSlopeStairsUp() => _slope_stairs_up;
        public bool IsOnSlopeStairsDown() => _slope_stairs_down;
        private bool _slope_stairs_down;
        private bool _slope_stairs_up;

        public void _OnSlopeStairsUpIn(BodyOnArea2D evt) {
            _slope_stairs_up = true;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_up " + _slope_stairs_up);
        }

        public void _OnSlopeStairsUpOut(BodyOnArea2D evt) {
            _slope_stairs_up = false;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_up " + _slope_stairs_up);
        }

        public void _OnSlopeStairsDownIn(BodyOnArea2D evt) {
            _slope_stairs_down = true;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_down " + _slope_stairs_down);
        }

        public void _OnSlopeStairsDownOut(BodyOnArea2D evt) {
            _slope_stairs_down = false;
            Debug(PlayerConfig.DEBUG_SLOPE_STAIRS, "_slope_stairs_down " + _slope_stairs_down);
        }

        public void _OnFallingPlatformOut(BodyOnArea2D evt) => PlatformManager.BodyStopFallFromPlatform(this);

        public void _OnSlopeStairsEnablerIn(BodyOnArea2D evt) => EnableSlopeStairs();

        public void _OnSlopeStairsDisablerIn(BodyOnArea2D evt) => DisableSlopeStairs();

        protected override void PhysicsProcess() {
            _stateMachine.Execute();
            PlayerActions.ClearJustState();
            /*
            _label.Text = "Floor: " + IsOnFloor() + "\n" +
                          "Slope: " + IsOnSlope() + "\n" +
                          "Stair: " + IsOnSlopeStairs() + "\n" +
                          "Moving: " + IsOnMovingPlatform() + "\n" +
                          "Falling: " + IsOnFallingPlatform();
            */
        }

        private EventWrapper w = new EventWrapper(null);

        public override void _UnhandledInput(InputEvent @event) {
            w.@event = @event;
            if (!PlayerActions.Update(w)) {
                _stateMachine._UnhandledInput(@event);
            }

            TestJumpActions();
        }

        private void TestJumpActions() {
            if (PlayerConfig.DEBUG_INPUT_EVENTS) {
                if (w.IsMotion()) {
                    GD.Print("Axis " + w.Device + "[" + w.Axis + "]:" + w.GetStrength() + " (" + w.AxisValue + ")");
                } else if (w.IsAnyButton()) {
                    GD.Print("Button " + w.Device + "[" + w.Button + "]:" + w.Pressed + " (" + w.Pressure + ")");
                } else if (w.IsAnyKey()) {
                    GD.Print("Key " + w.KeyString + " [" + w.Key + "] " + w.Pressed + "/" + w.Echo);
                } else {
                }
            }

            /**
            * Aqui se comprueba que el JustPressed, Pressed y JustReleased de las acciones del PlayerActions coinciden
            * con las del singleton Input de Godot. Se genera un texto con los 3 resultados y si no coinciden se pinta
            */
            // var mine = PlayerActions.Jump.JustPressed + " " + PlayerActions.Jump.JustReleased + " " +
            // PlayerActions.Jump.Pressed;
            // var godot = Input.IsActionJustPressed("ui_select") + " " + Input.IsActionJustReleased("ui_select") + " " +
            // Input.IsActionPressed("ui_select");
            // if (!mine.Equals(godot)) {
            // GD.Print("Mine:" + mine);
            // GD.Print("Godo:" + godot);
            // }
        }

        public void SetNextConfig(System.Collections.Generic.Dictionary<string, object> config) {
            _stateMachine.SetNextConfig(config);
        }

        public void SetNextConfig(string key, object value) {
            _stateMachine.SetNextConfig(key, value);
        }

        public System.Collections.Generic.Dictionary<string, object> GetNextConfig() {
            return _stateMachine.GetNextConfig();
        }

        public void SetNextState(Type nextStateType, bool immediate = false) {
            _stateMachine.SetNextState(nextStateType, immediate);
        }

        public void Flip(float xInput) {
            if (xInput == 0) return;
            Flip(xInput < 0);
        }

        public void Flip(bool left) {
            _sprite.FlipH = left;
        }

        private string _currentAnimation = null;
        private const string _JUMP_ANIMATION = "Jump";
        private const string _IDLE_ANIMATION = "Idle";
        private const string _RUN_ANIMATION = "Run";
        private const string _FALL_ANIMATION = "Fall";
        private const string _ATTACK_ANIMATION = "Attack";
        private const string _JUMP_ATTACK_ANIMATION = "JumpAttack";

        public void AnimateJump() => AnimationPlay(_JUMP_ANIMATION);
        public void AnimateIdle() => AnimationPlay(_IDLE_ANIMATION);
        public void AnimateRun() => AnimationPlay(_RUN_ANIMATION);
        public void AnimateFall() => AnimationPlay(_FALL_ANIMATION);
        public void AnimateAttack() => AnimationPlay(_ATTACK_ANIMATION);
        public void AnimateJumpAttack() => AnimationPlay(_JUMP_ATTACK_ANIMATION);
        private string _previousAnimation = null;

        public bool IsAttacking = false;
        private void AnimationPlay(string newAnimation) {
            if (_currentAnimation == newAnimation) return;
            if (IsAttacking) {
                _previousAnimation = newAnimation;
            } else {
                _previousAnimation = _currentAnimation;
                _animationPlayer.Play(newAnimation);
                _currentAnimation = newAnimation;
            }
        }

        public void AirAttack() {
            AnimateJumpAttack();
            GD.Print("IsAttacking: true (jump)");
            IsAttacking = true;
        }
        public void Attack() {
            AnimateAttack();
            GD.Print("IsAttacking: true (ground)");
            IsAttacking = true;
        }

        public void OnAnimationFinished(string animation) {
            var attackingAnimation = animation == _ATTACK_ANIMATION || animation == _JUMP_ATTACK_ANIMATION;
            if (attackingAnimation) {
                IsAttacking = false;
            }
            GD.Print("IsAttacking "+IsAttacking+ " (finished "+animation+" is attacking "+attackingAnimation+")");
            if (_previousAnimation != null) {
                AnimationPlay(_previousAnimation);
            }
        }

        public void DeathZone(Area2D deathArea2D) {
            GD.Print("MUETO!!");
        }

    }
/*

GameManager.connect("death", self, "on_death")


func on_death(_cause):
	print("MUETO")
	set_process(false)
	set_physics_process(false)
	#Engine.set_target_fps(30)


func debug_motion(delta):
	if C.DEBUG_ACCELERATION && motion.x != 0:
		if lastMotion.x == 0:
			movStartTimeACC = 0 # starts to move
		elif movStartTimeACC != -1:
			movStartTimeACC += delta
			if abs(motion.x) >= C.MAX_SPEED:
				print("Full throtle ", motion.x, " in ", movStartTimeACC, "s")
				movStartTimeACC = -1

	if C.DEBUG_MAX_SPEED:
		if motion.x != 0:
			if lastMotion.x == 0:
				movStartTimeMAXSPEED = 0
				movStartPosMAXSPEED = get_position()
			else:
				if movStartTimeMAXSPEED >= 1:
					var distance = get_position().distance_to(movStartPosMAXSPEED)
					# No funciona bien si se cambia de direccion...
					print("Moved from ", movStartPosMAXSPEED, " to ",  get_position(), " in ", movStartTimeMAXSPEED, "s. Speed: ", abs(round(distance)),"px/second")
					movStartTimeMAXSPEED = 0
					movStartPosMAXSPEED = get_position()
				else:
					movStartTimeMAXSPEED += delta

		else:
			movStartPosMAXSPEED = null

	if C.DEBUG_MOTION && (lastMotion.x != motion.x || lastMotion.y != motion.y): print(motion, motion-lastMotion)

func debug_player_masks():
	if C.DEBUG_COLLISION:
		print("Player:  ",int(get_collision_mask_bit(0)), int(get_collision_mask_bit(1)), int(get_collision_mask_bit(2)))

func debug_collision():
	if C.DEBUG_COLLISION && get_slide_count():
		debug_player_masks()
		for i in get_slide_count():
			var collision = get_slide_collision(i)
			print("Collider:",int(collision.collider.get_collision_layer_bit(0)), int(collision.collider.get_collision_layer_bit(1)), int(collision.collider.get_collision_layer_bit(2)), " ", collision.collider.get_class(), ":'", collision.collider.name+"'")



 */
}