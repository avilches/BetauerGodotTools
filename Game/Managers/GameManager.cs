using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Input;
using Betauer.Screen;
using Betauer.StateMachine;
using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Menu;

namespace Veronenger.Game.Managers {


    [Singleton]
    public class GameManager {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GameManager));

        private MainMenu _mainMenuScene;
        private MainMenuBottomBar _mainMenuBottomBarScene;
        private PauseMenu _pauseMenuScene;
        private SettingsMenu _settingsMenuScene;
        private Node _currentGameScene;
        private Node2D _playerScene;

        public enum Transition {
            Back,
            Pause,
            Quit,
            Settings,
            StartGame,
            OpenModal
        }

        public enum State {
            Loading,
            MainMenu,
            Settings,
            StartingGame,
            Gaming,
            PauseMenu,
            Modal,
        }

        public readonly Launcher Launcher = new Launcher();

        [Inject] private StageManager _stageManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private Func<SceneTree> GetTree;
        [Inject] private ResourceManager _resourceManager;
        [Inject] private InputManager _inputManager;

        private ActionState UiAccept => _inputManager.UiAccept;
        private ActionState UiCancel => _inputManager.UiCancel;
        private ActionState UiStart => _inputManager.UiStart;


        private StateMachineNode<State, Transition> _stateMachineNode;

        public void OnFinishLoad(SplashScreenController splashScreen) {
            _screenManager.Start(ApplicationConfig.Configuration);
            Launcher.WithParent(GetTree().Root);
            _mainMenuScene = _resourceManager.CreateMainMenu();
            _mainMenuBottomBarScene = _resourceManager.CreateMainMenuBottomBar();
            _pauseMenuScene = _resourceManager.CreatePauseMenu();
            _settingsMenuScene = _resourceManager.CreateSettingsMenu();
            _stateMachineNode = BuildStateMachine();

            // Never pause the pause, settings and the state machine, because they will not work!
            _settingsMenuScene.PauseMode = _pauseMenuScene.PauseMode = _stateMachineNode.PauseMode = 
                Node.PauseModeEnum.Process;

            GetTree().Root.AddChild(_pauseMenuScene);
            GetTree().Root.AddChild(_settingsMenuScene);
            GetTree().Root.AddChild(_mainMenuScene);
            GetTree().Root.AddChild(_mainMenuBottomBarScene);
            GetTree().Root.AddChild(_stateMachineNode);
            splashScreen.QueueFree();
        }
        
        private StateMachineNode<State, Transition> BuildStateMachine() {
            var builder = new StateMachineNode<State, Transition>(State.Loading, "GameManager", ProcessMode.Idle)
                .CreateBuilder();
            builder.AddListener(_mainMenuBottomBarScene);
            builder.State(State.Loading)
                .Execute(context => context.Set(State.MainMenu));

            builder.State(State.MainMenu)
                .On(Transition.StartGame, context => context.Set(State.StartingGame))
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Awake(from => {
                    if (from == State.Settings) _mainMenuScene.FocusSettings();
                })
                .Enter(async () => {
                    await _mainMenuScene.ShowMenu();
                })
                .Execute(async context => {
                    if (UiCancel.JustPressed) {
                        await _mainMenuScene.BackMenu();
                    }
                    return context.None();
                });

           builder.State(State.Settings)
               .On(Transition.Back, context => context.Pop())                                           
               .Enter(async () => await _settingsMenuScene.ShowSettingsMenu() )
               .Execute(context => {
                   if (UiCancel.JustPressed) {
                       return context.Pop();
                   }
                   return context.None();
               })
               .Exit(() => {
                   _settingsMenuScene.HideSettingsMenu();
               });
           
           builder.State(State.StartingGame)
                .Enter(async () => {
                    await _mainMenuScene.HideMainMenu();
                    _currentGameScene = _resourceManager.CreateWorld1();
                    await AddSceneDeferred(_currentGameScene);
                    AddPlayerToScene(_currentGameScene);
                })
                .Execute(context => context.Set(State.Gaming));
                
           builder.State(State.Gaming)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Pause, context => context.Push(State.PauseMenu))
                .On(Transition.Quit, context => context.Set(State.MainMenu))
                .Exit(() => {
                    _currentGameScene.PrintStrayNodes();
                    _currentGameScene.QueueFree();
                    _currentGameScene = null;
                });
           
           builder.State(State.PauseMenu)
                .On(Transition.Back, context => context.Pop())
                .On(Transition.Settings, context => context.Push(State.Settings))
                .Awake(from => {
                    if (from == State.Settings) _pauseMenuScene.FocusSettings();
                })
                .Enter(async () => {
                    GetTree().Paused = true;
                    await _pauseMenuScene.ShowPauseMenu();
                })
                .Execute(async context => {
                    if (UiCancel.JustPressed) {
                        if (_pauseMenuScene.IsRootMenuActive()) {
                            return context.Pop();
                        } else {
                            await _pauseMenuScene.BackMenu();
                        }
                    } else if (UiStart.JustPressed) {
                        return context.Pop();
                    }
                    return context.None();
                })
                .Exit(() => {
                    _pauseMenuScene.HidePauseMenu();
                    GetTree().Paused = false;
                });

           builder.On(Transition.OpenModal, context => context.Push(State.Modal));
           builder.State(State.Modal)
               .On(Transition.Quit, context => context.Set(State.MainMenu))
               .On(Transition.Back, context => context.Pop());
               

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

        public void TriggerExitGame() {
            _stateMachineNode.Trigger(Transition.Quit);
        }

        public async Task<bool> ModalBoxConfirmExitDesktop() {
            return await ShowModalBox("Exit game?");
        }

        public async Task<bool> ModalBoxConfirmQuitGame() {
            return await ShowModalBox("Quit game?", "Any progress not saved will be lost");
        }
        
        private async Task<bool> ShowModalBox(string title, string subtitle = null) {
            _stateMachineNode.Trigger(Transition.OpenModal);
            _mainMenuBottomBarScene.Save();
            _mainMenuBottomBarScene.ConfigureAcceptCancel();
            ModalBoxConfirm modalBoxConfirm = _resourceManager.CreateModalBoxConfirm();
            modalBoxConfirm.Title(title, subtitle);
            modalBoxConfirm.PauseMode = Node.PauseModeEnum.Process;
            GetTree().Root.AddChild(modalBoxConfirm);
            var result = await modalBoxConfirm.AwaitResult();
            _mainMenuBottomBarScene.Restore();
            modalBoxConfirm.QueueFree();
            _stateMachineNode.Trigger(Transition.Back);
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
            var position2D = nextScene.FindChild<Node2D>("PositionPlayer");
            if (position2D == null) {
                throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.Filename);
            }
            _playerScene.GlobalPosition = position2D.GlobalPosition;
        }

        private async Task AddSceneDeferred(Node scene) {
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(scene);
        }

        public async Task LoadAnimaDemo() {
            var nextScene = ResourceLoader.Load<PackedScene>("demos/AnimationsPreview.tscn").Instance();
            _currentGameScene = nextScene;
            await AddSceneDeferred(_currentGameScene);

            _screenManager.ChangeScreenConfiguration(ApplicationConfig.AnimaDemoConfiguration,
                ScreenService.Strategy.FitToScreen);
            _screenManager.CenterWindow();
        }
    }
}