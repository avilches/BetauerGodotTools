using System;
using Godot;
using Betauer;
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
    public class GameManager {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(GameManager));
        [Inject] public StageManager StageManager;
        [Inject] public InputManager InputManager;

        private Node _mainMenuScene;
        private Node _currentPlayingScene;
        private Node _playerScene;

        [Inject] private Func<SceneTree> GetTree;

        public async void LoadMainMenu(SplashScreenController splashScreenController) {
            _mainMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn").Instance();
            splashScreenController.QueueFree();
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(_mainMenuScene);
        }

        public async void StartGame() {
            _mainMenuScene = GetTree().Root.FindChild<Node>("MainMenu");
            _mainMenuScene.QueueFree();
            _mainMenuScene = null;

            _playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn").Instance();
            _currentPlayingScene = ResourceLoader.Load<PackedScene>("res://Worlds/World1.tscn").Instance();
            _currentPlayingScene.AddChild(_playerScene);
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(_currentPlayingScene);
        }

        public void ExitGameAndBackToMainMenu() {
            _currentPlayingScene.QueueFree();

            _mainMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn").Instance();
            GetTree().Root.AddChild(_mainMenuScene);
        }

        public async void QueueChangeScene(string scene) {
            StageManager.ClearTransition();
            _currentPlayingScene.RemoveChild(_playerScene);
            _currentPlayingScene.QueueFree();

            var nextScene = ResourceLoader.Load<PackedScene>(scene).Instance();
            nextScene.AddChild(_playerScene);
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(nextScene);
            _currentPlayingScene = nextScene;
        }

    }
}