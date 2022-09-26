using System;
using System.Threading.Tasks;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.Signal;
using Godot;

namespace Veronenger.Game.Managers {

    [Service]
    public class Game {

        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private StageManager StageManager { get; set; }
        [Inject] private MainResourceLoader MainResourceLoader { get; set; }
        [Inject] private DebugOverlay DefaultDebugOverlay { get; set; }
        
        private Node _currentGameScene;
        private Node2D _playerScene;

        public async Task Start() {
            _currentGameScene = MainResourceLoader.CreateWorld2();
            await AddSceneDeferred(_currentGameScene);
            AddPlayerToScene(_currentGameScene);
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
            var position2D = nextScene.GetNode<Node2D>("PositionPlayer");
            if (position2D == null) throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.Filename);
            AddPlayerToScene(nextScene, position2D.GlobalPosition);
        }

        private void AddPlayerToScene(Node nextScene, Vector2 position) {
            _playerScene = MainResourceLoader.CreatePlayer();
            _playerScene.GlobalPosition = position;
            nextScene.AddChild(_playerScene);
        }

        private async Task AddSceneDeferred(Node scene) {
            await SceneTree.AwaitIdleFrame();
            SceneTree.Root.AddChild(scene);
        }

        public void End() {
            _currentGameScene.PrintStrayNodes();
            _currentGameScene.QueueFree();
            _currentGameScene = null;
        }
    }
}