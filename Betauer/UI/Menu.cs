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
            var viewport = transition.FromMenu.Parent.GetTree().Root;
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
        private readonly Restorer _saver;
        internal readonly MenuController MenuController;
        public readonly List<Node> Children = new List<Node>();

        public readonly string Name;
        public CanvasItem Parent { get; }
        public bool WrapButtons { get; set; } = true;

        internal ActionMenu(MenuController menuController, string name, CanvasItem baseHolder) {
            MenuController = menuController;
            Name = name;
            Parent = baseHolder.Duplicate() as Control;
            _saver = baseHolder.CreateRestorer();
            baseHolder.GetParent().AddChildBelowNode(baseHolder, Parent);
        }

        public ActionMenu Save() {
            _saver.Save();
            Children.ForEach(button => {
                if (button is IActionControl control)
                    control.Save();
            });
            return this;
        }

        public ActionMenu Restore() {
            _saver.Restore();
            Children.ForEach(button => {
                if (button is IActionControl control)
                    control.Restore();
            });
            return this;
        }

        public ActionMenu AddNode(Node button) {
            Children.Add(button);
            return this;
        }

        public ActionButton CreateButton(string name, string title) {
            ActionButton button = new ActionButton(this);
            button.Name = name;
            button.Text = title;
            AddNode(button);
            return button;
        }

        public ActionCheckButton CreateCheckButton(string name, string title) {
            ActionCheckButton button = new ActionCheckButton(this);
            button.Name = name;
            button.Text = title;
            AddNode(button);
            return button;
        }

        public ActionMenu AddCheckButton(string name, string title, Action<bool> action) {
            var button = CreateCheckButton(name, title);
            button.Action = action;
            return this;
        }

        public ActionMenu AddCheckButton(string name, string title, Action<ActionCheckButton.Context> action) {
            var button = CreateCheckButton(name, title);
            button.ActionWithContext = action;
            return this;
        }

        public ActionMenu AddButton(string name, string title, Action action) {
            var button = CreateButton(name, title);
            button.Action = action;
            return this;
        }

        public ActionMenu AddButton(string name, string title, Action<ActionButton.Context> action) {
            var button = CreateButton(name, title);
            button.ActionWithContext = action;
            return this;
        }

        /*
         * Rebuild the menu ensures disabled buttons are not selectable when using previous-next
         * wrap true = link the first and last buttons
         */
        public async Task<ActionMenu> Refresh(Control? focused = null) {
            Control? first = null;
            Control? last = null;
            var takeNextFocus = false;
            foreach (var child in Parent.GetChildren()) {
                if (focused == null
                    && child is ActionButton control
                    && Children.Contains(control) // if the focused button doesn't belongs to the menu, ignore
                    && (control.HasFocus() || takeNextFocus)) {

                    if (control.Disabled) {
                        takeNextFocus = true;
                    } else {
                        focused = control;
                    }
                }
                Parent.RemoveChild(child as Node);
            }
            foreach (var actionButton in Children) {
                if (actionButton is Control control) {
                    if (control is BaseButton { Disabled: true } button) {
                        button.FocusMode = Control.FocusModeEnum.None;
                        Parent.AddChild(actionButton as Control);
                        last = control;
                    } else {
                        if (first == null) {
                            first = control;
                        }
                        Parent.AddChild(actionButton as Control);
                        last = control;
                        control.FocusMode = Control.FocusModeEnum.All;
                    }
                } else {
                    Parent.AddChild(actionButton);
                }
            }
            if (WrapButtons && first != null && last != null && first != last) {
                if (Parent is VBoxContainer) {
                    first.FocusNeighbourTop = "../" + last.Name;
                    last.FocusNeighbourBottom = "../" + first.Name;
                } else if (Parent is HBoxContainer) {
                    first.FocusNeighbourLeft = "../" + last.Name;
                    last.FocusNeighbourRight = "../" + first.Name;
                }
            }
            focused ??= first;
            if (focused != null) {
                await Parent.GetTree().AwaitIdleFrame();
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
            return Parent.GetChildren();
        }

        public void Hide() {
            Parent.Hide();
        }

        public async Task Show(ActionButton? focused = null) {
            await Refresh(focused);
            Parent.Show();
        }

        public ActionButton? GetButton(string name) {
            return GetControl<ActionButton>(name);
        }

        public ActionCheckButton? GetCheckButton(string name) {
            return GetControl<ActionCheckButton>(name);
        }

        public T? GetControl<T>(string name) where T : Control {
            return (T?)Children.Find(button => button.Name == name && button is T);
        }
    }


    public interface IActionControl {
        public void Save();
        void Restore();
    }

    public class BaseContext {
        public ActionMenu Menu { get; }

        public BaseContext(ActionMenu menu) {
            Menu = menu;
        }

        public async Task Refresh() {
            await Menu.Refresh();
        }
    }

    public class ActionButton : Button, IActionControl {
        public class Context : BaseContext {
            public ActionButton ActionButton { get; }

            public Context(ActionMenu menu, ActionButton actionButton) : base(menu) {
                ActionButton = actionButton;
            }

            public async Task Go(string toMenuName,
                Func<MenuTransition, Task>? goodbyeAnimation = null,
                Func<MenuTransition, Task>? newMenuAnimation = null) {
                await Go(toMenuName, null, goodbyeAnimation, newMenuAnimation);
            }

            public async Task Go(string toMenuName, string? toButtonName,
                Func<MenuTransition, Task>? goodbyeAnimation = null,
                Func<MenuTransition, Task>? newMenuAnimation = null) {
                await Menu.MenuController.Go(ActionButton, toMenuName, toButtonName, goodbyeAnimation,
                    newMenuAnimation);
            }

            public async Task Back(
                Func<MenuTransition, Task>? goodbyeAnimation,
                Func<MenuTransition, Task>? newMenuAnimation) {
                await Menu.MenuController.Back(ActionButton, goodbyeAnimation, newMenuAnimation);
            }

            public async Task Refresh() {
                await Menu.Refresh();
            }
        }

        private readonly ControlRestorer _saver;
        public ActionMenu Menu { get; }
        public Action? Action;
        public Action<Context>? ActionWithContext;

        // TODO: i18n
        internal ActionButton(ActionMenu menu) {
            Menu = menu;
            _saver = new ControlRestorer(this);
        }

        public override void _Pressed() {
            if (ActionWithContext != null) {
                ActionWithContext(new Context(Menu, this));
            } else {
                Action?.Invoke();
            }
        }

        public void Execute() {
            _Pressed();
        }

        public void Save() {
            _saver.Save();
        }

        public void Restore() {
            _saver.Restore();
        }
    }

    public class ActionCheckButton : CheckButton, IActionControl {
        public class Context : BaseContext {
            public ActionCheckButton ActionCheckButton { get; }

            public Context(ActionMenu menu, ActionCheckButton actionCheckButton) : base(menu) {
                ActionCheckButton = actionCheckButton;
            }

            public async Task Refresh() {
                await Menu.Refresh();
            }
        }

        private readonly ControlRestorer _saver;
        public ActionMenu Menu { get; }
        public Action<bool>? Action;
        public Action<Context>? ActionWithContext;

        // TODO: i18n
        internal ActionCheckButton(ActionMenu menu) {
            Menu = menu;
            _saver = new ControlRestorer(this);
        }

        public override void _Toggled(bool buttonPressed) {
            if (ActionWithContext != null) {
                ActionWithContext(new Context(Menu, this));
            } else {
                Action?.Invoke(buttonPressed);
            }
        }

        public void Execute() {
            _Pressed();
        }

        public void Save() {
            _saver.Save();
        }

        public void Restore() {
            _saver.Restore();
        }
    }

    public class MenuTransition {
        public readonly ActionMenu FromMenu;
        public readonly ActionButton? FromButton;
        public readonly ActionMenu ToMenu;
        public readonly ActionButton? ToButton;

        public MenuTransition(ActionMenu fromMenu, ActionButton? fromButton, ActionMenu toMenu,
            ActionButton? toButton) {
            FromMenu = fromMenu;
            FromButton = fromButton;
            ToMenu = toMenu;
            ToButton = toButton;
        }
    }

    internal class ActionState {
        internal readonly ActionMenu Menu;
        internal readonly ActionButton Button;

        internal ActionState(ActionMenu menu, ActionButton button) {
            Menu = menu;
            Button = button;
        }
    }
}