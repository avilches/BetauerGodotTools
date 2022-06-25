using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.UI {
    public class MenuController {
        private readonly Container _baseHolder;
        private readonly Node _baseParent;
        private readonly List<ActionMenu> _menus = new List<ActionMenu>();
        private readonly LinkedList<MenuState> _navigationState = new LinkedList<MenuState>();

        public ActionMenu? ActiveMenu { get; private set; } = null;

        public bool Available { get; private set; } = false;

        public MenuController(Container baseHolder) {
            _baseParent = baseHolder.GetParent();
            _baseHolder = baseHolder;
            _baseParent.RemoveChild(_baseHolder);
            _baseHolder.DisableAllNotifications();
            _baseHolder.Hide();
        }

        public ActionMenu AddMenu(string name, Action<ActionMenu> onShow = null) {
            var menu = new ActionMenu(this, name, _baseHolder, _baseParent, onShow);
            _menus.Add(menu);
            return menu;
        }

        public async Task Start(string? name = null) {
            ActiveMenu = name != null ? _menus.Find(menu => menu.Name == name) : _menus.First();
            ActiveMenu.Show();
            await _baseParent.GetTree().AwaitIdleFrame();
            Save();
            Available = true;
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
            if (!Available) return;
            try {
                Available = false;
                ActionMenu toMenu = GetMenu(toMenuName);
                _navigationState.AddLast(new MenuState(ActiveMenu, fromButton));

                ActionButton? toButton = toButtonName != null ? toMenu.GetButton(toButtonName) : null;
                MenuTransition transition = new MenuTransition(fromButton.Menu, fromButton, toMenu, toButton);

                await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
            } finally {
                Available = true;
            }
        }

        public async Task Back(
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            await Back(null, goodbyeAnimation, newMenuAnimation);
        }

        public async Task Back(ActionButton? fromButton,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (!Available || _navigationState.Count == 0) return;
            try {
                Available = false;
                MenuState lastState = _navigationState.Last();
                _navigationState.RemoveLast();
                ActionMenu fromMenu = fromButton != null ? fromButton.Menu : ActiveMenu!;
                ActionMenu toMenu = lastState.Menu;
                MenuTransition transition = new MenuTransition(fromMenu, fromButton, toMenu, lastState.Button);

                await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
            } finally {
                Available = true;
            }
        }


        private async Task PlayTransition(MenuTransition transition, 
            Func<MenuTransition, Task>? goodbyeAnimation,
            Func<MenuTransition, Task>? newMenuAnimation) {
            var viewport = transition.FromMenu.Container.GetTree().Root;
            try {
                viewport.GuiDisableInput = true;
                if (goodbyeAnimation != null) {
                    await goodbyeAnimation(transition);
                }
                transition.FromMenu.Hide();
                transition.FromMenu.Restore();

                transition.ToMenu.Restore();
                transition.ToMenu.Show(transition.ToButton);
                if (newMenuAnimation != null) {
                    await newMenuAnimation(transition);
                }
                ActiveMenu = transition.ToMenu;
            } finally {
                viewport.GuiDisableInput = false;
            }
        }
    }


    public class ActionMenu {
        private readonly Action<ActionMenu> _onShow;
        private Restorer _saver;
        private readonly Node _baseParent;
        internal readonly MenuController MenuController;

        public readonly string Name;
        public Container Container { get; }
        public bool WrapButtons { get; set; } = true;

        internal ActionMenu(MenuController menuController, string name, Container baseHolder,
            Node baseParent, Action<ActionMenu> onShow) {
            MenuController = menuController;
            Name = name;
            Container = (Container)baseHolder.Duplicate();
            _baseParent = baseParent;
            _saver = Container.CreateRestorer();
            _onShow = onShow;
        }

        private Control? _savedFocus;
        public ActionMenu Save() {
            _saver = new MultiRestorer(Container.GetChildren<Node>()).Save();
            return this;
        }
        
        public ActionMenu SaveFocus() {
            _savedFocus = GetFocusOwner();
            return this;
        }

        public ActionMenu Restore() {
            _saver.Restore();
            return this;
        }
        
        public ActionMenu RestoreFocus() {
            _savedFocus?.GrabFocus();
            _savedFocus = null;
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

        public ActionMenu AddCheckButton(string name, string title, Func<ActionCheckButton.InputEventContext, bool> action) {
            var button = CreateCheckButton(name, title);
            button.ActionWithInputEventContext = action;
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

        public ActionMenu AddButton(string name, string title, Func<ActionButton.InputEventContext, bool> action) {
            var button = CreateButton(name, title);
            button.ActionWithInputEventContext = action;
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

        public void DisableButtons() {
            foreach (var child in Container.GetChildren()) {
                if (child is BaseButton button) button.Disabled = true;
            }
        }

        public ActionMenu Refresh(BaseButton? focused = null) {
            focused = Container.RefreshNeighbours(focused, WrapButtons);
            focused?.GrabFocus();
            return this;
        }

        public Godot.Collections.Array GetChildren() {
            return Container.GetChildren();
        }

        public void Hide() {
            _baseParent.RemoveChild(Container);
        }

        public void Show(ActionButton? focused = null) {
            _onShow?.Invoke(this);
            _baseParent.AddChild(Container);
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
            return Container.GetNode<T>(name) ?? throw new NullReferenceException();
        }
    }


    public interface IActionControl {
    }

    public class BaseContext {
        public ActionMenu Menu { get; }
        public BaseContext(ActionMenu menu) => Menu = menu;
        public void Refresh() => Menu.Refresh();
    }

    public class ActionHSeparator : HSeparator, IActionControl {
    }

    public class ActionVSeparator : VSeparator, IActionControl {
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
            public InputEvent InputEvent { get; }

            public InputEventContext(ActionMenu menu, ActionButton actionButton, InputEvent @event) : base(menu, actionButton) {
                InputEvent = @event;
            }
        }

        public ActionMenu Menu { get; }
        public Action? Action;
        public Action<Context>? ActionWithContext;
        public Func<InputEventContext, bool>? ActionWithInputEventContext;

        // TODO: this class should extends ButtonWrapper

        // TODO: i18n
        internal ActionButton(ActionMenu menu) {
            Menu = menu;
            Connect(SignalConstants.BaseButton_PressedSignal, this, nameof(Execute));
        }

        public override void _Input(InputEvent @event) {
            // It takes into account if the Root.GuiDisableInput = true
            if (ActionWithInputEventContext != null && GetFocusOwner() == this) {
                if (ActionWithInputEventContext(new InputEventContext(Menu, this, @event))) {
                    GetTree().SetInputAsHandled();
                }
            }
        }

        public void Execute() {
            if (ActionWithContext != null) ActionWithContext(new Context(Menu, this));
            else Action?.Invoke();
        }

        private Action _onFocusEntered;
        private void ExecuteOnFocusEntered() => _onFocusEntered?.Invoke();
        public void OnFocusEntered(Action onFocus) {
            Connect(SignalConstants.Control_FocusEnteredSignal, this, nameof(ExecuteOnFocusEntered));
            _onFocusEntered = onFocus;
        }

        private Action _onFocusExited;
        private void ExecuteOnFocusExited() => _onFocusExited?.Invoke();
        public void OnFocusExited(Action onFocus) {
            Connect(SignalConstants.Control_FocusExitedSignal, this, nameof(ExecuteOnFocusExited));
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

        public class InputEventContext : Context {
            public InputEvent InputEvent { get; }

            public InputEventContext(ActionMenu menu, ActionCheckButton actionCheckButton, InputEvent @event) : base(menu, actionCheckButton) {
                InputEvent = @event;
            }
        }


        public ActionMenu Menu { get; }
        public Action<bool>? Action;
        public Action<Context>? ActionWithContext;
        public Func<InputEventContext, bool>? ActionWithInputEventContext;
        private Action _onFocusEntered;
        private Action _onFocusExited;
        
        // TODO: this class should extends ButtonWrapper

        // TODO: i18n
        internal ActionCheckButton(ActionMenu menu) {
            Menu = menu;
            Connect(SignalConstants.BaseButton_PressedSignal, this, nameof(Execute));
        }

        public override void _Input(InputEvent @event) {
            // It takes into account if the Root.GuiDisableInput = true
            if (ActionWithInputEventContext != null && GetFocusOwner() == this) {
                if (ActionWithInputEventContext(new InputEventContext(Menu, this, @event))) {
                    GetTree().SetInputAsHandled();
                }
            }
        }

        public void Execute() {
            if (ActionWithContext != null) ActionWithContext(new Context(Menu, this));
            else Action?.Invoke(Pressed);
        }

        private void ExecuteOnFocusEntered() => _onFocusEntered?.Invoke();
        public void OnFocusEntered(Action onFocus) {
            Connect(SignalConstants.Control_FocusEnteredSignal, this, nameof(ExecuteOnFocusEntered));
            _onFocusEntered = onFocus;
        }

        private void ExecuteOnFocusExited() => _onFocusExited?.Invoke();
        public void OnFocusExited(Action onFocus) {
            Connect(SignalConstants.Control_FocusExitedSignal, this, nameof(ExecuteOnFocusExited));
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

    internal class MenuState {
        internal readonly ActionMenu Menu;
        internal readonly ActionButton Button;

        internal MenuState(ActionMenu menu, ActionButton button) {
            Menu = menu;
            Button = button;
        }
    }
}