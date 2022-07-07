using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Betauer.UI {
    public class MenuContainer {
        internal readonly Container OriginalContainer;
        internal readonly Node Parent;
        private readonly Viewport _viewport;
        private readonly List<Menu> _menus = new List<Menu>();
        private readonly LinkedList<MenuState> _navigationState = new LinkedList<MenuState>();

        public Menu ActiveMenu { get; private set; } = null;
        public bool Available { get; private set; } = true;

        public bool DisableGuiInAnimations = true;

        internal Func<MenuTransition, Task>? DefaultGoGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? DefaultGoNewMenuAnimation = null;
        internal Func<MenuTransition, Task>? DefaultBackGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? DefaultBackNewMenuAnimation = null;

        public MenuContainer(Container originalContainer) {
            Parent = originalContainer.GetParent();
            _viewport = originalContainer.GetTree().Root;
            OriginalContainer = originalContainer;
            OriginalContainer.DisableAllNotifications();
            OriginalContainer.Visible = false;
            var menu = new Menu(this, null);
            _menus.Add(menu);
        }

        public MenuContainer ConfigureGoTransition(
            Func<MenuTransition, Task>? goGoodbyeAnimation,
            Func<MenuTransition, Task>? goNewMenuAnimation) {
            DefaultGoGoodbyeAnimation = goGoodbyeAnimation;
            DefaultGoNewMenuAnimation = goNewMenuAnimation;
            return this;
        }

        public MenuContainer ConfigureBackTransition(
            Func<MenuTransition, Task>? backGoodbyeAnimation,
            Func<MenuTransition, Task>? backNewMenuAnimation) {
            DefaultBackGoodbyeAnimation = backGoodbyeAnimation;
            DefaultBackNewMenuAnimation = backNewMenuAnimation;
            return this;
        }


        public Menu AddMenu(string name) {
            if (name == null) throw new ArgumentNullException(nameof(name), "Menu name can't be null");
            if (_menus.Any(menu => menu.Name == name)) {
                throw new ArgumentException("Duplicated menu name: " + name);
            }
            var menu = new Menu(this, name);
            _menus.Add(menu);
            return menu;
        }

        public Menu GetStartMenu() {
            return _menus.Find(menu => menu.Name == null);
        }

        public Menu GetMenu(string toMenuName) {
            return _menus.Find(menu => menu.Name == toMenuName);
        }

        public bool IsStartMenuActive() => IsMenuActive(null);
        public bool IsMenuActive(string name) => ActiveMenu.Name == name;

        public async Task Start(string? startMenuButtonName = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (!Available) return;
            try {
                Available = false;
                var fromMenu = ActiveMenu;
                var toMenu = GetStartMenu();
                BaseButton? toButton = startMenuButtonName != null ? toMenu.GetControl<BaseButton>(startMenuButtonName) : null;;
                lock (_navigationState) _navigationState.Clear();
                var transition = new MenuTransition(fromMenu, null, toMenu, toButton);
                newMenuAnimation ??= toMenu.GoNewMenuAnimation ?? DefaultGoNewMenuAnimation;
                await PlayTransition(transition, null, newMenuAnimation);
            } catch (Exception e) {
                throw e;
            } finally {
                Available = true;
            }
        }

        /// <summary>
        /// Close the current menu using a goodbye animation and show a new menu using the new menu animation.
        /// This method add to the stack a new state with the current menu and the current focused button,
        /// so when the Back() method is called later, the old state will be recover.
        /// 
        /// 1-Start() show root menu. Stack: []
        /// 2-Go("Settings") show Settings menu. Stack: ["Root"]
        /// 3-Go("Video") show Video menu. Stack: ["Root", "Settings"]
        /// 4-Back() show Settings menu. Stack: ["Root"]
        /// 5-Back() show Root menu. Stack: []
        /// </summary>
        /// <param name="toMenuName"></param>
        /// <param name="toButtonName"></param>
        /// <param name="fromButtonName"></param>
        /// <param name="goodbyeAnimation"></param>
        /// <param name="newMenuAnimation"></param>
        public async Task Go(string toMenuName, string? toButtonName = null, string? fromButtonName = null,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            await _Go(false, toMenuName, toButtonName, fromButtonName, goodbyeAnimation, newMenuAnimation);
        }

        /// <summary>
        /// Close the current menu using a goodbye animation and show the new menu using the new menu animation.
        /// This method doesn't modify the stack, so call to the Back() method later will return the previous state
        /// instead this one, as the Go() would do.
        /// 
        /// 1-Start() show root menu. Stack: []
        /// 2-Go("Settings") show Settings menu. Stack: ["Root"]
        /// 3-Replace("Video") show Video menu. Stack: ["Root"]
        /// 4-Back() (show Root menu). Stack: []
        ///
        /// 1-Start() Stack: []
        /// 2-Replace("Settings") show Settings menu. Stack: []
        /// 3-Back() doesn't do anything
        /// </summary>
        /// <param name="toMenuName"></param>
        /// <param name="toButtonName"></param>
        /// <param name="fromButtonName"></param>
        /// <param name="goodbyeAnimation"></param>
        /// <param name="newMenuAnimation"></param>
        public async Task Replace(string toMenuName, string? toButtonName = null, string? fromButtonName = null,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            await _Go(true, toMenuName, toButtonName, fromButtonName, goodbyeAnimation, newMenuAnimation);
        }

        /// <summary>
        /// Close the current menu using a goodbye animation and show the menu stored in the stack. If the stack is empty
        /// it doesn't do anything.
        /// </summary>
        /// <param name="fromButtonName"></param>
        /// <param name="goodbyeAnimation"></param>
        /// <param name="newMenuAnimation"></param>
        public async Task Back(string? fromButtonName = null, 
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (_navigationState.Count == 0) return;
            await _Back(false, fromButtonName, null, goodbyeAnimation, newMenuAnimation);
        }

        /// <summary>
        /// Close the current menu using a goodbye animation, show the root menu and reset the stack.
        /// </summary>
        /// <param name="fromButtonName"></param>
        /// <param name="rootButtonName"></param>
        /// <param name="goodbyeAnimation"></param>
        /// <param name="newMenuAnimation"></param>
        public async Task BackToStart(string? fromButtonName = null, string? rootButtonName = null,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (_navigationState.Count == 0 && IsStartMenuActive()) return;
            await _Back(true, fromButtonName, rootButtonName, goodbyeAnimation, newMenuAnimation);
        }

        private async Task _Go(bool replace, string toMenuName, string? toButtonName = null, string? fromButtonName = null,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (!Available) return;
            try {
                Available = false;
                var fromButton = fromButtonName != null
                    ? ActiveMenu.GetControl<BaseButton>(fromButtonName)
                    : ActiveMenu.GetFocused<BaseButton>();
                Menu toMenu = GetMenu(toMenuName);
                if (!replace)
                    lock (_navigationState)
                        _navigationState.AddLast(new MenuState(ActiveMenu, fromButton));
                var toButton = toButtonName != null ? toMenu.GetControl<BaseButton>(toButtonName) : null;
                var transition = new MenuTransition(ActiveMenu, fromButton, toMenu, toButton);
                goodbyeAnimation ??= ActiveMenu.GoGoodbyeAnimation ?? DefaultGoGoodbyeAnimation;
                newMenuAnimation ??= toMenu.GoNewMenuAnimation ?? DefaultGoNewMenuAnimation;
                await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
            } catch (Exception e) {
                throw e;
            } finally {
                Available = true;
            }
        }

        private async Task _Back(bool backToStartMenu, string? startMenuButtonName, string? fromButtonName = null,
            Func<MenuTransition, Task>? goodbyeAnimation = null,
            Func<MenuTransition, Task>? newMenuAnimation = null) {
            if (!Available) return;
            try {
                Available = false;
                Menu toMenu;
                BaseButton? toButton;
                lock (_navigationState) {
                    if (backToStartMenu) {
                        _navigationState.Clear();
                        toMenu = GetStartMenu();
                        toButton = startMenuButtonName != null ? toMenu.GetControl<BaseButton>(startMenuButtonName) : null;
                    } else {
                        MenuState lastState = _navigationState.Last();
                        _navigationState.RemoveLast();
                        toMenu = lastState.Menu;
                        toButton = lastState.Button;
                    }
                }
                var fromButton = fromButtonName != null ? ActiveMenu.GetControl<BaseButton>(fromButtonName) : ActiveMenu.GetFocused<BaseButton>();
                var transition = new MenuTransition(ActiveMenu, fromButton, toMenu, toButton);
                goodbyeAnimation ??= ActiveMenu.BackGoodbyeAnimation ?? DefaultBackGoodbyeAnimation;;
                newMenuAnimation ??= toMenu.BackNewMenuAnimation ?? DefaultBackNewMenuAnimation;;
                await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
            } catch (Exception e) {
                throw e;
            } finally {
                Available = true;
            }
        }

        private static readonly Color ModulateInvisible = new Color(0, 0, 0, 0);
        private async Task PlayTransition(MenuTransition transition, 
            Func<MenuTransition, Task>? goodbyeAnimation,
            Func<MenuTransition, Task>? newMenuAnimation) {
            try {
                if (DisableGuiInAnimations) _viewport.GuiDisableInput = true;

                if (transition.FromMenu != null) {
                    if (goodbyeAnimation != null) {
                        var saver = new MultiRestorer(transition.FromMenu.GetChildren()).Save();
                        await goodbyeAnimation(transition);
                        saver.Restore();
                    }
                    transition.FromMenu.Remove();
                }

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
                    transition.ToMenu.GetChildren().ForEach(e => e.Modulate = ModulateInvisible);
                    /*
                     * 2) Wait one frame, so the container can arrange the children positions safely.
                     */ 
                    await Parent.AwaitIdleFrame();
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
            } catch (Exception e) {
                throw e;
            } finally {
                if (DisableGuiInAnimations) _viewport.GuiDisableInput = false;
            }
        }

        public void QueueFree() {
            Available = false;
            _menus.ToList().ForEach(c => c.Container.QueueFree());
            _menus.Clear();
            _navigationState.Clear();
            ActiveMenu = null;
        }

    }


    public class Menu {
        private Action? _onShow;
        private Action? _onClose;
        private readonly MenuContainer _menuContainer;
        private Node Parent => _menuContainer.Parent;
        private Node OriginalContainer => _menuContainer.OriginalContainer;

        public readonly string? Name;
        public Container Container { get; }
        public bool WrapButtons { get; set; } = true;
        
        internal Func<MenuTransition, Task>? GoGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? GoNewMenuAnimation = null;
        internal Func<MenuTransition, Task>? BackGoodbyeAnimation = null;
        internal Func<MenuTransition, Task>? BackNewMenuAnimation = null;

        internal Menu(MenuContainer menuContainer, string? name) {
            _menuContainer = menuContainer;
            Name = name;
            Container = (Container)OriginalContainer.Duplicate();
            Container.Name = name ?? "StartMenu";
            Container.Visible = true;
        }

        public Menu OnShow(Action onShow) {
            _onShow = onShow;
            return this;
        }

        public Menu OnClose(Action onClose) {
            _onClose = onClose;
            return this;
        }

        public Menu ConfigureGoTransition(
            Func<MenuTransition, Task>? goGoodbyeAnimation,
            Func<MenuTransition, Task>? goNewMenuAnimation) {
            GoGoodbyeAnimation = goGoodbyeAnimation;
            GoNewMenuAnimation = goNewMenuAnimation;
            return this;
        }

        public Menu ConfigureBackTransition(
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

        public Menu AddNode(Node button) {
            Container.AddChild(button);
            return this;
        }

        public MultiRestorer DisableButtons(bool storeFocus = true) {
            var buttons = Container.GetChildren<BaseButton>();
            MultiRestorer restorer = new MultiRestorer(buttons, "disabled");
            if (storeFocus) {
                restorer.AddFocusRestorer(Container);
            }
            restorer.Save();
            foreach (var child in Container.GetChildren()) {
                if (child is BaseButton button) button.Disabled = true;
            }
            return restorer;
        }

        public Menu Refresh(BaseButton? focused = null) {
            focused = Container.RefreshNeighbours(focused, WrapButtons);
            focused?.GrabFocus();
            return this;
        }

        public List<Control> GetChildren() {
            return Container.GetChildren<Control>().ToList();
        }

        public List<Control> GetVisibleControl() {
            if (Parent is ScrollContainer scrollContainer) {
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
            _onClose?.Invoke();
            Parent.RemoveChild(Container);
        }

        internal void Show(BaseButton? focused = null) {
            _onShow?.Invoke();
            Parent.AddChildBelowNode(OriginalContainer, Container);
            Refresh(focused);
        }

        public T? GetControl<T>(string name) where T : Control {
            return Container.GetNode<T>(name) ?? throw new NullReferenceException();
        }

        /// <summary>
        /// Return the focused control in the container (if the focused button belongs to other container, it returns null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetFocused<T>() where T : Control {
            var focused = Container.GetFocusOwner();
            return focused == null ? null : Container.GetChildren<T>().Find(b => b == focused);
        }
    }

    public class MenuTransition {
        public readonly Menu? FromMenu;
        public readonly BaseButton? FromButton;
        public readonly Menu ToMenu;
        public readonly BaseButton? ToButton;

        public MenuTransition(Menu fromMenu, BaseButton? fromButton, Menu toMenu,
            BaseButton? toButton) {
            FromMenu = fromMenu;
            FromButton = fromButton;
            ToMenu = toMenu;
            ToButton = toButton;
        }
    }

    internal class MenuState {
        internal readonly Menu Menu;
        internal readonly BaseButton? Button;

        internal MenuState(Menu menu, BaseButton? button) {
            Menu = menu;
            Button = button;
        }
    }
}