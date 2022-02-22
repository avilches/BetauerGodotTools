using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Betauer.Screen;
using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Menu;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Managers {
    /**
     * GameManager es Node para estar en autoload y recibir eventos
     * Crea automaticamente a los otros managers (Manager = siempre cargado)
     * Los Controller son scripts de objetos de le escena que se cargan y se registran en los managers
     *
     * Los Manager actuan de intermediarios entre objetos que no se conocen entre si. Por ejemplo: las death zones,
     * plataformas o stages se añaden en sus managers, que escucha a las señales que estos objetos producen.
     * Por otro lado, el jugador se registra en estos mismos manager para escuchar estas señales, sin llegar a saber
     * donde estan realmente estos objetos (plataformas o areas).
     *
     */
    [Singleton]
    public class GameManager  {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GameManager));

        private MainMenu _mainMenuScene;
        private PauseMenu _pauseMenuScene;
        private Node _currentGameScene;
        private Node2D _playerScene;

        private bool _paused = false;
        private bool _canGameBePaused = false;

        public bool IsGamePaused() => _paused;
        public bool CanGameBePaused() => _canGameBePaused;

        private readonly Launcher _launcher = new Launcher();

        [Inject] private StageManager _stageManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private Func<SceneTree> GetTree;
        [Inject] private ResourceManager _resourceManager;

        public async void OnFinishLoad(SplashScreenController splashScreen) {
            splashScreen.QueueFree();
            CreateAndConfigurePauseMenu();

            _mainMenuScene = _resourceManager.CreateMainMenu();
            GetTree().Root.AddChild(_mainMenuScene);
            await ShowMainMenu();
        }

        private void CreateAndConfigurePauseMenu() {
            _pauseMenuScene = _resourceManager.CreatePauseMenu();
            _pauseMenuScene.PauseMode = Node.PauseModeEnum.Process;
            _pauseMenuScene.DisableAllNotifications();
            _pauseMenuScene.Visible = false;
            var canvasLayer = new CanvasLayer();
            canvasLayer.Name = "PauseMenuLayer";
            canvasLayer.Layer = 1;
            canvasLayer.AddChild(_pauseMenuScene);
            GetTree().Root.AddChild(canvasLayer);
        }

        private async Task ShowMainMenu() {
            await _mainMenuScene.ShowMenu();
        }

        public async void StartGame() {
            await _mainMenuScene.CloseMainMenu();
            _currentGameScene = _resourceManager.CreateWorld1();
            await AddSceneDeferred(_currentGameScene);
            AddPlayerToScene(_currentGameScene);
            _canGameBePaused = true;
        }

        public async void ShowPauseMenu() {
            _pauseMenuScene.Visible = true;
            await _pauseMenuScene.ShowMenu();
            GetTree().Paused = true;
            _paused = true;
            _pauseMenuScene.EnableAllNotifications();
        }

        public void ClosePauseMenu() {
            _pauseMenuScene.Visible = false;
            _pauseMenuScene.DisableAllNotifications();
            GetTree().Paused = false;
            _paused = false;
        }

        public async Task ExitGameAndBackToMainMenu() {
            _currentGameScene.PrintStrayNodes();
            _currentGameScene.QueueFree();
            _currentGameScene = null;
            await ShowMainMenu();
        }

        public async void QueueChangeSceneWithPlayer(string sceneName) {
            _canGameBePaused = false;
            _stageManager.ClearTransition();
            _currentGameScene.QueueFree();

            var nextScene = ResourceLoader.Load<PackedScene>(sceneName).Instance();
            await AddSceneDeferred(nextScene);
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
            _canGameBePaused = true;
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

            _screenManager.ChangeScreenConfiguration(ApplicationConfig.Configuration2, ScreenService.Strategy.FitToScreen);
            _screenManager.CenterWindow();

        }

    }
}