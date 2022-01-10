using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Animation;
using Godot;

namespace Betauer.UI {
    public class MultipleActionMenu {
        private readonly Control _baseHolder;
        private List<ActionMenu> _menus = new List<ActionMenu>();
        internal readonly LinkedList<ActionState> _nestedPathMenus = new LinkedList<ActionState>();

        public MultipleActionMenu(Control baseHolder) {
            _baseHolder = baseHolder;
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
                    menu.Show();
                } else {
                    menu.Hide();
                }
            }
        }

        public ActionMenu GetMenu(string toMenuName) {
            return _menus.Find(menu => menu.Name == toMenuName);
        }
    }


    public class ActionMenu {
        private readonly ControlRestorer _saver;
        private readonly Control _holder;
        internal readonly MultipleActionMenu MultipleActionMenu;
        public readonly List<ActionButton> Buttons = new List<ActionButton>();

        public readonly string Name;
        public Control Holder => _holder;
        public bool WrapButtons = true;

        internal ActionMenu(MultipleActionMenu multipleActionMenu, string name, Control baseHolder) {
            MultipleActionMenu = multipleActionMenu;
            Name = name;
            _holder = baseHolder.Duplicate() as Control;
            _saver = new ControlRestorer(_holder);
            baseHolder.GetParent().AddChildBelowNode(baseHolder, _holder);
        }

        public void Save() {
            _holder.Save();
            Buttons.ForEach(button => button.Save());
        }

        public void Restore() {
            _saver.Restore();
            Buttons.ForEach(button => button.Restore());
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
        public async Task<ActionMenu> Refresh(ActionButton focused = null) {
            ActionButton first = null;
            ActionButton last = null;
            var takeNextFocus = false;
            foreach (var child in _holder.GetChildren()) {
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
                _holder.RemoveChild(child as Node);
            }
            foreach (var actionButton in Buttons) {
                if (actionButton.Disabled) {
                    actionButton.FocusMode = Control.FocusModeEnum.None;
                    _holder.AddChild(actionButton);
                    last = actionButton;
                } else {
                    if (first == null) {
                        first = actionButton;
                    }
                    _holder.AddChild(actionButton);
                    last = actionButton;
                    actionButton.FocusMode = Control.FocusModeEnum.All;
                }
            }
            if (WrapButtons && first != null && last != null && first != last) {
                if (_holder is VBoxContainer) {
                    first.FocusNeighbourTop = "../" + last.Name;
                    last.FocusNeighbourBottom = "../" + first.Name;
                } else if (_holder is HBoxContainer) {
                    first.FocusNeighbourLeft = "../" + last.Name;
                    last.FocusNeighbourRight = "../" + first.Name;
                }
            }
            await _holder.GetTree().AwaitIdleFrame();
            (focused ?? first)?.GrabFocus();
            return this;
        }

        // public async Task Focus(ActionButton button) {
        // await _holder.GetTree().AwaitIdleFrame();
        // button.GrabFocus();
        // }

        public void FocusFirst() {
            foreach (var child in _holder.GetChildren()) {
                if (child is ActionButton { Disabled: false } action) {
                    GD.Print("Setting focus on first button " + action.Text);
                    action.GrabFocus();
                    return;
                }
            }
        }

        public IEnumerable GetButtons() {
            return _holder.GetChildren();
        }

        public void Hide() {
            _holder.Hide();
        }

        public async Task Show(ActionButton focused = null) {
            _holder.Show();
            await Refresh(focused);
        }

        public ActionButton GetButton(string name) {
            return Buttons.Find(button => button.Name == name);
        }
    }


    public class ActionButton : DiButton {
        private readonly ControlRestorer _saver;
        private readonly ActionMenu _actionMenu;

        public ActionMenu Menu => _actionMenu;
        public Action? Action;
        public Action<ActionButton>? ActionWithButton;

        // TODO: i18n
        internal ActionButton(ActionMenu actionMenu) {
            _actionMenu = actionMenu;
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
            await _actionMenu.Refresh();
        }


        public void Save() {
            _saver.Save();
        }

        public void Restore() {
            _saver.Restore();
        }


        public async Task Go(string toMenuName, Func<MenuTransition, Task> goodbyeAnimation,
            Func<MenuTransition, Task> newMenuAnimation) {
            ActionMenu toMenu = _actionMenu.MultipleActionMenu.GetMenu(toMenuName);
            _actionMenu.MultipleActionMenu._nestedPathMenus.AddLast(new ActionState(_actionMenu, this));

            MenuTransition transition = new MenuTransition(_actionMenu, this, toMenu);
            GetTree().Root.GuiDisableInput = true;

            _actionMenu.Save();
            GD.Print("Go1");
            await goodbyeAnimation(transition);
            _actionMenu.Hide();
            _actionMenu.Restore();

            await toMenu.Show();
            toMenu.Save();
            GD.Print("Go2");
            await newMenuAnimation(transition);
            toMenu.Restore();

            GetTree().Root.GuiDisableInput = false;
        }

        public async Task Back(Func<MenuTransition, Task> goodbyeAnimation,
            Func<MenuTransition, Task> newMenuAnimation) {
            var _nestedPathMenus = _actionMenu.MultipleActionMenu._nestedPathMenus;
            if (_nestedPathMenus.Count == 0) {
                // back from root menu!
                return;
            }
            ActionState lastState = _nestedPathMenus.Last();
            _nestedPathMenus.RemoveLast();

            ActionMenu toMenu = lastState.Menu;
            MenuTransition transition = new MenuTransition(_actionMenu, this, toMenu);
            GetTree().Root.GuiDisableInput = true;

            _actionMenu.Save();
            GD.Print("Back1");
            await goodbyeAnimation(transition);
            _actionMenu.Hide();
            _actionMenu.Restore();

            toMenu.Hide();
            await toMenu.Show(lastState.Button);
            toMenu.Save();
            GD.Print("Back2");
            await newMenuAnimation(transition);
            toMenu.Restore();

            GetTree().Root.GuiDisableInput = false;
        }
    }

    public class MenuTransition {
        public readonly ActionMenu FromMenu;
        public readonly ActionButton FromButton;
        public readonly ActionMenu ToMenu;


        public MenuTransition(ActionMenu fromMenu, ActionButton fromButton, ActionMenu toMenu) {
            FromMenu = fromMenu;
            FromButton = fromButton;
            ToMenu = toMenu;
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