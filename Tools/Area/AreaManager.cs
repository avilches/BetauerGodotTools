using Betauer.Tools.Character;
using Godot;
using Godot.Collections;
using static Betauer.Tools.LayerConstants;


namespace Betauer.Tools.Area {
    public class AreaManager : Node, IStageController, ISceneController {
        private StageController _stageController;
        private SceneController _sceneController;

        public override void _EnterTree() {
            _stageController = new StageController();
            _sceneController = new SceneController();
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

        public void RegisterDeathZone(CollisionShape2D deathCollisionShape2D) {
            var deathArea2D = deathCollisionShape2D.GetParent<Area2D>();
            deathArea2D.CollisionLayer = 0;
            deathArea2D.CollisionMask = 0;
            deathArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
            deathArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_death_zone),
                new Array {deathArea2D, deathCollisionShape2D.Shape});
        }

        public void _on_player_entered_death_zone(Area2D player, Area2D deathArea2D, RectangleShape2D shape2D) {
            GameManager.Instance.PlayerEnteredDeathZone(deathArea2D);
        }

        public void RegisterSceneChange(Area2D sceneChangeArea2D, string scene) {
            sceneChangeArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_scene_change),
                new Array {sceneChangeArea2D, scene});
            _sceneController.RegisterSceneChange(sceneChangeArea2D);
        }

        public void _on_player_entered_scene_change(Area2D player, Area2D stageEnteredArea2D, string scene) {
            _sceneController._on_player_entered_scene_change(player, stageEnteredArea2D, scene);
        }
    }
}