using System;
using System.Linq;
using Betauer.Camera;
using Betauer.Core;
using Betauer.DI;
using Betauer.Core.Nodes;
using Godot;
using Veronenger.Controller.Character;

namespace Veronenger.Managers {

    [Service]
    public class Game {

        [Inject] private SceneTree SceneTree { get; set; }
        [Inject] private StageManager StageManager { get; set; }
        [Inject] private PlatformManager PlatformManager { get; set; }
        [Inject] private Factory<Node> World3 { get; set; }
        [Inject] private Factory<PlayerNode> Player { get; set; }
        
        private Node _currentGameScene;
        private PlayerNode _playerScene;

        public void Start() {
            StageManager.ClearState();
            StartWorld3();
        }

        public void StartNew() {
            // _currentGameScene = MainResourceLoader.CreateWorld2Empty();
            var tileMap = _currentGameScene.GetNode<TileMap>("RealTileMap");
            new WorldGenerator().Generate(tileMap);
            AddPlayerToScene(_currentGameScene, Vector2.Zero);
            SceneTree.Root.AddChildDeferred(_currentGameScene);
        }

        public void StartWorld3() {
            _currentGameScene = World3.Get();

            _currentGameScene.GetChildren().OfType<TileMap>().ForEach(PlatformManager.ConfigureTileMapCollision);
            _currentGameScene.GetChildren().OfType<CanvasModulate>().ForEach(cm => cm.Visible = true);
            _currentGameScene.GetNode<Node>("Lights").GetChildren().OfType<PointLight2D>().ForEach(light => {
                if (light.Name.ToString().StartsWith("Candle")) {
                    CandleOff(light);
                }
            });
            PlatformManager.ConfigurePlatformsCollisions();
            _currentGameScene.GetNode<Node>("Stages").GetChildren().OfType<Area2D>().ForEach(StageManager.ConfigureStage);
            AddPlayerToScene(_currentGameScene);
            
            SceneTree.Root.AddChildDeferred(_currentGameScene);
        }

        private void CandleOff(PointLight2D light) {
            light.Enabled = true;
            light.Color = new Color("ffd1c8");
            light.TextureScale = 0.2f;
            light.ShadowEnabled = true;
            light.ShadowFilter = Light2D.ShadowFilterEnum.None;
            light.GetNode<Area2D>("Area2D")
                ?.OnBodyEntered(LayerConstants.LayerPlayerBody, (PlayerNode player) => CandleOn(light));
        }

        private void CandleOn(PointLight2D light) {
            light.Enabled = true;
            light.Color = new Color("ffd1c8");
            light.TextureScale = 0.8f;
            // light.ShadowEnabled = false;
        }

        private void QueueChangeSceneWithPlayer(string sceneName) {
            StageManager.ClearState();
            _currentGameScene.QueueFree();
            var nextScene = ResourceLoader.Load<PackedScene>(sceneName).Instantiate();
            AddPlayerToScene(nextScene);
            _currentGameScene = nextScene;
            SceneTree.Root.AddChildDeferred(nextScene);
        }

        private void AddPlayerToScene(Node nextScene) {
            var marker2D = nextScene.GetNode<Node2D>("PositionPlayer");
            if (marker2D == null) throw new Exception("Node PositionPlayer not found when loading scene " + nextScene.SceneFilePath);
            AddPlayerToScene(nextScene, marker2D.GlobalPosition);
        }

        private void AddPlayerToScene(Node nextScene, Vector2 position) {
            _playerScene = Player.Get();
            nextScene.AddChild(_playerScene);
            // TODO: this shows a warning "!is_inside_tree()" is true. Returned: get_transform()" but it still works 
            _playerScene.GlobalPosition = position;
        }

        public void End() {
            Node.PrintOrphanNodes();
            _currentGameScene.QueueFree();
            _currentGameScene = null;
        }
    }
}