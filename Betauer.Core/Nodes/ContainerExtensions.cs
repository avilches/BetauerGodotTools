using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Restorer;
using Godot;

namespace Betauer.Core.Nodes {
    public static partial class ContainerExtensions {
        /// <summary>
        /// It loops the children (only the buttons) and fixes all the FocusNeighbor* fields, taking into account
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
                        previous.FocusNeighborBottom = "../" + button.Name;
                        button.FocusNeighborTop = "../" + previous.Name;
                    } else if (container is HBoxContainer) {
                        previous.FocusNeighborRight = "../" + button.Name;
                        button.FocusNeighborLeft = "../" + previous.Name;
                    }
                }
                first ??= button;
                previous = button;
            }
            last = previous;

            if (wrapButtons && first != null && last != null && first != last) {
                if (container is VBoxContainer) {
                    first.FocusNeighborTop = "../" + last.Name;
                    last.FocusNeighborBottom = "../" + first.Name;
                } else if (container is HBoxContainer) {
                    first.FocusNeighborLeft = "../" + last.Name;
                    last.FocusNeighborRight = "../" + first.Name;
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
            MultiRestorer restorer = buttons.CreateRestorer("disabled", "focus_mode");
            if (storeFocus) {
                restorer.Add(container.CreateChildFocusedRestorer());
            }
            restorer.Save();
            container.GetChildren().OfType<BaseButton>().ForEach(button => button.SetFocusDisabled(true));
            return restorer;
        }

        public static List<T> GetVisibleControl<T>(this Container container) where T : Control{
            if (container is ScrollContainer scrollContainer) {
                var topVisible = scrollContainer.ScrollVertical;
                var bottomVisible = scrollContainer.Size.y + scrollContainer.ScrollVertical;
                return scrollContainer.GetChild(0).GetChildren().OfType<T>()
                    .Where(control =>
                        control.Position.y >= topVisible &&
                        control.Position.y + control.Size.y <= bottomVisible)
                    .ToList();
            }
            return container.GetChildren().OfType<T>().ToList();
        }
        
        /// <summary>
        /// Return the focused control in the container (if the focused button belongs to other container, it returns null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetChildFocused<T>(this Container container) where T : Control {
            Control globalFocused = container.GetViewport().GuiGetFocusOwner();
            var children = container is ScrollContainer scrollContainer
                ? scrollContainer.GetChild(0).GetChildren()
                : container.GetChildren();
            return globalFocused == null ? null : children.OfType<T>().FirstOrDefault(b => b == globalFocused);
        }

        public static MarginContainer SetMargin(this MarginContainer margin, int top, int right, int bottom, int left) {
            margin.AddThemeConstantOverride("margin_top", top);
            margin.AddThemeConstantOverride("margin_right", right);
            margin.AddThemeConstantOverride("margin_bottom", bottom);
            margin.AddThemeConstantOverride("margin_left", left);
            return margin;
        }

    }
}