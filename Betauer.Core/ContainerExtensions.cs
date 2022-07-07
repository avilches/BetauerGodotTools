using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer {
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
        
        /// <summary>
        /// Set disabled = true in all children buttons. It returns a MultiRestorer so the previous state before the
        /// change can be recover. 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="storeFocus"></param>
        /// <returns></returns>
        public static MultiRestorer DisableButtons(this Container container, bool storeFocus = true) {
            var buttons = container.GetChildren().OfType<BaseButton>();
            MultiRestorer restorer = new MultiRestorer(buttons, "disabled");
            if (storeFocus) {
                restorer.AddChildFocusRestorer(container);
            }
            restorer.Save();
            foreach (var child in container.GetChildren().OfType<BaseButton>()) {
                if (child is BaseButton button) button.Disabled = true;
            }
            return restorer;
        }

        public static List<T> GetVisibleControl<T>(this Container container) where T : Control{
            if (container is ScrollContainer scrollContainer) {
                var topVisible = scrollContainer.ScrollVertical;
                var bottomVisible = scrollContainer.RectSize.y + scrollContainer.ScrollVertical;
                return scrollContainer.GetChild(0).GetChildren().OfType<T>()
                    .Where(control =>
                        control.RectPosition.y >= topVisible &&
                        control.RectPosition.y + control.RectSize.y <= bottomVisible)
                    .ToList();
            }
            return container.GetChildren<T>();
        }
        
        /// <summary>
        /// Return the focused control in the container (if the focused button belongs to other container, it returns null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetChildFocused<T>(this Container container) where T : Control {
            var globalFocused = container.GetFocusOwner();
            var children = container is ScrollContainer scrollContainer
                ? scrollContainer.GetChild(0).GetChildren()
                : container.GetChildren();
            return globalFocused == null ? null : children.OfType<T>().FirstOrDefault(b => b == globalFocused);
        }

    }
}