using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Signal;
using Betauer.StateMachine;
using DemoAnimation.Game.Controller.Menu;
using Godot;

namespace DemoAnimation.Game.Managers {
    [Service]
    public class GameManager : StateMachineNode<GameManager.State, GameManager.Transition> {
        public enum Transition {
            FinishLoading,
            Back,
            ModalBoxConfirmExitDesktop,
        }
    
        public enum State {
            Init,
            MainMenu,
            ModalExitDesktop,
            ExitDesktop,
        }
    
        private MainMenu _mainMenuScene;
        private Node _currentGameScene;

        private readonly Launcher _launcher = new Launcher();

        [Inject] private ScreenSettingsManager _screenSettingsManager { get; set; }
        [Inject] private SceneTree _sceneTree { get; set; }
        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }
        [Inject] private InputAction UiStart { get; set; }

        public GameManager() : base(State.Init) {
            var builder = CreateBuilder();
            builder.On(Transition.FinishLoading, context => context.PopPush(State.MainMenu));
            builder.State(State.Init)
                .Enter(() => {
                    _screenSettingsManager.Setup();
                    _launcher.WithParent(_sceneTree.Root);
                    _mainMenuScene = (MainMenu)ResourceLoader.Load<PackedScene>("res://Scenes/Menu/MainMenu.tscn")
                        .Instance();
                    _sceneTree.Root.AddChild(_mainMenuScene);
                })
                .Execute(context => context.Set(State.MainMenu));

            builder.State(State.MainMenu)
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Execute(async context => {
                    if (UiCancel.JustPressed()) {
                        await _mainMenuScene.BackMenu();
                    }
                    return context.None();
                });

            builder.On(Transition.ModalBoxConfirmExitDesktop, context => context.Push(State.ModalExitDesktop));
            builder.State(State.ModalExitDesktop)
                .On(Transition.Back, context => context.Pop())
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Exit game?");
                    return result ? context.Push(State.ExitDesktop) : context.Pop();
                });

            builder.State(State.ExitDesktop)
                .Enter(() => _sceneTree.Notification(MainLoop.NotificationWmQuitRequest));
            
            builder.Build();
        }

        public void TriggerModalBoxConfirmExitDesktop() {
            Enqueue(Transition.ModalBoxConfirmExitDesktop);
        }

        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            ModalBoxConfirm modalBoxConfirm =
                (ModalBoxConfirm)ResourceLoader.Load<PackedScene>("res://Scenes/Menu/ModalBoxConfirm.tscn").Instance();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.PauseMode = Node.PauseModeEnum.Process;
            _sceneTree.Root.AddChild(modalBoxConfirm);
            var result = await modalBoxConfirm.AwaitResult();
            modalBoxConfirm.QueueFree();
            return result;
        }
        private async Task AddSceneDeferred(Node scene) {
            await _sceneTree.AwaitIdleFrame();
            _sceneTree.Root.AddChild(scene);
        }

        public async Task LoadAnimaDemo() {
            var nextScene = ResourceLoader.Load<PackedScene>("demos/AnimationsPreview.tscn").Instance();
            _currentGameScene = nextScene;
            await AddSceneDeferred(_currentGameScene);
        }

    }
}