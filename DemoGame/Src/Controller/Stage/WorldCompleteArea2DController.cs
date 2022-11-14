using Godot;
using Betauer.DI;
using Veronenger.Managers;

namespace Veronenger.Controller.Stage {
    public partial class WorldCompleteArea2DController :Area2D {
        [Inject] public CharacterManager CharacterManager { get; set; }

        [Export(PropertyHint.File, "*.tscn")] private string nextScene;


        public override void _Ready() {
            CharacterManager.ConfigureSceneChange(this, nextScene);
        }
    }
}