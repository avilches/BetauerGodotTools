using Godot;
using Godot.Collections;
using Tools;
using Veronenger.Game.Controller.Camera;
using static Veronenger.Game.Tools.LayerConstants;

namespace Veronenger.Game.Managers {
    public class StageManager : Object /* needed to connect to signals */ {
        private IPlayerStageChange _stageCameraController;

        public void ListenPlayerStageChanges(IPlayerStageChange stageCameraController) {
            // TODO: now, only the StageCameraController is subscribed to these events. It could a list instead...?
            _stageCameraController = stageCameraController;
        }

        public void ConfigurePlayerStageDetector(Area2D stageDetector) {
            stageDetector.CollisionLayer = 0;
            stageDetector.CollisionMask = 0;
            stageDetector.SetCollisionMaskBit(PLAYER_DETECTOR_LAYER, true);
        }

        public void ConfigureStage(Area2D stageArea2D, CollisionShape2D stageCollisionShape2D) {
            stageArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_entered, this, nameof(_on_player_entered_stage),
                new Array { stageArea2D, stageCollisionShape2D.Shape });
            stageArea2D.Connect(GodotConstants.GODOT_SIGNAL_area_exited, this, nameof(_on_player_exited_stage),
                new Array { stageArea2D });
            stageArea2D.CollisionLayer = 0;
            stageArea2D.CollisionMask = 0;
            stageArea2D.SetCollisionLayerBit(PLAYER_DETECTOR_LAYER, true);
        }


        public void _on_player_entered_stage(Area2D player, Area2D stageEnteredArea2D, RectangleShape2D shape2D) {
            _stageCameraController?._on_player_entered_stage(player, stageEnteredArea2D, shape2D);
        }

        public void _on_player_exited_stage(Area2D player, Area2D stageExitedArea2D) {
            _stageCameraController?._on_player_exited_stage(player, stageExitedArea2D);
        }
    }
}