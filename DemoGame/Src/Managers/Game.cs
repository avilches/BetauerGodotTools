using System;
using System.Linq;
using System.Threading.Tasks;
using Betauer.DI;
using Betauer.Core.Nodes;
using Betauer.Core.Signal;
using Betauer.DI.Factory;
using Veronenger.Character.Npc;
using Godot;
using Veronenger.Character.Player;
using Veronenger.Config;
using Veronenger.Persistent;
using Veronenger.Transient;
using Veronenger.UI;

namespace Veronenger.Managers;

[Service]
public class Game {
    [Inject] private SceneTree SceneTree { get; set; }
    [Inject] private ItemRepository ItemRepository { get; set; }
    [Inject] private HUD HudScene { get; set; }
    [Inject] private ItemConfigManager ItemConfigManager { get; set; }
    [Inject] private StageManager StageManager { get; set; }
    [Inject] private IFactory<Node> World3 { get; set; }
    [Inject] private IFactory<PlayerNode> PlayerFactory { get; set; }

    [Inject] private PoolManager PoolManager { get; set; }
    [Inject] private PoolFactory<ProjectileTrail> ProjectilePool { get; set; }
    [Inject] private PoolFactory<ZombieNode> ZombiePool { get; set; }

    private Node _currentGameScene;

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
        ItemRepository.Clear();
        ItemRepository.AddMeleeWeapon(ItemConfigManager.Knife, "Knife", 6f,"K1");
        ItemRepository.AddMeleeWeapon(ItemConfigManager.Metalbar, "Metalbar", 9f, "M1");
        
        var slowGun = ItemRepository.AddRangeWeapon(ItemConfigManager.SlowGun, "Gun", 6f, "SG");
        slowGun.DelayBetweenShots = 0.2f;
        
        var gun = ItemRepository.AddRangeWeapon(ItemConfigManager.Gun, "Gun", 9f, "G");
        gun.DelayBetweenShots = 0.4f;
        
        var shotgun = ItemRepository.AddRangeWeapon(ItemConfigManager.Shotgun, "Shotgun", 22f,"SG-");
        shotgun.DelayBetweenShots = 1f;
        shotgun.EnemiesPerHit = 2;
        
        var machinegun = ItemRepository.AddRangeWeapon(ItemConfigManager.MachineGun, "Maching gun", 4, "MG");
        machinegun.DelayBetweenShots = 0.05f;
        machinegun.EnemiesPerHit = 3;
        machinegun.Auto = true;
        
        GD.PushWarning("World3 creation start");
        _currentGameScene = World3.Get();
        GD.PushWarning("World3 creation end");
        AddPlayerToScene(_currentGameScene);
        SceneTree.Root.AddChild(_currentGameScene);
        await SceneTree.AwaitProcessFrame();

        var bullets = new Node();
        bullets.Name = "Bullets";
        _currentGameScene.AddChild(bullets);
        HudScene.StartGame();
    }

    public void ZombieSpawn(Node scene, Vector2 position) {
        var zombieNode = ZombiePool.Get();
        ItemRepository.AddEnemy(ItemConfigManager.ZombieConfig, "Zombie", zombieNode);
        zombieNode.AddToScene(scene, position);
    }

    public ProjectileTrail NewBullet() {
        var projectileTrail = ProjectilePool.Get();
        _currentGameScene.AddChild(projectileTrail);
        return projectileTrail;
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
        var playerNode = PlayerFactory.Get();
        nextScene.GetNode<Marker2D>("SpawnPlayer").AddChild(playerNode);
        ItemRepository.SetPlayer(playerNode);
    }


    public void End() {
        // Removing nodes from scene avoid get freed along with the scene and make them available in the Pool again
        PoolManager.RemoveFromScene();
        Node.PrintOrphanNodes();
        HudScene.EndGame();
        _currentGameScene.QueueFree();
        _currentGameScene = null;
    }

    public void InstantiateNewZombie() {
        var position = _currentGameScene.GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().First().GlobalPosition;
        ZombieSpawn(_currentGameScene, position);
    }
}