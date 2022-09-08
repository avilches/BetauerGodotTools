using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes.Property;
using Betauer.Nodes.Property.Callback;
using Betauer.OnReady;
using Betauer.Restorer;
using Betauer.Signal;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;
using Container = Godot.Container;

namespace Veronenger.Game.Controller.Menu {
    public class PauseMenu : CanvasLayer {
        private static readonly KeyframeAnimation PartialFadeOut = KeyframeAnimation.Create()
            .SetDuration(0.3f)
            .AnimateKeys(Properties.Opacity)
            .From(0)
            .KeyframeTo(1f, 0.4f)
            .EndAnimate();

        [OnReady("Node2D")] private Node2D _container;

        [OnReady("Node2D/BackgroundFader")] private ColorRect _backgroundFader;

        [OnReady("Node2D/CenterContainer/VBoxContainer/Menu")]
        private Container _menuBase;

        [OnReady("Node2D/CenterContainer/VBoxContainer/Title")]
        private Label _title;

        private MenuContainer _menuContainer;

        [Inject] private GameManager _gameManager { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }

        public override void _Ready() {
            _menuContainer = BuildMenu();
            _container.Hide();;
        }

        public async Task ShowPauseMenu() {
            _container.Show();
            PartialFadeOut.Play(_backgroundFader, 0f, 0.5f);
            await _menuContainer.Start();
        }

        public void HidePauseMenu() {
            // _launcher.RemoveAll();
            _container.Hide();
        }

        private Restorer _menuRestorer;

        public void DisableMenus() {
            _menuRestorer = _menuContainer.ActiveMenu.DisableButtons();
        }

        public void EnableMenus() {
            _menuRestorer?.Restore();
        }

        public MenuContainer BuildMenu() {
            // TODO i18n
            _title.Text = "Paused";
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuContainer(_menuBase);
            var startMenu = mainMenu.GetStartMenu();
            startMenu.AddButton("Resume", "Resume").OnPressed(() => _gameManager.TriggerBack());
            startMenu.AddButton("Settings", "Settings").OnPressed(() => _gameManager.TriggerSettings());
            startMenu.AddButton("QuitGame", "Quit game").OnPressed(() => _gameManager.TriggerModalBoxConfirmQuitGame());
            return mainMenu;
        }

        public void OnInput(InputEvent e) {
            if (UiCancel.IsEventJustPressed(e)) {
                if (_menuContainer.IsStartMenuActive()) {
                    _gameManager.TriggerBack();
                } else {
                    _menuContainer.Back();
                }
                GetTree().SetInputAsHandled();

            } else if (UiStart.IsJustPressed()) {
                _gameManager.TriggerBack();
                GetTree().SetInputAsHandled();
                
            }
        }
    }
}