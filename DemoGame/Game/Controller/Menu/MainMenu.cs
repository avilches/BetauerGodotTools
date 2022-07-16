using System;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.Signal;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class MainMenu : Control {
        private const float FadeMainMenuEffectTime = 0.75f;


        [OnReady("GridContainer/MarginContainer/VBoxContainer/Menu")]
        private Godot.Container _menuBase;

        [OnReady("GridContainer/MarginContainer/VBoxContainer/VBoxContainer/ver")]
        private Label _version;

        private MenuContainer _menuContainer;

        [Inject] private GameManager _gameManager;
        private readonly Launcher _launcher = new Launcher();

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

        public override void _Ready() {
            _version.Text = AppInfo.Version + " - Betauer 2022";
            _launcher.WithParent(this);
            _menuContainer = BuildMenu();
        }

        public async Task ShowMenu() {
            GetTree().Root.GuiDisableInput = true;
            Visible = true;
            var modulate = Colors.White;
            modulate.a = 0;
            Modulate = modulate;
            await _menuContainer.Start();
            await _launcher.Play(Template.FadeIn, this, 0f, FadeMainMenuEffectTime).Await();
            GetTree().Root.GuiDisableInput = false;
        }

        public async Task HideMainMenu() {
            GetTree().Root.GuiDisableInput = true;
            await _launcher.Play(Template.FadeOut, this, 0f, FadeMainMenuEffectTime).Await();
            Visible = false;
            GetTree().Root.GuiDisableInput = false;
        }

        private Restorer _menuRestorer;

        public void DisableMenus() {
            _menuRestorer = _menuContainer.ActiveMenu.DisableButtons();
        }

        public void EnableMenus() {
            _menuRestorer?.Restore();
        }

        public MenuContainer BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuContainer(_menuBase);
            var startMenu = mainMenu.GetStartMenu();
            startMenu.AddButton("Start", "Start").OnPressed(() => _gameManager.TriggerStartGame()).Unwatch();
            startMenu.AddButton("Settings", "Settings").OnPressed(() => _gameManager.TriggerSettings()).Unwatch();
            startMenu.AddButton("Exit", "Exit").OnPressed(() => _gameManager.TriggerModalBoxConfirmExitDesktop()).Unwatch();
            return mainMenu;
        }

        public void DimOut() {
            _launcher.Play(Template.FadeOut, this, 0f, 1f).Await();
        }

        public void RollbackDimOut() {
            _launcher.RemoveAll();
            Modulate = Colors.White;
        }

        public void Execute() {
            if (UiCancel.JustPressed) { 
                if (_menuContainer.IsStartMenuActive()) {
                    _gameManager.TriggerModalBoxConfirmExitDesktop();
                } else {
                    _menuContainer.Back();
                }
            }
        }
    }
}