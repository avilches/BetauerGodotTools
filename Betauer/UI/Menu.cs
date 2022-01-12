using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Animation;
using Godot;

namespace Betauer.UI {
    public class MenuController {
        private readonly Control _baseHolder;
        private readonly List<ActionMenu> _menus = new List<ActionMenu>();

        private readonly LinkedList<ActionState> _navigationState = new LinkedList<ActionState>();
        private ActionMenu _activeMenu = null;

        public MenuController(Control baseHolder) {
            _baseHolder = baseHolder;
            _baseHolder.DisableAllNotifications();
            baseHolder.Hide();
        }

        public ActionMenu AddMenu(string name) {
            var menu = new ActionMenu(this, name, _baseHolder);
            _menus.Add(menu);
            return menu;
        }

        public async Task Show(string name) {
            foreach (var menu in _menus) {
                if (menu.Name == name) {
                    _activeMenu = menu;
                    await menu.Show();
                } else {
                    menu.Hide();
                }
            }
        }

        public ActionMenu GetMenu(string toMenuName) {
            return _menus.Find(menu => menu.Name == toMenuName);
        }

        public async Task Go(ActionButton fromButton, string toMenuName, string? toButtonName,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            ActionMenu toMenu = GetMenu(toMenuName);
            _navigationState.AddLast(new ActionState(_activeMenu, fromButton));

            ActionButton? toButton = toButtonName != null ? toMenu.GetButton(toButtonName) : null;
            MenuTransition transition = new MenuTransition(fromButton.Menu, fromButton, toMenu, toButton);

            await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
        }

        public async Task Back(
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            await Back(null, goodbyeAnimation, newMenuAnimation);
        }

        public async Task Back(ActionButton? fromButton,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (_navigationState.Count == 0) {
                // back from root menu!
                return;
            }
            ActionState lastState = _navigationState.Last();
            _navigationState.RemoveLast();
            ActionMenu fromMenu = fromButton != null ? fromButton.Menu : _activeMenu;
            ActionMenu toMenu = lastState.Menu;
            MenuTransition transition = new MenuTransition(fromMenu, fromButton, toMenu, lastState.Button);

            await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
        }


        internal async Task PlayTransition(
            MenuTransition transition, Func<MenuTransition, Task>? goodbyeAnimation,
            Func<MenuTransition, Task>? newMenuAnimation) {
            var viewport = transition.FromMenu.Control.GetTree().Root;
            try {
                viewport.GuiDisableInput = true;
                if (goodbyeAnimation != null) {
                    await goodbyeAnimation(transition);
                }
                transition.FromMenu.Hide();

                transition.ToMenu.Restore();
                await transition.ToMenu.Show(transition.ToButton);
                if (newMenuAnimation != null) {
                    await newMenuAnimation(transition);
                }
                _activeMenu = transition.ToMenu;
            } finally {
                viewport.GuiDisableInput = false;
            }
        }
    }


    public class ActionMenu {
        private readonly ControlRestorer _saver;
        internal readonly MenuController MenuController;
        public readonly List<ActionButton> Buttons = new List<ActionButton>();

        public readonly string Name;
        public Control Control { get; }
        public bool WrapButtons { get; set; }

        internal ActionMenu(MenuController menuController, string name, Control baseHolder) {
            MenuController = menuController;
            Name = name;
            Control = baseHolder.Duplicate() as Control;
            _saver = new ControlRestorer(Control);
            baseHolder.GetParent().AddChildBelowNode(baseHolder, Control);
        }

        public ActionMenu Save() {
            _saver.Save();
            Buttons.ForEach(button => button.Save());
            return this;
        }

        public ActionMenu Restore() {
            _saver.Restore();
            Buttons.ForEach(button => button.Restore());
            return this;
        }

        public ActionButton CreateButton(string name, string title) {
            ActionButton button = new ActionButton(this);
            button.Name = name;
            button.Text = title;
            Buttons.Add(button);
            return button;
        }

        public ActionMenu AddButton(string name, string title, Action action) {
            var button = CreateButton(name, title);
            button.Action = action;
            return this;
        }

        public ActionMenu AddButton(string name, string title, Action<ActionButton> action) {
            var button = CreateButton(name, title);
            button.ActionWithButton = action;
            return this;
        }

