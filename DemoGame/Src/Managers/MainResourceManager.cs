using System;
using Betauer.DI;
using Betauer.Loader;
using Godot;
using Veronenger.Controller.Character;
using Veronenger.Controller.UI;

namespace Veronenger.Managers {

    [Service]
    public class MainResourceLoader : ResourceLoaderContainer {
        [Load("res://Assets/UI/Consoles/Xbox 360 Controller Updated.png")] 
        public Texture2D Xbox360ButtonsTexture2D;

        [Load("res://Assets/UI/Consoles/Xbox One Controller Updated.png")]
        public Texture2D XboxOneButtonsTexture2D;

        [Load("res://Scenes/UI/RedefineActionButton.tscn")]
        public Func<RedefineActionButton> RedefineActionButtonFactory;
        
        [Load("res://Worlds/World3.tscn")] public Func<Node> CreateWorld3;

        [Load("res://Scenes/Player.tscn")] public Func<PlayerController> CreatePlayer;

        [Load("res://Assets/UI/my_theme.tres")] public Theme MyTheme;
        
        [Load("res://Assets/UI/DebugConsole.tres")] public Theme DebugConsoleTheme;

        public override void DoOnProgress(float progress) {
        }

    }
}