using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(MainMenu));
        [Inject] public InputManager InputManager;
        [Inject] public GameManager GameManager;

        private SceneTree _sceneTree;

        private MultipleActionMenu _menu;

        private Launcher _launcher;


        public override async void Ready() {
            Engine.TimeScale = 0.25f;
            _sceneTree = GetTree();
            _launcher = new Launcher().CreateNewTween(this);
            await BuildMenu();
            // Go(_mainMenu, _options, _optionsMenu);
        }

        public async Task BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            _menu = new MultipleActionMenu(_menuBase);
            BuildMainMenu();
            BuildOptionsMenu();
            await _menu.Show("Root");
        }

        private void BuildMainMenu() {
            _menu.AddMenu("Root")
                .AddButton("NewGame", "New game", async (ActionButton but) => {
                    GD.Print("New Game");
                    var _continue = but.Menu.GetButton("Continue");
                    _continue.Disabled = !_continue.Disabled;
                    await but.Refresh();
                })
                .AddButton("Continue", "Continue", async (ActionButton but) => {
                    GD.Print("Continue");
                    but.Disabled = true;
                    await but.Refresh();
                })
                .AddButton("Options", "Options",
                    (ActionButton but) => { but.Go("Options", GoGoodbyeAnimation, GoNewMenuAnimation); })
                .AddButton("Quit", "Quit", (ActionButton but) => { GameManager.Quit(); });

            _menu.GetMenu("Root").GetButton("Continue").Disabled = true;
        }

        private async Task GoGoodbyeAnimation(MenuTransition transition) {
            // await _launcher.Play(Template.BackOutLeftFactory.Get(150), _menuHolder, 0f, MenuEffectTime).Await();
            GD.Print("Go1");
            await _launcher.Play(Template.RollOut, transition.FromButton, 0f, 0.25f).Await();
            GD.Print("Go2");
            await _launcher.Play(Template.FadeOutDown, transition.FromMenu.Holder, 0f, 0.25f).Await();
        }

        private async Task GoNewMenuAnimation(MenuTransition transition) {
            int x = 0;
            GD.Print("Go3");
            foreach (var button in transition.ToMenu.GetButtons()) {
                _launcher.Play(Template.RotateInDownRight, button as Control, x * 0.05f, MenuEffectTime)
                    .Await();
                x++;
            }
            // await _launcher.Play(Template.BackInRightFactory.Get(200), _menuHolder, 0f, MenuEffectTime).Await();
        }


        private void BuildOptionsMenu() {
            _menu.AddMenu("Options")
                .AddButton("Video","Video", async (ActionButton but) => { GD.Print("New Game"); })
                .AddButton("Controls","Controls", (ActionButton but) => GD.Print("Controls"))
                .AddButton("Sound","Sound", (ActionButton but) => GD.Print("Options"))
                .AddButton("Back","Back", async (ActionButton but) =>
                    but.Back(BackGoodbyeAnimation, BackNewMenuAnimation)
                );
        }

        private async Task BackGoodbyeAnimation(MenuTransition transition) {
            await _launcher.Play(Template.BackOutRightFactory.Get(200), transition.FromMenu.Holder, 0f,
                    MenuEffectTime)
                .Await();
        }

        private async Task BackNewMenuAnimation(MenuTransition transition) {
            await _launcher.Play(Template.BackInLeftFactory.Get(150), transition.ToMenu.Holder, 0f,
                    MenuEffectTime)
                .Await();
        }

        private const float MenuEffectTime = 0.25f;


        // private float delay = 0f;
        // public override void _Process(float delta) {
        // delay += delta;
        // if (delay > 5f) {
        // GameManager.Quit();
        // }

        // }
    }
}