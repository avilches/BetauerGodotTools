using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.Tests.Animation;
using Betauer.UI;
using Godot;
using NUnit.Framework;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public partial class MainMenu : DiControl {
        [OnReady("MarginContainer/HBoxContainer/VBoxContainer/Menu")]
        private VBoxContainer _menuHolder;

        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(MainMenu));
        [Inject] public InputManager InputManager;
        [Inject] public GameManager GameManager;

        private SceneTree _sceneTree;

        private ActionMenu _mainMenu;
        private ActionMenu _optionsMenu;

        private ActionButton _newGame;
        private ActionButton _continue;
        private ActionButton _options;
        private ActionButton _quit;
        private Launcher _launcher;
        private IRestorer _saver;

        private readonly LinkedList<ActionState> _nestedPathMenus = new LinkedList<ActionState>();

        private struct ActionState {
            public ActionMenu Menu;
            public ActionButton Button;

            public ActionState(ActionMenu menu, ActionButton button) {
                Menu = menu;
                Button = button;
            }
        }

        public override void Ready() {
            _sceneTree = GetTree();
            _launcher = new Launcher().CreateNewTween(this);
            BuildMenu();
            // Go(_mainMenu, _options, _optionsMenu);
        }



        public void BuildMenu() {
            BuildMainMenu();
            BuildOptionsMenu();
            _nestedPathMenus.Clear();
            _saver = _menuHolder.Save();
            _mainMenu.Refresh();
        }

        private void BuildMainMenu() {
            _mainMenu = new ActionMenu(_menuHolder);

            // Main menu
            _newGame = _mainMenu.CreateButton("New game", async () => {
                GD.Print("New Game");
            });
            _continue = _mainMenu.CreateButton("Continue", () => {
                GD.Print("Continue");
            });
            _options = _mainMenu.CreateButton("Options", async () => {
                await Go(_mainMenu, _options, _optionsMenu);
            });
            _quit = _mainMenu.CreateButton("Quit", () => {
                GameManager.Quit();
            });
        }


        private void BuildOptionsMenu() {
            _optionsMenu = new ActionMenu(_menuHolder);
            // Options menu
            // _optionsMenu.CreateButton("Video", async () => { GD.Print("New Game"); });
            // _optionsMenu.CreateButton("Controls", () => GD.Print("Controls"));
            // _optionsMenu.CreateButton("Sound", () => GD.Print("Options"));
            _optionsMenu.CreateButton("Back", async () => {
                Back();
            });
        }

        private async Task Go(ActionMenu fromMenu, ActionButton fromButton, ActionMenu toMenu) {
            _nestedPathMenus.AddLast(new ActionState(fromMenu, fromButton));
            await _launcher.Play(Template.FadeOutLeft, _menuHolder, 0f, 0.01f).Await();
            _saver.Rollback();
            toMenu.Refresh();
            await _launcher.Play(Template.FadeInRight, _menuHolder, 0f, 0.01f).Await();
            // Back();
        }

        private async Task Back() {
            if (_nestedPathMenus.Count > 0) {
                ActionState lastState =_nestedPathMenus.Last();
                _nestedPathMenus.RemoveLast();
                await _launcher.Play(Template.FadeOutRight, _menuHolder, 0f, 0.01f).Await();
                _saver.Rollback();
                lastState.Menu.Refresh();
                await lastState.Menu.Focus(lastState.Button);
                await _launcher.Play(Template.FadeInLeft, _menuHolder, 0f, 0.01f).Await();
                // Go(_mainMenu, _options, _optionsMenu);
            }
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