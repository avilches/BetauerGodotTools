using System;
using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Input;

using Betauer.UI;
using Godot;

namespace Veronenger.Game.Controller.Menu {
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

        private MenuController _menuController;

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

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

            _menuController = BuildMenu();
            await _menuController.Start("Root");
        }

        public Task<bool> AwaitResult() {
            return _promise.Task;
        }

        public MenuController BuildMenu() {
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("No", "No", (ctx) => {
                    _promise.TrySetResult(false);
                })
                .AddButton("Yes", "Yes", (ctx) => {
                    _promise.TrySetResult(true);
                });
            var noButton = mainMenu.GetMenu("Root")!.GetButton("No");
            var yesButton = mainMenu.GetMenu("Root")!.GetButton("Yes");
            noButton!.RectMinSize = yesButton!.RectMinSize = new Vector2(60, 0);
            return mainMenu;
        }

        public override void _Process(float delta) {
            if (UiCancel.JustPressed) {
                _promise.TrySetResult(false);
            }
        }
    }
}