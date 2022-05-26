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

        public const string Loading = nameof(Loading);
        public const string MainMenu = nameof(MainMenu);
        public const string MainMenuSettings = nameof(MainMenuSettings);
        public const string StartGame = nameof(StartGame);
        public const string Gaming = nameof(Gaming);
        public const string PauseMenu = nameof(PauseMenu);
        public const string PauseMenuSettings = nameof(PauseMenuSettings);

        public readonly Launcher Launcher = new Launcher();

        [Inject] private StageManager _stageManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private Func<SceneTree> GetTree;
        [Inject] private ResourceManager _resourceManager;
        [Inject] private InputManager _inputManager;

        private StateMachineNode _stateMachineNode;

        public void OnFinishLoad(SplashScreenController splashScreen) {
            splashScreen.QueueFree();
            _stateMachineNode = new StateMachineNode("GameManager", StateMachineNode.ProcessMode.Idle);
            GetTree().Root.AddChild(_stateMachineNode);
            _stateMachineNode.PauseMode = Node.PauseModeEnum.Process;
            var builder = _stateMachineNode.CreateBuilder();

            builder.State(Loading)
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
                .Execute(context => context.NextFrame(MainMenu));

            builder.State(MainMenu)
                .Enter(async context => await _mainMenuScene.ShowMenu())
                .Execute(async context => await _mainMenuScene.Execute(context));

           builder.State(MainMenuSettings)
               .Enter(async context => await _settingsMenuScene.ShowSettingsMenu() )
               .Execute(async context => await _settingsMenuScene.Execute(context))
               .Exit(() => {
                   _settingsMenuScene.HideSettingsMenu();
                   _mainMenuScene.FocusSettings();
               });
           
           builder.State(StartGame)
                .Enter(async context => {
                    await _mainMenuScene.HideMainMenu();
                    _currentGameScene = _resourceManager.CreateWorld1();
                    await AddSceneDeferred(_currentGameScene);
                    AddPlayerToScene(_currentGameScene);
                })
                .Execute(context => context.Immediate(Gaming));
                
           builder.State(Gaming)
                .Enter(async context => {
                })
                .Exit(async () => {
                    _currentGameScene.PrintStrayNodes();
                    _currentGameScene.QueueFree();
                    _currentGameScene = null;
                });
           
           builder.State(PauseMenu)
                .Enter(async context => {
                    GetTree().Paused = true;
                    await _pauseMenuScene.ShowPauseMenu();
                })
                .Execute(async context => await _pauseMenuScene.Execute(context))
                .Exit(() => {
                    _pauseMenuScene.HidePauseMenu();
                    GetTree().Paused = false;
                });

           builder.State(PauseMenuSettings)
               .Enter(async context => await _settingsMenuScene.ShowSettingsMenu())
               .Execute(async context => await _settingsMenuScene.Execute(context))
               .Exit(async () => {
                   _settingsMenuScene.HideSettingsMenu();
                   await _pauseMenuScene.ShowPauseMenu();
                   _pauseMenuScene.FocusSettings();
               });
            
            builder.Build();
            _stateMachineNode.SetNextState(Loading);

        }

        public void xStartGame() {
            _stateMachineNode.SetNextState(StartGame);
        }

        public void ShowPauseMenu() {
            _stateMachineNode.PushNextState(PauseMenu);
        }

        public void ShowPauseMenuSettings() {
            _stateMachineNode.PushNextState(PauseMenuSettings);
        }

        public void ShowMainMenuSettings() {
            _stateMachineNode.PushNextState(MainMenuSettings);
        }

        public void GoGaming() {
            _stateMachineNode.PopNextState();
        }

        public async Task ExitGameAndBackToMainMenu() {
            _stateMachineNode.SetNextState(MainMenu);
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
            _stateMachineNode.PopNextState();
        }

        public async Task ExitGameAndBackToMainMenu() {
            _stateMachineNode.SetNextState(MainMenu);
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