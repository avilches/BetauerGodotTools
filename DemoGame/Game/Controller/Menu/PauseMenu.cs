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
using Container = Godot.Container;

namespace Veronenger.Game.Controller.Menu {
    public class PauseMenu : CanvasLayer {
        private static readonly SequenceTemplate PartialFadeOut = TemplateBuilder.Create()
            .SetDuration(0.3f)
            .AnimateKeys(Property.Opacity)
            .KeyframeTo(0f, 0f)
            .KeyframeTo(1f, 0.8f)
            .EndAnimate()
            .BuildTemplate();

        [OnReady("Node2D")] private Node2D _container;

        [OnReady("Node2D/BackgroundFader")] private ColorRect _backgroundFader;

        [OnReady("Node2D/CenterContainer/VBoxContainer/Menu")]
        private Container _menuBase;

        [OnReady("Node2D/CenterContainer/VBoxContainer/Title")]
        private Label _title;

        private MenuContainer _menuContainer;

        [Inject] private GameManager _gameManager;

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

        private readonly Launcher _launcher = new Launcher();

        public override void _Ready() {
            _launcher.WithParent(this);
            _menuContainer = BuildMenu();
            HidePauseMenu();
        }

        public async Task ShowPauseMenu() {
            _container.Show();
            _launcher.Play(PartialFadeOut, _backgroundFader, 0f, 0.5f);
            await _menuContainer.Start();
        }

        public void HidePauseMenu() {
            _launcher.RemoveAll();
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

        public async Task Execute() {
            if (UiCancel.JustPressed) {
                if (_menuContainer.IsStartMenuActive()) {
                    _gameManager.TriggerBack();
                } else {
                    await _menuContainer.Back();
                }
            } else if (UiStart.JustPressed) {
                _gameManager.TriggerBack();
            }
        }
    }
}