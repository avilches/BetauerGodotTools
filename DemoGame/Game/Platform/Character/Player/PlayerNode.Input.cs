using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Betauer.Input;
using Veronenger.Game.Platform.Character.InputActions;
using Veronenger.Game.UI.Settings;

namespace Veronenger.Game.Platform.Character.Player; 

public partial class PlayerNode {

	public PlatformPlayerActions PlayerActions { get; private set; }
	
	private float XInput => PlayerActions.Lateral.Strength;
	private float YInput => PlayerActions.Vertical.Strength;
	private bool IsPressingRight => PlayerActions.Right.IsPressed;
	private bool IsPressingLeft => PlayerActions.Left.IsPressed;
	private bool IsPressingUp => PlayerActions.Up.IsPressed;
	private bool IsPressingDown => PlayerActions.Down.IsPressed;
	private InputAction Jump => PlayerActions.Jump;
	private InputAction Attack => PlayerActions.Attack;
	private InputAction Float => PlayerActions.Float;
	private InputAction NextItem => PlayerActions.NextItem;
	private InputAction PrevItem => PlayerActions.PrevItem;
	private InputAction Drop => PlayerActions.Drop;

	private float MotionX => PlatformBody.MotionX;
	private float MotionY => PlatformBody.MotionY;

	[Inject] private ILazy<SettingsMenu> SettingsMenuLazy { get; set; }

	public void SetPlayerActions(PlatformPlayerActions playerActions) {
		PlayerActions = playerActions;
		Name = $"Player{playerActions.PlayerId}";
		Label.Text = $"P{playerActions.PlayerId}";
	}
}
