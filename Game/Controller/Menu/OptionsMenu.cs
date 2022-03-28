using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.Screen;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class OptionsMenu : DiNode {
        [OnReady("MarginContainer/HBoxContainer/VBoxContainer/Menu")]
        private Godot.Container _menuBase;

        private MenuController _menuController;

        [Inject] private GameManager _gameManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private InputManager _inputManager;

        private ActionState UiAccept => _inputManager.UiAccept;
        private ActionState UiCancel => _inputManager.UiCancel;
        private ActionState UiStart => _inputManager.UiStart;

        private Launcher _launcher;

        public override async void Ready() {
            _launcher = new Launcher().WithParent(this);
            _menuController = BuildMenu();
            await ShowMenu();
            UpdateResolutionButton();
        }

        public async Task ShowMenu() {
            await _menuController.Start("Root");
        }

        public MenuController BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("Continue", "Continue", (ctx) => {
                    _gameManager.ClosePauseMenu();
                })
                .AddButton("Options", "Options",
                    (ctx) => ctx.Go("Options", GoGoodbyeAnimation, GoNewMenuAnimation))
                .AddButton("EndGame", "End game", async (ctx) => {
                    _gameManager.ClosePauseMenu();
                    await _gameManager.ExitGameAndBackToMainMenu();
                });

            mainMenu.AddMenu("Options")
                .AddButton("Video", "Video", (ctx) => { ctx.Go("Video", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("Controls", "Controls", () => GD.Print("Controls"))
                .AddHSeparator()
                .AddButton("Back", "Back", (ctx) =>
                    ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation)
                );

            mainMenu.AddMenu("Video")
                .AddCheckButton("Fullscreen", "Fullscreen", (ActionCheckButton.InputEventContext ctx) => {
                    if (!ctx.InputEvent.IsActionPressed("ui_left") &&
                        !ctx.InputEvent.IsActionPressed("ui_right") &&
                        !ctx.InputEvent.IsActionPressed("ui_accept")) return false;
                    var newState = !ctx.Pressed;
                    ctx.ActionCheckButton.Pressed = newState;
                    _scale.Disabled = _borderless.Disabled = newState;
                    _borderless.Pressed = false;
                    ctx.Refresh();

                    _screenManager.SetFullscreen(newState);
                    return true;
                })
                .AddCheckButton("PixelPerfect", "Pixel perfect", (ActionCheckButton.InputEventContext ctx) => {
                    if (!ctx.InputEvent.IsActionPressed("ui_left") &&
                        !ctx.InputEvent.IsActionPressed("ui_right") &&
                        !ctx.InputEvent.IsActionPressed("ui_accept")) return false;
                    var newState = !ctx.Pressed;
                    ctx.ActionCheckButton.Pressed = newState;
                    _screenManager.SetPixelPerfect(newState);
                    return true;
                })
                .AddButton("Scale", "", (ActionButton.InputEventContext ctx) => {
                    List<ScaledResolution> resolutions = _screenManager.GetResolutions();
                    Resolution resolution = _screenManager.Settings.WindowedResolution;
                    var pos = resolutions.FindIndex(scaledResolution => scaledResolution.Size == resolution.Size);
                    pos = pos == -1 ? 0 : pos;

                    if (ctx.InputEvent.IsActionPressed("ui_left")) {
                        if (pos > 0) {
                            _screenManager.SetWindowed(resolutions[pos - 1]);
                            UpdateResolutionButton();
                            return true;
                        }
                    } else if (ctx.InputEvent.IsActionPressed("ui_right")) {
                        if (pos < resolutions.Count - 1) {
                            _screenManager.SetWindowed(resolutions[pos + 1]);
                        } else {
                            _scale.Disabled = _borderless.Disabled = true;
                            _borderless.Pressed = false;
                            _fullscreenButton.Pressed = true;
                            _fullscreenButton.GrabFocus();
                            ctx.Refresh();

                            _screenManager.SetFullscreen(true);
                        }
                        UpdateResolutionButton();
                        return true;
                    } else if (ctx.InputEvent.IsActionPressed("ui_accept")) {
                        _screenManager.SetWindowed(pos == resolutions.Count - 1
                            ? resolutions[0]
                            : resolutions[pos + 1]);
                        UpdateResolutionButton();
                        return true;
                    }
                    return false;
                })
                .AddCheckButton("Borderless", "Borderless window", (ActionCheckButton.InputEventContext ctx) => {
                    if (!ctx.InputEvent.IsActionPressed("ui_left") &&
                        !ctx.InputEvent.IsActionPressed("ui_right") &&
                        !ctx.InputEvent.IsActionPressed("ui_accept")) return false;
                    var newState = !ctx.Pressed;
                    ctx.ActionCheckButton.Pressed = newState;
                    _screenManager.SetBorderless(newState);
                    return true;
                })
                .AddCheckButton("VSync", "Vertical Sync", (ActionCheckButton.InputEventContext ctx) => {
                    if (!ctx.InputEvent.IsActionPressed("ui_left") &&
                        !ctx.InputEvent.IsActionPressed("ui_right") &&
                        !ctx.InputEvent.IsActionPressed("ui_accept")) return false;
                    var newState = !ctx.Pressed;
                    ctx.ActionCheckButton.Pressed = newState;
                    _screenManager.SetVSync(newState);
                    return true;
                })
                .AddButton("Back", "Back", (ctx) =>
                    ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation)
                );

            // mainMenu.GetMenu("Root").GetButton("Continue")!.Disabled = true;

            _videoMenu = mainMenu.GetMenu("Video");
            _fullscreenButton = _videoMenu.GetCheckButton("Fullscreen")!;
            _scale = mainMenu.GetMenu("Video").GetButton("Scale")!;
            _scale.OnFocusEntered(UpdateResolutionButton);
            _scale.OnFocusExited(UpdateResolutionButton);
            _borderless = mainMenu.GetMenu("Video").GetCheckButton("Borderless")!;

            // Load data from settings
            _fullscreenButton.Pressed = _screenManager.Settings.Fullscreen;
            _videoMenu.GetCheckButton("PixelPerfect")!.Pressed = _screenManager.Settings.PixelPerfect;
            _videoMenu.GetCheckButton("VSync")!.Pressed = OS.VsyncEnabled;
            _borderless.Pressed = _screenManager.Settings.Borderless;
            _scale.Disabled = _borderless.Disabled = _screenManager.Settings.Fullscreen;

            _scale.Menu.Refresh();
            return mainMenu;
        }

        private ActionMenu _videoMenu;
        private ActionCheckButton _fullscreenButton;
        private ActionButton _scale;
        private ActionCheckButton _borderless;


        private void UpdateResolutionButton() {
            List<ScaledResolution> resolutions = _screenManager.GetResolutions();
            Resolution resolution = _screenManager.Settings.WindowedResolution;
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
            if (!_gameManager.IsGamePaused()) {
                return;
            }
            if (UiCancel.IsEventPressed(@event)) {
                if (_menuController.ActiveMenu?.Name == "Root") {
                    _gameManager.ClosePauseMenu();
                } else {
                    _menuController.Back(BackGoodbyeAnimation, BackNewMenuAnimation);
                }
            } else if (UiStart.IsEventPressed(@event)) {
                _gameManager.ClosePauseMenu();
            }
        }
    }
}