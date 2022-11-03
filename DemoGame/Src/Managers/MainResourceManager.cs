using System;
using Betauer.DI;
using Betauer.Loader;
using Godot;
using Veronenger.Controller.Menu;
using Veronenger.Controller.UI;

namespace Veronenger.Managers {

    [Service]
    public class MainResourceLoader : ResourceLoaderContainer {
        [Load("res://Assets/UI/Consoles/Xbox 360 Controller Updated.png")] 
        public Texture Xbox360ButtonsTexture;

        [Load("res://Assets/UI/Consoles/Xbox One Controller Updated.png")]
        public Texture XboxOneButtonsTexture;

        [Load("res://Scenes/UI/RedefineActionButton.tscn")]
        public Func<RedefineActionButton> RedefineActionButtonFactory;
        
        [Load("res://Worlds/World1.tscn")] public Func<Node> CreateWorld1;
        [Load("res://Worlds/World2.tscn")] public Func<Node> CreateWorld2;
        [Load("res://Worlds/World2-empty.tscn")] public Func<Node> CreateWorld2Empty;

        [Load("res://Scenes/Player.tscn")] public Func<Node2D> CreatePlayer;

        [Load("res://Assets/UI/my_theme.tres")] public Theme MyTheme;
        
        [Load("res://Assets/UI/DebugConsole.tres")] public Theme DebugConsoleTheme;

        public override void DoOnProgress(LoadingProgress context) {
        }

    }
}