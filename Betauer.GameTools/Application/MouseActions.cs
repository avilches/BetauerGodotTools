using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Betauer.Application;

[Configuration]
public class MouseActions {
    [Service]
    private InputAction LMB => InputAction.Create("LMB")
        .Click(MouseButton.Left)
        .Build();

    [Service]
    private InputAction MMB => InputAction.Create("MMB")
        .Click(MouseButton.Middle)
        .Build();

    [Service]
    private InputAction RMB => InputAction.Create("RMB")
        .Click(MouseButton.Right)
        .Build();

    [Service]
    private InputAction MWU => InputAction.Create("MWU")
        .Click(MouseButton.WheelUp)
        .Build();

    [Service]
    private InputAction MWL => InputAction.Create("MWL")
        .Click(MouseButton.WheelLeft)
        .Build();

    [Service]
    private InputAction MWR => InputAction.Create("MWR")
        .Click(MouseButton.WheelRight)
        .Build();

    [Service]
    private InputAction MWD => InputAction.Create("MWD")
        .Click(MouseButton.WheelDown)
        .Build();
}