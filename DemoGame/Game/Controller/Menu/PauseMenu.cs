using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
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

        private MenuController _menuController;

        [Inject] private GameManager _gameManager;

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

        private readonly Launcher _launcher = new Launcher();

        public override void _Ready() {
            _launcher.WithParent(this);
            _menuController = BuildMenu();
            HidePauseMenu();
        }

        public async Task ShowPauseMenu() {
            _container.Show();
            _launcher.Play(PartialFadeOut, _backgroundFader, 0f, 0.5f);
            await _menuController.Start();
        }

        public void HidePauseMenu() {
            _launcher.RemoveAll();
            _container.Hide();
        }

        private Restorer _menuRestorer;

        public void DisableMenus() {
            _menuRestorer = _menuController.ActiveMenu.DisableButtons(); // TODO: .AddFocusRestorer();
        }

        public void EnableMenus() {
            _menuRestorer?.Restore();
        }

        public MenuController BuildMenu() {
            // TODO i18n
            _title.Text = "Paused";
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            var menu = mainMenu.GetStartMenu();
            menu.AddButton("Resume", "Resume").OnPressed(() => _gameManager.TriggerBack());
            menu.AddButton("Settings", "Settings").OnPressed(() => _gameManager.TriggerSettings());
            menu.AddButton("QuitGame", "Quit game").OnPressed(() => _gameManager.TriggerModalBoxConfirmQuitGame());
            return mainMenu;
        }

        public bool IsRootMenuActive() {
            return _menuController.IsStartMenuActive();
        }

        public async Task BackMenu() {
            await _menuController.Back();
        }

    }
}