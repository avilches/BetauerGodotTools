using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Core.Nodes;
using Betauer.Core.Restorer;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.UI;

public class MenuContainer {
    internal readonly Container OriginalContainer;
    internal readonly Node Parent;
    private readonly Viewport _viewport;
    private readonly List<Menu> _menus = new();
    private readonly LinkedList<MenuState> _navigationState = new();

    public Menu ActiveMenu { get; private set; } = null;
    public bool Available { get; private set; } = true;

    public bool DisableGuiInAnimations = true;

    internal Func<MenuTransition, Task>? DefaultGoGoodbyeAnimation = null;
    internal Func<MenuTransition, Task>? DefaultGoNewMenuAnimation = null;
    internal Func<MenuTransition, Task>? DefaultBackGoodbyeAnimation = null;
    internal Func<MenuTransition, Task>? DefaultBackNewMenuAnimation = null;

    public MenuContainer(Container originalContainer) {
        Parent = originalContainer.GetParent();
        _viewport = originalContainer.GetViewport();
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

    public Menu GetRootMenu() {
        return _menus.Find(menu => menu.Name == null);
    }

    public Menu GetMenu(string toMenuName) {
        return _menus.Find(menu => menu.Name == toMenuName);
    }

    public bool IsRootMenuActive() => IsMenuActive(null);
    public bool IsMenuActive(string name) => ActiveMenu.Name == name;

    private readonly object _lockObject = new object();
    public async Task Start(string? startMenuButtonName = null,
        Func<MenuTransition, Task>? newMenuAnimation = null) {
        if (!Available) return;
        lock (_lockObject) {
            if (!Available) return;
            Available = false;
        }
        try {
            var fromMenu = ActiveMenu;
            var toMenu = GetRootMenu();
            BaseButton? toButton = startMenuButtonName != null ? toMenu.GetControl<BaseButton>(startMenuButtonName) : null;
            _navigationState.Clear();
            var transition = new MenuTransition(fromMenu, null, toMenu, toButton);
            newMenuAnimation ??= toMenu.GoNewMenuAnimation ?? DefaultGoNewMenuAnimation;
            await PlayTransition(transition, null, newMenuAnimation);
        } finally {
            lock (_lockObject) {
                Available = true;
            }
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
        if (_navigationState.Count == 0 && IsRootMenuActive()) return;
        await _Back(true, fromButtonName, rootButtonName, goodbyeAnimation, newMenuAnimation);
    }

    private async Task _Go(bool replace, string toMenuName, string? toButtonName = null, string? fromButtonName = null,
        Func<MenuTransition, Task>? goodbyeAnimation = null,
        Func<MenuTransition, Task>? newMenuAnimation = null) {
        if (!Available) return;
        lock (_lockObject) {
            if (!Available) return;
            Available = false;
        }
        try {
            var fromButton = fromButtonName != null
                ? ActiveMenu.GetControl<BaseButton>(fromButtonName)
                : ActiveMenu.GetChildFocused();
            Menu toMenu = GetMenu(toMenuName);
            if (!replace)
                _navigationState.AddLast(new MenuState(ActiveMenu, fromButton));
            var toButton = toButtonName != null ? toMenu.GetControl<BaseButton>(toButtonName) : null;
            var transition = new MenuTransition(ActiveMenu, fromButton, toMenu, toButton);
            goodbyeAnimation ??= ActiveMenu.GoGoodbyeAnimation ?? DefaultGoGoodbyeAnimation;
            newMenuAnimation ??= toMenu.GoNewMenuAnimation ?? DefaultGoNewMenuAnimation;
            await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
        } finally {
            lock (_lockObject) {
                Available = true;
            }
        }
    }

    private async Task _Back(bool backToStartMenu, string? startMenuButtonName, string? fromButtonName = null,
        Func<MenuTransition, Task>? goodbyeAnimation = null,
        Func<MenuTransition, Task>? newMenuAnimation = null) {
        if (!Available) return;
        lock (_lockObject) {
            if (!Available) return;
            Available = false;
        }
        try {
            Menu toMenu;
            BaseButton? toButton;
            if (backToStartMenu) {
                _navigationState.Clear();
                toMenu = GetRootMenu();
                toButton = startMenuButtonName != null ? toMenu.GetControl<BaseButton>(startMenuButtonName) : null;
            } else {
                MenuState lastState = _navigationState.Last();
                _navigationState.RemoveLast();
                toMenu = lastState.Menu;
                toButton = lastState.Button;
            }
            var fromButton = fromButtonName != null ? ActiveMenu.GetControl<BaseButton>(fromButtonName) : ActiveMenu.GetChildFocused();
            var transition = new MenuTransition(ActiveMenu, fromButton, toMenu, toButton);
            goodbyeAnimation ??= ActiveMenu.BackGoodbyeAnimation ?? DefaultBackGoodbyeAnimation;
            newMenuAnimation ??= toMenu.BackNewMenuAnimation ?? DefaultBackNewMenuAnimation;
            await PlayTransition(transition, goodbyeAnimation, newMenuAnimation);
        } finally {
            lock (_lockObject) {
                Available = true;
            }
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
                    var saver = transition.FromMenu.GetChildren().CreateRestorer();
                    saver.Save();
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
                var saver = transition.ToMenu.GetChildren().CreateRestorer("modulate");
                saver.Save();
                transition.ToMenu.GetChildren().ForEach(e => e.Modulate = ModulateInvisible);
                /*
                 * 2) Wait one frame, so the container can arrange the children positions safely.
                 */ 
                await Parent.AwaitProcessFrame();
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
        Container.Name = name ?? "RootMenuContainer";
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
        var instance = button.Instantiate<TScene>();
        Container.AddChild(instance);
        return instance;
    }

    public Menu AddNode(Node button) {
        Container.AddChild(button);
        return this;
    }

    internal void Remove() {
        _onClose?.Invoke();
        Parent.RemoveChild(Container);
    }

    internal void Show(BaseButton? focused = null) {
        _onShow?.Invoke();
        Parent.AddChild(Container);
        Parent.MoveChild(Container, OriginalContainer.GetIndex());
        Refresh(focused);
    }

    public T? GetControl<T>(string name) where T : Control {
        return Container.GetNode<T>(name) ?? throw new NullReferenceException();
    }

    public Menu Refresh(BaseButton? focused = null) {
        focused = Container.RefreshNeighbours(focused, WrapButtons);
        focused?.GrabFocus();
        return this;
    }

    public List<Control> GetChildren() {
        return Container.GetChildren().OfType<Control>().ToList();
    }

    public BaseButton? GetChildFocused() {
        return Container.GetChildFocused<BaseButton>();
    }

    public List<Control> GetVisibleControl() {
        return Container.GetVisibleControl<Control>();
    }

    public Restorer DisableButtons(bool storeFocus = true) {
        return Container.DisableButtons(storeFocus);
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