using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.UI {
    public class MenuController {
        private readonly Container _originalContainer;
        private readonly Node _parent;
        private readonly List<ActionMenu> _menus = new List<ActionMenu>();
        private readonly LinkedList<MenuState> _navigationState = new LinkedList<MenuState>();

        public ActionMenu ActiveMenu { get; private set; } = null;

        public bool Available { get; private set; } = false;

        public bool DisableGuiInAnimations = true;

        internal Func<MenuTransition, Task>? DefaultGoGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? DefaultGoNewMenuAnimation = null;
        internal Func<MenuTransition, Task>? DefaultBackGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? DefaultBackNewMenuAnimation = null;

        public MenuController(Container originalContainer) {
            _parent = originalContainer.GetParent();
            _originalContainer = originalContainer;
            _originalContainer.DisableAllNotifications();
            _originalContainer.Visible = false;
        }

        public MenuController ConfigureGoTransition(
            Func<MenuTransition, Task>? goGoodbyeAnimation,
            Func<MenuTransition, Task>? goNewMenuAnimation) {
            DefaultGoGoodbyeAnimation = goGoodbyeAnimation;
            DefaultGoNewMenuAnimation = goNewMenuAnimation;
            return this;
        }

        public MenuController ConfigureBackTransition(
            Func<MenuTransition, Task>? backGoodbyeAnimation,
            Func<MenuTransition, Task>? backNewMenuAnimation) {
            DefaultBackGoodbyeAnimation = backGoodbyeAnimation;
            DefaultBackNewMenuAnimation = backNewMenuAnimation;
            return this;
        }


        public void QueueFree() {
            _menus.ToList().ForEach(c => c.Container.QueueFree());
        }

        public ActionMenu AddMenu(string name, Action<ActionMenu> onShow = null) {
            var menu = new ActionMenu(this, name, _originalContainer, _parent, onShow);
            _menus.Add(menu);
            return menu;
        }

        public async Task Start(string? name = null) {
            ActiveMenu = name != null ? _menus.Find(menu => menu.Name == name) : _menus.First();
            ActiveMenu.Show();
            Available = true;
        }

        public ActionMenu GetMenu(string toMenuName) {
            return _menus.Find(menu => menu.Name == toMenuName);
        }

        public async Task Go(string toMenuName, string? toButtonName = null, string? fromButtonName = null) { //},
            // Func<MenuTransition, Task>? goodbyeAnimation = null,
            // Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (!Available) return;
            try {
                Available = false;
                var fromButton = fromButtonName != null ? ActiveMenu.GetControl<BaseButton>(fromButtonName) : ActiveMenu.Container.GetFocusOwner() as BaseButton;
                ActionMenu toMenu = GetMenu(toMenuName);
                _navigationState.AddLast(new MenuState(ActiveMenu, fromButton));

                BaseButton? toButton = toButtonName != null ? toMenu.GetControl<BaseButton>(toButtonName) : null;
                MenuTransition transition = new MenuTransition(ActiveMenu, fromButton, toMenu, toButton);
                Func<MenuTransition, Task>? goodbyeAnimation = toMenu.GoGoodbyeAnimation ?? DefaultGoGoodbyeAnimation;
                Func<MenuTransition, Task>? newMenuAnimation = toMenu.GoNewMenuAnimation ?? DefaultGoNewMenuAnimation;
                await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
            } finally {
                Available = true;
            }
        }

        // public async Task Back(
        //     Func<MenuTransition, Task>? goodbyeAnimation = null,
        //     Func<MenuTransition, Task>? newMenuAnimation = null) {
        //     await Back(null, goodbyeAnimation, newMenuAnimation);
        // }

        public async Task Back(string? fromButtonName = null) { // ,
            // Func<MenuTransition, Task>? goodbyeAnimation = null,
            // Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (!Available || _navigationState.Count == 0) return;
            try {
                Available = false;
                MenuState lastState = _navigationState.Last();
                _navigationState.RemoveLast();
                ActionMenu toMenu = lastState.Menu;
                var fromButton = fromButtonName != null ? ActiveMenu.GetControl<BaseButton>(fromButtonName) : ActiveMenu.Container.GetFocusOwner() as BaseButton;
                MenuTransition transition = new MenuTransition(ActiveMenu, fromButton, toMenu, lastState.Button);
                Func<MenuTransition, Task>? goodbyeAnimation = toMenu.BackGoodbyeAnimation ?? DefaultBackGoodbyeAnimation;;
                Func<MenuTransition, Task>? newMenuAnimation = toMenu.BackNewMenuAnimation ?? DefaultBackNewMenuAnimation;;
                await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
            } finally {
                Available = true;
            }
        }

        private static readonly Color Invisible = new Color(0, 0, 0, 0);
        private async Task PlayTransition(MenuTransition transition, 
            Func<MenuTransition, Task>? goodbyeAnimation,
            Func<MenuTransition, Task>? newMenuAnimation) {
            var viewport = transition.FromMenu.Container.GetTree().Root;
            try {
                if (DisableGuiInAnimations) viewport.GuiDisableInput = true;
                
                if (goodbyeAnimation != null) {
                    var saver = new MultiRestorer(transition.FromMenu.GetChildren()).Save();
                    await goodbyeAnimation(transition);
                    saver.Restore();
                }
                transition.FromMenu.Remove();

                transition.ToMenu.Show(transition.ToButton);
                if (newMenuAnimation != null) {
                    /* Hack time:
                     * HBox/VBoxContainers need at least one frame with non-visible children to arrange their positions.
                     * The problem is we want the container arrange the children without showing them before the animation
                     * starts. So:
                     * 
                     * 1) show the children with their modulates to Color(0,0,0,0) so they will be invisible for
                     * human eye, but "visible" for the container and it can arrange them.
                     */
                    var saver = new MultiRestorer(transition.ToMenu.GetChildren(), "modulate").Save();
                    transition.ToMenu.GetChildren().ForEach(e => e.Modulate = Invisible);
                    /*
                     * 2) Wait one frame, so the container can arrange the children positions safely.
                     */ 
                    await _parent.AwaitIdleFrame();
                    /*
                     * 3) Restore the old modulates and start the animation.
                     */
                    saver.Restore();
                    await newMenuAnimation(transition);
                    /*
                     * Done! How to test if this is working? With the Bounce animation and a VBoxContainer. The animation
                     * changes the vertical position, so, without this trick, all the children appear overlapped in
                     * the (0,0) position.
                     * Without the modulate, the elements will appear one frame. This could be ok in animation like
                     * Bounce, but using any FadeIn* animation will cause a weird effect. 
                     */
                }
                ActiveMenu = transition.ToMenu;
            } finally {
                if (DisableGuiInAnimations) viewport.GuiDisableInput = false;
            }
        }
    }


    public class ActionMenu {
        private readonly Action<ActionMenu> _onShow;
        private readonly Node _parent;
        private readonly Node _originalContainer;
        internal readonly MenuController MenuController;

        public readonly string Name;
        public Container Container { get; }
        public bool WrapButtons { get; set; } = true;
        
        internal Func<MenuTransition, Task>? GoGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? GoNewMenuAnimation = null;
        internal Func<MenuTransition, Task>? BackGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? BackNewMenuAnimation = null;

        internal ActionMenu(MenuController menuController, string name, Container originalContainer,
            Node parent, Action<ActionMenu> onShow) {
            _originalContainer = originalContainer;
            _parent = parent;
            _onShow = onShow;
            MenuController = menuController;
            Name = name;
            Container = (Container)originalContainer.Duplicate();
            Container.Visible = true;
        }

        public ActionMenu ConfigureGoTransition(
            Func<MenuTransition, Task>? goGoodbyeAnimation,
            Func<MenuTransition, Task>? goNewMenuAnimation) {
            GoGoodbyeAnimation = goGoodbyeAnimation;
            GoNewMenuAnimation = goNewMenuAnimation;
            return this;
        }

        public ActionMenu ConfigureBackTransition(
            Func<MenuTransition, Task>? backGoodbyeAnimation,
            Func<MenuTransition, Task>? backNewMenuAnimation) {
            BackGoodbyeAnimation = backGoodbyeAnimation;
            BackNewMenuAnimation = backNewMenuAnimation;
            return this;
        }

        public Button AddButton(string name, string title) {
            Button button = new Button();
            button.Name = name;
            button.Text = title;
            AddNode(button);
            return button;
        }

        public HSeparator AddHSeparator() {
            HSeparator separator = new HSeparator();
            AddNode(separator);
            return separator;
        }

        public VSeparator AddVSeparator() {
            VSeparator separator = new VSeparator();
            AddNode(separator);
            return separator;
        }

        public TScene Add<TScene>(PackedScene button) where TScene : Node {
            var instance = button.Instance<TScene>();
            Container.AddChild(instance);
            return instance;
        }

        public ActionMenu AddNode(Node button) {
            Container.AddChild(button);
            return this;
        }

        public bool IsFocusedAndDisabled() {
            foreach (var child in Container.GetChildren()) {
                if (child is BaseButton { Disabled: true } disabledButton && disabledButton.HasFocus()) return true;
            }
            return false;
        }

        public MultiRestorer DisableButtons() {
            var buttons = Container.GetChildren<BaseButton>();
            var restorer = new MultiRestorer(buttons, "disabled");
            foreach (var child in Container.GetChildren()) {
                if (child is BaseButton button) button.Disabled = true;
            }
            return restorer;
        }

        public ActionMenu Refresh(BaseButton? focused = null) {
            focused = Container.RefreshNeighbours(focused, WrapButtons);
            focused?.GrabFocus();
            return this;
        }

        public List<Control> GetChildren() {
            return Container.GetChildren<Control>().ToList();
        }

        public List<Control> GetVisibleControl() {
            if (_parent is ScrollContainer scrollContainer) {
                var topVisible = scrollContainer.ScrollVertical;
                var bottomVisible = scrollContainer.RectSize.y + scrollContainer.ScrollVertical;
                return Container.GetChildren<Control>()
                    .Where(control =>
                        control.RectPosition.y >= topVisible &&
                        control.RectPosition.y + control.RectSize.y <= bottomVisible)
                    .ToList();
            }
            return GetChildren();
        }

        internal void Remove() {
            _parent.RemoveChild(Container);
        }

        internal void Show(BaseButton? focused = null) {
            _onShow?.Invoke(this);
            _parent.AddChildBelowNode(_originalContainer, Container);
            Refresh(focused);
        }

        public T? GetControl<T>(string name) where T : Control {
            return Container.GetNode<T>(name) ?? throw new NullReferenceException();
        }
    }

    public class MenuTransition {
        public readonly ActionMenu FromMenu;
        public readonly BaseButton? FromButton;
        public readonly ActionMenu ToMenu;
        public readonly BaseButton? ToButton;

        public MenuTransition(ActionMenu fromMenu, BaseButton? fromButton, ActionMenu toMenu,
            BaseButton? toButton) {
            FromMenu = fromMenu;
            FromButton = fromButton;
            ToMenu = toMenu;
            ToButton = toButton;
        }
    }

    internal class MenuState {
        internal readonly ActionMenu Menu;
        internal readonly BaseButton? Button;

        internal MenuState(ActionMenu menu, BaseButton? button) {
            Menu = menu;
            Button = button;
        }
    }
}