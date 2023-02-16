using System.Linq;
using System.Threading.Tasks;
using Betauer.Core;
using Betauer.DI;
using Betauer.Core.Nodes;
using Betauer.Core.Pool;
using Betauer.Core.Signal;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Items;
using Veronenger.UI;

namespace Veronenger.Managers;

[Service]
public class Game {

    [Inject] private SceneTree SceneTree { get; set; }
    [Inject] private World World { get; set; }
    [Inject] private HUD HudScene { get; set; }
    [Inject] private WeaponConfigManager WeaponConfigManager { get; set; }
    [Inject] private StageManager StageManager { get; set; }
    [Inject] private CharacterManager CharacterManager { get; set; }
    [Inject] private PlatformManager PlatformManager { get; set; }
    [Inject] private Factory<Node> World3 { get; set; }
    [Inject] private Factory<PlayerNode> Player { get; set; }
    [Inject] Texture2D Icon { get; set; }
    
    private Node _currentGameScene;
    private PlayerNode _playerScene;
    public MiniPoolBusy<Bullet> _bulletPool;

    public async Task Start() {
        StageManager.ClearState();
        await StartWorld3();
    }

    public void StartNew() {
        // _currentGameScene = MainResourceLoader.CreateWorld2Empty();
        var tileMap = _currentGameScene.GetNode<TileMap>("RealTileMap");
        new WorldGenerator().Generate(tileMap);
        AddPlayerToScene(_currentGameScene);
        SceneTree.Root.AddChildDeferred(_currentGameScene);
    }

    public async Task StartWorld3() {
        World.Clear();
        World.CreateMeleeWeapon(WeaponConfigManager.Knife, "Knife", "K1");
        World.CreateMeleeWeapon(WeaponConfigManager.Metalbar, "Metalbar", "M1");
        World.CreateRangeWeapon(WeaponConfigManager.Gun, "Gun", "G1");
        
        GD.PushWarning("World3 creation start");
        _currentGameScene = World3.Get();
        GD.PushWarning("World3 creation end");
        AddPlayerToScene(_currentGameScene);
        SceneTree.Root.AddChild(_currentGameScene);
        await SceneTree.AwaitProcessFrame();

        _currentGameScene.GetChildren().OfType<TileMap>().ForEach(PlatformManager.ConfigureTileMapCollision);
        _currentGameScene.GetChildren().OfType<CanvasModulate>().ForEach(cm => cm.Visible = true);
        _currentGameScene.GetNode<Node>("Lights").GetChildren().OfType<PointLight2D>().ForEach(light => {
            if (light.Name.ToString().StartsWith("Candle")) {
                CandleOff(light);
            }
        });
        PlatformManager.ConfigurePlatformsCollisions();
        _currentGameScene.GetNode<Node>("Stages").GetChildren().OfType<Area2D>().ForEach(StageManager.ConfigureStage);

        var bullets = new Node();
        bullets.Name = "Bullets";
        _currentGameScene.AddChild(bullets);
        _bulletPool = new MiniPoolBusy<Bullet>(() => new Bullet(_currentGameScene, Icon), 1);
        HudScene.StartGame();
    }

    public Bullet NewBullet() => _bulletPool.Get();

    private void CandleOff(PointLight2D light) {
        light.Enabled = true;
        light.Color = new Color("ffd1c8");
        light.TextureScale = 0.2f;
        light.ShadowEnabled = true;
        light.ShadowFilter = Light2D.ShadowFilterEnum.None;
        light.GetNode<Area2D>("Area2D")
            ?.OnBodyEntered(LayerConstants.LayerPlayerBody, (player) => {
                if (player is CharacterBody2D character && CharacterManager.IsPlayer(character)) CandleOn(light);
            });
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
        _playerScene = Player.Get();
        nextScene.GetNode<Marker2D>("SpawnPlayer").AddChild(_playerScene);
    }


    public void End() {
        Node.PrintOrphanNodes();
        HudScene.EndGame();
        _currentGameScene.QueueFree();
        _currentGameScene = null;
    }
}