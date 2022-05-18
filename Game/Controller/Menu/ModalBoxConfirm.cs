using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class ModalBoxConfirm : DiNode {
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

        [Inject] private GameManager _gameManager;
        [Inject] private InputManager _inputManager;

        private ActionState UiAccept => _inputManager.UiAccept;
        private ActionState UiCancel => _inputManager.UiCancel;
        private ActionState UiStart => _inputManager.UiStart;
        private readonly TaskCompletionSource<bool> _promise = new TaskCompletionSource<bool>();


        private string titleText;
        private string subtitleText;

        public void Title(string title, string subtitle = null) {
            titleText = title;
            subtitleText = subtitle;
        }

        public override async void Ready() {
            // TODO i18n
            if (subtitleText != null) {
                doubleContainer.Visible = true;
                singleContainer.Visible = false;
                _title.Text = titleText;
                _subtitle.Text = subtitleText;
            } else {
                doubleContainer.Visible = false;
                singleContainer.Visible = true;
                _singleTitle.Text = titleText;
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
            noButton.RectMinSize = yesButton.RectMinSize = new Vector2(60, 0);
            return mainMenu;
        }

        public override void _Input(InputEvent @event) {
            if (_gameManager.IsModal() && UiCancel.IsEventPressed(@event)) {
                _promise.TrySetResult(false);
                GetTree().SetInputAsHandled();
            }
        }
    }
}