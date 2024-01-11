using Betauer.DI.Attributes;
using Betauer.Input;
using Betauer.Input.Joypad;
using Godot;

namespace Veronenger.Game.Platform.Character.InputActions;

[Singleton]
public class PlatformMultiPlayerContainer : MultiPlayerContainer<PlatformPlayerActions> {
}

public class PlatformPlayerActions : PlayerActionsContainer {

    public AxisAction Lateral { get; } = AxisAction.Create("Lateral").SaveAs("Controls/Lateral").Build();

    public AxisAction Vertical { get; } = AxisAction.Create("Vertical").SaveAs("Controls/Vertical").Build();

    public InputAction Up { get; } = InputAction.Create("Up")
        .AxisName("Vertical")
        .SaveAs("Controls/Up")
        .Keys(Key.Up)
        .Buttons(JoyButton.DpadUp)
        .NegativeAxis(JoyAxis.LeftY)
        .DeadZone(0.5f)
        .Build();

    public InputAction Down { get; } = InputAction.Create("Down")
        .AxisName("Vertical")
        .SaveAs("Controls/Down")
        .Keys(Key.Down)
        .Buttons(JoyButton.DpadDown)
        .PositiveAxis(JoyAxis.LeftY)
        .DeadZone(0.5f)
        .Build();

    public InputAction Left { get; } = InputAction.Create("Left")
        .AxisName("Lateral")
        .SaveAs("Controls/Left")
        .Keys(Key.Left)
        .Buttons(JoyButton.DpadLeft)
        .NegativeAxis(JoyAxis.LeftX)
        .DeadZone(0.5f)
        .Build();

    public InputAction Right { get; } = InputAction.Create("Right")
        .AxisName("Lateral")
        .SaveAs("Controls/Right")
        .Keys(Key.Right)
        .Buttons(JoyButton.DpadRight)
        .PositiveAxis(JoyAxis.LeftX)
        .DeadZone(0.5f)
        .Build();

    public InputAction Jump { get; } = InputAction.Create("Jump")
        .SaveAs("Controls/Jump")
        .Keys(Key.Space)
        .Buttons(JoyButton.A)
        .Build(true);

    public InputAction Attack { get; } = InputAction.Create("Attack")
        .SaveAs("Controls/Attack")
        .Keys(Key.C)
        .Click(MouseButton.Left)
        .Buttons(JoyButton.B)
        .Build();

    public InputAction Drop { get; } = InputAction.Create("Drop")
        .SaveAs("Controls/Drop")
        .Keys(Key.G)
        .Build();

    public InputAction NextItem { get; } = InputAction.Create("NextItem")
        .SaveAs("Controls/NextItem")
        .Keys(Key.E)
        .Buttons(JoyButton.RightShoulder)
        .Build();

    public InputAction PrevItem { get; } = InputAction.Create("PrevItem")
        .SaveAs("Controls/PrevItem")
        .Keys(Key.Q)
        .Buttons(JoyButton.LeftShoulder)
        .Build();

    public InputAction Float { get; } = InputAction.Create("Float")
        .SaveAs("Controls/Float")
        .Keys(Key.F)
        .Buttons(JoyButton.Y)
        .Build();
}