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
            _menuController = await BuildMenu();
            // Go(_mainMenu, _options, _optionsMenu);
        }

        public async Task<MenuController> BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("NewGame", "New game", (ctx) => {
                    GD.Print("New Game");
                    var continueButton = ctx.Menu.GetButton("Continue");
                    continueButton!.Disabled = !continueButton.Disabled;
                    continueButton.Restore();
                    ctx.Refresh();
                })
                .AddButton("Continue", "Continue", async (ctx) => {
                    GD.Print("Continue");
                    ctx.ActionButton.Disabled = true;
                    await ctx.Refresh();
                    await _launcher.Play(Template.FadeOut, ctx.ActionButton, 0f,
                            MenuEffectTime)
                        .Await();
                })
                .AddButton("Options", "Options",
                    (ctx) => ctx.Go("Options", GoGoodbyeAnimation, GoNewMenuAnimation))
                .AddButton("Quit", "Quit", (ctx) => GameManager.Quit())
                .Save();


            mainMenu.AddMenu("Options")
                .AddButton("Video", "Video", () => { GD.Print("New Game"); })
                .AddButton("Controls", "Controls", () => GD.Print("Controls"))
                .AddNode(new HSeparator())
                .AddCheckButton("Sound", "Sound", (ctx) => GD.Print("Options "+ctx.ActionCheckButton.Pressed))
                .AddButton("Back", "Back", (ctx) =>
                    ctx.Back(BackGoodbyeAnimation, BackNewMenuAnimation)
                )
                .Save();

            mainMenu.GetMenu("Root").GetButton("Continue")!.Disabled = true;
            mainMenu.GetMenu("Options").GetCheckButton("Sound")!.Pressed = true;

            await mainMenu.Show("Root");
            return mainMenu;
        }

        private async Task GoGoodbyeAnimation(MenuTransition transition) {
            // await _launcher.Play(Template.BackOutLeftFactory.Get(150), transition.FromMenu.Control, 0f, MenuEffectTime).Await();
            GD.Print("Go1");
            await _launcher.Play(Template.RollOut, transition.FromButton, 0f, 0.25f).Await();
            GD.Print("Go2");
            await _launcher.Play(Template.FadeOutDown, transition.FromMenu.Parent, 0f, 0.25f).Await();
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
            foreach (var button in transition.ToMenu.GetButtons()) {
                var actionButton = button as ActionButton;
                // actionButton.Modulate =
                // new Color(actionButton.Modulate.r, actionButton.Modulate.g, actionButton.Modulate.b, 0);
                lastToWaitFor = _launcher.Play(s, actionButton, x * 0.05f, MenuEffectTime);
                x++;
            }
            await lastToWaitFor.Await();
            // await _launcher.Play(Template.BackInRightFactory.Get(200), _menuHolder, 0f, MenuEffectTime).Await();
        }


        private async Task BackGoodbyeAnimation(MenuTransition transition) {
            await _launcher.Play(Template.BackOutRightFactory.Get(200), transition.FromMenu.Parent, 0f,
                    MenuEffectTime)
                .Await();
        }

        private async Task BackNewMenuAnimation(MenuTransition transition) {
            await _launcher.Play(Template.BackInLeftFactory.Get(150), transition.ToMenu.Parent, 0f,
                    MenuEffectTime)
                .Await();
        }

        private const float MenuEffectTime = 0.25f;

        public override void _Input(InputEvent @event) {
            if (@event.IsAction("ui_cancel")) {
                _menuController.Back();
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