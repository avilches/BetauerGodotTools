using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Animation;
using Godot;

namespace Betauer.UI {
    public class MenuController {
        internal static Logger Logger = LoggerFactory.GetLogger(typeof(MenuController));

        private readonly Container _baseHolder;
        private readonly List<ActionMenu> _menus = new List<ActionMenu>();
        private readonly LinkedList<ActionState> _navigationState = new LinkedList<ActionState>();

        public ActionMenu? ActiveMenu { get; private set; } = null;

        public MenuController(Container baseHolder) {
            _baseHolder = baseHolder;
            _baseHolder.DisableAllNotifications();
            baseHolder.Hide();
        }

        public ActionMenu AddMenu(string name, Action<ActionMenu> onShow = null) {
            var menu = new ActionMenu(this, name, _baseHolder, onShow);
            _menus.Add(menu);
            return menu;
        }

        public async Task Start(string name) {
            foreach (var menu in _menus) {
                if (menu.Name == name) {
                    ActiveMenu = menu;
                    menu.Show();
                } else {
                    menu.Hide();
                }
            }
            await _baseHolder.GetTree().AwaitIdleFrame();
            Save();
        }

        public void Save() {
            foreach (var menu in _menus) menu.Save();
        }

        public ActionMenu GetMenu(string toMenuName) {
            return _menus.Find(menu => menu.Name == toMenuName);
        }

        public async Task Go(ActionButton fromButton, string toMenuName, string? toButtonName,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            ActionMenu toMenu = GetMenu(toMenuName);
            _navigationState.AddLast(new ActionState(ActiveMenu, fromButton));

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
            ActionMenu fromMenu = fromButton != null ? fromButton.Menu : ActiveMenu;
            ActionMenu toMenu = lastState.Menu;
            MenuTransition transition = new MenuTransition(fromMenu, fromButton, toMenu, lastState.Button);

            await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
        }


        internal async Task PlayTransition(
            MenuTransition transition, Func<MenuTransition, Task>? goodbyeAnimation,
            Func<MenuTransition, Task>? newMenuAnimation) {
            var viewport = transition.FromMenu.Container.GetTree().Root;
            try {
                viewport.GuiDisableInput = true;
                if (goodbyeAnimation != null) {
                    try {
                        await goodbyeAnimation(transition);
                    } catch (Exception e) {
                        Logger.Error(e);
                    }
                }
                transition.FromMenu.Hide();
                transition.FromMenu.Restore();

                transition.ToMenu.Restore();
                transition.ToMenu.Show(transition.ToButton);
                if (newMenuAnimation != null) {
                    try {
                        await newMenuAnimation(transition);
                    } catch (Exception e) {
                        Logger.Error(e);
                    }
                }
                ActiveMenu = transition.ToMenu;
            } finally {
                viewport.GuiDisableInput = false;
            }
        }
    }


    public class ActionMenu {
        private readonly Action<ActionMenu> _onShow;
        private readonly Restorer _saver;
        internal readonly MenuController MenuController;

        public readonly string Name;
        public Container Container { get; }
        public bool WrapButtons { get; set; } = true;

        internal ActionMenu(MenuController menuController, string name, Container baseHolder,
            Action<ActionMenu> onShow) {
            MenuController = menuController;
            Name = name;
            Container = baseHolder.Duplicate() as Container;
            _saver = Container.CreateRestorer();
            _onShow = onShow;
            baseHolder.GetParent().AddChildBelowNode(baseHolder, Container);
        }

        public ActionMenu Save() {
            _saver.Save();
            foreach (var button in Container.GetChildren())
                if (button is IActionControl control) control.Save();
            return this;
        }

