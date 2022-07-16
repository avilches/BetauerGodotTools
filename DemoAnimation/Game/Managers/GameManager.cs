using System;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.Memory;
using Betauer.StateMachine;
using DemoAnimation.Game.Controller;
using DemoAnimation.Game.Controller.Menu;
using Godot;

namespace DemoAnimation.Game.Managers {
    [Singleton]
    public class GameManager {
        private MainMenu _mainMenuScene;
        private Node _currentGameScene;

        public enum Transition {
            Back,
            ModalBoxConfirmExitDesktop,
        }

        public enum State {
            Loading,
            MainMenu,
            ModalExitDesktop,
            ExitDesktop,
        }

        private readonly Launcher _launcher = new Launcher();

        [Inject] private SettingsManager _settingsManager;
        [Inject] private Func<SceneTree> GetTree;

        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

        private StateMachineNode<State, Transition> _stateMachineNode;

        public void OnFinishLoad(SplashScreenController splashScreen) {
            _settingsManager.Start(GetTree(), ApplicationConfig.Configuration);
            _launcher.WithParent(GetTree().Root);
            _mainMenuScene = (MainMenu)ResourceLoader.Load<PackedScene>("res://Scenes/Menu/MainMenu.tscn").Instance();
            _stateMachineNode = BuildStateMachine();

            GetTree().Root.AddChild(_mainMenuScene);
            GetTree().Root.AddChild(_stateMachineNode);
            splashScreen.QueueFree();
        }

        private StateMachineNode<State, Transition> BuildStateMachine() {
            var builder = new StateMachineNode<State, Transition>(State.Loading, "GameManager", ProcessMode.Idle)
                .CreateBuilder();
            builder.State(State.Loading)
                .Execute(context => context.Replace(State.MainMenu));

            builder.State(State.MainMenu)
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Execute(async context => {
                    if (UiCancel.JustPressed) {
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
                .Enter(() => GetTree().Notification(MainLoop.NotificationWmQuitRequest));

            return builder.Build();
        }

        public void TriggerModalBoxConfirmExitDesktop() {
            _stateMachineNode.Enqueue(Transition.ModalBoxConfirmExitDesktop);
        }

        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            ModalBoxConfirm modalBoxConfirm =
                (ModalBoxConfirm)ResourceLoader.Load<PackedScene>("res://Scenes/Menu/ModalBoxConfirm.tscn").Instance();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.PauseMode = Node.PauseModeEnum.Process;
            GetTree().Root.AddChild(modalBoxConfirm);
            var result = await modalBoxConfirm.AwaitResult();
            modalBoxConfirm.QueueFree();
            return result;
        }
        private async Task AddSceneDeferred(Node scene) {
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(scene);
        }

        public async Task LoadAnimaDemo() {
            var nextScene = ResourceLoader.Load<PackedScene>("demos/AnimationsPreview.tscn").Instance();
            _currentGameScene = nextScene;
            await AddSceneDeferred(_currentGameScene);
        }
    }
}