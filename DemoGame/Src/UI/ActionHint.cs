using System.Collections.Generic;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Godot;
using Veronenger.UI.Consoles;

namespace Veronenger.UI; 

public partial class ActionHint : HBoxContainer {
	[OnReady("%Label1")] private Label _label1;
	[OnReady("%ControlConsoleButton")] private Control _controlConsoleButton;
	[OnReady("%ConsoleButton")] private ConsoleButton _consoleButton;
	[OnReady("%KeyButton")] private Label _keyButton;
	[OnReady("%Label2")] private Label _label2;
	[Inject] private InputActionsContainer _inputActionsContainer { get; set; }


	public ActionHint Labels(string? label1, string? label2) {
		_label1.Text = label1;
		_label2.Text = label2;
		_label1.Visible = !string.IsNullOrEmpty(label1);
		_label2.Visible = !string.IsNullOrEmpty(label2);
		return this;
	}

	// If using keyboard
	private string? _key;
		
	// If using gamepad
	private JoyButton _joyButton;
	private JoyAxis _joyAxis;
	private bool _animate;
		
	private bool _isUsingKeyboard;

	public ActionHint AxisAction(AxisAction axisAction, bool animate) {
		_joyAxis = axisAction.Positive.Axis;
		_joyButton = JoyButton.Invalid;
		_animate = animate;

		var keyList = new List<string>(2);
		var keys = axisAction.Negative.Keys;
		if (keys.Count > 0) keyList.Add(keys[0].ToString());
		var otherKeys = axisAction.Positive.Keys;
		if (otherKeys.Count > 0) keyList.Add(otherKeys[0].ToString());
		_key = $"[{string.Join(" ", keyList)}]";

		Refresh();
		return this;
	}
	
	public ActionHint InputAction(InputAction action, bool animate) {
		var buttons = action.Buttons;
		_joyAxis = JoyAxis.Invalid;
		_joyButton = buttons.Count > 0 ? buttons[0] : JoyButton.Invalid;
		_animate = animate;

		var keys = action.Keys;
		_key = keys.Count > 0 ? $"[{keys[0]}]" : "";
			
		Refresh();
		return this;
	}

	public ActionHint Refresh() {
		if (_key == null) return this;
		
		if (_isUsingKeyboard) {
			_keyButton.Visible = true;
			_controlConsoleButton.Visible = false;
			_keyButton.Text = _key;
			
		} else {
			_keyButton.Visible = false;
			_controlConsoleButton.Visible = true;

			if (_joyButton != JoyButton.Invalid) {
				_consoleButton.SetButton(_joyButton, _animate ? ConsoleButtonState.Animated : ConsoleButtonState.Normal);
			} else if (_joyAxis != JoyAxis.Invalid) {
				_consoleButton.SetAxis(_joyAxis, _animate ? ConsoleButtonState.Animated : ConsoleButtonState.Normal);
			} else {
				_controlConsoleButton.Visible = false;
			}
		}
		return this;
	}

	public override void _Input(InputEvent e) {
		if ((e.IsAnyKey() || e.IsMouse()) && !_isUsingKeyboard) {
			_isUsingKeyboard = true;
			Refresh();
		} else if ((e.IsAnyButton() || e.IsAnyAxis()) && _isUsingKeyboard) {
			_isUsingKeyboard = false;
			Refresh();
		}
	}
}