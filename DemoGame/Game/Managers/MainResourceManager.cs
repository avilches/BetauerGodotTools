using System;
using Betauer.DI;
using Betauer.Loader;
using Godot;
using Veronenger.Game.Controller.Menu;

namespace Veronenger.Game.Managers {

    [Service]
    public class MainResourceLoader : ResourceLoaderContainer {
        [Resource("res://Assets/UI/Consoles/Xbox 360 Controller Updated.png")] 
        public Texture Xbox360ButtonsTexture;

        [Resource("res://Assets/UI/Consoles/Xbox One Controller Updated.png")]
        public Texture XboxOneButtonsTexture;
        
        [Scene("res://Worlds/World1.tscn")] public Func<Node> CreateWorld1;
        [Scene("res://Worlds/World2.tscn")] public Func<Node> CreateWorld2;

        [Scene("res://Scenes/Player.tscn")] public Func<Node2D> CreatePlayer;
    }
}