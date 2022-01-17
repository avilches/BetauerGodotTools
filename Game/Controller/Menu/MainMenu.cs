using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.Screen;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class MainMenu : DiControl {
        [OnReady("MarginContainer/HBoxContainer/VBoxContainer/Menu")]
        private VBoxContainer _menuBase;

        private MenuController _menuController;

        [Inject] public InputManager InputManager;
        [Inject] public GameManager GameManager;

        private Launcher _launcher;

        public override async void Ready() {
            _launcher = new Launcher().CreateNewTween(this);
            _menuController = BuildMenu();
            await _menuController.Start("Root");
            UpdateResolutionButton();
        }

        [Inject] public ScreenManager ScreenManager;

        public MenuController BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("NewGame", "New game", async (ctx) => {
                    // GD.Print("New Game");
                    GameManager.StartGame();
                    // var continueButton = ctx.Menu.GetButton("Continue");
                    // continueButton!.Disabled = !continueButton.Disabled;
                    // await _launcher.Play(Template.FadeIn, continueButton, 0f,
                    //         MenuEffectTime)
                    //     .Await();
                    // continueButton.Save();

                    ctx.Refresh();
                })
                .AddButton("Continue", "Continue", async (ctx) => {
                    GD.Print("Continue");
                    ctx.ActionButton.Disabled = true;
                    ctx.Refresh();
                    await _launcher.Play(Template.FadeOut, ctx.ActionButton, 0f,
                            MenuEffectTime)
                        .Await();
                    ctx.ActionButton.Save();
                })
                .AddButton("Options", "Options",
                    (ctx) => ctx.Go("Options", GoGoodbyeAnimation, GoNewMenuAnimation))
                .AddButton("Quit", "Quit", (ctx) => GameManager.Quit());

            var hSeparator = new HSeparator();
            hSeparator.Name = "Sep";
            mainMenu.AddMenu("Options", (menu) => { GD.Print(menu.Name); })
                .AddButton("Video", "Video", (ctx) => { ctx.Go("Video", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("Controls", "Controls", () => GD.Print("Controls"))
                .AddHSeparator()
                .AddButton("Back", "Back", (ctx) =>
                    ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation)
                );

            mainMenu.AddMenu("Video")
                .AddCheckButton("Fullscreen", "Fullscreen", (ActionCheckButton.Context ctx) => {
                    ScreenManager.SetFullscreen(ctx.Pressed);
                    _borderless.Pressed = OS.WindowBorderless;
                    _scale.Disabled = _borderless.Disabled = ctx.Pressed;
                    ctx.Refresh();
                })
                .AddCheckButton("PixelPerfect", "Pixel perfect", (ActionCheckButton.Context ctx) => {
                    ScreenManager.SetPixelPerfect(ctx.Pressed);
                })
                .AddButton("Scale", "", (ActionButton.InputEventContext ctx) => {
                    List<ScaledResolution> resolutions = ScreenManager.GetResolutions();
                    Resolution resolution = ScreenManager.CurrentWindowedResolution;
                    var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == resolution.Size);
                    pos = pos == -1 ? 0 : pos;

                    if (ctx.InputEvent.IsActionReleased("ui_left")) {
                        if (pos > 0) ScreenManager.SetWindowed(resolutions[pos - 1]);
                    } else if (ctx.InputEvent.IsActionReleased("ui_right")) {
                        if (pos < resolutions.Count - 1) ScreenManager.SetWindowed(resolutions[pos + 1]);
                    } else if (ctx.InputEvent.IsActionReleased("ui_accept")) {
                        ScreenManager.SetWindowed(pos == resolutions.Count - 1
                            ? resolutions[0]
                            : resolutions[pos + 1]);
                    }
                    UpdateResolutionButton();
                })
                .AddCheckButton("Borderless", "Borderless window", (ctx) => {
                    ScreenManager.SetBorderless(ctx.Pressed);
                    // UpdateResolutionButton();
                    // ctx.Menu.GetCheckButton("Fullscreen")!.Pressed = ScreenManager.IsFullscreen();
                })
                .AddButton("Back", "Back", (ctx) =>
                    ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation)
                );

            mainMenu.GetMenu("Root").GetButton("Continue")!.Disabled = true;

            _videoMenu = mainMenu.GetMenu("Video");
            _fullscreenButton = _videoMenu.GetCheckButton("Fullscreen")!;
            _scale = mainMenu.GetMenu("Video").GetButton("Scale")!;
            _borderless = mainMenu.GetMenu("Video").GetCheckButton("Borderless")!;

            _fullscreenButton.Pressed = ScreenManager.IsFullscreen();
            _videoMenu.GetCheckButton("PixelPerfect").Pressed = ScreenManager.CurrentPixelPerfect;
            _borderless.Pressed = OS.WindowBorderless;
            _scale.Disabled = _borderless.Disabled = ScreenManager.IsFullscreen();
            _scale.OnFocusEntered(UpdateResolutionButton);
            _scale.OnFocusExited(UpdateResolutionButton);
            _scale.Menu.Refresh();
            return mainMenu;
        }

        private ActionMenu _videoMenu;
        private ActionCheckButton _fullscreenButton;
        private ActionButton _scale;
        private ActionCheckButton _borderless;


        private void UpdateResolutionButton() {
            List<ScaledResolution> resolutions = ScreenManager.GetResolutions();
            Resolution resolution = ScreenManager.CurrentWindowedResolution;
            var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == resolution.Size);
            pos = pos == -1 ? 0 : pos;

            var prefix = "";
            var suffix = "";
            if (_menuController!.ActiveMenu!.GetFocusOwner() == _scale) {
                prefix = pos > 0 ? "< " : "";
                suffix = pos < resolutions.Count - 1 ? " >" : "";
            }
            ScaledResolution scaledResolution = resolutions[pos];
            var res = scaledResolution.ToString();
            if (scaledResolution.IsPixelPerfectScale()) {
                if (scaledResolution.GetPixelPerfectScale() == 1) {
                    res += " (Original)";
                } else {
                    res += " (x" + scaledResolution.GetPixelPerfectScale() + ")";
                }
            }
            _scale!.Text = prefix + res + suffix;
        }

        private async Task GoGoodbyeAnimation(MenuTransition transition) {
            // await _launcher.Play(Template.BackOutLeftFactory.Get(150), transition.FromMenu.Control, 0f, MenuEffectTime).Await();
            // await _launcher.Play(Template.FadeOut, transition.FromButton, 0f, MenuEffectTime*2).Await();
            GD.Print("Go1");
            LoopStatus lastToWaitFor = null;
            int x = 0;
            foreach (var child in transition.FromMenu.GetChildren()) {
                if (child is Control control) {
                    // actionButton.Modulate =
                    // new Color(actionButton.Modulate.r, actionButton.Modulate.g, actionButton.Modulate.b, 0);
                    lastToWaitFor = _launcher.Play(Template.FadeOutLeft, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
            // GD.Print("Go2");
            // await _launcher.Play(Template.FadeOutDown, transition.FromMenu.CanvasItem, 0f, 0.25f).Await();
        }

        private async Task GoNewMenuAnimation(MenuTransition transition) {
            int x = 0;
            GD.Print("Go3");
            LoopStatus lastToWaitFor = null;
            foreach (var child in transition.ToMenu.GetChildren()) {
                if (child is Control control) {
                    control.Modulate = new Color(1f, 1f, 1f, 0f);
                    lastToWaitFor = _launcher.Play(Template.FadeInRight, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
            // await _launcher.Play(Template.BackInRightFactory.Get(200), _menuHolder, 0f, MenuEffectTime).Await();
        }


        private async Task BackGoodbyeAnimation(MenuTransition transition) {
            LoopStatus lastToWaitFor = null;
            int x = 0;
            foreach (var child in transition.FromMenu.GetChildren()) {
                if (child is Control control) {
                    // control.Modulate = new Color(1f,1f,1f, 0f);
                    lastToWaitFor = _launcher.Play(Template.FadeOutRight, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
            // await _launcher.Play(Template.BackOutRightFactory.Get(200), transition.FromMenu.CanvasItem, 0f,
            // MenuEffectTime)
            // .Await();
        }

        private async Task BackNewMenuAnimation(MenuTransition transition) {
            // await _launcher.Play(Template.BackInLeftFactory.Get(150), transition.ToMenu.CanvasItem, 0f,
            // MenuEffectTime)
            // .Await();
            LoopStatus lastToWaitFor = null;
            int x = 0;
            foreach (var child in transition.ToMenu.GetChildren()) {
                if (child is Control control) {
                    control.Modulate = new Color(1f, 1f, 1f, 0f);
                    lastToWaitFor = _launcher.Play(Template.FadeInLeft, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
        }

        private const float MenuEffectTime = 0.10f;

        public override void _Input(InputEvent @event) {
            // if (@event.IsAction("ui_up") ||
            //     @event.IsAction("ui_down") ||
            //     @event.IsAction("ui_left") ||
            //     @event.IsAction("ui_right") ||
            //     @event.IsAction("ui_page_up") ||
            //     @event.IsAction("ui_page_down") ||
            //     @event.IsAction("ui_home") ||
            //     @event.IsAction("ui_focus_next") ||
            //     @event.IsAction("ui_focus_prev") ||
            //     @event.IsAction("ui_accept") ||
            //     @event.IsAction("ui_cancel")) {
            // }

            if (@event.IsAction("ui_cancel")) {
                _menuController.Back(BackGoodbyeAnimation, BackNewMenuAnimation);
            }

            var action = InputManager.OnEvent(@event);
            if (action != null) {
                // _sceneTree.SetInputAsHandled();
                // InputMap.Singleton
            }
            InputManager.Debug(action);
        }


        // private float delay = 0f;
        // public override void _Process(float delta) {
        // delay += delta;
        // if (delay > 5f) {
        // GameManager.Quit();
        // }

        // }
    }
}