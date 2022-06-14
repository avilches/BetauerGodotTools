using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Betauer;
using Betauer.DI;
using Godot;
using Veronenger.Game.Controller.Menu;
using Veronenger.Game.Controller.UI;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ResourceManager {
        [Inject] private Func<SceneTree> GetTree;

        private const string MainMenu = "res://Scenes/Menu/MainMenu.tscn";
        private const string MainMenuBottomBar = "res://Scenes/Menu/MainMenuBottomBar.tscn";
        private const string PauseMenu = "res://Scenes/Menu/PauseMenu.tscn";
        private const string SettingsMenu = "res://Scenes/Menu/SettingsMenu.tscn";
        private const string ModalBoxConfirm = "res://Scenes/Menu/ModalBoxConfirm.tscn";

        private const string World1 = "res://Worlds/World1.tscn";
        private const string World2 = "res://Worlds/World2.tscn";

        private const string Player = "res://Scenes/Player.tscn";
        
        public const string Xbox360Buttons = "res://Assets/UI/Consoles/Xbox 360 Controller Updated.png";
        public const string XboxOneButtons = "res://Assets/UI/Consoles/Xbox One Controller Updated.png";


        public Texture Xbox360ButtonsTexture => Resource<Texture>(Xbox360Buttons); 
        public Texture XboxOneButtonsTexture => Resource<Texture>(XboxOneButtons); 

        public Node CreateWorld1() => Resource<PackedScene>(World1).Instance();
        public Node CreateWorld2() => Resource<PackedScene>(World2).Instance();

        public MainMenu CreateMainMenu() => Resource<PackedScene>(MainMenu).Instance<MainMenu>();
        public MainMenuBottomBar CreateMainMenuBottomBar() => Resource<PackedScene>(MainMenuBottomBar).Instance<MainMenuBottomBar>();
        public PauseMenu CreatePauseMenu() => Resource<PackedScene>(PauseMenu).Instance<PauseMenu>();
        public SettingsMenu CreateSettingsMenu() => Resource<PackedScene>(SettingsMenu).Instance<SettingsMenu>();

        public ModalBoxConfirm CreateModalBoxConfirm() =>
            Resource<PackedScene>(ModalBoxConfirm).Instance<ModalBoxConfirm>();

        public MainMenuBottomBar CreateBottomBar() =>
            Resource<PackedScene>(ModalBoxConfirm).Instance<MainMenuBottomBar>();

        public Node2D CreatePlayer() => Resource<PackedScene>(Player).Instance<Node2D>();

        private Dictionary<string, Resource> _resources;

        public async Task Load(Action<LoadingContext> progress, Func<Task> awaiter) {
            string[] resourcesToLoad = {
                Xbox360Buttons,
                XboxOneButtons,

                PauseMenu,
                MainMenu,
                MainMenuBottomBar,
                SettingsMenu,
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

        public T Resource<T>(string res) where T : class => _resources[res] as T;
    }
}