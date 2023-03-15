using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
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

[Singleton]
public class Game {
    [Inject] private SceneTree SceneTree { get; set; }
    [Inject] private ItemRepository ItemRepository { get; set; }
    [Inject] private IFactory<HUD> HudScene { get; set; }
    [Inject] private ItemConfigManager ItemConfigManager { get; set; }
    [Inject] private StageManager StageManager { get; set; }
    [Inject] private IFactory<Node> World3 { get; set; }
    [Inject] private IFactory<PlayerNode> PlayerFactory { get; set; }

    [Inject] private ResourceLoaderContainer ResourceLoaderContainer { get; set; }
    [Inject] private PoolManager<INodeLifecycle> PoolManager { get; set; }
    [Inject] private PoolFromNodeFactory<ProjectileTrail> ProjectilePool { get; set; }
    [Inject] private PoolFromNodeFactory<ZombieNode> ZombiePool { get; set; }

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
        var x = Stopwatch.StartNew();
        await ResourceLoaderContainer.LoadResources("game", (rp => {
            Console.WriteLine((rp.TotalPercent * 100f) + "% " +
                              (rp.Resource != null ? rp.Resource + ": " + (rp.ResourcePercent * 100f) + "%" : ""));
        }));
        Console.WriteLine("ResourceLoaderContainer.LoadResources: "+x.ElapsedMilliseconds+"ms");
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
        HudScene.Get().StartGame();
    }

    public void ZombieSpawn(Node scene, Vector2 position) {
        var zombieNode = ZombiePool.Get();
        Console.WriteLine("ZombieSpawn "+zombieNode.GetInstanceId());
        ItemRepository.AddEnemy(ItemConfigManager.ZombieConfig, "Zombie", zombieNode);
        zombieNode.AddToScene(scene, position);
    }

    public ProjectileTrail NewBullet() {
        var projectileTrail = ProjectilePool.Get();
        Console.WriteLine("NewBullet "+projectileTrail.GetInstanceId());
        projectileTrail.AddToScene(_currentGameScene, Vector2.Zero);
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

    public async Task End() {
        HudScene.Get().EndGame();
        // FreeSceneAndKeepPoolData();
        FreeSceneAndUnloadResources();
        _currentGameScene.QueueFree();
        _currentGameScene = null;
        await SceneTree.AwaitProcessFrame();
        GC.GetTotalMemory(true);

        Node.PrintOrphanNodes();
    }

    private void FreeSceneAndUnloadResources() {
        // To ensure the pool nodes are freed along with the scene:
        
        // 1. All busy elements are still attached to the tree and will be destroyed with the scene, so we don't need to
        // do anything with them.
        // But the non busy and valid element are outside of the scene tree in the pool, so, loop them and free them:
        PoolManager.ForEachElementInAllPools(e => e.Free());
        // 2. Remove the data from the pool to avoid having references to busy elements which are going to die with the scene
        PoolManager.Clear();
        ResourceLoaderContainer.UnloadResources("game");
    }

    private void FreeSceneAndKeepPoolData() {
        // This line keeps the godot nodes in the pool removing them from scene, because the scene is going to be freed
        PoolManager.ForEachElementInAllPools(e => e.RemoveFromScene());
    }

    public void InstantiateNewZombie() {
        var position = _currentGameScene.GetNode("EnemySpawn").GetChildren().OfType<Marker2D>().First().GlobalPosition;
        ZombieSpawn(_currentGameScene, position);
    }
}