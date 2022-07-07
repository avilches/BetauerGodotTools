using System;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.StateMachine;
using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Menu;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class GameManager : Godot.Object {

        private MainMenu _mainMenuScene;
        public MainMenuBottomBar MainMenuBottomBarScene;
        private PauseMenu _pauseMenuScene;
        private SettingsMenu _settingsMenuScene;
        private Node _currentGameScene;
        private Node2D _playerScene;

        public enum Transition {
            Back,
            Pause,
            Settings,
            StartGame,
            ModalBoxConfirmExitDesktop,
            ModalBoxConfirmQuitGame
        }

        public enum State {
            Loading,
            MainMenu,
            Settings,
            StartingGame,
            Gaming,
            PauseMenu,
            ModalQuitGame,
            ModalExitDesktop,
            ExitDesktop,
        }

        private readonly Launcher _launcher = new Launcher();

        [Inject] private StageManager _stageManager;
        [Inject] private SettingsManager _settingsManager;
        [Inject] private Func<SceneTree> GetTree;
        [Inject] private ResourceManager _resourceManager;

        [Inject] private ActionState PixelPerfect;
        [Inject] private ActionState UiAccept;
        [Inject] private ActionState UiCancel;
        [Inject] private ActionState UiStart;

        private StateMachineNode<State, Transition> _stateMachineNode;

        public void OnFinishLoad(SplashScreenController splashScreen) {
            _settingsManager.Start(GetTree(), ApplicationConfig.Configuration);
            _launcher.WithParent(GetTree().Root);
            _mainMenuScene = _resourceManager.CreateMainMenu();
            MainMenuBottomBarScene = _resourceManager.CreateMainMenuBottomBar();
            _pauseMenuScene = _resourceManager.CreatePauseMenu();
            _settingsMenuScene = _resourceManager.CreateSettingsMenu();
            _stateMachineNode = BuildStateMachine();

            // Never pause the pause, settings and the state machine, because they will not work!
            _settingsMenuScene.PauseMode = _pauseMenuScene.PauseMode = _stateMachineNode.PauseMode =
                Node.PauseModeEnum.Process;

            GetTree().Root.AddChild(_pauseMenuScene);
            GetTree().Root.AddChild(_settingsMenuScene);
            GetTree().Root.AddChild(_mainMenuScene);
            GetTree().Root.AddChild(MainMenuBottomBarScene);
            GetTree().Root.AddChild(_stateMachineNode);
            splashScreen.QueueFree();
        }

        private StateMachineNode<State, Transition> BuildStateMachine() {
            var builder = new StateMachineNode<State, Transition>(State.Loading, "GameManager", ProcessMode.Idle)
                .CreateBuilder();
            builder.AddListener(MainMenuBottomBarScene);
            builder.State(State.Loading)
                .Execute(context => context.Replace(State.MainMenu));

            builder.State(State.MainMenu)
                .On(Transition.StartGame, context => context.Replace(State.StartingGame))
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Suspend(() => _mainMenuScene.DisableMenus())
                .Awake(() => _mainMenuScene.EnableMenus())
                .Enter(async () => await _mainMenuScene.ShowMenu())
                .Execute(_mainMenuScene.Execute);

            builder.State(State.Settings)
                .On(Transition.Back, context => context.Pop())
                .Enter(_settingsMenuScene.ShowSettingsMenu)
                .Execute(_settingsMenuScene.Execute)
                .Exit(_settingsMenuScene.HideSettingsMenu);

            builder.State(State.StartingGame)
                .Enter(async () => {
                    await _mainMenuScene.HideMainMenu();
                    _currentGameScene = _resourceManager.CreateWorld1();
                    await AddSceneDeferred(_currentGameScene);
                    AddPlayerToScene(_currentGameScene);
                })
                .Execute(context => context.Replace(State.Gaming));

            builder.State(State.Gaming)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Pause, context => context.Push(State.PauseMenu))
                .Execute(context => {
                    if (UiStart.JustPressed) {
                        return context.Trigger(Transition.Pause);
                    } else if (PixelPerfect.JustPressed) {
                        _settingsManager.SetPixelPerfect(!_settingsManager.SettingsFile.PixelPerfect);
                    }
                    return context.None();
                })
                .Exit(() => {
                    _currentGameScene.PrintStrayNodes();
                    _currentGameScene.QueueFree();
                    _currentGameScene = null;
                });

            builder.State(State.PauseMenu)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Suspend(() => _pauseMenuScene.DisableMenus())
                .Awake(() => _pauseMenuScene.EnableMenus())
                .Enter(async () => {
                    GetTree().Paused = true;
                    await _pauseMenuScene.ShowPauseMenu();
                })
                .Execute(_pauseMenuScene.Execute)
                .Exit(() => {
                    _pauseMenuScene.EnableMenus();
                    _pauseMenuScene.HidePauseMenu();
                    GetTree().Paused = false;
                });

            builder.On(Transition.ModalBoxConfirmQuitGame, context => context.Push(State.ModalQuitGame));
            builder.State(State.ModalQuitGame)
                .On(Transition.Back, context => context.Pop())
                .Execute(async (context) => {
                    var result = await ShowModalBox("Quit game?", "Any progress not saved will be lost");
                    return result ? context.Replace(State.MainMenu) : context.Pop();
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

        public void TriggerStartGame() {
            _stateMachineNode.Trigger(Transition.StartGame);
        }

        public void TriggerPauseMenu() {
            _stateMachineNode.Trigger(Transition.Pause);
        }

        public void TriggerSettings() {
            _stateMachineNode.Trigger(Transition.Settings);
        }

        public void TriggerBack() {
            _stateMachineNode.Trigger(Transition.Back);
        }

        public void TriggerModalBoxConfirmExitDesktop() {
            _stateMachineNode.Trigger(Transition.ModalBoxConfirmExitDesktop);
        }

        public void TriggerModalBoxConfirmQuitGame() {
            _stateMachineNode.Trigger(Transition.ModalBoxConfirmQuitGame);
        }

        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            ModalBoxConfirm modalBoxConfirm = _resourceManager.CreateModalBoxConfirm();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.PauseMode = Node.PauseModeEnum.Process;
            GetTree().Root.AddChild(modalBoxConfirm);
            var result = await modalBoxConfirm.AwaitResult();
            modalBoxConfirm.QueueFree();
            return result;
        }

        public async void QueueChangeSceneWithPlayer(string sceneName) {
            _stageManager.ClearTransition();
            _currentGameScene.QueueFree();

            var nextScene = ResourceLoader.Load<PackedScene>(sceneName).Instance();
            await AddSceneDeferred(nextScene);
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
        }

        private void AddPlayerToScene(Node nextScene) {
            _playerScene = _resourceManager.CreatePlayer();
            nextScene.AddChild(_playerScene);
            var position2D = nextScene.GetNode<Node2D>("PositionPlayer");
            if (position2D == null) {
                throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.Filename);
            }
            _playerScene.GlobalPosition = position2D.GlobalPosition;
        }

        private async Task AddSceneDeferred(Node scene) {
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(scene);
        }
    }
}