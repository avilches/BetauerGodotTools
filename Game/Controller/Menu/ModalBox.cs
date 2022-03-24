using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.UI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Menu {
    public class ModalBox : DiNode {
        private static readonly SequenceTemplate PartialFadeOut = TemplateBuilder.Create()
            .SetDuration(0.3f)
            .AnimateKeys(Property.Opacity)
            .KeyframeTo(0f, 0f)
            .KeyframeTo(1f, 0.8f)
            .EndAnimate()
            .BuildTemplate();


        [OnReady("CenterContainer/VBoxContainer/Menu")]
        private Godot.Container _menuBase;

        [OnReady("CenterContainer/VBoxContainer/Title")]
        private Label _title;

        [OnReady("ColorRect")] private ColorRect _colorRect;

        private MenuController _menuController;

        [Inject] private GameManager _gameManager;
        [Inject] private InputManager _inputManager;

        private ActionState UiAccept => _inputManager.UiAccept;
        private ActionState UiCancel => _inputManager.UiCancel;
        private ActionState UiStart => _inputManager.UiStart;
        private readonly TaskCompletionSource<bool> _promise = new TaskCompletionSource<bool>();


        public override async void Ready() {
            _menuController = BuildMenu();
            await _menuController.Start("Root");
            Console.WriteLine("Modal ready, fading...");
            // _gameManager.Launcher.Play(PartialFadeOut, _colorRect, 0f, 0.3f);
        }

        public Task<bool> AwaitResult() {
            return _promise.Task;
        }

        public MenuController BuildMenu() {
            // TODO i18n
            _title.Text = "Are you sure?";
            foreach (var child in _menuBase.GetChildren()) (child as Node)?.Free();

            var mainMenu = new MenuController(_menuBase);
            mainMenu.AddMenu("Root")
                .AddButton("No", "No", (ctx) => {
                    _promise.TrySetResult(false);
                })
                .AddButton("Yes", "Yes", (ctx) => {
                    _promise.TrySetResult(true);
                });
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