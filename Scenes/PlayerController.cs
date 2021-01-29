using System;
using System.Collections.Generic;
using Godot;
using Veronenger.tools.input;

public class PlayerController : CharacterController {
    private StateIdle idle;
    private StateRun run;
    public bool IsUsingKeyboard = true;
    public readonly DirectionInput lateralMotion;
    public PlayerConfig playerConfig => (PlayerConfig) CharacterConfig;
    private readonly StateMachine _stateMachine;
    private readonly ActionInputList _actionInputList;


    public readonly ActionState Jump;
    public readonly ActionState Attack;
    public PlayerController() {
        CharacterConfig = new PlayerConfig();

        // State Machine
        _stateMachine = new StateMachine(playerConfig);
        idle = new StateIdle(this);
        run = new StateRun(this);

        // Mapping
        _actionInputList = new ActionInputList(this);
        Jump = _actionInputList.AddAction("Jump");
        Attack = _actionInputList.AddAction("Attack");
        lateralMotion = _actionInputList.AddDirectionalMotion("Lateral");
        ConfigureMapping();
    }


    public void ConfigureMapping() {
        // TODO: subscribe to signal with the mapping preferences on load or on change
        lateralMotion.ConfigureDefaults();
        lateralMotion.AxisDeadZone = 0.5f;

        Jump.Configure(KeyList.Space, JoystickList.XboxA);
        Attack.Configure(KeyList.C, JoystickList.XboxX);
    }

    public override void _EnterTree() {
        _stateMachine.SetNextState(idle);
    }

    protected override void PhysicsProcess() {
        _stateMachine.Execute();
        _actionInputList.ClearJustState();
    }

    private EventWrapper w = new EventWrapper(null);

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is InputEventJoypadMotion joypadMotion) {
            // GD.Print("Axis " + joypadMotion.Device + "[" + joypadMotion.Axis + "]:" + joypadMotion.AxisValue+ " "+joypadMotion.IsActionType());
        } else if (@event is InputEventJoypadButton joypadButton) {
            GD.Print("Button " + joypadButton.Device + "[" + joypadButton.ButtonIndex + "]:" + joypadButton.Pressed +
                     " " + joypadButton.Pressure);
        } else if (@event is InputEventKey eventKey) {
            // GD.Print(eventKey.GetType().FullName+" - "+eventKey.IsActionPressed("ui_right", true)+":"+eventKey.IsActionReleased("ui_right")+":"+eventKey.GetActionStrength("ui_right") + " / "+eventKey.IsActionPressed("ui_left", true) +":"+eventKey.IsActionReleased("ui_left") +":"+eventKey.GetActionStrength("ui_left"));
            // GD.Print(eventKey.Pressed+"/"+eventKey.Echo+" "+eventKey.Scancode+" "+eventKey.IsAction());
        } else {
            // GD.Print(@event.Device + "[" + @event.AsText() + "] pressed:" + @event.IsPressed() + " type:" +
            // @event.IsActionType());
        }

        // if (eventKey.Pressed && eventKey.Scancode == (int) KeyList.Escape)
        // GetTree().Quit();
        w.@event = @event;
        if (!_actionInputList.Update(w)) {
            _stateMachine._UnhandledInput(@event);
        }
    }


    public void GoToRunState() {
        // Change to run is immediate
        _stateMachine.ChangeStateTo(run);
    }

    public void GoToIdleState() {
        // Idle is deferred to the next frame
        _stateMachine.SetNextState(idle);
    }

    public void PrintActionMap() {
        // foreach (string actionName in InputMap.GetActions()) {
        // InputMap.EraseAction(actionName);
        // }
        foreach (string actionName in InputMap.GetActions()) {
            foreach (InputEvent action in InputMap.GetActionList(actionName)) {
                var message = actionName + ">" + action.GetType() + ": " + action.Device + " " + action.AsText() +
                              " > ";
                if (action is InputEventKey k) {
                    message += k.Scancode + "(" + OS.GetScancodeString(k.Scancode) + ")";
                } else if (action is InputEventJoypadButton b) {
                    message += b.ButtonIndex;
                } else if (action is InputEventJoypadMotion m) {
                    message += m.Axis;
                }

                GD.Print(message);
            }
        }
    }
}