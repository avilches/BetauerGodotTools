using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.DI;
using Betauer.Screen;
using Veronenger.Game.Controller;
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
        [Inject] public StageManager StageManager;

        private PackedScene SceneWorld1;
        private PackedScene SceneWorld2;
        private PackedScene MainMenu;
        private PackedScene Player;

        private Node _mainMenuScene;
        private Node _currentGameScene;
        private Node2D _playerScene;

        [Inject] private ScreenManager ScreenManager;
        [Inject] private Func<SceneTree> GetTree;

        public void Start(SplashScreenController splashScreen) {
            SceneWorld1 = Load("res://Worlds/World1.tscn");
            SceneWorld2 = Load("res://Worlds/World2.tscn");
            Player = Load("res://Scenes/Player.tscn");
            splashScreen.QueueFree();
            MainMenu = Load("res://Scenes/MainMenu.tscn");
            CreateMainMenu();
            // Called from splash screen ready, no need to wait for idle frame
            GetTree().Root.AddChild(_mainMenuScene);
        }

        private PackedScene Load(string scene) {
            return ResourceLoader.Load<PackedScene>(scene);
        }


        public async void StartGame() {
            _mainMenuScene.QueueFree();
            _mainMenuScene = null;

            _currentGameScene = SceneWorld1.Instance();
            await AddSceneDeferred(_currentGameScene);
            AddPlayerToScene(_currentGameScene);
        }


        public void ExitGameAndBackToMainMenu() {
            _currentGameScene.QueueFree();
            _currentGameScene = null;

            CreateMainMenu();
            AddSceneDeferred(_mainMenuScene);
        }

        public async void QueueChangeSceneWithPlayer(string sceneName) {
            StageManager.ClearTransition();
            _currentGameScene.QueueFree();

            var nextScene = ResourceLoader.Load<PackedScene>(sceneName).Instance();
            await AddSceneDeferred(nextScene);
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
        }

        private void AddPlayerToScene(Node nextScene) {
            _playerScene = (Node2D)Player.Instance();
            nextScene.AddChild(_playerScene);
            var position2D = nextScene.FindChild<Node2D>("PositionPlayer");
            if (position2D == null) {
                throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.Filename);
            }
            _playerScene.GlobalPosition = position2D.GlobalPosition;
        }

        private void CreateMainMenu() {
            _mainMenuScene = MainMenu.Instance();
        }

        private async Task AddSceneDeferred(Node scene) {
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(scene);
        }

        public async Task LoadAnimaDemo() {
            _mainMenuScene.QueueFree();
            var nextScene = ResourceLoader.Load<PackedScene>("demos/AnimationsPreview.tscn").Instance();
            _currentGameScene = nextScene;
            await AddSceneDeferred(_currentGameScene);

            ScreenManager.ChangeScreenConfiguration(ScreenSettings.Configuration2, ScreenService.Strategy.FitToScreen);
            ScreenManager.CenterWindow();

        }
    }
}