        public ActionMenu Restore() {
            _saver.Restore();
            foreach (var button in Container.GetChildren())
                if (button is IActionControl control) control.Restore();
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

        public ActionHSeparator CreateHSeparator() {
            ActionHSeparator separator = new ActionHSeparator();
            AddNode(separator);
            return separator;
        }

        public ActionVSeparator CreateVSeparator() {
            ActionVSeparator separator = new ActionVSeparator();
            AddNode(separator);
            return separator;
        }

        public ActionMenu AddNode(Node button) {
            Container.AddChild(button);
            return this;
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

        public ActionMenu AddButton(string name, string title, Action<ActionButton.InputEventContext> action) {
            var button = CreateButton(name, title);
            button.ActionWithInputEvent = action;
            return this;
        }

        public ActionMenu AddHSeparator() {
            CreateHSeparator();
            return this;
        }

        public ActionMenu AddVSeparator() {
            CreateVSeparator();
            return this;
        }

        public Control? GetFocusOwner() {
            return Container.GetFocusOwner();
        }

        public bool IsFocusedAndDisabled() {
            foreach (var child in Container.GetChildren()) {
                if (child is BaseButton { Disabled: true } disabledButton && disabledButton.HasFocus()) return true;
            }
            return false;
        }

        public ActionMenu Refresh(Control? focused = null) {
            Control? first = null;
            Control? last = null;
            Control? previous = null;
            var takeNextFocus = false;
            foreach (var child in Container.GetChildren()) {
                if (child is Control control) {
                    if (control is VSeparator || control is HSeparator) continue;
                    var isDisabled = control is BaseButton { Disabled: true };

                    if (focused == null && (control.HasFocus() || takeNextFocus)) {
                        // Try to find the first not disabled focused control
                        if (isDisabled) takeNextFocus = true;
                        else focused = control;
                    }

                    control.FocusMode = isDisabled ? Control.FocusModeEnum.None : Control.FocusModeEnum.All;

                    if (previous != null) {
                        if (Container is VBoxContainer) {
                            previous.FocusNeighbourBottom = "../" + control.Name;
                            control.FocusNeighbourTop = "../" + previous.Name;
                        } else if (Container is HBoxContainer) {
                            previous.FocusNeighbourRight = "../" + control.Name;
                            control.FocusNeighbourLeft = "../" + previous.Name;
                        }
                    }
                    first ??= control;
                    previous = control;
                }
            }
            last = previous;

            if (WrapButtons && first != null && last != null && first != last) {
                if (Container is VBoxContainer) {
                    first.FocusNeighbourTop = "../" + last.Name;
                    last.FocusNeighbourBottom = "../" + first.Name;
                } else if (Container is HBoxContainer) {
                    first.FocusNeighbourLeft = "../" + last.Name;
                    last.FocusNeighbourRight = "../" + first.Name;
                }
            }
            focused ??= first;
            focused?.GrabFocus();
            return this;
        }

        public IEnumerable GetChildren() {
            return Container.GetChildren();
        }

        public void Hide() {
            Container.Hide();
        }

        public void Show(ActionButton? focused = null) {
            _onShow?.Invoke(this);
            Refresh(focused);
            Container.Show();
        }

        public ActionButton? GetButton(string name) {
            return GetControl<ActionButton>(name);
        }

        public ActionCheckButton? GetCheckButton(string name) {
            return GetControl<ActionCheckButton>(name);
        }

        public T? GetControl<T>(string name) where T : Control {
            return Container.FindChild<T>(name);
        }
    }


    public interface IActionControl {
        public void Save();
        void Restore();
    }

    public class BaseContext {
        public ActionMenu Menu { get; }
        public BaseContext(ActionMenu menu) => Menu = menu;
        public void Refresh() => Menu.Refresh();
    }

    public class ActionHSeparator : HSeparator, IActionControl {
        private readonly ControlRestorer _saver;
        public ActionHSeparator() => _saver = new ControlRestorer(this);
        public void Save() => _saver.Save();
        public void Restore() => _saver.Restore();
    }

    public class ActionVSeparator : VSeparator, IActionControl {
        private readonly ControlRestorer _saver;
        public ActionVSeparator() => _saver = new ControlRestorer(this);
        public void Save() => _saver.Save();
        public void Restore() => _saver.Restore();
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
                Func<MenuTransition, Task>? goodbyeAnimation = null,
                Func<MenuTransition, Task>? newMenuAnimation = null) {
                await Menu.MenuController.Back(ActionButton, goodbyeAnimation, newMenuAnimation);
            }
        }

        public class InputEventContext : Context {
            public Godot.InputEvent InputEvent { get; }

            public InputEventContext(ActionMenu menu, ActionButton actionButton, Godot.InputEvent @event) : base(menu, actionButton) {
                InputEvent = @event;
            }
        }

        private readonly ControlRestorer _saver;
        public ActionMenu Menu { get; }
        public Action? Action;
        public Action<Context>? ActionWithContext;
        public Action<InputEventContext>? ActionWithInputEvent;

        // TODO: i18n
        internal ActionButton(ActionMenu menu) {
            Menu = menu;
            _saver = new ControlRestorer(this);
            Connect(GodotConstants.GODOT_SIGNAL_pressed, this, nameof(Execute));
        }

        public override void _Input(InputEvent @event) {
            if (ActionWithInputEvent != null && GetFocusOwner() == this) {
                ActionWithInputEvent(new InputEventContext(Menu, this, @event));
            }
        }

        public void Execute() {
            if (ActionWithContext != null) ActionWithContext(new Context(Menu, this));
            else Action?.Invoke();
        }
        public void Save() =>_saver.Save();
        public void Restore() => _saver.Restore();

        private Action _onFocusEntered;
        private void ExecuteOnFocusEntered() => _onFocusEntered?.Invoke();
        public void OnFocusEntered(Action onFocus) {
            Connect(GodotConstants.GODOT_SIGNAL_focus_entered, this, nameof(ExecuteOnFocusEntered));
            _onFocusEntered = onFocus;
        }

        private Action _onFocusExited;
        private void ExecuteOnFocusExited() => _onFocusExited?.Invoke();
        public void OnFocusExited(Action onFocus) {
            Connect(GodotConstants.GODOT_SIGNAL_focus_exited, this, nameof(ExecuteOnFocusExited));
            _onFocusExited = onFocus;
        }
    }

    public class ActionCheckButton : CheckButton, IActionControl {
        public class Context : BaseContext {
            public ActionCheckButton ActionCheckButton { get; }

            public bool Pressed => ActionCheckButton.Pressed;

            public Context(ActionMenu menu, ActionCheckButton actionCheckButton) : base(menu) {
                ActionCheckButton = actionCheckButton;
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
            Connect(GodotConstants.GODOT_SIGNAL_pressed, this, nameof(Execute));
        }

        public void Execute() {
            if (ActionWithContext != null) ActionWithContext(new Context(Menu, this));
            else Action?.Invoke(Pressed);
        }

        public void Save() =>_saver.Save();
        public void Restore() => _saver.Restore();

        private Action _onFocusEntered;
        private void ExecuteOnFocusEntered() => _onFocusEntered?.Invoke();
        public void OnFocusEntered(Action onFocus) {
            Connect(GodotConstants.GODOT_SIGNAL_focus_entered, this, nameof(ExecuteOnFocusEntered));
            _onFocusEntered = onFocus;
        }

        private Action _onFocusExited;
        private void ExecuteOnFocusExited() => _onFocusExited?.Invoke();
        public void OnFocusExited(Action onFocus) {
            Connect(GodotConstants.GODOT_SIGNAL_focus_exited, this, nameof(ExecuteOnFocusExited));
            _onFocusExited = onFocus;
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