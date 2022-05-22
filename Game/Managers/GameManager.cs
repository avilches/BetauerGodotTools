using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Screen;
using Betauer.StateMachine;
using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Menu;
using Veronenger.Game.Controller.UI;

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

        public bool IsGamePaused() => _stateMachineNode.StateMachine.CurrentState.Name == nameof(State.PauseMenu);
        public bool IsGaming() =>_stateMachineNode.StateMachine.CurrentState.Name == nameof(State.Gaming);
        public bool IsMainMenu() =>_stateMachineNode.StateMachine.CurrentState.Name == nameof(State.MainMenu);
        public bool IsSettings() => _stateMachineNode.StateMachine.CurrentState.Name == nameof(State.MainMenuSettings) ||
                                    _stateMachineNode.StateMachine.CurrentState.Name == nameof(State.PauseMenuSettings);

        public readonly Launcher Launcher = new Launcher();

        [Inject] private StageManager _stageManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private Func<SceneTree> GetTree;
        [Inject] private ResourceManager _resourceManager;
        
        

        public enum State {
            Loading,
            MainMenu,
            MainMenuSettings,
            StartGame,
            Gaming,
            PauseMenu,
            PauseMenuSettings
        }

        private StateMachineNode _stateMachineNode;

        public void OnFinishLoad(SplashScreenController splashScreen) {
            splashScreen.QueueFree();
            _stateMachineNode = new StateMachineNode("GameManager", StateMachineNode.ProcessMode.Idle);
            GetTree().Root.AddChild(_stateMachineNode);
            _stateMachineNode.PauseMode = Node.PauseModeEnum.Process;
            var builder = _stateMachineNode.CreateBuilder();
            
            
            builder.State(nameof(State.Loading))
                .Enter(context => {
                    _screenManager.Start(ApplicationConfig.Configuration);
                    Launcher.WithParent(GetTree().Root);
                    _mainMenuScene = _resourceManager.CreateMainMenu();
                    _mainMenuBottomBarScene = _resourceManager.CreateMainMenuBottomBar();
                    _pauseMenuScene = _resourceManager.CreatePauseMenu();
                    _settingsMenuScene = _resourceManager.CreateSettingsMenu();

                    // Never pause the pause menu and the settings menu!
                    _settingsMenuScene.PauseMode = _pauseMenuScene.PauseMode = Node.PauseModeEnum.Process;
                    GetTree().Root.AddChild(_pauseMenuScene);
                    GetTree().Root.AddChild(_settingsMenuScene);

                    GetTree().Root.AddChild(_mainMenuScene);
                    GetTree().Root.AddChild(_mainMenuBottomBarScene);
                })
                .Execute(context => NextState.NextFrame(nameof(State.MainMenu)));

            builder.State(nameof(State.MainMenu))
                .Enter(async context => {
                    if (context.FromState.Name == nameof(State.Loading)) {
                        _mainMenuScene.ShowMenu();
                    } else if (context.FromState.Name == nameof(State.MainMenuSettings)) {
                        _settingsMenuScene.HideSettingsMenu();
                        _mainMenuScene.FocusSettings();
                    } else if (context.FromState.Name == nameof(State.PauseMenu)) {
                        _pauseMenuScene.HidePauseMenu();
                        GetTree().Paused = false;
                        _currentGameScene.PrintStrayNodes();
                        _currentGameScene.QueueFree();
                        _currentGameScene = null;
                        await _mainMenuScene.ShowMenu();
                    } else {
                        Console.WriteLine("WAAAT??? " + context.FromState.Name);
                    }
                });

            builder.State(nameof(State.MainMenuSettings))
                .Enter(async context => {
                    if (context.FromState.Name == nameof(State.MainMenu)) {
                        await _settingsMenuScene.ShowSettingsMenu();
                    } else {
                        Console.WriteLine("WAAAT??? " + context.FromState.Name);
                    }
                });

            builder.State(nameof(State.PauseMenuSettings))
                .Enter(async context => {
                    if (context.FromState.Name == nameof(State.PauseMenu)) {
                        await _settingsMenuScene.ShowSettingsMenu();
                    } else {
                        Console.WriteLine("WAAAT??? " + context.FromState.Name);
                    }
                });

            builder.State(nameof(State.StartGame))
                .Enter(async context => {
                    if (context.FromState.Name == nameof(State.MainMenu)) {
                        await _mainMenuScene.HideMainMenu();
                        _currentGameScene = _resourceManager.CreateWorld1();
                        await AddSceneDeferred(_currentGameScene);
                        AddPlayerToScene(_currentGameScene);
                    } else {
                        Console.WriteLine("WAAAT??? " + context.FromState.Name);
                    }
                })
                .Execute(context => NextState.Immediate(nameof(State.Gaming)));

            builder.State(nameof(State.Gaming))
                .Enter(async context => {
                    if (context.FromState.Name == nameof(State.PauseMenu)) {
                        _pauseMenuScene.HidePauseMenu();
                        GetTree().Paused = false;
                    } else if (context.FromState.Name == nameof(State.StartGame)) {
                    } else {
                        Console.WriteLine("WAAAT??? " + context.FromState.Name);
                    }
                });
                
            
            builder.State(nameof(State.PauseMenu))
                .Enter(async context => {
                    if (context.FromState.Name == nameof(State.PauseMenuSettings)) {
                        _settingsMenuScene.HideSettingsMenu();
                        await _pauseMenuScene.ShowPauseMenu();
                        _pauseMenuScene.FocusSettings();
                    } else if (context.FromState.Name == nameof(State.Gaming)) {
                        GetTree().Paused = true;
                        await _pauseMenuScene.ShowPauseMenu();
                    } else {
                        Console.WriteLine("WAAAT??? " + context.FromState.Name);
                    }
                });
            
            builder.Build();
            _stateMachineNode.SetNextState(nameof(State.Loading));

        }

        public void StartGame() {
            _stateMachineNode.SetNextState(nameof(State.StartGame));
        }

        public void ShowPauseMenu() {
            _stateMachineNode.SetNextState(nameof(State.PauseMenu));
        }

        public void ShowPauseMenuSettings() {
            _stateMachineNode.SetNextState(nameof(State.PauseMenuSettings));
        }

        public void ShowMainMenuSettings() {
            _stateMachineNode.SetNextState(nameof(State.MainMenuSettings));
        }

        public void CloseSettingsMenu() {
            if (_stateMachineNode.StateMachine.CurrentState.Name == nameof(State.MainMenuSettings)) {
                _stateMachineNode.SetNextState(nameof(State.MainMenu));
            } else {
                _stateMachineNode.SetNextState(nameof(State.PauseMenu));
            }
        }

        public async Task<bool> ModalBoxConfirmExitDesktop() {
            return await ShowModalBox("Exit game?");
        }

        public async Task<bool> ModalBoxConfirmQuitGame() {
            return await ShowModalBox("Quit game?", "Any progress not saved will be lost");
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

        public void GoGaming() {
            _stateMachineNode.SetNextState(nameof(State.Gaming));
        }

        public async Task ExitGameAndBackToMainMenu() {
            _stateMachineNode.SetNextState(nameof(State.MainMenu));
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