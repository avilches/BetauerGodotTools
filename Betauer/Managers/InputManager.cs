using System.Collections.Generic;
using System.Linq;
using Betauer.DI;
using Betauer.Input;
using Godot;

namespace Betauer.Managers {
    public class InputManager {

        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(InputManager));

        public void Debug(InputEvent e, bool actionsOnly = true) {
            /*
            var action = new ActionState("a"); // FindAction(e);
            string actionName = null;
            switch (action) {
                case ActionState state:
                    actionName = state.Name;
                    break;
                case DirectionalAction directional:
                    // TODO: move this code to DirectionalAction, and create a IsPressed(e) and IsReleased(e)
                    if (directional.Strength == 0f) {
                        actionName = e.IsActionReleased(directional.NegativeName)
                            ? directional.NegativeName
                            : directional.PositiveName;
                    } else {
                        actionName = directional.Strength < 0 ? directional.NegativeName : directional.PositiveName;
                    }
                    break;
            }
            if (actionName == null && actionsOnly) return;
            var wrapper = new EventWrapper(e);
            if (wrapper.IsMotion()) {
                Logger.Debug(
                    $"Axis {wrapper.Device}[{wrapper.Axis}]:{wrapper.GetStrength()} ({wrapper.AxisValue}) {actionName}");
            } else if (wrapper.IsAnyButton()) {
                Logger.Debug(
                    $"Button {wrapper.Device}[{wrapper.Button}]:{wrapper.Pressed} ({wrapper.Pressure}) {actionName}");
            } else if (wrapper.IsAnyKey()) {
                Logger.Debug(
                    $"Key \"{wrapper.KeyString}\" #{wrapper.Key} Pressed:{wrapper.Pressed}/Echo:{wrapper.Echo} {actionName}");
            }
        */
        }
    }
}