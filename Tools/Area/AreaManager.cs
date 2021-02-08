using Betauer.Tools.Character;
using Godot;
using Godot.Collections;

namespace Betauer.Tools.Area {
    public class AreaManager : Node, IStageController {
        private StageController _stageController;

        public override void _EnterTree() {
            _stageController = new StageController(this);
            GameManager.Instance.AddManager(this);
        }

        public void RegisterStage(CollisionShape2D stageCollisionShape2D) {
            var stageArea2D = stageCollisionShape2D.GetParent<Area2D>();
            stageArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_stage),
                new Array {stageArea2D, stageCollisionShape2D.Shape});
            stageArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_exited, this, nameof(_on_player_exited_stage),
                new Array {stageArea2D, stageCollisionShape2D.Shape});
            _stageController.RegisterStage(stageCollisionShape2D);
        }

        public void _on_player_entered_stage(Area2D player, Area2D stageEnteredArea2D, RectangleShape2D shape2D) {
            _stageController._on_player_entered_stage(player, stageEnteredArea2D, shape2D);
        }

        public void _on_player_exited_stage(Area2D player, Area2D stageExitedArea2D, RectangleShape2D stageShape2D) {
            _stageController._on_player_exited_stage(player, stageExitedArea2D, stageShape2D);
        }

        public void RegisterPlayer(CharacterController player) {
            _stageController.RegisterPlayer(player);
        }
    }
}