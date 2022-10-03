using System;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Bus;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.Restorer;
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

        [Inject] private Multicast<MenuFlowManager.Transition> MenuFlowBus { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }

        public override void _Ready() {
            _version.Text = AppInfo.Version + " - Betauer 2022";
            _menuContainer = BuildMenu();
        }

        public async Task ShowMenu() {
            GetTree().Root.GuiDisableInput = true;
            Visible = true;
            var modulate = Colors.White;
            modulate.a = 0;
            Modulate = modulate;
            await _menuContainer.Start();
            await Templates.FadeIn.Play(this, 0f, FadeMainMenuEffectTime).AwaitFinished();
            GetTree().Root.GuiDisableInput = false;
        }

        public async Task HideMainMenu() {
            GetTree().Root.GuiDisableInput = true;
            await Templates.FadeOut.Play(this, 0f, FadeMainMenuEffectTime).AwaitFinished();
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
            startMenu.AddButton("Start", "Start").OnPressed(() => MenuFlowBus.Publish(MenuFlowManager.Transition.StartGame));
            startMenu.AddButton("Settings", "Settings").OnPressed(() => MenuFlowBus.Publish(MenuFlowManager.Transition.Settings));
            startMenu.AddButton("Exit", "Exit").OnPressed(() => MenuFlowBus.Publish(MenuFlowManager.Transition.ExitDesktop));
            return mainMenu;
        }

        private SceneTreeTween _sceneTreeTween;
        public void DimOut() {
            _sceneTreeTween?.Kill();
            _sceneTreeTween = Templates.FadeOut.Play(this, 0f, 1f);
        }

        public void RollbackDimOut() {
            _sceneTreeTween?.Kill();
            _sceneTreeTween = null;
            Modulate = Colors.White;
        }

        public void OnInput(InputEvent e) {
            if (UiCancel.IsEventJustPressed(e)) { 
                if (_menuContainer.IsStartMenuActive()) {
                    MenuFlowBus.Publish(MenuFlowManager.Transition.ModalBoxConfirmExitDesktop);
                } else {
                    _menuContainer.Back();
                }
                GetTree().SetInputAsHandled();
            }
        }

    }
}