using System;
using Godot;
using Tools;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Stage {
    public class WorldCompleteArea2DController : DiArea2D {
        [Inject] public AreaManager AreaManager;

        [Export(PropertyHint.File, "*.tscn")] private string nextScene;


        public override void _EnterTree() {
            AreaManager.ConfigureSceneChange(this, nextScene);
        }
    }
}