using System.Threading.Tasks;
using Betauer.Animation.Tween;
using Betauer.Bus;
using Betauer.DI;
using Betauer.Input;
using Betauer.Nodes.Property;
using Betauer.OnReady;
using Betauer.Restorer;
using Betauer.Signal;
using Betauer.UI;
using Godot;
using Veronenger.Managers;
using Container = Godot.Container;

namespace Veronenger.Controller.Menu {
    public class PauseMenu : CanvasLayer {
        private static readonly KeyframeAnimation PartialFadeOut = KeyframeAnimation.Create()
            .SetDuration(0.3f)
            .AnimateKeys(Properties.Opacity)
            .From(0)
            .KeyframeTo(1f, 0.4f)
            .EndAnimate();

        [OnReady("BackgroundFader")] private ColorRect _backgroundFader;
        [OnReady("Node2D/BlackBar")] private ColorRect _blackBar;

        [OnReady("CenterContainer")]
        private Container _centerContainer;

        [OnReady("CenterContainer/VBoxContainer/Menu")]
        private Container _menuBase;

        [OnReady("CenterContainer/VBoxContainer/Title")]
        private Label _title;

        private MenuContainer _menuContainer;

        [Inject] private MainStateMachine MainStateMachine { get; set; }

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction ControllerStart { get; set; }
        [Inject] private Bus Bus { get; set; }

        public override void _Ready() {
            _menuContainer = BuildMenu();
            Hide();
            GetTree().OnScreenResized(_BlackBarPosition).DisconnectIfInvalid(this);
        }

        public Task ShowPauseMenu() {
            Show();
            PartialFadeOut.Play(_backgroundFader, 0f, 0.5f);
            _blackBar.RectSize = new Vector2(20000, _blackBar.RectSize.y);
            _BlackBarPosition();
            return _menuContainer.Start();
        }

        private void _BlackBarPosition() {
            _blackBar.RectPosition = new Vector2(-10000, _centerContainer.RectPosition.y);
        }

        public void HidePauseMenu() {
            // _launcher.RemoveAll();
            Hide();
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
            var startMenu = mainMenu.GetRootMenu();
            startMenu.AddButton("Resume", "Resume").OnPressed(() => Bus.Publish(MainEvent.Back));
            startMenu.AddButton("Settings", "Settings").OnPressed(() => Bus.Publish(MainEvent.Settings));
            startMenu.AddButton("QuitGame", "Quit game").OnPressed(() => Bus.Publish(MainEvent.ModalBoxConfirmQuitGame));
            return mainMenu;
        }

        public void OnInput(InputEvent e) {
            if (UiCancel.IsEventJustPressed(e)) {
                if (_menuContainer.IsRootMenuActive()) {
                    Bus.Publish(MainEvent.Back);
                } else {
                    _menuContainer.Back();
                }
                GetTree().SetInputAsHandled();

            } else if (ControllerStart.IsJustPressed()) {
                Bus.Publish(MainEvent.Back);
                GetTree().SetInputAsHandled();
                
            }
        }
    }
}