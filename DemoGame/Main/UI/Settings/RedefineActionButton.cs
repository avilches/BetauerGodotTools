using System.Linq;
using Betauer.Input;
using Betauer.NodePath;
using Godot;
using Veronenger.UI.Consoles;

namespace Veronenger.Main.UI; 

public partial class RedefineActionButton : Button {
	[NodePath("%ActionName")] private Label _actionNameLabel;
	[NodePath("%ConsoleButton")] private ConsoleButton _consoleButton;
	[NodePath("%Key")] private Label _keyLabel;
		
	public InputAction InputAction;
	public string ActionName;
	public bool IsKey { get; private set; }
	public bool IsButton => !IsKey;

	public void SetInputAction(string actionName, InputAction inputAction, bool key) {
		InputAction = inputAction;
		ActionName = " " + actionName;
		IsKey = key;
	}

	public override void _Ready() {
		Refresh();
		if (IsKey) {
			_consoleButton.QueueFree();
		} else {
			_keyLabel.QueueFree();
		}
	}

	public void Refresh() {
		if (InputAction == null) return;
		_actionNameLabel.Text = ActionName;
		if (IsKey) {
			_consoleButton.Visible = false;
			_keyLabel.Visible = true;
			if (InputAction.Keys.Count > 0) {
				_keyLabel.Text = InputAction.Keys.First().ToString();
			}
		} else {
			_consoleButton.Visible = true;
			_keyLabel.Visible = false;
				
			_consoleButton.InputAction(InputAction, ConsoleButtonState.Normal);
		}
	}
}
