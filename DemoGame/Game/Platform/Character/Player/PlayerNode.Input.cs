using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Betauer.Input.Joypad;
using Veronenger.Game.Platform.Character.InputActions;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game.Platform.Character.Player; 

public partial class PlayerNode {

	private PlayerJoypadController _joypadController;
	private float XInput => _joypadController.Lateral.Strength;
	private float YInput => _joypadController.Vertical.Strength;
	private bool IsPressingRight => _joypadController.Right.IsPressed;
	private bool IsPressingLeft => _joypadController.Left.IsPressed;
	private bool IsPressingUp => _joypadController.Up.IsPressed;
	private bool IsPressingDown => _joypadController.Down.IsPressed;
	private InputAction Jump => _joypadController.Jump;
	private InputAction Attack => _joypadController.Attack;
	private InputAction Float => _joypadController.Float;
	private InputAction NextItem => _joypadController.NextItem;
	private InputAction PrevItem => _joypadController.PrevItem;
	private InputAction Drop => _joypadController.Drop;

	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;

	public PlayerMapping PlayerMapping { get; set; }

	[Inject] private ILazy<SettingsMenu> SettingsMenuLazy { get; set; }

	public void SetPlayerMapping(PlayerMapping playerMapping) {
		Name = $"Player{playerMapping.Player}";
		Label.Text = $"P{playerMapping.Player}";
		PlayerMapping = playerMapping;

		_joypadController = PlayerActionsContainer.CreateJoypadController<PlayerJoypadController>(PlayerMapping);
		SettingsMenuLazy.Get().OnRedefine += _joypadController.Redefine;
		TreeExiting += () => {
			SettingsMenuLazy.Get().OnRedefine -= _joypadController.Redefine;
			_joypadController.Disconnect();
		};
		// PlayerMapping.OnJoypadIdChanged += () => {
			//Console.WriteLine("OnJoypadChanged:"+PlayerMapping);
		// };
		// PlayerMapping.OnJoypadConnect += () => {
			//Console.WriteLine("OnJoypadConnect:"+PlayerMapping);
		// };
		// PlayerMapping.OnJoypadDisconnect += () => {
			//Console.WriteLine("OnJoypadDisconnect:"+PlayerMapping);
		// };
	}
}
