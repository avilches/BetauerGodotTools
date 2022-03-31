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
        private const string OptionsMenu = "res://Scenes/OptionsMenu.tscn";
        private const string ModalBoxConfirm = "res://Scenes/ModalBoxConfirm.tscn";

        private const string World1 = "res://Worlds/World1.tscn";
        private const string World2 = "res://Worlds/World2.tscn";

        private const string Player = "res://Scenes/Player.tscn";

        public Node CreateWorld1() => Resource<PackedScene>(World1).Instance();
        public Node CreateWorld2() => Resource<PackedScene>(World2).Instance();

        public MainMenu CreateMainMenu() => Resource<PackedScene>(MainMenu).Instance<MainMenu>();
        public PauseMenu CreatePauseMenu() => Resource<PackedScene>(PauseMenu).Instance<PauseMenu>();
        public OptionsMenu CreateOptionsMenu() => Resource<PackedScene>(OptionsMenu).Instance<OptionsMenu>();

        public ModalBoxConfirm CreateModalBoxConfirm() =>
            Resource<PackedScene>(ModalBoxConfirm).Instance<ModalBoxConfirm>();

        public Node2D CreatePlayer() => Resource<PackedScene>(Player).Instance<Node2D>();

        private Dictionary<string, Resource> _resources;

        public async Task Load(Action<LoadingContext> progress, Func<Task> awaiter) {
            string[] resourcesToLoad = {
                PauseMenu,
                MainMenu,
                OptionsMenu,
                ModalBoxConfirm,

                World1,
                World2,

                Player,
            };
            _resources = await Loader.Load(resourcesToLoad, progress, awaiter);
            // foreach (var resourcesValue in _resources.Values) {
            // GD.Print(resourcesValue);
            // }
        }

        private T Resource<T>(string res) where T : class => _resources[res] as T;
    }
}