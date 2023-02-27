using Betauer.DI;
using Betauer.Input;
using Betauer.NodePath;
using Godot;

namespace Veronenger.UI.Consoles;

public partial class ConsoleButton : Sprite2D {

	[Inject] protected Xbox360ControllerSpriteConfig Xbox360Controller { get; set; }
	[Inject] protected XboxOneControllerSpriteConfig XboxOneController { get; set; }
	
	[NodePath("AnimationPlayer")] private AnimationPlayer _animation;

	private IControllerSpriteConfig _config;
	
	public ConsoleButtonState State { get; private set; } = ConsoleButtonState.Normal;
	public string? Animation { get; private set; }
	public JoyButton Button { get; private set; } = JoyButton.Invalid;
	public JoyAxis Axis { get; private set; } = JoyAxis.Invalid;
	public ConsoleButtonView? Current => Axis != JoyAxis.Invalid ? _config.Get(Axis) : _config.Get(Button);

	public override void _Ready() {
		if (_config == null) {
			// TODO: this should depends on the controller connected
			Configure(XboxOneController);
		}
	}

	public override void _Input(InputEvent e) {
		string? name = e.GetJoypadName();
		if (name == null) return;
		if (name.StartsWith("Xbox Series") || name.StartsWith("Xbox One")) {
			Configure(XboxOneController);
		} else {
			Configure(Xbox360Controller);
		}
	}

	public void Configure(IControllerSpriteConfig config) {
		var refresh = _config != config;
		_config = config;
		Texture = _config.Texture2D;
		if (refresh) Refresh();
	}

	public void InputAction(InputAction action, ConsoleButtonState state) {
		var buttons = action.Buttons;
		if (buttons.Count > 0) SetButton(buttons[0], state);
	}

	public void InputAction(AxisAction axis, ConsoleButtonState state) {
		SetAxis(axis.Positive.Axis, state);
	}

	public void SetButton(JoyButton button, ConsoleButtonState state) {
		State = state;
		Animation = null;
		Button = button;
		Axis = JoyAxis.Invalid;
		FindAnimation();
		Refresh();
	}

	public void SetAxis(JoyAxis axis, ConsoleButtonState state) {
		State = state;
		Animation = null;
		Button = JoyButton.Invalid;
		Axis = axis;
		FindAnimation();
		Refresh();
	}

	public void Animate(string animation) {
		State = ConsoleButtonState.Animated;
		Animation = animation;
		Button = JoyButton.Invalid;
		Axis = JoyAxis.Invalid;
		Refresh();
	}

	public void SetPressed(bool pressed) {
		State = pressed ? ConsoleButtonState.Pressed : ConsoleButtonState.Normal;
		Refresh();
	}

	public bool HasAnimation(string animation) {
		return _animation.HasAnimation(animation);
	}

	private void FindAnimation() {
		if (State == ConsoleButtonState.Animated) {
			var animation = Current?.Animation;
			if (animation != null && _animation.HasAnimation(animation)) {
				Animation = animation;
			} else {
				State = ConsoleButtonState.Normal;
				Animation = null;
			}
		}
	}
	
	private void Refresh() {
		if (State == ConsoleButtonState.Animated) {
			_animation.Play(Animation);
		} else {
			_animation.Stop();
			var view = Current;
			Frame = view != null 
				? State == ConsoleButtonState.Pressed 
					? view.FramePressed 
					: view.Frame 
				: 0;
		}
	}
}
