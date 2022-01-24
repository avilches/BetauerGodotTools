using System;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Veronenger.Game.Controller;
using Veronenger.Game.Controller.Character;
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

        private static PackedScene SceneWorld1() => ResourceLoader.Load<PackedScene>("res://Worlds/World1.tscn");
        private static PackedScene SceneWorld2() => ResourceLoader.Load<PackedScene>("res://Worlds/World2.tscn");
        private static PackedScene MainMenu() => ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn");
        private static PackedScene Player() => ResourceLoader.Load<PackedScene>("res://Scenes/Player.tscn");

        private Node _mainMenuScene;
        private Node _currentGameScene;
        private Node2D _playerScene;

        [Inject] private Func<SceneTree> GetTree;

        public void Start(SplashScreenController splashScreen) {
            splashScreen.QueueFree();
            SceneWorld1();
            SceneWorld2();
            MainMenu();
            Player();

            CreateMainMenu();
            AddSceneDeferred(_mainMenuScene);
        }

        public async void StartGame() {
            _mainMenuScene.QueueFree();
            _mainMenuScene = null;

            _currentGameScene = SceneWorld1().Instance();
            await AddSceneDeferred(_currentGameScene);
            AddPlayerToScene(_currentGameScene);
        }


        public void ExitGameAndBackToMainMenu() {
            _currentGameScene.QueueFree();
            _currentGameScene = null;

            CreateMainMenu();
            AddSceneDeferred(_mainMenuScene);
        }

        public async void QueueChangeScene(string scene) {
            StageManager.ClearTransition();
            _currentGameScene.QueueFree();

            var nextScene = ResourceLoader.Load<PackedScene>(scene).Instance();
            await AddSceneDeferred(nextScene);
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
        }

        private void AddPlayerToScene(Node nextScene) {
            _playerScene = (Node2D)Player().Instance();
            nextScene.AddChild(_playerScene);
            var position2D = nextScene.FindChild<Node2D>("PositionPlayer");
            if (position2D == null) {
                throw new Exception("Node PositionPlayer not found when loading scene "+nextScene.Filename);
            }
            _playerScene.GlobalPosition = position2D.GlobalPosition;
        }

        private void CreateMainMenu() {
            _mainMenuScene = MainMenu().Instance();
        }

        private async Task AddSceneDeferred(Node scene) {
            await GetTree().AwaitIdleFrame();
            GetTree().Root.AddChild(scene);
        }
    }
}