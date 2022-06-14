using Godot;
using Betauer;
using Betauer.DI;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Stage {
    public class WorldCompleteArea2DController :Area2D {
        [Inject] public CharacterManager CharacterManager;

        [Export(PropertyHint.File, "*.tscn")] private string nextScene;


        public override void _Ready() {
            CharacterManager.ConfigureSceneChange(this, nextScene);
        }
    }
}