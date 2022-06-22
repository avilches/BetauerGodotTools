using System.Linq;
using Godot;

namespace Betauer.UI {
    public static partial class ContainerExtensions {
        /// <summary>
        /// It loops the children (only the buttons) and fixes all the FocusNeighbour* fields, taking into account
        /// if any button is disabled. It tries to find (and returns) the right button to focus, which will be always a
        /// non-disabled button. The focused button could be the <paramref name="focused">focused</paramref> parameter
        /// or, if the parameter is null, the already focused button.
        /// <code>
        /// container.RefreshNeighbours()?.GrabFocus();
        /// </code>
        /// </summary>
        /// <param name="focused">The button to focus. If disabled, it focuses the next non disabled</param>
        /// <param name="wrapButtons">Link the last button with the first and vice-versa. True by default</param>
        /// <returns>The focused button</returns>
        ///
        public static BaseButton? RefreshNeighbours(this Container container, BaseButton? focused = null,
            bool wrapButtons = true) {
            BaseButton? first = null;
            BaseButton? last = null;
            BaseButton? previous = null;
            var takeNextFocus = false;
            foreach (var button in container.GetChildren().OfType<Button>()) {
                var isDisabled = button.Disabled;

                if (focused == null && (button.HasFocus() || takeNextFocus)) {
                    // Try to find the first not disabled focused control
                    if (isDisabled) takeNextFocus = true;
                    else focused = button;
                }

                button.FocusMode = isDisabled ? Control.FocusModeEnum.None : Control.FocusModeEnum.All;

                if (previous != null) {
                    if (container is VBoxContainer) {
                        previous.FocusNeighbourBottom = "../" + button.Name;
                        button.FocusNeighbourTop = "../" + previous.Name;
                    } else if (container is HBoxContainer) {
                        previous.FocusNeighbourRight = "../" + button.Name;
                        button.FocusNeighbourLeft = "../" + previous.Name;
                    }
                }
                first ??= button;
                previous = button;
            }
            last = previous;

            if (wrapButtons && first != null && last != null && first != last) {
                if (container is VBoxContainer) {
                    first.FocusNeighbourTop = "../" + last.Name;
                    last.FocusNeighbourBottom = "../" + first.Name;
                } else if (container is HBoxContainer) {
                    first.FocusNeighbourLeft = "../" + last.Name;
                    last.FocusNeighbourRight = "../" + first.Name;
                }
            }
            return focused ?? first;
        }
    }
}