using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Application;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Signal;
using Betauer.StateMachine;
using DemoAnimation.Game.Controller.Menu;
using Godot;

namespace DemoAnimation.Game.Managers {
    [Service]
    public class MainStateMachine : StateMachineNode<MainStateMachine.State, MainStateMachine.Transition> {
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
    
        private Node _currentGameScene;

        [Load("res://Assets/UI/my_theme.tres")]
        private Theme MyTheme;

        [Load("res://Scenes/Menu/MainMenu.tscn")]
        private MainMenu _mainMenuScene;

        [Inject] private ScreenSettingsManager ScreenSettingsManager { get; set; }
        [Inject] private DebugOverlay DefaultDebugOverlay { get; set; }
        [Inject] private SceneTree _sceneTree { get; set; }
        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }

        public MainStateMachine() : base(State.Init) {
            On(Transition.FinishLoading, context => context.PopPush(State.MainMenu));
            State(State.Init)
                .Enter(async () => {
                    await new ResourceLoaderContainer().From(this).Load();
                    ScreenSettingsManager.Setup();
                    DefaultDebugOverlay.Theme = MyTheme;
                    DefaultDebugOverlay.MonitorFpsAndMemory();
                    DefaultDebugOverlay.CreateMonitor().Show(ScreenSettingsManager.GetStateAsString); 
                    DefaultDebugOverlay.MonitorInternals();
                    _sceneTree.Root.AddChild(_mainMenuScene);
                })
                .Execute(context => context.Set(State.MainMenu))
                .Build();
                

            State(State.MainMenu)
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Execute(async context => {
                    if (UiCancel.IsJustPressed()) {
                        await _mainMenuScene.BackMenu();
                    }
                    return context.None();
                })
                .Build();
                

            On(Transition.ModalBoxConfirmExitDesktop, context => context.Push(State.ModalExitDesktop));
            State(State.ModalExitDesktop)
                .On(Transition.Back, context => context.Pop())
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Exit game?");
                    return result ? context.Push(State.ExitDesktop) : context.Pop();
                })
                .Build();

            State(State.ExitDesktop)
                .Enter(() => _sceneTree.Notification(MainLoop.NotificationWmQuitRequest))
                .Build();
            
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