using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Animation;
using Betauer.Application;
using Betauer.Application.Monitor;
using Betauer.Application.Screen;
using Betauer.DI;
using Betauer.Input;
using Betauer.Loader;
using Betauer.Core.Signal;
using Betauer.StateMachine;
using Betauer.StateMachine.Async;
using DemoAnimation.Controller.Menu;
using Godot;

namespace DemoAnimation.Managers {
    [Service]
    public class MainStateMachine : StateMachineNodeAsync<MainStateMachine.State, MainStateMachine.Transition> {
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
        [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private InputAction UiAccept { get; set; }
        [Inject] private InputAction UiCancel { get; set; }

        public MainStateMachine() : base(State.Init) {
            On(Transition.FinishLoading).PopPush(State.MainMenu);
            State(State.Init)
                .Enter(async () => {
                    await new ResourceLoaderContainer().From(this).Load();
                    ScreenSettingsManager.Setup();
                    DebugOverlayManager.OverlayContainer.Theme = MyTheme;
                    DebugOverlayManager.DebugConsole.Theme = MyTheme;
                    SceneTree.Root.AddChild(_mainMenuScene);
                })
                .If(() => true).Set(State.MainMenu)
                .Build();
                

            State(State.MainMenu)
                .OnInput(e => _mainMenuScene.OnInput(e))
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Build();
                

            var modalResponse = false;
            On(Transition.ModalBoxConfirmExitDesktop).Push(State.ModalExitDesktop);
            State(State.ModalExitDesktop)
                .On(Transition.Back).Pop()
                .Enter(() => _mainMenuScene.DimOut())
                .Exit(() => _mainMenuScene.RollbackDimOut())
                .Execute(async () => {
                    modalResponse = await ShowModalBox("Exit game?");
                })
                .If(() => modalResponse).Set(State.MainMenu)
                .If(() => !modalResponse).Pop()
                .Build();

            State(State.ExitDesktop)
                .Enter(() => SceneTree.Notification(MainLoop.NotificationWmQuitRequest))
                .Build();
            
        }

        public void TriggerModalBoxConfirmExitDesktop() {
            Enqueue(Transition.ModalBoxConfirmExitDesktop);
        }

        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            ModalBoxConfirm modalBoxConfirm =
                (ModalBoxConfirm)ResourceLoader.Load<PackedScene>("res://Scenes/Menu/ModalBoxConfirm.tscn").Instance();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.ProcessMode = Node.ProcessModeEnum.Process;
            SceneTree.Root.AddChild(modalBoxConfirm);
            var result = await modalBoxConfirm.AwaitResult();
            modalBoxConfirm.QueueFree();
            return result;
        }
        private async Task AddSceneDeferred(Node scene) {
            await SceneTree.AwaitProcessFrame();
            SceneTree.Root.AddChild(scene);
        }

        public async Task LoadAnimaDemo() {
            var nextScene = ResourceLoader.Load<PackedScene>("demos/AnimationsPreview.tscn").Instance();
            _currentGameScene = nextScene;
            await AddSceneDeferred(_currentGameScene);
        }

    }
}