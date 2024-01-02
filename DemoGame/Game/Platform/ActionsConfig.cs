using Betauer.Application.Settings.Attributes;
using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Attributes;
using Godot;

namespace Veronenger.Game.Platform; 

[Configuration]
[SettingsContainer("SettingsContainer")]
[InputActionsContainer("PlayerActionsContainer")]
public class ActionsConfig {

	[Singleton] public InputActionsContainer PlayerActionsContainer => new();

	[AxisAction(SaveAs = "Controls/Lateral")] 
	private AxisAction Lateral => AxisAction.Create().Build();

	[AxisAction] 
	private AxisAction Vertical => AxisAction.Create().Build();

	[InputAction(AxisName = "Vertical", SaveAs = "Controls/Up")]
	private InputAction Up => InputAction.Create()
		.Keys(Key.Up)
		.Buttons(JoyButton.DpadUp)
		.NegativeAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.Build();

	[InputAction(AxisName = "Vertical", SaveAs = "Controls/Down")]
	private InputAction Down => InputAction.Create()
		.Keys(Key.Down)
		.Buttons(JoyButton.DpadDown)
		.PositiveAxis(JoyAxis.LeftY)
		.DeadZone(0.5f)
		.Build();

	[InputAction(AxisName = "Lateral", SaveAs = "Controls/Left")]
	private InputAction Left => InputAction.Create()
		.Keys(Key.Left)
		.Buttons(JoyButton.DpadLeft)
		.NegativeAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.Build();

	[InputAction(AxisName = "Lateral", SaveAs = "Controls/Right")]
	private InputAction Right => InputAction.Create()
		.Keys(Key.Right)
		.Buttons(JoyButton.DpadRight)
		.PositiveAxis(JoyAxis.LeftX)
		.DeadZone(0.5f)
		.Build();

	[InputAction(SaveAs = "Controls/Jump")]
	private InputAction Jump => InputAction.Create()
		.Keys(Key.Space)
		.Buttons(JoyButton.A)
		.Build(true);

	[InputAction(SaveAs = "Controls/Attack")]
	private InputAction Attack => InputAction.Create()
		.Keys(Key.C)
		.Click(MouseButton.Left)
		.Buttons(JoyButton.B)
		.Build();

	[InputAction()]
	private InputAction Drop => InputAction.Create()
		.Keys(Key.G)
		.Build();

	[InputAction(SaveAs = "Controls/NextItem")]
	private InputAction NextItem => InputAction.Create()
		.Keys(Key.E)
		.Buttons(JoyButton.RightShoulder)
		.Build();

	[InputAction(SaveAs = "Controls/PrevItem")]
	private InputAction PrevItem => InputAction.Create()
		.Keys(Key.Q)
		.Buttons(JoyButton.LeftShoulder)
		.Build();

	[InputAction(SaveAs = "Controls/Float")]
	private InputAction Float => InputAction.Create()
		.Keys(Key.F)
		.Buttons(JoyButton.Y)
		.Build();
}
