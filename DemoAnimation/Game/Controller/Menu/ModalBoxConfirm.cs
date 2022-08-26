using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Input;
using Betauer.OnReady;
using Betauer.Signal;
using Betauer.UI;
using Godot;

namespace DemoAnimation.Game.Controller.Menu {
    public class ModalBoxConfirm : Node {
        [OnReady("Panel/VBoxContainer/Menu")]
        private Godot.Container _menuBase;

        [OnReady("Panel/VBoxContainer/VBoxContainerDouble")]
        private Godot.Container doubleContainer;

        [OnReady("Panel/VBoxContainer/VBoxContainerDouble/Title")]
        private Label _title;
        [OnReady("Panel/VBoxContainer/VBoxContainerDouble/Subtitle")]
        private Label _subtitle;

        [OnReady("Panel/VBoxContainer/VBoxContainerSingle")]
        private Godot.Container singleContainer;

        [OnReady("Panel/VBoxContainer/VBoxContainerSingle/Title")]
        private Label _singleTitle;

        [OnReady("ColorRect")] private ColorRect _colorRect;

        private MenuContainer _menuContainer;

        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }

        private readonly TaskCompletionSource<bool> _promise = new TaskCompletionSource<bool>();

        private string? _titleText;
        private string? _subtitleText;

        public void Title(string title, string subtitle = null) {
            _titleText = title;
            _subtitleText = subtitle;
        }

        public override async void _Ready() {
            // TODO i18n
            if (_subtitleText != null) {
                doubleContainer.Visible = true;
                singleContainer.Visible = false;
                _title.Text = _titleText;
                _subtitle.Text = _subtitleText;
            } else {
                doubleContainer.Visible = false;
                singleContainer.Visible = true;
                _singleTitle.Text = _titleText;
            }

            _menuContainer = BuildMenu();
            await _menuContainer.Start();
        }

        public Task<bool> AwaitResult() {
            return _promise.Task;
        }

        public MenuContainer BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuContainer(_menuBase);
            var startMenu = mainMenu.GetStartMenu();
            var noButton = startMenu.AddButton("No", "No");
            var yesButton = startMenu.AddButton("Yes", "Yes");
            noButton.OnPressed(() => _promise.TrySetResult(false));
            yesButton.OnPressed(() => _promise.TrySetResult(true));
            noButton!.RectMinSize = yesButton!.RectMinSize = new Vector2(60, 0);
            return mainMenu;
        }

        public override void _Process(float delta) {
            if (UiCancel.IsJustPressed()) {
                _promise.TrySetResult(false);
            }
        }
    }
}