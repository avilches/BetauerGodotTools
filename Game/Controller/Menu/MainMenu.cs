using System;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
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
            _menuController.Start("Root");
            // Go(_mainMenu, _options, _optionsMenu);
        }

        public MenuController BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("NewGame", "New game", async (ctx) => {
                    GD.Print("New Game");
                    var continueButton = ctx.Menu.GetButton("Continue");
                    continueButton!.Disabled = !continueButton.Disabled;
                    await _launcher.Play(Template.FadeIn, continueButton, 0f,
                            MenuEffectTime)
                        .Await();
                    continueButton.Save();

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
            mainMenu.AddMenu("Options", (menu) => {
                    GD.Print(menu.Name);
                })
                .AddButton("Video", "Video", () => {
                    Exception e = null;
                    var eMessage = e.Message;
                    GD.Print("New Game");

                })
                .AddButton("Controls", "Controls", () => GD.Print("Controls"))
                .AddNode(hSeparator)
                .AddCheckButton("Sound", "Sound", (ctx) => {
                    GD.Print("Options " + ctx.ActionCheckButton.Pressed);
                    // hSeparator.GrabFocus();
                })
                .AddButton("Back", "Back", (ctx) =>
                    ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation)
                );

            mainMenu.GetMenu("Root").GetButton("Continue")!.Disabled = true;
            mainMenu.GetMenu("Options").GetCheckButton("Sound")!.Pressed = true;
            return mainMenu;
        }

        private async Task GoGoodbyeAnimation(MenuTransition transition) {
            // await _launcher.Play(Template.BackOutLeftFactory.Get(150), transition.FromMenu.Control, 0f, MenuEffectTime).Await();
            GD.Print("Go1");
            await _launcher.Play(Template.RollOut, transition.FromButton, 0f, 0.25f).Await();
            GD.Print("Go2");
            await _launcher.Play(Template.FadeOutDown, transition.FromMenu.CanvasItem, 0f, 0.25f).Await();
        }

        private async Task GoNewMenuAnimation(MenuTransition transition) {
            int x = 0;

            var s = SequenceBuilder.Create()
                .AnimateSteps(null, Property.Modulate)
                .From(new Color(1f, 0f, 0f)).To(new Color(1f, 1f, 1f, 1f), 0.25f)
                .EndAnimate()
                .Parallel()
                .ImportTemplate(Template.RotateInDownRight);

            GD.Print("Go3");
            LoopStatus lastToWaitFor = null;
            foreach (var child in transition.ToMenu.GetChildren()) {
                if (child is Control control) {
                    // actionButton.Modulate =
                    // new Color(actionButton.Modulate.r, actionButton.Modulate.g, actionButton.Modulate.b, 0);
                    lastToWaitFor = _launcher.Play(s, control, x * 0.05f, MenuEffectTime);
                    x++;
                }
            }
            await lastToWaitFor.Await();
            // await _launcher.Play(Template.BackInRightFactory.Get(200), _menuHolder, 0f, MenuEffectTime).Await();
        }


        private async Task BackGoodbyeAnimation(MenuTransition transition) {
            await _launcher.Play(Template.BackOutRightFactory.Get(200), transition.FromMenu.CanvasItem, 0f,
                    MenuEffectTime)
                .Await();
        }

        private async Task BackNewMenuAnimation(MenuTransition transition) {
            await _launcher.Play(Template.BackInLeftFactory.Get(150), transition.ToMenu.CanvasItem, 0f,
                    MenuEffectTime)
                .Await();
        }

        private const float MenuEffectTime = 0.25f;

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