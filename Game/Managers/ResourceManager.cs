using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer;
using Betauer.DI;
using Godot;
using Veronenger.Game.Controller.Menu;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ResourceManager {
        [Inject] private Func<SceneTree> GetTree;

        private const string MainMenu = "res://Scenes/MainMenu.tscn";
        private const string PauseMenu = "res://Scenes/PauseMenu.tscn";
        private const string World1 = "res://Worlds/World1.tscn";
        private const string World2 = "res://Worlds/World2.tscn";
        private const string Player = "res://Scenes/Player.tscn";

        public Node CreateWorld1() => Resource<PackedScene>(World1).Instance();
        public Node CreateWorld2() => Resource<PackedScene>(World2).Instance();
        public MainMenu CreateMainMenu() => Resource<PackedScene>(MainMenu).Instance<MainMenu>();
        public PauseMenu CreatePauseMenu() => Resource<PackedScene>(PauseMenu).Instance<PauseMenu>();
        public Node2D CreatePlayer() => Resource<PackedScene>(Player).Instance<Node2D>();

        private Dictionary<string, Resource> _resources;

        public async Task Load(Action<LoadingContext> progress, Func<Task> awaiter) {
            string[] resourcesToLoad = {
                PauseMenu,
                MainMenu,
                World1,
                World2,
                Player,
                "res://Scenes/SplashScreen.tscn",
                "res://Worlds/Environment/GVaniaBridgeTileMap.tscn",
                "res://Worlds/Environment/gvania-bridge-tileset.png",
                "res://Worlds/Environment/GVania/street-lamp.png",
                "res://Worlds/Environment/GVania/dragon-bones-head.png",
                "res://Worlds/Environment/GVania/dragon-bones-fang.png",
                "res://Worlds/Environment/GVania/block.png",
                "res://Worlds/Environment/GVania/dragon-bones-ribs.png",
                "res://Worlds/Environment/GVania/statue.png",
                "res://Worlds/Environment/GVania-town-tileset.png",
                "res://Worlds/Environment/GVania-church-tileset.png",
                "res://Worlds/Environment/GVania/big-block.png",
                "res://Worlds/Environment/GVania-patreon-horror.png",
                "res://Scenes/WorldComplete.tscn",
                "res://icon.png",
                "res://Worlds/Environment/gvania-bridge-background.png",
                "res://Worlds/Environment/gvania-bridge-middleground.png",
                "res://Worlds/OnEnterStart.gd",
                "res://Worlds/OnEnterStop.gd",
                "res://Scenes/SlopeStairs.tscn",
                "res://Worlds/Environment/anokalisa/village.png",
                "res://Worlds/Environment/anokalisa/gallery.png",
                "res://Worlds/Environment/anokalisa/LegacyVaniaMock.tscn",
                "res://Worlds/Environment/anokalisa/LegacyVania.tscn",
                "res://Scenes/Zombie.tscn",
                "res://Scenes/Enemy.tscn",
                "res://Characters/Zombie/zombie-sheet.png",
                "res://Assets/Lato-Bold-m.tres",
                "res://Assets/Lato-Bold.ttf",
                "res://Assets/Lato-Bold-XL.tres",
                "res://Assets/SimpleBox/SimpleBox.tres",
                "res://Assets/SimpleBox/Montserrat-Bold.ttf",
                "res://Assets/SimpleBox/icons.svg",
                "res://Characters/Player-heroine/run.png",
                "res://Characters/Player-heroine/idle.png",
                "res://Characters/Player-heroine/jump.png",
                "res://Game/Controller/Stage/StageCameraController.cs"
            };
            _resources = await Loader.Load(resourcesToLoad, progress, awaiter);
            // foreach (var resourcesValue in _resources.Values) {
                // GD.Print(resourcesValue);
            // }
        }

        private T Resource<T>(string res) where T : class =>  _resources[res] as T;
    }
}