        /*
         * Rebuild the menu ensures disabled buttons are not selectable when using previous-next
         * wrap true = link the first and last buttons
         */
        public async Task<ActionMenu> Refresh(ActionButton? focused = null) {
            ActionButton? first = null;
            ActionButton? last = null;
            var takeNextFocus = false;
            foreach (var child in Control.GetChildren()) {
                if (focused == null && child is ActionButton control
                                    && Buttons.Contains(
                                        control) // if the focused button doesn't belongs to the menu, ignore
                                    && (control.HasFocus() || takeNextFocus)) {
                    if (control.Disabled) {
                        takeNextFocus = true;
                    } else {
                        focused = control;
                    }
                }
                Control.RemoveChild(child as Node);
            }
            foreach (var actionButton in Buttons) {
                if (actionButton.Disabled) {
                    actionButton.FocusMode = Control.FocusModeEnum.None;
                    Control.AddChild(actionButton);
                    last = actionButton;
                } else {
                    if (first == null) {
                        first = actionButton;
                    }
                    Control.AddChild(actionButton);
                    last = actionButton;
                    actionButton.FocusMode = Control.FocusModeEnum.All;
                }
            }
            if (WrapButtons && first != null && last != null && first != last) {
                if (Control is VBoxContainer) {
                    first.FocusNeighbourTop = "../" + last.Name;
                    last.FocusNeighbourBottom = "../" + first.Name;
                } else if (Control is HBoxContainer) {
                    first.FocusNeighbourLeft = "../" + last.Name;
                    last.FocusNeighbourRight = "../" + first.Name;
                }
            }
            focused ??= first;
            if (focused != null) {
                await Control.GetTree().AwaitIdleFrame();
                focused.GrabFocus();
            }
            return this;
        }

        /*
        public async Task Focus(ActionButton button) {
            await Control.GetTree().AwaitIdleFrame();
            button.GrabFocus();
        }

        public void FocusFirst() {
            foreach (var child in Control.GetChildren()) {
                if (child is ActionButton { Disabled: false } action) {
                    GD.Print("Setting focus on first button " + action.Text);
                    action.GrabFocus();
                    return;
                }
            }
        }
        */

        public IEnumerable GetButtons() {
            return Control.GetChildren();
        }

        public void Hide() {
            Control.Hide();
        }

        public async Task Show(ActionButton? focused = null) {
            await Refresh(focused);
            Control.Show();
        }

        public ActionButton? GetButton(string name) {
            return Buttons.Find(button => button.Name == name);
        }
    }


    public class ActionButton : DiButton {
        private readonly ControlRestorer _saver;
        public readonly ActionMenu Menu;
        public Action? Action;
        public Action<ActionButton>? ActionWithButton;

        // TODO: i18n
        internal ActionButton(ActionMenu menu) {
            Menu = menu;
            _saver = new ControlRestorer(this);
        }

        public override void Ready() {
            Connect(GodotConstants.GODOT_SIGNAL_pressed, this, nameof(_Pressed));
        }

        public void _Pressed() {
            if (ActionWithButton != null) {
                ActionWithButton(this);
            } else {
                Action?.Invoke();
            }
        }

        public void Execute() {
            _Pressed();
        }

        public async Task Refresh() {
            await Menu.Refresh();
        }


        public void Save() {
            _saver.Save();
        }

        public void Restore() {
            _saver.Restore();
        }


        public async Task Go(string toMenuName,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            await Go(toMenuName, null, goodbyeAnimation, newMenuAnimation);
        }

        public async Task Go(string toMenuName, string? toButtonName,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            await Menu.MenuController.Go(this, toMenuName, toButtonName, goodbyeAnimation, newMenuAnimation);
        }

        public async Task Back(
            Func<MenuTransition, Task>? goodbyeAnimation,
            Func<MenuTransition, Task>? newMenuAnimation) {
            await Menu.MenuController.Back(this, goodbyeAnimation, newMenuAnimation);
        }
    }

    public class MenuTransition {
        public readonly ActionMenu FromMenu;
        public readonly ActionButton? FromButton;
        public readonly ActionMenu ToMenu;
        public readonly ActionButton? ToButton;

        public MenuTransition(ActionMenu fromMenu, ActionButton? fromButton, ActionMenu toMenu, ActionButton? toButton) {
            FromMenu = fromMenu;
            FromButton = fromButton;
            ToMenu = toMenu;
            ToButton = toButton;
        }
    }

    internal struct ActionState {
        internal ActionMenu Menu;
        internal ActionButton Button;

        internal ActionState(ActionMenu menu, ActionButton button) {
            Menu = menu;
            Button = button;
        }
    }
}