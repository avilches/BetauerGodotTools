using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class MainMenu : DiControl {
        private const float MenuEffectTime = 0.10f;
        private const float FadeMainMenuEffectTime = 0.75f;


        [OnReady("GridContainer/MarginContainer/VBoxContainer/Menu")]
        private Godot.Container _menuBase;

        private MenuController _menuController;

        [Inject] private GameManager _gameManager;
        [Inject] private InputManager _inputManager;

        private ActionState UiCancel => _inputManager.UiCancel;
        private ActionButton _optionsButton;

        public override async void Ready() {
            _menuController = BuildMenu();
            _optionsButton = _menuController.GetMenu("Root")!.GetButton("Settings");
            await ShowMenu();
        }

        public async Task ShowMenu() {
            GetTree().Root.GuiDisableInput = true;
            Visible = true;
            var modulate = Colors.White;
            modulate.a = 0;
            Modulate = modulate;
            await _menuController.Start("Root");
            GetTree().Root.GuiDisableInput = false;
            await _gameManager.Launcher.Play(Template.FadeIn, this, 0f, FadeMainMenuEffectTime).Await();
        }

        public void FocusOptions() {
            _optionsButton.GrabFocus();
        }

        public async Task HideMainMenu() {
            GetTree().Root.GuiDisableInput = true;
            await _gameManager.Launcher.Play(Template.FadeOut, this, 0f, FadeMainMenuEffectTime).Await();
            Visible = false;
            GetTree().Root.GuiDisableInput = false;
        }

        public MenuController BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("Start", "Start", (ctx) => {
                    _gameManager.StartGame();
                    // var continueButton = ctx.Menu.GetButton("Continue");
                    // continueButton!.Disabled = !continueButton.Disabled;
                    // await _launcher.Play(Template.FadeIn, continueButton, 0f,
                    //         MenuEffectTime)
                    //     .Await();
                    // continueButton.Save();
                    // ctx.Refresh();
                })
                // .AddButton("Continue", "Continue", async (ctx) => {
                //     // GD.Print("Continue");
                //     // ctx.ActionButton.Disabled = true;
                //     // ctx.Refresh();
                //     // await _launcher.Play(Template.FadeOut, ctx.ActionButton, 0f,
                //     //         MenuEffectTime)
                //     //     .Await();
                //     // ctx.ActionButton.Save();
                //     _gameManager.LoadAnimaDemo();
                // })
                .AddButton("Settings", "Settings", async (ctx) => {
                    _gameManager.ShowOptionsMenu();
                })
                .AddButton("Exit", "Exit", async (ctx) => {
                    // QueueFree();
                    _gameManager.Launcher.Play(Template.FadeOut, this, 0f, 0.3f).Await();
                    var exit = await _gameManager.ModalBoxConfirmExitDesktop();
                    if (exit) {
                        // await _gameManager.Launcher.Play(Template.FadeOut, this, 0f, 0.2f).Await();
                        GetTree().Notification(MainLoop.NotificationWmQuitRequest);
                    } else {
                        _gameManager.Launcher.RemoveAll();
                        Modulate = Colors.White;
                        ctx.ActionButton.GrabFocus();
                    }
                });

            return mainMenu;
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
                    lastToWaitFor = _gameManager.Launcher.Play(Template.FadeOutLeft, control, x * 0.05f, MenuEffectTime);
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
                    lastToWaitFor = _gameManager.Launcher.Play(Template.FadeInRight, control, x * 0.05f, MenuEffectTime);
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
                    lastToWaitFor = _gameManager.Launcher.Play(Template.FadeOutRight, control, x * 0.05f, MenuEffectTime);
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
                    lastToWaitFor = _gameManager.Launcher.Play(Template.FadeInLeft, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
        }

        public override void _Input(InputEvent @event) {
            if (_gameManager.IsMainMenu() && UiCancel.IsEventPressed(@event)) {
                _menuController.Back(BackGoodbyeAnimation, BackNewMenuAnimation);
            }
        }
    }
}