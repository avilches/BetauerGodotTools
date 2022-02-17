using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Betauer;
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
        private Node _currentGameScene;
        private Node2D _playerScene;

        [Inject] private StageManager _stageManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private Func<SceneTree> GetTree;
        [Inject] private ResourceManager _resourceManager;

        public void Start(SplashScreenController splashScreen) {
            splashScreen.QueueFree();
            ShowMainMenu();
        }

        private async void ShowMainMenu() {
            _mainMenuScene = _resourceManager.CreateMainMenu();
            await AddSceneDeferred(_mainMenuScene);
        }

        private void CloseMainMenu() {
            _mainMenuScene?.QueueFree();
            _mainMenuScene = null;
        }

        public async void StartGame() {
            CloseMainMenu();
            _currentGameScene = _resourceManager.CreateWorld1();
            await AddSceneDeferred(_currentGameScene);
            AddPlayerToScene(_currentGameScene);
        }

        public void ExitGameAndBackToMainMenu() {
            _currentGameScene.PrintStrayNodes();
            _currentGameScene.QueueFree();
            _currentGameScene = null;
            ShowMainMenu();
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

            _screenManager.ChangeScreenConfiguration(ApplicationConfig.Configuration2, ScreenService.Strategy.FitToScreen);
            _screenManager.CenterWindow();

        }
    }
}